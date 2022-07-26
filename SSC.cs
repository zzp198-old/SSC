using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Chat;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace SSC;

public class SSC : Mod
{
    public static readonly string SavePath = Path.Combine(Main.SavePath, "SSC");

    public override void Load()
    {
        if (!Directory.Exists(Path.Combine(SavePath, "Client")))
        {
            Directory.CreateDirectory(Path.Combine(SavePath, "Client"));
        }
    }

    public override void Unload()
    {
        if (Directory.Exists(Path.Combine(SavePath, "Client")))
        {
            Directory.Delete(Path.Combine(SavePath, "Client"), true);
        }
    }

    public override void HandlePacket(BinaryReader b, int _)
    {
        var type = (PID)b.ReadByte();
        Logger.Debug($"NetMode: {Main.netMode} id:{Main.myPlayer} receive {type} from {_}");

        switch (type)
        {
            case PID.SteamID:
            {
                var id = b.ReadString();
                if (string.IsNullOrEmpty(id))
                {
                    NetMessage.BootPlayer(_, NetworkText.FromLiteral($"Unexpected SteamID: {id}"));
                }
                else if (Main.player.Any(i => i.active && i.GetModPlayer<SSCPlayer>().SteamID == id))
                {
                    NetMessage.BootPlayer(_, NetworkText.FromLiteral($"SteamID already exists: {id}"));
                }
                else
                {
                    if (!Directory.Exists(Path.Combine(SavePath, "Server", id)))
                    {
                        Directory.CreateDirectory(Path.Combine(SavePath, "Server", id));
                    }

                    Main.player[_].GetModPlayer<SSCPlayer>().SteamID = id;
                }
            }
                break;
            case PID.LoadMap:
            {
                var compound = TagIO.Read(b);

                var dir = Path.Combine(SavePath, "Client", Main.ActivePlayerFileData.Name);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                if (compound.ContainsKey("Terraria"))
                {
                    File.WriteAllBytes(Path.Combine(dir, Main.ActiveWorldFileData.UniqueId + ".map"),
                        compound.GetByteArray("Terraria"));
                }

                if (compound.ContainsKey("tModLoader"))
                {
                    File.WriteAllBytes(Path.Combine(dir, Main.ActiveWorldFileData.UniqueId + ".tmap"),
                        compound.GetByteArray("tModLoader"));
                }

                Main.Map.Load();
            }
                break;
            case PID.SaveMap:
            {
                var id = Main.player[_].GetModPlayer<SSCPlayer>().SteamID;
                if (string.IsNullOrEmpty(id))
                {
                    NetMessage.BootPlayer(_, NetworkText.FromLiteral($"Unexpected SteamID: {id}"));
                    return;
                }

                var compound = TagIO.Read(b);
                if (compound.ContainsKey("Terraria"))
                {
                    File.WriteAllBytes(Path.Combine(SavePath, "Server", id, Main.ActiveWorldFileData.UniqueId + ".map"),
                        compound.GetByteArray("Terraria"));
                }

                if (compound.ContainsKey("tModLoader"))
                {
                    File.WriteAllBytes(
                        Path.Combine(SavePath, "Server", id, Main.ActiveWorldFileData.UniqueId + ".tmap"),
                        compound.GetByteArray("tModLoader"));
                }

                ChatHelper.SendChatMessageToClient(NetworkText.FromLiteral("Map data saved successfully."),
                    Color.Green, _);
            }
                break;
            case PID.LoadSSC:
            {
                var i = b.ReadInt32();
                var compound = (TagCompound)TagIO.ReadTag(b, out var name);
                var plr = Path.Combine(SavePath, "Client", name + ".plr");
                File.WriteAllBytes(plr, compound.GetByteArray("Terraria"));
                if (compound.ContainsKey("tModLoader"))
                {
                    File.WriteAllBytes(Path.ChangeExtension(plr, ".tplr"), compound.GetByteArray("tModLoader"));
                }

                var data = Player.LoadPlayer(plr, false);
                Main.player[i] = data.Player;
                if (i == Main.myPlayer)
                {
                    data.SetAsActive();
                    Main.LocalPlayer.GetModPlayer<SSCPlayer>().State = true;
                }

                Main.player[i].Spawn(PlayerSpawnContext.SpawningIntoWorld);
            }
                break;
            case PID.SaveSSC:
            {
                var compound = (TagCompound)TagIO.ReadTag(b, out var name);
                var id = Main.player[_].GetModPlayer<SSCPlayer>().SteamID;
                if (string.IsNullOrEmpty(id))
                {
                    NetMessage.BootPlayer(_, NetworkText.FromLiteral($"Unexpected SteamID: {id}"));
                    return;
                }

                var plr = Path.Combine(Main.SavePath, "SSC", "Server", id, name + ".plr");
                if (!File.Exists(plr))
                {
                    ChatHelper.SendChatMessageToClient(
                        NetworkText.FromLiteral("Player does not exist, unable to save."),
                        Color.Red, _);
                    return;
                }

                File.WriteAllBytes(plr, compound.GetByteArray("Terraria"));
                if (compound.ContainsKey("tModLoader"))
                {
                    File.WriteAllBytes(Path.ChangeExtension(plr, ".tplr"), compound.GetByteArray("tModLoader"));
                }

                ChatHelper.SendChatMessageToClient(NetworkText.FromLiteral("Player data saved successfully."),
                    Color.Green, _);
            }
                break;
            case PID.KillMeForGood:
            {
                var name = b.ReadString();
                var id = Main.player[_].GetModPlayer<SSCPlayer>().SteamID;
                if (string.IsNullOrEmpty(id))
                {
                    NetMessage.BootPlayer(_, NetworkText.FromLiteral($"Unexpected SteamID: {id}"));
                    return;
                }

                if (File.Exists(Path.Combine(SavePath, "Server", id, name + ".plr")))
                {
                    File.Delete(Path.Combine(SavePath, "Server", id, name + ".plr"));
                }

                if (File.Exists(Path.Combine(SavePath, "Server", id, name + ".tplr")))
                {
                    File.Delete(Path.Combine(SavePath, "Server", id, name + ".tplr"));
                }

                ChatHelper.SendChatMessageToClient(NetworkText.FromLiteral($"Player {name} deleted successfully."),
                    Color.Yellow, _);
            }
                break;
            default:
                NetMessage.BootPlayer(_, NetworkText.FromLiteral($"Unexpected PacketID: {type}"));
                break;
        }
    }
}

public enum PID : byte
{
    SteamID,
    LoadSSC,
    SaveSSC,
    LoadMap,
    SaveMap,
    KillMeForGood,
}