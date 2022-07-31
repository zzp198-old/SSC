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
            NETCore.ErasePLR(Main.myPlayer, SteamUser.GetSteamID().m_SteamID.ToString());
        }
    }
}