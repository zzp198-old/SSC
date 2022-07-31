using System;
using System.IO;
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
            NETCore.CS_ErasePLR(Main.myPlayer, SteamUser.GetSteamID().m_SteamID);
        }
    }
}