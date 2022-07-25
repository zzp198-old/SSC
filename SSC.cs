using System.Data.SQLite;
using System.IO;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SSC;

public class SSC : Mod
{
    public static SQLiteConnection DB;

    public override void Load()
    {
        if (Main.dedServ)
        {
            DB = new SQLiteConnection($"Data Source={Path.Combine(Main.SavePath, "SSC.db")};Pooling=true;");
        }
    }

    public override void Unload()
    {
        DB?.Close();
    }

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
            Main.player[_].GetModPlayer<SteamPlayer>().SteamID = id;
        }
    }
}

public enum PID : byte
{
    SteamID,
}