using System.IO;
using Steamworks;
using Terraria;
using Terraria.ModLoader;

namespace QOS;

public class QOS : Mod
{
    internal static string SavePath => Path.Combine(Main.SavePath, nameof(QOS));
    internal static ulong ClientID => SteamUser.GetSteamID().m_SteamID;
    internal static Common.Configs.ClientConfig ClientConfig => ModContent.GetInstance<Common.Configs.ClientConfig>();
    internal static Common.Configs.ServerConfig ServerConfig => ModContent.GetInstance<Common.Configs.ServerConfig>();

    public override void Load()
    {
        Main.runningCollectorsEdition = true;
        if (Main.dedServ)
        {
            Utils.TryCreatingDirectory(SavePath);
        }
    }

    internal enum PID : byte
    {
        CreateSSC,
    }

    public override void HandlePacket(BinaryReader binary, int plr)
    {
        var type = binary.ReadByte();
        switch ((PID)type)
        {
            case PID.CreateSSC:
            {
                Class.SSC.Systems.NetCodeSystem.CreateSSC(binary, plr);
                break;
            }
            default:
                break;
        }
    }
}