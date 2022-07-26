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
    public override void HandlePacket(BinaryReader b, int _)
    {
        var type = (PID)b.ReadByte();
        Logger.Debug($"NetMode: {Main.netMode} id:{Main.myPlayer} receive {type} from {_}");
        if (type == PID.SteamID)
        {
            var id = b.ReadString();
            if (string.IsNullOrEmpty(id))
            {
                NetMessage.BootPlayer(_, NetworkText.FromLiteral($"Unexpected SteamID: {id}"));
                return;
            }

            if (Main.player.Any(i => i.active && i.GetModPlayer<SSCPlayer>().SteamID == id))
            {
                NetMessage.BootPlayer(_, NetworkText.FromLiteral($"SteamID already exists: {id}"));
                return;
            }

            Main.player[_].GetModPlayer<SSCPlayer>().SteamID = id;
            ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral($"SteamID {id} login succeeded."), Color.Green);
        }

        if (type == PID.ApplySSC)
        {
            if (!Directory.Exists(Path.Combine(Main.SavePath, "SSC", "Client")))
            {
                Directory.CreateDirectory(Path.Combine(Main.SavePath, "SSC", "Client"));
            }

            var who = b.ReadInt32();
            var compound = (TagCompound)TagIO.ReadTag(b, out var name);
            File.WriteAllBytes(Path.Combine(Main.SavePath, "SSC", "Client", name + ".plr"),
                compound.GetByteArray("Terraria"));
            File.WriteAllBytes(Path.Combine(Main.SavePath, "SSC", "Client", name + ".tplr"),
                compound.GetByteArray("tModLoader"));

            var data = Player.LoadPlayer(Path.Combine(Main.SavePath, "SSC", "Client", name + ".plr"), false);
            Main.player[who] = data.Player;
            if (who == Main.myPlayer)
            {
                data.SetAsActive();
                Main.LocalPlayer.GetModPlayer<SSCPlayer>().Sended = true;
            }

            Main.player[who].Spawn(PlayerSpawnContext.SpawningIntoWorld);
        }

        if (type == PID.SaveSSC)
        {
            var compound = (TagCompound)TagIO.ReadTag(b, out var name);
            var id = Main.player[_].GetModPlayer<SSCPlayer>().SteamID;
            if (string.IsNullOrEmpty(id))
            {
                NetMessage.BootPlayer(_, NetworkText.FromLiteral($"Unexpected SteamID: {id}"));
                return;
            }

            File.WriteAllBytes(Path.Combine(Main.SavePath, "SSC", id, name + ".plr"),
                compound.GetByteArray("Terraria"));
            File.WriteAllBytes(Path.Combine(Main.SavePath, "SSC", id, name + ".tplr"),
                compound.GetByteArray("tModLoader"));

            ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral($"{id} save success."), Color.Green);
        }
    }
}

public enum PID : byte
{
    SteamID,
    ApplySSC,
    SaveSSC,
}