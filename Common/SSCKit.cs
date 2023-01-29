using System.IO;
using System.Reflection;
using Microsoft.Xna.Framework;
using Steamworks;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.Utilities;

namespace SSC.Common;

public static class SSCKit
{
    public static string SavePath => Path.Combine(Main.SavePath, nameof(SSC));
    public static ulong ClientID => SteamUser.GetSteamID().m_SteamID;

    public static void BootPlayer(int plr, string msg)
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

    public static string DifficultyTextValue(byte difficulty)
    {
        return Language.GetTextValue(difficulty switch
        {
            0 => "UI.Softcore",
            1 => "UI.Mediumcore",
            2 => "UI.Hardcore",
            3 => "UI.Creative",
            _ => "Unknown"
        });
    }

    public static Color DifficultyTextColor(byte difficulty)
    {
        return difficulty switch
        {
            1 => Main.mcColor,
            2 => Main.hcColor,
            3 => Main.creativeModeColor,
            _ => Color.White
        };
    }

    public static void InternalSavePlayerFile(PlayerFileData data)
    {
        var invoke = typeof(Player).GetMethod("InternalSavePlayerFile", (BindingFlags)40);
        FileUtilities.ProtectedInvoke(() => invoke?.Invoke(null, new object[] { data }));
    }
}