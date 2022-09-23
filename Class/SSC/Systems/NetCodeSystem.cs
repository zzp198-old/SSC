using System.IO;
using Terraria.ModLoader;

namespace QOS.Class.SSC.Systems;

public class NetCodeSystem : ModSystem
{
    internal static bool SyncSCC;

    public static void CreateSSC(BinaryReader binary, int plr)
    {
    }

    public override void NetSend(BinaryWriter binary)
    {
        binary.Write(SyncSCC);
        if (!SyncSCC)
        {
            return;
        }


        SyncSCC = false;
    }

    public override void NetReceive(BinaryReader binary)
    {
        if (!binary.ReadBoolean())
        {
            return;
        }
    }
}