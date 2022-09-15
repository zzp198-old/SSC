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

namespace QOS;

public partial class QOS : Mod
{
    // 因为功能烦杂,并且相互之间还会产生联动,代码很乱,暂时没找到好的管理架构.
    internal static string QOSSavePath => Path.Combine(Main.SavePath, "QOS");
    internal static string SSCSavePath => Path.Combine(QOSSavePath, "SSC");
    internal static Mod Mod => ModContent.GetInstance<QOS>();
    internal static Common.Configs.ClientConfig CC => ModContent.GetInstance<Common.Configs.ClientConfig>();
    internal static Common.Configs.ServerConfig SC => ModContent.GetInstance<Common.Configs.ServerConfig>();
    internal static ulong ClientID => SteamUser.GetSteamID().m_SteamID;
    internal static Player My => Main.player[Main.myPlayer];

    public override void Load()
    {
        if (!Main.dedServ)
        {
            return;
        }

        Utils.TryCreatingDirectory(QOSSavePath);
        Utils.TryCreatingDirectory(SSCSavePath);
    }

    public override void HandlePacket(BinaryReader bin, int plr)
    {
        switch ((ID)bin.ReadByte())
        {
            case ID.CreateSSC:
            {
                var id = bin.ReadUInt64();
                var name = bin.ReadString();
                var difficulty = bin.ReadByte();

                if (string.IsNullOrWhiteSpace(name))
                {
                    ChatHelper.SendChatMessageToClient(NetworkText.FromKey("Net.EmptyName"), Color.Red, plr);
                    return;
                }

                if (name.Length > Player.nameLen)
                {
                    ChatHelper.SendChatMessageToClient(NetworkText.FromKey("Net.NameTooLong"), Color.Red, plr);
                    return;
                }

                if (name.IndexOfAny(Path.GetInvalidFileNameChars()) != -1)
                {
                    ChatHelper.SendChatMessageToClient(NetworkText.FromKey("Mods.QOS.SSC.InvalidFileName"), Color.Red, plr);
                    return;
                }

                if (Directory.GetFiles(SSCSavePath, $"{name}.plr", SearchOption.AllDirectories).Length > 0) // 防止同名注册
                {
                    ChatHelper.SendChatMessageToClient(NetworkText.FromKey(Lang.mp[5].Key, name), Color.Red, plr);
                    return;
                }

                try
                {
                    Directory.CreateDirectory(Path.Combine(SSCSavePath, id.ToString()));
                    var data = new PlayerFileData(Path.Combine(SSCSavePath, id.ToString(), $"{name}.plr"), false)
                    {
                        Metadata = FileMetadata.FromCurrentSettings(FileType.Player),
                        Player = new Player { name = name, difficulty = difficulty }
                    };
                    QOSKit.SetupPlayerStatsAndInventoryBasedOnDifficulty(data.Player); // savedPerPlayerFieldsThatArentInThePlayerClass
                    // 保存并不一定成功,如果其他mod的数据涉及到服务端未加载的内容,会导致异常并且丢失所有mod数据
                    QOSKit.InternalSavePlayer(data);
                }
                catch (Exception e)
                {
                    ChatHelper.SendChatMessageToClient(NetworkText.FromLiteral(e.ToString()), Color.Red, plr);
                    ChatHelper.SendChatMessageToClient(NetworkText.FromKey("Mods.QOS.SSC.CreateException"), Color.Yellow, plr);
                    ChatHelper.SendChatMessageToClient(NetworkText.FromKey("Mods.QOS.SSC.CreateExceptionSolution"), Color.Yellow, plr);
                    throw;
                }
                finally
                {
                    NetMessage.TrySendData(MessageID.WorldData, plr);
                }

                break;
            }
            case ID.RemoveSSC:
            {
                var id = bin.ReadUInt64();
                var name = bin.ReadString();

                File.Delete(Path.Combine(SSCSavePath, id.ToString(), $"{name}.plr"));
                File.Delete(Path.Combine(SSCSavePath, id.ToString(), $"{name}.tplr"));

                NetMessage.TrySendData(MessageID.WorldData, plr);
                break;
            }
            case ID.ChooseSSC:
            {
                var id = bin.ReadUInt64();
                var name = bin.ReadString();

                if (SC.ReviveSeal && Main.npc.Any(npc => npc.active && npc.boss)) // 不能使用CurrentFrameFlags,需要排除常驻的四柱和随时刷新的探测器
                {
                    ChatHelper.SendChatMessageToClient(NetworkText.FromKey("Mods.QOS.Config.ReviveSeal.Tooltip"), Color.Red, plr);
                    return;
                }

                if (Netplay.Clients.Where(x => x.IsActive).Any(x => Main.player[x.Id].name == name)) // 防止在线玩家重复
                {
                    ChatHelper.SendChatMessageToClient(NetworkText.FromKey(Lang.mp[5].Key, name), Color.Red, plr);
                    return;
                }

                var data = Player.LoadPlayer(Path.Combine(SSCSavePath, id.ToString(), $"{name}.plr"), false);

                if (data.Player.difficulty == PlayerDifficultyID.Creative && !Main.GameModeInfo.IsJourneyMode)
                {
                    ChatHelper.SendChatMessageToClient(NetworkText.FromKey("Net.PlayerIsCreativeAndWorldIsNotCreative"), Color.Red, plr);
                    return;
                }

                if (data.Player.difficulty != PlayerDifficultyID.Creative && Main.GameModeInfo.IsJourneyMode)
                {
                    ChatHelper.SendChatMessageToClient(NetworkText.FromKey("Net.PlayerIsNotCreativeAndWorldIsCreative"), Color.Red, plr);
                    return;
                }

                if (!SystemLoader.CanWorldBePlayed(data, Main.ActiveWorldFileData, out var mod)) // 兼容其他mod的游玩规则
                {
                    ChatHelper.SendChatMessageToClient(
                        NetworkText.FromLiteral(mod.WorldCanBePlayedRejectionMessage(data, Main.ActiveWorldFileData)), Color.Red, plr);
                    return;
                }

                // 指定Client挂载全部数据,不管是否需要同步的,以确保mod的本地数据同步.(发送给全部Client会出现显示错误,会先Spawn)
                var binary = QOSKit.Plr2Byte(data.Path);
                var mp = GetPacket();
                mp.Write((byte)ID.LoadSSC);
                mp.Write(binary.Length);
                mp.Write(binary);
                mp.Send(plr);

                // 客户端的返回数据会更改服务端的Client名称,不添加的话,离开时的提示信息有误且后进的玩家无法被先进的玩家看到(虽然死亡能解除)
                NetMessage.SendData(MessageID.PlayerInfo, plr);
                break;
            }
            case ID.LoadSSC:
            {
                var name = Path.Combine(Path.GetTempPath(), $"{DateTime.UtcNow.Ticks}.plr");

                QOSKit.Byte2Plr(bin.ReadBytes(bin.ReadInt32()), name);

                var data = new PlayerFileData(Path.Combine(Main.PlayerPath, $"{ClientID}.SSC"), false) // 只有这里会设置后缀为SSC
                {
                    Metadata = FileMetadata.FromCurrentSettings(FileType.Player),
                    Player = Player.LoadPlayer(name, false).Player
                };
                data.MarkAsServerSide();
                data.SetAsActive();

                data.Player.Spawn(PlayerSpawnContext.SpawningIntoWorld); // SetPlayerDataToOutOfClassFields,设置临时物品
                try
                {
                    Player.Hooks.EnterWorld(Main.myPlayer); // 其他mod如果没有防御性编程可能会报错
                }
                catch (Exception e)
                {
                    ChatHelper.SendChatMessageToClient(NetworkText.FromLiteral(e.ToString()), Color.Red, plr);
                    throw;
                }
                finally
                {
                    Common.Systems.SSCSystem.UI.SetState(null);
                }

                break;
            }
            case ID.SaveSSC:
            {
                var id = bin.ReadUInt64();
                var name = Path.Combine(SSCSavePath, id.ToString(), $"{bin.ReadString()}.plr");
                var binary = bin.ReadBytes(bin.ReadInt32());

                if (File.Exists(name))
                {
                    QOSKit.Byte2Plr(binary, name);
                }
                else
                {
                    ChatHelper.SendChatMessageToClient(NetworkText.FromKey("Mods.QOS.SSC.FileNotFound"), Color.Red, plr);
                }

                break;
            }
            default:
            {
                QOSKit.Boot(plr, "Invalid Package ID");
                break;
            }
        }
    }
}