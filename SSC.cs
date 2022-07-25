using System;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.IO;
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

            // if (Main.player.Any(i => i.active && i.GetModPlayer<SteamPlayer>().SteamID == id))
            // {
            //     NetMessage.BootPlayer(_, NetworkText.FromLiteral($"SteamID repeat: {id}"));
            //     return;
            // }

            Main.player[_] = new Player
            {
                name = id,
                difficulty = (byte)Main.GameMode,
                dead = true,
                ghost = true,
                savedPerPlayerFieldsThatArentInThePlayerClass = new Player.SavedPlayerDataWithAnnoyingRules()
            };
            Main.player[_].Spawn(PlayerSpawnContext.SpawningIntoWorld);
            Main.player[_].GetModPlayer<SteamPlayer>().SteamID = id;
        }
    }
}

public enum PID : byte
{
    SteamID,
}