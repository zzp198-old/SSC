using SSC.Component;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SSC
{
    public class SSCPlayer : ModPlayer
    {
        public static BitsByte SSCReady;
        public static int SSCCountdown;

        public override void OnEnterWorld(Player player)
        {
            SSCReady = new BitsByte();
            if (Main.netMode == NetmodeID.SinglePlayer) return;

            SSCStats.Reset();
            SSCStats.Request();
        }

        public override void SendClientChanges(ModPlayer clientPlayer)
        {
            SSCStats.RequestStorage();
        }
    }
}