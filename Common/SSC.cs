using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace SSC.Common;

public partial class SSC : Mod
{
    public SSC()
    {
    }

    public override void Load()
    {
        if (Main.dedServ)
        {
            Utils.TryCreatingDirectory(SSCKit.SavePath);
        }
    }

    public override void HandlePacket(BinaryReader bin, int plr)
    {
        var type = bin.ReadByte();
        switch ((PID)type)
        {
            case PID.CreateSSC:
                Systems.NetworkSystem.CreateSSC(bin, plr);
                break;
            case PID.RemoveSSC:
                Systems.NetworkSystem.RemoveSSC(bin, plr);
                break;
            case PID.ChooseSSC:
                Systems.NetworkSystem.ChooseSSC(bin, plr);
                break;
            case PID.LoadSSC:
                Systems.NetworkSystem.LoadSSC(bin, plr);
                break;
            case PID.SaveSSC:
                Systems.NetworkSystem.SaveSSC(bin, plr);
                break;
            default:
                SSCKit.BootPlayer(plr, $"Unexpected message id:{type}");
                break;
        }
    }

    ~SSC()
    {
    }
}