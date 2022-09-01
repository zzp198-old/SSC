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
using Terraria.ModLoader.IO;

namespace SSC;

public class SSC : Mod
{
    internal static SSC Mod => ModContent.GetInstance<SSC>();

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
                if (Directory.GetFiles(Path.Combine(Main.SavePath, "SSC"), $"{name}.plr", SearchOption.AllDirectories).Length > 0)
                {
                    ChatHelper.SendChatMessageToClient(NetworkText.FromLiteral("Name already exists"), Color.Red, who);
                    return;
                }

                // 需要在服务端进行保存,不然Client型mod可以本地修改数据并保存存档发送的方式带入非初始物品(修改StartInventory而不需要同步的本地mod)
                var data = new PlayerFileData(Path.Combine(Main.SavePath, "SSC", id.ToString(), $"{name}.plr"), false)
                {
                    Metadata = FileMetadata.FromCurrentSettings(FileType.Player),
                    Player = new Player { name = name, difficulty = mode }
                };
                SSCKit.SetupPlayerStatsAndInventoryBasedOnDifficulty(data.Player);

                Directory.CreateDirectory(Path.Combine(Main.SavePath, "SSC", id.ToString()));
                SSCKit.InternalSavePlayer(data);

                NetMessage.TrySendData(MessageID.WorldData, who);
                break;
            }
            case ID.RemoveSSC:
            {
                var id = bin.ReadUInt64();
                var name = bin.ReadString();

                File.Delete(Path.Combine(Main.SavePath, "SSC", id.ToString(), $"{name}.plr"));
                File.Delete(Path.Combine(Main.SavePath, "SSC", id.ToString(), $"{name}.tplr"));

                NetMessage.TrySendData(MessageID.WorldData, who);
                break;
            }
            case ID.ChooseSSC:
            {
                var id = bin.ReadUInt64();
                var name = bin.ReadString();

                // 防止在线玩家名称重复
                if (Netplay.Clients.Where(x => x.IsActive).Any(x => Main.player[x.Id].name == name))
                {
                    ChatHelper.SendChatMessageToClient(NetworkText.FromKey(Lang.mp[5].Key, name), Color.Red, who);
                    return;
                }

                var data = Player.LoadPlayer(Path.Combine(Main.SavePath, "SSC", id.ToString(), $"{name}.plr"), false);
                // 防止旅程模式误入导致的隐患和崩溃

                if (!SystemLoader.CanWorldBePlayed(data, Main.ActiveWorldFileData, out var mod))
                {
                    var msg = mod.WorldCanBePlayedRejectionMessage(data, Main.ActiveWorldFileData);
                    ChatHelper.SendChatMessageToClient(NetworkText.FromLiteral(msg), Color.Red, who);
                    return;
                }

                // if (data.Player.difficulty == 3 && !Main.GameModeInfo.IsJourneyMode)
                // {
                //     ChatHelper.SendChatMessageToClient(NetworkText.FromKey("Net.PlayerIsNotCreativeAndWorldIsCreative"), Color.Red,
                //         who);
                //     return;
                // }
                //
                // if (data.Player.difficulty != 3 && Main.GameModeInfo.IsJourneyMode)
                // {
                //     ChatHelper.SendChatMessageToClient(NetworkText.FromKey("Net.PlayerIsNotCreativeAndWorldIsCreative"), Color.Red,
                //         who);
                //     return;
                // }

                // 压缩玩家为byte[],客户端同步SSC玩家
                var memoryStream = new MemoryStream();
                TagIO.ToStream(new TagCompound
                {
                    { "PLR", File.ReadAllBytes(Path.Combine(Main.SavePath, "SSC", id.ToString(), $"{name}.plr")) },
                    { "TPLR", File.ReadAllBytes(Path.Combine(Main.SavePath, "SSC", id.ToString(), $"{name}.tplr")) },
                }, memoryStream);
                var array = memoryStream.ToArray();

                // 指定Client挂载全部数据,不管是否需要同步的,以确保mod的本地数据同步.(发送给全部Client会出现显示错误,会先Spawn)
                var mp = GetPacket();
                mp.Write((byte)ID.LoadSSC);
                mp.Write(array.Length);
                mp.Write(array);
                mp.Send(who);

                // 其他Client者通过原生代码同步必要的数据,包括mod的net同步.
                // 客户端的返回数据会更改服务端的Client名称,不添加的话,离开时的提示信息有误且后进的玩家无法被先进的玩家看到(虽然死亡能解除)
                NetMessage.SendData(MessageID.PlayerInfo, who);
                break;
            }
            case ID.LoadSSC:
            {
                var compound = TagIO.FromStream(new MemoryStream(bin.ReadBytes(bin.ReadInt32())));

                var name = Path.Combine(Path.GetTempPath(), $"{SteamUser.GetSteamID().m_SteamID}.plr");
                File.WriteAllBytes(name, compound.GetByteArray("PLR"));
                File.WriteAllBytes(Path.ChangeExtension(name, ".tplr"), compound.GetByteArray("TPLR"));

                var data = new PlayerFileData(Path.Combine(Main.PlayerPath, $"{SteamUser.GetSteamID().m_SteamID}.SSC"), false)
                {
                    Metadata = FileMetadata.FromCurrentSettings(FileType.Player),
                    Player = Player.LoadPlayer(name, false).Player
                };
                data.MarkAsServerSide();
                data.SetAsActive();

                Main.player[Main.myPlayer].Spawn(PlayerSpawnContext.SpawningIntoWorld); // SetPlayerDataToOutOfClassFields
                try
                {
                    Player.Hooks.EnterWorld(Main.myPlayer); // 其他mod如果没有防御性编程可能会报错
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                    throw;
                }
                finally
                {
                    ModContent.GetInstance<SSCSyS>().UI.SetState(null);
                }

                break;
            }
            case ID.SaveSSC:
            {
                var id = bin.ReadUInt64();
                var name = Path.Combine(Main.SavePath, "SSC", id.ToString(), $"{bin.ReadString()}.plr");
                var compound = TagIO.FromStream(new MemoryStream(bin.ReadBytes(bin.ReadInt32())));

                if (File.Exists(name))
                {
                    File.WriteAllBytes(name, compound.GetByteArray("PLR"));
                    File.WriteAllBytes(Path.ChangeExtension(name, ".tplr"), compound.GetByteArray("TPLR"));
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