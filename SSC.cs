using System.IO;
using Steamworks;
using Terraria;
using Terraria.ModLoader;

namespace SSC;

public class SSC : Mod
{
    internal static string SavePath => Path.Combine(Main.SavePath, "SSC");
    internal static ulong SteamID => SteamUser.GetSteamID().m_SteamID;

    public override void HandlePacket(BinaryReader b, int from)
    {
        var type = b.ReadByte();
        Logger.Debug($"{Main.myPlayer}({Main.netMode}) receive {(ID)type}({b.BaseStream.Length}) from {from}");

        switch ((ID)type)
        {
            case ID.SSCInit:
            {
                var id = b.ReadUInt64();
                Directory.CreateDirectory(Path.Combine(SavePath, id.ToString()));
                break;
            }
            default:
                SSCUtils.Boot(from, $"Unexpected Package ID: {type}");
                break;
        }
    }

    public enum ID
    {
        SSCInit,
    }
}