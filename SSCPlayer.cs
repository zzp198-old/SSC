using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;

namespace SSC;

public class SSCPlayer : ModPlayer
{
    public override void OnEnterWorld(Player player)
    {
        if (Main.netMode == NetmodeID.MultiplayerClient)
        {
            Anonymous();

            Main.NewText("Hey, you are in SSC mode and your character has been erase.");
            Main.NewText("You can't play until you create and select player in UI.");
        }
    }

    static void Anonymous(Player Player)
    {
        Player = new Player
        {
            name = "Anonymous",
            difficulty = (byte)Main.GameMode, // Correspond to each other and can be used directly.
            savedPerPlayerFieldsThatArentInThePlayerClass = new Player.SavedPlayerDataWithAnnoyingRules() // must
        };

        new PlayerFileData
        {
            // this.Name = this.Player.name
            Metadata = FileMetadata.FromCurrentSettings(FileType.Player), // must
        }.SetAsActive(); // If there is no resurrection, the character will be fixed to (0, 0).
    }
}