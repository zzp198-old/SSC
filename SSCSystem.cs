using System.IO;
using Steamworks;
using Terraria;
using Terraria.IO;
using Terraria.ModLoader;

namespace SSC;

public class SSCSystem : ModSystem
{
    public override void Load()
    {
        var dir = Path.Combine(Main.SavePath, "SSC", "Cache");
        Directory.CreateDirectory(dir);
    }

    public override void OnWorldLoad()
    {
        var mp = SSC.GetPacket(SSC.ID.SSCInit);
        mp.Write(Main.myPlayer);
        mp.Write(SSC.SteamID);
        mp.Write(byte.MaxValue);
        mp.Send();
    }
}