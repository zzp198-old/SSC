using System;
using System.IO;
using MonoMod.Cil;
using Terraria;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Utilities;

namespace SSC;

public class HookSystem : ModSystem
{
    public override void Load()
    {
        On.Terraria.Main.SelectPlayer += Hook1;
    }

    public override void Unload()
    {
        On.Terraria.Main.SelectPlayer -= Hook1;
    }

    static void Hook1(On.Terraria.Main.orig_SelectPlayer invoke, PlayerFileData data)
    {
        if (Main.menuMultiplayer)
        {
            Console.WriteLine("SSC Hook1.");
            data = new PlayerFileData(Path.Combine(Main.SavePath, "SSC.plr"), false)
            {
                Name = data.Name,
                Metadata = FileMetadata.FromCurrentSettings(FileType.Player),
                Player = new Player
                {
                    name = data.Player.name,
                    difficulty = data.Player.difficulty,
                    savedPerPlayerFieldsThatArentInThePlayerClass = new Player.SavedPlayerDataWithAnnoyingRules(),
                }
            };
        }

        invoke(data);
    }
}