using System;
using System.IO;
using System.Reflection;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Chat;
using Terraria.ID;
using Terraria.IO;
using Terraria.Map;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Social;

namespace SSC;

public class SSCPlayer : ModPlayer
{
    public string SteamID;

    public bool State;
    public int Cooldown;

    public override void OnEnterWorld(Player player)
    {
        if (Main.netMode == NetmodeID.MultiplayerClient)
        {
            Main.NewTextMultiline(
                "SSC has been enabled and the local player has been erased." +
                "Please synchronize the cloud player through the command.");
            var packet = Mod.GetPacket();
            packet.Write((byte)PID.SteamID);
            packet.Write(SocialAPI.Friends.GetUsername());
            packet.Send();
        }
    }

    public override void PostUpdate()
    {
        Main.ServerSideCharacter = Main.netMode == NetmodeID.MultiplayerClient;

        if (Main.netMode == NetmodeID.MultiplayerClient && State)
        {
            Cooldown++;
            if (Cooldown > 1800)
            {
                typeof(Player).GetMethod("InternalSavePlayerFile", BindingFlags.NonPublic | BindingFlags.Static)
                    ?.Invoke(null, new object[] { Main.ActivePlayerFileData });

                var compound = new TagCompound();
                compound.Set("Terraria", File.ReadAllBytes(Main.ActivePlayerFileData.Path));
                if (File.Exists(Path.ChangeExtension(Main.ActivePlayerFileData.Path, ".tplr")))
                {
                    compound.Set("tModLoader",
                        File.ReadAllBytes(Path.ChangeExtension(Main.ActivePlayerFileData.Path, ".tplr")!));
                }

                var packet = Mod.GetPacket();
                packet.Write((byte)PID.SaveSSC);
                TagIO.WriteTag(Main.ActivePlayerFileData.Name, compound, packet);
                packet.Send();


                // typeof(MapHelper).GetMethod("InternalSaveMap", BindingFlags.NonPublic | BindingFlags.Static)
                //     ?.Invoke(null, Array.Empty<object>());
                Main.Map.Save();
                var map_name = Path.Combine(SSC.SavePath, "Client", Main.ActivePlayerFileData.Name,
                    Main.ActiveWorldFileData.UniqueId + ".map");
                compound = new TagCompound();
                if (File.Exists(map_name))
                {
                    compound.Set("Terraria", File.ReadAllBytes(map_name));
                }

                if (File.Exists(Path.ChangeExtension(map_name, ".tmap")))
                {
                    compound.Set("tModLoader",
                        File.ReadAllBytes(Path.ChangeExtension(map_name, ".tmap")!));
                }

                packet = Mod.GetPacket();
                packet.Write((byte)PID.SaveMap);
                TagIO.Write(compound, packet);
                packet.Send();

                Cooldown = 0;
            }
        }
    }
}