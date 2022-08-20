using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace SSC.Core;

public class SyncSystem : ModSystem
{
    internal static TagCompound Database;

    public override void NetSend(BinaryWriter b)
    {
        Database = new TagCompound();
        Directory.GetDirectories(Path.Combine(Main.SavePath, "SSC")).ToList().ForEach(i =>
        {
            var dir = new DirectoryInfo(i);
            var data = new TagCompound();

            dir.GetFiles("*.plr").ToList().ForEach(j =>
            {
                var plr = Player.LoadPlayer(j.FullName, false);
                data.Set("name", plr.Player.name);
                data.Set("mode", plr.Player.difficulty);
            });
            Database.Set(dir.Name, data, true);
        });
        TagIO.Write(Database, b);
    }

    public override void NetReceive(BinaryReader b)
    {
        Database = TagIO.Read(b);
    }
}