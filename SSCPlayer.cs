using SSC.UI.State;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SSC;

public class SSCPlayer : ModPlayer
{
    public override void OnEnterWorld(Player player)
    {
        // if (Main.netMode == NetmodeID.MultiplayerClient)
        // {
        SSCSystem.UI.SetState(new ListState());
        // }
    }
}