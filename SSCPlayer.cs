using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.Social;

namespace SSC;

public class SSCPlayer : ModPlayer
{
    public string SteamID;

    public override void OnEnterWorld(Player player)
    {
        SteamID = null;


        if (Main.netMode == NetmodeID.MultiplayerClient)
        {
            Main.player[Main.myPlayer] = new Player()
            {
                name = "Anonymous",
                difficulty = (byte)Main.GameMode,
                savedPerPlayerFieldsThatArentInThePlayerClass = new Player.SavedPlayerDataWithAnnoyingRules(),
            };

            var packet = Mod.GetPacket();
            packet.Write((byte)PID.SteamID);
            packet.Write(SocialAPI.Friends.GetUsername());
            packet.Send();
        }
    }
}