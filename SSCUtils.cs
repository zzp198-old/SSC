using System;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace SSC;

public static class SSCUtils
{
    public static void Boot(int plr, string msg)
    {
        switch (Main.netMode)
        {
            case NetmodeID.MultiplayerClient:
                Netplay.Disconnect = true;
                Main.statusText = msg;
                Main.menuMode = MenuID.MultiplayerJoining;
                break;
            case NetmodeID.Server:
                NetMessage.BootPlayer(plr, NetworkText.FromLiteral(msg));
                break;
            default:
                throw new Exception("Boot can only be used in C/S.");
        }
    }

    public static bool CheckName(int plr, string name, out string msg)
    {
        if (Netplay.Clients.Where(x => x.IsActive).Any(x => x.Id != plr && Main.player[x.Id].name == name))
        {
            msg = NetworkText.FromKey(Lang.mp[5].Key, (object)name).ToString();
            return false;
        }

        msg = "";
        return true;
    }
}