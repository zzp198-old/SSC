using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;

namespace QOS.Common.Players;

public class JoinTeamPlayer : ModPlayer
{
    internal bool Joined;

    public override void PostUpdate()
    {
        if (Joined)
        {
            return;
        }

        Joined = true;
        if (Main.netMode != NetmodeID.MultiplayerClient || QOS.CC.JoinTeam == Team.None)
        {
            return;
        }

        Player.team = (byte)QOS.CC.JoinTeam;
        NetMessage.SendData(MessageID.PlayerTeam, number: Player.whoAmI);
    }
}