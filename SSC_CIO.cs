using System.IO;
using Terraria;
using Terraria.IO;
using Terraria.ModLoader.IO;

namespace SSC;

public class SSC_CIO
{
    TagCompound CopyLP()
    {
        var pd = Main.ActivePlayerFileData;
        var wd = Main.ActiveWorldFileData;
        Player.SavePlayer(pd);

        var c = new TagCompound
        {
            { "plr", File.ReadAllBytes(pd.Path) }
        };
        var tplr = Path.ChangeExtension(pd.Path, ".tplr");
        if (File.Exists(tplr))
        {
            c.Add("tplr", File.ReadAllBytes(tplr));
        }

        var map = Path.Combine(Path.ChangeExtension(pd.Path, "")!, wd.UniqueId + ".map");
        if (File.Exists(map))
        {
            c.Add("map", File.ReadAllBytes(map));
        }

        var tmap = Path.ChangeExtension(map, "tmap");
        if (File.Exists(tmap))
        {
            c.Add("tmap", File.ReadAllBytes(tmap));
        }

        return c;
    }
}