using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SSC.Common.Systems;

public class SteamSystem : ModSystem
{
    public override void OnWorldLoad()
    {
        if (Main.netMode == NetmodeID.MultiplayerClient)
        {
            var p = Mod.GetPacket();
            

        }
    }
}