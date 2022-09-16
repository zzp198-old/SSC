using QOS.Common.Configs;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace QOS.Common.Players;

public class ForcePVPPlayer : ModPlayer
{
    public override void PostUpdate()
    {
        if (Player.whoAmI != Main.myPlayer || QOS.SC.ForcePVP == ServerConfig.EForcePVP.Normal)
        {
            return;
        }

        switch (Player.hostile)
        {
            case true when QOS.SC.ForcePVP == ServerConfig.EForcePVP.Off:
                Player.hostile = false;
                NetMessage.SendData(MessageID.TogglePVP, number: Player.whoAmI);
                break;
            case false when QOS.SC.ForcePVP == ServerConfig.EForcePVP.On:
                Player.hostile = true;
                NetMessage.SendData(MessageID.TogglePVP, number: Player.whoAmI);
                break;
        }
    }
}