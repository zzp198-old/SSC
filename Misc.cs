using System.IO;
using Terraria;
using Terraria.IO;

namespace SSC;

public static class Misc
{
    public static PlayerFileData NewSSC(string name)
    {
        var data = new PlayerFileData(Path.Combine(Main.PlayerPath, $"{name}.plr"), false)
        {
            Metadata = FileMetadata.FromCurrentSettings(FileType.Player),
            Player = new Player
            {
                name = name,
                savedPerPlayerFieldsThatArentInThePlayerClass = new Player.SavedPlayerDataWithAnnoyingRules()
            }
        };
        data.MarkAsServerSide();
        return data;
    }
}