using System;
using System.IO;
using System.Reflection;
using MonoMod.RuntimeDetour.HookGen;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Config.UI;

namespace QOS.Common;

public class QOS : Mod
{
    internal static QOS Mod => ModContent.GetInstance<QOS>();
    internal static Configs.ClientConfig CC => ModContent.GetInstance<Configs.ClientConfig>();
    internal static Configs.ServerConfig SC => ModContent.GetInstance<Configs.ServerConfig>();

    public QOS()
    {
        // var m = typeof(ConfigElement).GetMethod(nameof(ConfigElement.OnBind), (BindingFlags)20);
        // HookEndpointManager.Add(m, (Action<ConfigElement> invoke, ConfigElement element) =>
        // {
        //     var expanded = element.GetType().GetField("expanded", (BindingFlags)36);
        //     if (expanded != null) expanded.SetValue(element, false);
        //     invoke(element);
        // });
    }

    public override void Load()
    {
        if (Main.dedServ)
        {
            Utils.TryCreatingDirectory(QOSKit.SavePath);
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

    public override void HandlePacket(BinaryReader bin, int plr)
    {
        var type = bin.ReadByte();
        switch ((PID)type)
        {
            case PID.CreateSSC:
                Unity.SSC.Systems.NetCodeSystem.CreateSSC(bin, plr);
                break;
            case PID.RemoveSSC:
                Unity.SSC.Systems.NetCodeSystem.RemoveSSC(bin, plr);
                break;
            case PID.ChooseSSC:
                Unity.SSC.Systems.NetCodeSystem.ChooseSSC(bin, plr);
                break;
            case PID.LoadSSC:
                Unity.SSC.Systems.NetCodeSystem.LoadSSC(bin, plr);
                break;
            case PID.SaveSSC:
                Unity.SSC.Systems.NetCodeSystem.SaveSSC(bin, plr);
                break;
            default:
                QOSKit.BootPlayer(plr, "");
                break;
        }
    }
}