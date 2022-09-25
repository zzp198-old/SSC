using System.IO;
using Terraria.ModLoader;

namespace QOS.Class.SSC.Systems;

public class NetCodeSystem : ModSystem
{
    public override bool IsLoadingEnabled(Mod mod)
    {
        return Configs.SSCConfig.Instance != null;
    }

    public static void CreateSSC(BinaryReader binary, int plr)
    {
    }

    public override void NetSend(BinaryWriter binary)
    {
        
    }

    public override void NetReceive(BinaryReader binary)
    {
    }
}