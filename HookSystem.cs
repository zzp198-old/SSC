using System;
using System.IO;
using Terraria;
using Terraria.IO;
using Terraria.ModLoader;

namespace SSC;

public class HookSystem : ModSystem
{
    public override void Load()
    {
        On.Terraria.Main.SelectPlayer += Hook1;
        On.Terraria.Player.KillMeForGood += Hook2;
    }

    public override void Unload()
    {
        On.Terraria.Main.SelectPlayer -= Hook1;
        On.Terraria.Player.KillMeForGood -= Hook2;
    }


    static void Hook1(On.Terraria.Main.orig_SelectPlayer invoke, PlayerFileData data)
    {
        if (Main.menuMultiplayer)
        {
            Console.WriteLine("SSC Hook1.");
            data = new PlayerFileData(Path.Combine(SSC.SavePath, "Client", "Anonymous.plr"), false)
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

    private void Hook2(On.Terraria.Player.orig_KillMeForGood invoke, Player self)
    {
        if (self.whoAmI == Main.myPlayer)
        {
            var packet = Mod.GetPacket();
            packet.Write((byte)PID.KillMeForGood);
            packet.Write(Main.LocalPlayer.name);
            packet.Send();
        }

        invoke(self);
    }
}