using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Steamworks;
using Terraria;
using Terraria.Chat;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader.IO;

namespace SSC;

public partial class SSC : Mod
{
    public static string SavePath => Path.Combine(Main.SavePath, "SSC");
    public static ulong SteamID => SteamUser.GetSteamID().m_SteamID;

    public override void Load()
    {
        // 偷藏私货
        Main.runningCollectorsEdition = true;
    }

    public override void HandlePacket(BinaryReader b, int from)
    {
        var type = b.ReadByte();
        Logger.Debug($"{Main.myPlayer}({Main.netMode}) receive {(ID)type} from {from}");

        switch ((ID)type)
        {
            case ID.SSCInit:
            {
                // 在初始化net交互前(Hello)初始化并进行同步是最佳选择,如果在初始化net交互中改变player,会导致同步失败.
                // (位置必须是触发一次且后续不会被误触发,且因为逻辑较弱,需要幽灵化限制.InnerClientLoop全局net交互流程)
                var account = b.ReadUInt64();
                // 创建存储文件夹
                Directory.CreateDirectory(Path.Combine(Main.SavePath, "SSC", account.ToString()));
                break;
            }
            case ID.SSCList:
            {
                switch (Main.netMode)
                {
                    case NetmodeID.MultiplayerClient:
                    {
                        var count = b.ReadInt32();
                        var data = new List<(string, byte)>();
                        for (var i = 0; i < count; i++)
                        {
                            data.Add((b.ReadString(), b.ReadByte()));
                        }

                        SSCUI.Refresh(data);
                        break;
                    }
                    case NetmodeID.Server:
                    {
                        SendSSCList(b.ReadUInt64(), from);
                        break;
                    }
                }

                break;
            }
            case ID.CreateSSC:
            {
                var account = b.ReadUInt64();
                var name = b.ReadString();
                var difficulty = b.ReadByte();

                // 基本名称校验
                if (!SSCUtils.CheckName(name, out var msg))
                {
                    ChatHelper.SendChatMessageToClient(NetworkText.FromLiteral(msg), Color.Red, from);
                    return;
                }

                // 防止同名注册
                if (Directory.GetFiles(Path.Combine(Main.SavePath, "SSC"), $"{name}.plr", SearchOption.AllDirectories).Length > 0)
                {
                    ChatHelper.SendChatMessageToClient(NetworkText.FromLiteral("Name already exists"), Color.Red, from);
                    return;
                }

                // 创建人物
                var data = new PlayerFileData(Path.Combine(Main.SavePath, "SSC", account.ToString(), $"{name}.plr"), false)
                {
                    Metadata = FileMetadata.FromCurrentSettings(FileType.Player),
                    Player = new Player { name = name, difficulty = difficulty }
                };
                SSCUtils.SetupPlayerStatsAndInventoryBasedOnDifficulty(data.Player);
                SSCUtils.InternalSavePlayer(data);

                // 客户端刷新列表
                SendSSCList(account, from);
                break;
            }
            case ID.RemoveSSC:
            {
                var account = b.ReadUInt64();
                var name = b.ReadString();

                // 删除人物
                File.Delete(Path.Combine(Main.SavePath, "SSC", account.ToString(), $"{name}.plr"));
                File.Delete(Path.Combine(Main.SavePath, "SSC", account.ToString(), $"{name}.tplr"));

                // 客户端刷新列表
                SendSSCList(account, from);
                break;
            }
            case ID.SelectSSC:
            {
                var account = b.ReadUInt64();
                var dir = Path.Combine(Main.SavePath, "SSC", account.ToString());
                var name = b.ReadString();

                // 防止名称重复
                if (Netplay.Clients.Any(x => from != x.Id && x.IsActive && Main.player[x.Id].name == account.ToString()))
                {
                    SSCUtils.Boot(from, NetworkText.FromKey(Lang.mp[5].Key, account.ToString()).ToString());
                    return;
                }

                var player = Player.LoadPlayer(Path.Combine(dir, $"{name}.plr"), false).Player;

                // 防止旅程模式误入导致的隐患和崩溃
                if (player.difficulty == 3 && !Main.GameModeInfo.IsJourneyMode)
                {
                    ChatHelper.SendChatMessageToClient(NetworkText.FromKey("Net.PlayerIsCreativeAndWorldIsNotCreative"), Color.Red, from);
                    return;
                }

                if (player.difficulty != 3 && Main.GameModeInfo.IsJourneyMode)
                {
                    ChatHelper.SendChatMessageToClient(NetworkText.FromKey("Net.PlayerIsNotCreativeAndWorldIsCreative"), Color.Red, from);
                    return;
                }

                // 服务端加载SSC玩家
                Main.player[from] = player;

                // 压缩为byte[],客户端同步SSC玩家
                var memoryStream = new MemoryStream();
                var root = new TagCompound
                {
                    { "plr", File.ReadAllBytes(Path.Combine(dir, $"{name}.plr")) },
                    { "tplr", File.ReadAllBytes(Path.Combine(dir, $"{name}.tplr")) },
                };
                TagIO.ToStream(root, memoryStream);
                var data = memoryStream.ToArray();

                var mp = GetPacket(ID.SSCBinary);
                mp.Write(from);
                mp.Write(account);
                mp.Write(data.Length);
                mp.Write(data);
                mp.Send();
                break;
            }
            case ID.SSCBinary:
            {
                switch (Main.netMode)
                {
                    case NetmodeID.MultiplayerClient:
                    {
                        var whoAmI = b.ReadInt32();
                        var account = b.ReadUInt64();
                        var root = TagIO.FromStream(new MemoryStream(b.ReadBytes(b.ReadInt32())));

                        // 解压数据
                        File.WriteAllBytes(Path.Combine(Path.GetTempPath(), $"{account}.plr"), root.GetByteArray("plr"));
                        File.WriteAllBytes(Path.Combine(Path.GetTempPath(), $"{account}.tplr"), root.GetByteArray("tplr"));
                        var data = Player.LoadPlayer(Path.Combine(Path.GetTempPath(), $"{account}.plr"), false);

                        // 应用SSC,并Hook入口代码
                        Main.player[whoAmI] = data.Player;
                        if (whoAmI == Main.myPlayer)
                        {
                            Main.player[whoAmI].Spawn(PlayerSpawnContext.SpawningIntoWorld);
                            Player.Hooks.EnterWorld(whoAmI); // case 12 可能会在第二次加载时报错,看其他开发者的防御性编程能力了

                            UISystem.UI.SetState(null);
                        }

                        break;
                    }
                    case NetmodeID.Server:
                    {
                        var account = b.ReadUInt64();
                        var dir = Path.Combine(Main.SavePath, "SSC", account.ToString());
                        var name = b.ReadString();
                        var root = TagIO.FromStream(new MemoryStream(b.ReadBytes(b.ReadInt32())));

                        if (!File.Exists(Path.Combine(dir, $"{name}.plr")))
                        {
                            ChatHelper.SendChatMessageToClient(NetworkText.FromLiteral("Player does not exist."), Color.Red, from);
                            return;
                        }

                        File.WriteAllBytes(Path.Combine(dir, $"{name}.plr"), root.GetByteArray("plr"));
                        File.WriteAllBytes(Path.Combine(dir, $"{name}.tplr"), root.GetByteArray("tplr"));
                        break;
                    }
                }

                break;
            }
            default:
                SSCUtils.Boot(from, "Packet index out of range.");
                break;
        }
    }

    internal static ModPacket GetPacket(ID id)
    {
        var mp = ModContent.GetInstance<SSC>().GetPacket();
        mp.Write((byte)id);
        return mp;
    }

    private static void SendSSCList(ulong account, int toClient)
    {
        var dir = Path.Combine(Main.SavePath, "SSC", account.ToString());
        var list = Directory.GetFiles(dir, "*.plr").ToList();

        var mp = GetPacket(ID.SSCList);
        mp.Write(list.Count);
        list.ForEach(plr =>
        {
            var data = Player.LoadPlayer(plr, false);
            mp.Write(data.Player.name);
            mp.Write(data.Player.difficulty);
        });
        mp.Send(toClient);
    }

    internal static void SendSaveSSC(ulong account, Player player)
    {
        SSCUtils.InternalSavePlayer(new PlayerFileData(Path.Combine(Path.GetTempPath(), $"{account}.plr"), false)
        {
            Metadata = FileMetadata.FromCurrentSettings(FileType.Player), Player = player
        });

        var memoryStream = new MemoryStream();
        var root = new TagCompound
        {
            { "plr", File.ReadAllBytes(Path.Combine(Path.GetTempPath(), $"{account}.plr")) },
            { "tplr", File.ReadAllBytes(Path.Combine(Path.GetTempPath(), $"{account}.tplr")) },
        };
        TagIO.ToStream(root, memoryStream);
        var data = memoryStream.ToArray();

        var mp = GetPacket(ID.SSCBinary);
        mp.Write(account);
        mp.Write(player.name);
        mp.Write(data.Length);
        mp.Write(data);
        mp.Send();
    }
}