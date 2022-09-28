using System;
using System.ComponentModel;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.IO;

namespace QOS.Common.Configs;

[Label("$Mods.QOS.Common.Configs.ServerConfig.Label")]
public class ServerConfig : ModConfig
{
    public override ConfigScope Mode => ConfigScope.ClientSide;

    [Label("$Mods.QOS.Common.Configs.ServerConfig.OB.Label")]
    [Tooltip("$Mods.QOS.Common.Configs.ServerConfig.OB.Tooltip")]
    [BackgroundColor(200, 55, 55, 200)]
    [DefaultValue(false), ReloadRequired]
    public bool OB = false;

    [Label("$Mods.QOS.Common.Configs.ServerConfig.ReviveSeal.Label")]
    [Tooltip("$Mods.QOS.Common.Configs.ServerConfig.ReviveSeal.Tooltip")]
    [BackgroundColor(200, 55, 55, 200)]
    [DefaultValue(false), ReloadRequired]
    public bool ReviveSeal = false;

    [Header("$Mods.QOS.Common.Configs.ServerConfig.SSCHeader")]
    [Label("$Mods.QOS.Common.Configs.ServerConfig.SSC.Label")]
    [Tooltip("$Mods.QOS.Common.Configs.ServerConfig.SSC.Tooltip")]
    [BackgroundColor(200, 55, 55, 200)]
    [DefaultValue(false), ReloadRequired]
    public bool SSC = false;

    [Label("$Mods.QOS.Common.Configs.ServerConfig.StartLife.Label")]
    [Tooltip("$Mods.QOS.Common.Configs.ServerConfig.StartLife.Tooltip")]
    [BackgroundColor(200, 55, 55, 200)]
    [Slider, SliderColor(255, 0, 0), Range(1, 500)]
    [DefaultValue(100), ReloadRequired]
    public int StartLife = 100;

    [Label("$Mods.QOS.Common.Configs.ServerConfig.StartMana.Label")]
    [Tooltip("$Mods.QOS.Common.Configs.ServerConfig.StartMana.Tooltip")]
    [BackgroundColor(200, 55, 55, 200)]
    [Slider, SliderColor(0, 0, 255), Range(1, 200)]
    [DefaultValue(20), ReloadRequired]
    public int StartMana = 20;

    public override void OnLoaded()
    {
        ModNet
    }
}