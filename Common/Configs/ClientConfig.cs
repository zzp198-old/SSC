using System.ComponentModel;
using Terraria.ModLoader.Config;

// ReSharper disable UnassignedField.Global
namespace QOS.Common.Configs;

[Label("$Mods.QOS.Config.ClientConfig")]
public class ClientConfig : ModConfig
{
    public override ConfigScope Mode => ConfigScope.ClientSide;

    [Label("$Mods.QOS.Config.Smooth.Label"),
     Tooltip("$Mods.QOS.Config.Smooth.Tooltip")]
    [DefaultValue(false)]
    public bool Smooth;
}