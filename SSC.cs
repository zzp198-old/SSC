using Microsoft.Xna.Framework;
using Steamworks;
using Terraria;
using Terraria.ModLoader;

namespace SSC;

public partial class SSC : Mod
{
    internal static SSC Mod => ModContent.GetInstance<SSC>();
    internal static ulong Sid = SteamUser.GetSteamID().m_SteamID;

    public static Color ChatColor(byte mode)
    {
        return mode switch
        {
            1 => Main.mcColor, 2 => Main.hcColor,
            3 => Main.creativeModeColor, _ => Color.White
        };
    }
}