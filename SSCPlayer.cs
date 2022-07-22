using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.Social;

namespace SSC;

public class SSCPlayer : ModPlayer
{
    public bool SSCLogin;

    // Spawn -> Player.Hooks.EnterWorld
    public override void OnEnterWorld(Player player)
    {
        SSCLogin = false;
        // SocialAPI.Friends.GetUsername()
        if (Main.netMode == NetmodeID.MultiplayerClient)
        {
            Anonymous();

            Main.NewText("Hey, you are in SSC mode and your character has been erase.");
            Main.NewText("You can't play until you create and select player in UI.");
        }
    }

    static void Anonymous()
    {
        new PlayerFileData
        {
            // this.Name = this.Player.name
            Metadata = FileMetadata.FromCurrentSettings(FileType.Player), // must
            Player = new Player
            {
                name = "Anonymous",
                difficulty = (byte)Main.GameMode, // Correspond to each other and can be used directly.
                savedPerPlayerFieldsThatArentInThePlayerClass = new Player.SavedPlayerDataWithAnnoyingRules() // must
            },
        }.SetAsActive(); // If there is no resurrection, the character will be fixed to (0, 0).
    }
}