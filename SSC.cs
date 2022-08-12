using System;
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

namespace SSC;

public partial class SSC : Mod
{
    public static ulong SteamID => SteamUser.GetSteamID().m_SteamID;

    public override void Load()
    {
        var dir = Path.Combine(Main.SavePath, "SSC", "Temp");
        Directory.CreateDirectory(dir);
    }

    public override void HandlePacket(BinaryReader b, int from)
    {
        var type = b.ReadByte();
        switch ((ID)type)
        {
            case ID.SSCInit:
            {
                // 初始化过程可以通过Hij拦截3,4消息实现,但通过封包的形式实现可以无视后续的更新变动.
                var whoAmI = b.ReadInt32();
                var account = b.ReadUInt64();
                var difficulty = b.ReadByte();
                difficulty = difficulty == byte.MaxValue ? (byte)Main.GameMode : difficulty;

                // 初始化玩家
                Main.player[whoAmI] = new Player
                {
                    name = account.ToString(), difficulty = difficulty, // Spawn时会用到临时Item
                    savedPerPlayerFieldsThatArentInThePlayerClass = new Player.SavedPlayerDataWithAnnoyingRules()
                };
                if (whoAmI == Main.myPlayer)
                {
                    var data = new PlayerFileData { Player = Main.player[whoAmI] };
                    data.MarkAsServerSide();
                    data.SetAsActive();
                }

                if (Main.netMode == NetmodeID.Server)
                {
                    // 基本名称校验
                    if (!SSCUtils.CheckName(account.ToString(), out var msg))
                    {
                        SSCUtils.Boot(whoAmI, msg);
                        return;
                    }

                    // 防止名称重复
                    if (Netplay.Clients.Any(x => whoAmI != x.Id && x.IsActive && Main.player[x.Id].name == account.ToString()))
                    {
                        SSCUtils.Boot(whoAmI, NetworkText.FromKey(Lang.mp[5].Key, account.ToString()).ToString());
                        return;
                    }

                    // 校验通过,创建存储文件夹
                    Directory.CreateDirectory(Path.Combine(Main.SavePath, "SSC", account.ToString()));

                    // 服务端分发,所有客户端同步初始化
                    var mp = GetPacket(ID.SSCInit);
                    mp.Write(whoAmI);
                    mp.Write(account);
                    mp.Write(difficulty);
                    mp.Send(whoAmI);
                    mp.Send();
                }

                break;
            }
            case ID.SSCList:
                switch (Main.netMode)
                {
                    case NetmodeID.Server:
                    {
                        SendSSCList(b.ReadUInt64(), from);
                        break;
                    }
                    case NetmodeID.MultiplayerClient:
                    {
                        var count = b.ReadInt32();
                        for (var i = 0; i < count; i++)
                        {
                            // TODO
                            Console.WriteLine(b.ReadString());
                            Console.WriteLine(b.ReadByte());
                        }

                        break;
                    }
                }

                break;
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
            case ID.SSCBinary:
            {
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
}