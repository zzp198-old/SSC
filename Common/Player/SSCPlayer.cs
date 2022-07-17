using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace SSC.Common.Player;

public class SSCPlayer : ModPlayer
{
    public override void OnEnterWorld(Terraria.Player player)
    {
        if (Main.netMode == NetmodeID.MultiplayerClient)
        {
            var dir = Path.Combine(SSC.SavePath, "Cache");
            Directory.CreateDirectory(dir);
            File.WriteAllBytes(Path.Combine(dir, "Guest.plr"), ModContent.GetFileBytes("SSC/Guest.plr"));
            File.WriteAllBytes(Path.Combine(dir, "Guest.tplr"), ModContent.GetFileBytes("SSC/Guest.tplr"));

            Terraria.Player.LoadPlayer(Path.Combine(dir, "Guest.plr"), false).SetAsActive();
            Main.LocalPlayer.Spawn(PlayerSpawnContext.SpawningIntoWorld);
            Main.LocalPlayer.KillMe(PlayerDeathReason.LegacyEmpty(), 0, 0);

            var packet = Mod.GetPacket();
            packet.Write((int)PacketID.UUID);
            packet.Write(Main.clientUUID);
            packet.Send();
        }


        // if (this.whoAmI != Main.myPlayer)
        // {
        //     int num1 = (int) ((double) this.position.X + (double) (this.width / 2)) / 16;
        //     int num2 = (int) ((double) this.position.Y + (double) (this.height / 2)) / 16;
        //     if (Main.netMode == 1 && !Main.sectionManager.TilesLoaded(num1 - 3, num2 - 3, num1 + 3, num2 + 3))
        //         flag1 = true;
        //     if (flag1)
        //     {
        //         this.outOfRange = true;
        //         this.numMinions = 0;
        //         this.slotsMinions = 0.0f;
        //         this.itemAnimation = 0;
        //         this.UpdateBuffs(i);
        //         this.PlayerFrame();
        //     }
        // }
    }
}