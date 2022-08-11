using System.IO;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Utilities;

namespace SSC;

public class MainPlayer : ModPlayer
{
    internal ulong SteamID;
    internal int SaveTime;

    public override void OnEnterWorld(Player player)
    {
        if (Main.netMode == NetmodeID.MultiplayerClient)
        {
            Main.mapStyle = 0;
            MainSystem.UI.SetState(MainSystem.MainLayout);
        }
    }

    public override void PreUpdate()
    {
        if (Player.whoAmI == Main.myPlayer && Main.netMode == NetmodeID.MultiplayerClient)
        {
            if (SteamID != 0)
            {
                SaveTime++;
                if (SaveTime >= ModContent.GetInstance<MainConfig>().SaveTime)
                {
                    SaveTime = 0;

                    var path = Path.Combine(Main.SavePath, "SSC", "Temp", $"{Main.LocalPlayer.name}.plr");
                    var data = new PlayerFileData(path, false)
                    {
                        Metadata = FileMetadata.FromCurrentSettings(FileType.Player),
                        Player = Main.LocalPlayer
                    };
                    FileUtilities.ProtectedInvoke(() => typeof(Player).GetMethod("InternalSavePlayerFile",
                        (BindingFlags)40)?.Invoke(null, new object[] { data }));
                    var compound = new TagCompound();
                    if (File.Exists(path))
                    {
                        compound.Set("Terraria", File.ReadAllBytes(path));
                    }

                    if (File.Exists(Path.ChangeExtension(path, ".tplr")))
                    {
                        compound.Set("tModLoader", File.ReadAllBytes(Path.ChangeExtension(path, ".tplr")));
                    }

                    var memoryStream = new MemoryStream();
                    TagIO.ToStream(compound, memoryStream);
                    var bytes = memoryStream.ToArray();

                    var p = Mod.GetPacket();
                    p.Write((byte)SSC.ID.AskSave);
                    p.Write(SteamID);
                    p.Write(Main.LocalPlayer.name);
                    p.Write(bytes.Length);
                    p.Write(bytes);
                    p.Send();
                }
            }
        }
    }
}