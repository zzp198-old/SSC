using SSC.Common.System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SSC.Common.Player;

public class SSCPlayer : ModPlayer
{
    public override void OnEnterWorld(Terraria.Player _)
    {
        // if (Main.netMode == NetmodeID.MultiplayerClient)
        // {
        var UISystem = ModContent.GetInstance<UISystem>();
        UISystem.UI.SetState(UISystem.UIState);
        // }
    }
}