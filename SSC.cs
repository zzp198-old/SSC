using Microsoft.Xna.Framework;
using Steamworks;
using Terraria;
using Terraria.ModLoader;

namespace SSC;

public partial class SSC : Mod
{
    internal static SSC Mod => ModContent.GetInstance<SSC>();
    internal static ulong Sid = SteamUser.GetSteamID().m_SteamID;
}