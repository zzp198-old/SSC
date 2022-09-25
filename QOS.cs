using System;
using System.IO;
using Microsoft.Xna.Framework;
using Steamworks;
using Terraria;
using Terraria.Chat;
using Terraria.Localization;
using Terraria.ModLoader;

namespace QOS;

public class QOS : Mod
{
    internal static QOS Mod => ModContent.GetInstance<QOS>();
    internal static string SavePath => Path.Combine(Main.SavePath, nameof(QOS));
    internal static ulong ClientID => SteamUser.GetSteamID().m_SteamID;

    public override void Load()
    {
        if (Main.dedServ) Utils.TryCreatingDirectory(SavePath);

        // Config, I18N, System/Player, Mod, Recipe, Content......
        Common.Configs.ClassConfig.Instance.DynamicModify();
    }

    public override void HandlePacket(BinaryReader binary, int plr)
    {
        try
        {
            var type = binary.ReadByte();
            switch ((PID)type)
            {
                case PID.CreateSSC:
                    Class.SSC.Systems.NetCodeSystem.CreateSSC(binary, plr);
                    break;
                case PID.RemoveSSC:
                case PID.ChooseSSC:
                case PID.LoadSSC:
                case PID.SaveSSC:
                default:
                    // TODO
                    Common.QOSKit.BootPlayer(plr, "");
                    break;
            }
        }
        catch (Exception e)
        {
            ChatHelper.SendChatMessageToClient(NetworkText.FromLiteral(e.ToString()), Color.Red, plr);
            Console.WriteLine(e);
            throw;
        }
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