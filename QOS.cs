using System.IO;
using Steamworks;
using Terraria;
using Terraria.ModLoader;

namespace QOS;

public class QOS : Mod
{
    internal static QOS Mod;
    internal static string SavePath => Path.Combine(Main.SavePath, nameof(QOS));
    internal static ulong ClientID => SteamUser.GetSteamID().m_SteamID;

    public QOS()
    {
        // Config -> I18N -> Content -> [Gore -> Music -> Background] -> Mod.Load
        ContentAutoloadingEnabled = false; // ModConfig & ILoadable
    }

    public override void Load()
    {
        Mod = this;
        Mod.GetConfig(nameof(Common.Configs.UnityConfig)).OnLoaded();

        if (Main.dedServ)
        {
            Utils.TryCreatingDirectory(SavePath);
        }
    }

    public override void Unload()
    {
        Mod = null;
    }

    public override void HandlePacket(BinaryReader bin, int plr)
    {
    }

    internal enum PID : byte
    {
        CreateSSC,
        RemoveSSC,
        ChooseSSC,
        LoadSSC,
        SaveSSC
    }
}