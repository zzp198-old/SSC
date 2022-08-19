using Steamworks;
using Terraria.ModLoader;

namespace SSC;

public partial class SSC : Mod
{
    internal static SSC Mod => ModContent.GetInstance<SSC>();
    internal static ulong SID = SteamUser.GetSteamID().m_SteamID;
}