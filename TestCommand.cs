using Steamworks;
using Terraria.ModLoader;

namespace SSC;

public class TestCommand : ModCommand
{
    public override string Command => "test";
    public override CommandType Type => CommandType.Chat;

    public override void Action(CommandCaller caller, string input, string[] args)
    {
        NETCore.ObtainPLR(SteamUser.GetSteamID().m_SteamID, "");
    }
}