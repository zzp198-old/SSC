using System.IO;
using Steamworks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace QOS.Common;

internal static class QOSKit
{
    internal static string SavePath => Path.Combine(Main.SavePath, nameof(QOS));
    internal static ulong ClientID => SteamUser.GetSteamID().m_SteamID;

    internal static void BootPlayer(int plr, string msg)
    {
        switch (Main.netMode)
        {
            case NetmodeID.MultiplayerClient:
                Netplay.Disconnect = true;
                Main.statusText = msg;
                Main.menuMode = 14;
                break;
            case NetmodeID.Server:
                NetMessage.BootPlayer(plr, NetworkText.FromLiteral(msg));
                break;
        }
    }
}