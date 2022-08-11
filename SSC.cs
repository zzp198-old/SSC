using System.IO;
using System.Linq;
using Steamworks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;

namespace SSC;

public partial class SSC : Mod
{
    public static ulong SteamID => SteamUser.GetSteamID().m_SteamID;

    public override void HandlePacket(BinaryReader b, int _)
    {
        var type = b.ReadByte();
        switch ((ID)type)
        {
            case ID.SSCInit:
            {
                // 初始化过程可以通过Hij拦截3,4消息实现,但通过封包的形式实现可以无视后续的更新变动.
                var i = b.ReadInt32();
                var id = b.ReadUInt64();
                var mode = b.ReadByte();
                mode = mode == byte.MaxValue ? (byte)Main.GameMode : mode;

                // 初始化玩家
                Main.player[i] = new Player
                {
                    name = id.ToString(), difficulty = mode, // Spawn时会用到Item4.
                    savedPerPlayerFieldsThatArentInThePlayerClass = new Player.SavedPlayerDataWithAnnoyingRules()
                };
                if (i == Main.myPlayer)
                {
                    var data = new PlayerFileData { Player = Main.player[i] };
                    data.MarkAsServerSide();
                    data.SetAsActive();
                }

                if (Main.netMode == NetmodeID.Server)
                {
                    // 名称校验
                    if (!SSCUtils.CheckName(i, id.ToString(), out var msg))
                    {
                        SSCUtils.Boot(i, msg);
                        return;
                    }

                    // 服务端分发,所有客户端同步初始化
                    var mp = GetPacket(ID.SSCInit);
                    mp.Write(i);
                    mp.Write(id);
                    mp.Write(mode);
                    mp.Send(i);
                    mp.Send();
                }

                break;
            }
            default:
                SSCUtils.Boot(_, "Packet index out of range.");
                break;
        }
    }

    public static ModPacket GetPacket(ID id)
    {
        var mp = ModContent.GetInstance<SSC>().GetPacket();
        mp.Write((byte)id);
        return mp;
    }
}