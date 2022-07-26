using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Chat;
using Terraria.Localization;
using Terraria.ModLoader;

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
    }
}

public enum PID : byte
{
    SteamID,
}