using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace SSC;

public static class SSCTools
{
    public static void Boot(int whoAmI, string tip)
    {
        switch (Main.netMode)
        {
            case NetmodeID.MultiplayerClient:
                Netplay.Disconnect = true;
                Main.statusText = tip;
                Main.menuMode = MenuID.MultiplayerJoining;
                break;
            case NetmodeID.Server:
                NetMessage.BootPlayer(whoAmI, NetworkText.FromLiteral(tip));
                break;
            default:
                throw new Exception("Boot can only be used in C/S.");
        }
    }
}