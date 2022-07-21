using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;

namespace SSC;

public class SSCPlayer : ModPlayer
{

    public override void OnEnterWorld(Player player)
    {
        Anonymous();
        Main.NewText("Hey, you are in SSC mode and you character will been erase.");
        Main.NewText("You can't play until you create and select player in UI.");

        // var P = ModContent.GetInstance<SSC>().GetPacket();
        // P.Write((int)SSC.PKG_ID.RequestPlayerList);
        // P.Write(Main.clientUUID);
        // P.Send();
    }

    static void Anonymous()
    {
        new PlayerFileData
        {
            // this.Name = this.Player.name
            Metadata = FileMetadata.FromCurrentSettings(FileType.Player), // don't remove
            Player = new Player
            {
                name = "Anonymous",
                difficulty = (byte)Main.GameMode // Correspond to each other and can be used directly.
            }
        }.SetAsActive(); // If there is no resurrection, the character will be fixed to (0, 0).
    }
}