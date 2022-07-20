using Terraria;
using Terraria.IO;
using Terraria.ModLoader;

namespace SSC;

public class SSCPlayer : ModPlayer
{
    public override void OnEnterWorld(Player player)
    {
        // Reset LP
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

        var P = ModContent.GetInstance<SSC>().GetPacket();
        P.Write((int)SSC.PKG_ID.RequestPlayerList);
        P.Write(Main.clientUUID);
        P.Send();
    }
}