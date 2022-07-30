using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace SSC;

public static class Utils
{
    public static void Boot(int id, string tip)
    {
        switch (Main.netMode)
        {
            case NetmodeID.MultiplayerClient:
                Netplay.Disconnect = true;
                Main.statusText = tip;
                Main.menuMode = MenuID.MultiplayerJoining;
                break;
            case NetmodeID.Server:
                NetMessage.BootPlayer(id, NetworkText.FromLiteral(tip));
                break;
            default:
                throw new Exception("Boot can only be used in C/S");
        }
    }
}