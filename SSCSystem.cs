using Steamworks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SSC;

public class SSCSystem : ModSystem
{
    public override void OnWorldLoad()
    {
        if (Main.netMode == NetmodeID.MultiplayerClient)
        {
            var p = Mod.GetPacket();
            p.Write((byte)PID.Steam);
            p.Write(Main.myPlayer);
            p.Write(SteamUser.GetSteamID().m_SteamID);
            p.Send();
        }
    }
}