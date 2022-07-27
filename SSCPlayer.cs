using System.IO;
using System.Reflection;
using Steamworks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace SSC;

public class SSCPlayer : ModPlayer
{
    public string SteamID;

    public bool State;
    public int Cooldown;

    public override void OnEnterWorld(Player player)
    {
        if (Main.netMode == NetmodeID.MultiplayerClient && !State)
        {
            var packet = Mod.GetPacket();
            packet.Write((byte)PID.SteamID);
            packet.Write(SteamUser.GetSteamID().ToString());
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
                Cooldown = 0;

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
            }
        }
    }
}