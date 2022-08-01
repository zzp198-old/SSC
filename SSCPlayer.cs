using System.IO;
using Steamworks;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace SSC;

public class SSCPlayer : ModPlayer
{
    public ulong SteamID;
    public bool Selected;
    public int Countdown;

    public override void OnEnterWorld(Player player)
    {
        if (Main.netMode == NetmodeID.MultiplayerClient)
        {
            SSCSystem.UI.SetState(SSCSystem.SSCState);
            var p = Mod.GetPacket();
            p.Write((byte)PID.PLRList);
            p.Write(SteamUser.GetSteamID().m_SteamID);
            p.Send();
        }
    }

    public override void PreUpdate()
    {
        if (Main.myPlayer != Player.whoAmI || Player.ghost || !Selected) return;

        Countdown++;
        if (Countdown < 600) return;
        Countdown = 0;

        var path = Path.Combine(SSC.SavePath, $"{Main.LocalPlayer.name}.plr");
        var data = new PlayerFileData(path, false)
        {
            Metadata = FileMetadata.FromCurrentSettings(FileType.Player),
            Player = Main.LocalPlayer
        };
        Utils.InternalSavePlayerFile.Invoke(null, new object[] { data });
        var compound = new TagCompound();
        if (File.Exists(path))
        {
            compound.Set("Terraria", File.ReadAllBytes(path));
        }

        if (File.Exists(Path.ChangeExtension(path, ".tplr")))
        {
            compound.Set("tModLoader", File.ReadAllBytes(Path.ChangeExtension(path, ".tplr")));
        }

        var p = Mod.GetPacket();
        p.Write((byte)PID.SavePLR);
        p.Write(SteamUser.GetSteamID().m_SteamID);
        p.Write(Main.LocalPlayer.name);
        TagIO.Write(compound, p);
        p.Send();
    }
}