using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SSC;

public class SSC : Mod
{
    public static readonly string ClientSavePath = Path.Combine(Main.SavePath, "SSC", "Client");
    public static readonly string ServerSavePath = Path.Combine(Main.SavePath, "SSC", "Server");

    public override void HandlePacket(BinaryReader b, int _)
    {
        var type = (PID)b.ReadByte();
        if (type == PID.SteamID)
        {
            var id = b.ReadString();
            if (string.IsNullOrWhiteSpace(id))
            {
                SSCUtils.Kick(_, $"Unexpected SteamID: {id}");
                return;
            }

            Main.player[_].GetModPlayer<SSCPlayer>().SteamID = id;
        }
    }
}

public enum PID : byte
{
    SteamID,
}