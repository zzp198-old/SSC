using System.IO;
using Steamworks;
using Terraria;
using Terraria.ModLoader;

namespace QOS;

public class QOS : Mod
{
    internal static string SavePath => Path.Combine(Main.SavePath, nameof(QOS));
    internal static ulong ClientID => SteamUser.GetSteamID().m_SteamID;

    public override void Load()
    {
        if (Main.dedServ)
        {
            Utils.TryCreatingDirectory(SavePath);
        }

        // Config, I18N, System/Player, Mod, Recipe, Content......
        Common.Configs.ClassConfig.Instance.DynamicModify();
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