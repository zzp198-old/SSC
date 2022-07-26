using System;
using System.IO;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Social;

namespace SSC;

public class SSCPlayer : ModPlayer
{
    public string SteamID;
    public bool Sended;
    public int Cooldown;

    public override void OnEnterWorld(Player player)
    {
        var packet = Mod.GetPacket();
        packet.Write((byte)PID.SteamID);
        packet.Write(SocialAPI.Friends.GetUsername());
        packet.Send();
    }

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
            // TODO
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

    public override void PostUpdate()
    {
        if (Sended && Main.netMode == NetmodeID.MultiplayerClient)
        {
            Cooldown++;
            if (Cooldown > 600)
            {
                typeof(Player).GetMethod("InternalSavePlayerFile", BindingFlags.NonPublic | BindingFlags.Static)
                    ?.Invoke(null, new object[] { Main.ActivePlayerFileData });

                var compound = new TagCompound();

                compound.Set("Terraria", File.ReadAllBytes(Main.ActivePlayerFileData.Path));
                compound.Set("tModLoader",
                    File.ReadAllBytes(Path.ChangeExtension(Main.ActivePlayerFileData.Path, ".tplr")!));


                var packet = Mod.GetPacket();
                packet.Write((byte)PID.SaveSSC);
                TagIO.WriteTag(Main.ActivePlayerFileData.Name, compound, packet);
                packet.Send();

                Cooldown = 0;
            }
        }
    }
}