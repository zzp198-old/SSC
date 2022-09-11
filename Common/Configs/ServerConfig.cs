using System.ComponentModel;
using Terraria.ModLoader.Config;

// ReSharper disable UnassignedField.Global
namespace QOS.Common.Configs;

[Label("$Mods.QOS.Config.ServerConfig")]
public class ServerConfig : ModConfig
{
    public override ConfigScope Mode => ConfigScope.ServerSide;

    [Label("$Mods.QOS.Config.SSC.Label"),
     Tooltip("$Mods.QOS.Config.SSC.Tooltip")]
    [DefaultValue(false)]
    [ReloadRequired]
    public bool SSC;

    [Label("$Mods.QOS.Config.ReviveSeal.Label"),
     Tooltip("$Mods.QOS.Config.ReviveSeal.Tooltip")]
    [DefaultValue(false)]
    [ReloadRequired]
    public bool ReviveSeal;
}