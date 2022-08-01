using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SSC;

public class SSCPlayer : ModPlayer
{
    public override void OnEnterWorld(Player player)
    {
        if (Main.netMode == NetmodeID.MultiplayerClient)
        {
            try
            {
                SSCSystem.UI.SetState(new SSCState());
            }
            catch (Exception e)
            {
                Utils.Boot(Main.myPlayer, e.ToString());
                throw;
            }
        }
    }
}