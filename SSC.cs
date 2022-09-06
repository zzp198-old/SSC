using System;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Steamworks;
using Terraria;
using Terraria.Chat;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SSC;

public class SSC : Mod
{
    internal static SSC Mod => ModContent.GetInstance<SSC>();
    internal static string SavePath => Path.Combine(Main.SavePath, nameof(SSC));
    internal static ulong ClientID => SteamUser.GetSteamID().m_SteamID;

    internal enum ID
    {
        CreateSSC,
        RemoveSSC,
        ChooseSSC,
        LoadSSC,
        SaveSSC,
    }

    public override void HandlePacket(BinaryReader bin, int who)
    {
        var type = bin.ReadByte();
        switch ((ID)type)
        {
            case ID.CreateSSC:
            {
                var id = bin.ReadUInt64();
                var name = bin.ReadString();
                var mode = bin.ReadByte();

                if (string.IsNullOrWhiteSpace(name))
                {
                    ChatHelper.SendChatMessageToClient(NetworkText.FromKey("Net.EmptyName"), Color.Red, who);
                    return;
                }

                if (name.Length > Player.nameLen)
                {
                    ChatHelper.SendChatMessageToClient(NetworkText.FromKey("Net.NameTooLong"), Color.Red, who);
                    return;
                }

                if (name.IndexOfAny(Path.GetInvalidFileNameChars()) != -1)
                {
                    ChatHelper.SendChatMessageToClient(NetworkText.FromKey("Illegal characters in name"), Color.Red, who);
                    return;
                }

                // 防止同名注册
                if (Directory.GetFiles(SavePath, $"{name}.plr", SearchOption.AllDirectories).Length > 0)
                {
                    ChatHelper.SendChatMessageToClient(NetworkText.FromKey(Lang.mp[5].Key, name), Color.Red, who);
                    return;
                }

                try
                {
                    Directory.CreateDirectory(Path.Combine(SavePath, id.ToString()));

                    var data = new PlayerFileData(Path.Combine(SavePath, id.ToString(), $"{name}.plr"), false)
                    {
                        Metadata = FileMetadata.FromCurrentSettings(FileType.Player),
                        Player = new Player { name = name, difficulty = mode }
                    };
                    SSCKit.SetupPlayerStatsAndInventoryBasedOnDifficulty(data.Player);
                    // 保存并不一定成功,如果其他mod的数据涉及到服务端未加载的内容,会导致异常并且无法正常生成tplr文件
                    SSCKit.InternalSavePlayer(data);
                }
                catch (Exception e)
                {
                    ChatHelper.SendChatMessageToClient(NetworkText.FromLiteral(e.ToString()), Color.Red, who);
                    throw;
                }
                finally
                {
                    NetMessage.TrySendData(MessageID.WorldData, who);
                }

                break;
            }
            case ID.RemoveSSC:
            {
                var id = bin.ReadUInt64();
                var name = bin.ReadString();

                File.Delete(Path.Combine(SavePath, id.ToString(), $"{name}.plr"));
                File.Delete(Path.Combine(SavePath, id.ToString(), $"{name}.tplr"));

                NetMessage.TrySendData(MessageID.WorldData, who);
                break;
            }
            case ID.ChooseSSC:
            {
                var id = bin.ReadUInt64();
                var name = bin.ReadString();

                // Boss存活期间禁止复活
                if (ModContent.GetInstance<SSCSet>().NoSpawnWhenBossFight && Main.npc.Any(npc => npc.boss && npc.active))
                {
                    // TODO
                    ChatHelper.SendChatMessageToClient(NetworkText.FromLiteral("You are cursed, boss is still alive!"), Color.Yellow, who);
                    return;
                }

                // 防止在线玩家名称重复
                if (Netplay.Clients.Where(x => x.IsActive).Any(x => Main.player[x.Id].name == name))
                {
                    ChatHelper.SendChatMessageToClient(NetworkText.FromKey(Lang.mp[5].Key, name), Color.Red, who);
                    return;
                }

                var data = Player.LoadPlayer(Path.Combine(SavePath, id.ToString(), $"{name}.plr"), false);

                if (data.Player.difficulty == PlayerDifficultyID.Creative && !Main.GameModeInfo.IsJourneyMode)
                {
                    ChatHelper.SendChatMessageToClient(NetworkText.FromKey("Net.PlayerIsCreativeAndWorldIsNotCreative"), Color.Red, who);
                    return;
                }

                if (data.Player.difficulty != PlayerDifficultyID.Creative && Main.GameModeInfo.IsJourneyMode)
                {
                    ChatHelper.SendChatMessageToClient(NetworkText.FromKey("Net.PlayerIsNotCreativeAndWorldIsCreative"), Color.Red, who);
                    return;
                }

                // 兼容其他mod的进入规则.
                if (!SystemLoader.CanWorldBePlayed(data, Main.ActiveWorldFileData, out var mod))
                {
                    ChatHelper.SendChatMessageToClient(NetworkText.FromLiteral(
                        mod.WorldCanBePlayedRejectionMessage(data, Main.ActiveWorldFileData)
                    ), Color.Red, who);
                    return;
                }

                // 指定Client挂载全部数据,不管是否需要同步的,以确保mod的本地数据同步.(发送给全部Client会出现显示错误,会先Spawn)
                var bytes = SSCKit.Plr2Byte(data.Path);
                var mp = GetPacket();
                mp.Write((byte)ID.LoadSSC);
                mp.Write(bytes.Length);
                mp.Write(bytes);
                mp.Send(who);

                // 客户端的返回数据会更改服务端的Client名称,不添加的话,离开时的提示信息有误且后进的玩家无法被先进的玩家看到(虽然死亡能解除)
                NetMessage.SendData(MessageID.PlayerInfo, who);
                break;
            }
            case ID.LoadSSC:
            {
                var name = Path.Combine(Path.GetTempPath(), $"{DateTime.UtcNow.Ticks}.plr");

                SSCKit.Byte2Plr(bin.ReadBytes(bin.ReadInt32()), name);

                var data = new PlayerFileData(Path.Combine(Main.PlayerPath, $"{ClientID}.SSC"), false) // 只有这里会设置后缀为SSC
                {
                    Metadata = FileMetadata.FromCurrentSettings(FileType.Player),
                    Player = Player.LoadPlayer(name, false).Player
                };
                data.MarkAsServerSide();
                data.SetAsActive();

                data.Player.Spawn(PlayerSpawnContext.SpawningIntoWorld); // SetPlayerDataToOutOfClassFields
                try
                {
                    Player.Hooks.EnterWorld(Main.myPlayer); // 其他mod如果没有防御性编程可能会报错
                }
                catch (Exception e)
                {
                    ChatHelper.SendChatMessageToClient(NetworkText.FromLiteral(e.ToString()), Color.Red, who);
                    throw;
                }
                finally
                {
                    SSCSyS.UI.SetState(null);
                }

                break;
            }
            case ID.SaveSSC:
            {
                var id = bin.ReadUInt64();
                var name = Path.Combine(SavePath, id.ToString(), $"{bin.ReadString()}.plr");
                var bytes = bin.ReadBytes(bin.ReadInt32());

                if (File.Exists(name))
                {
                    SSCKit.Byte2Plr(bytes, name);
                }
                else
                {
                    ChatHelper.SendChatMessageToClient(NetworkText.FromLiteral("Can't Save SSC."), Color.Red, who);
                }

                break;
            }
            default:
            {
                SSCKit.Boot(who, $"Unexpected packet ID: {type}");
                break;
            }
        }
    }
}