using System.IO;
using Terraria;
using Terraria.IO;
using Terraria.ModLoader;

namespace SSC.Common.Player;

public class SSCPlayer : ModPlayer
{
    public override void OnEnterWorld(Terraria.Player player)
    {
        Directory.Delete(SSC.CachePath, true);
        Directory.CreateDirectory(SSC.CachePath);
        Terraria.Player.SavePlayer(new PlayerFileData(Path.Combine(SSC.CachePath, "Anonymous.plr"), false)
        {
            // this.Name = this.Player.name
            Metadata = FileMetadata.FromCurrentSettings(FileType.Player),
            Player = new Terraria.Player
            {
                name = "Anonymous",
                difficulty = (byte)Main.GameMode // Correspond to each other and can be used directly.
            }
        }, true);
    }
}