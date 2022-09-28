using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace QOS.Common.Configs;

[Label("$Mods.QOS.Common.Configs.ClientConfig.Label")]
public class ClientConfig : ModConfig
{
    public override ConfigScope Mode => ConfigScope.ClientSide;

    [Header("$Mods.QOS.Common.Configs.ClientConfig.MISCHeader")]
    [Label("$Mods.QOS.Common.Configs.ClientConfig.HWYT.Label")]
    [Tooltip("$Mods.QOS.Common.Configs.ClientConfig.HWYT.Tooltip")]
    [BackgroundColor(200, 55, 55, 200)]
    [DefaultValue(false), ReloadRequired]
    public bool HWYT = false;

    [Label("$Mods.QOS.Common.Configs.ClientConfig.Cai.Label")]
    [Tooltip("$Mods.QOS.Common.Configs.ClientConfig.Cai.Tooltip")]
    [BackgroundColor(200, 55, 55, 200)]
    [DefaultValue(false), ReloadRequired]
    public bool Cai = false;

    [Label("$Mods.QOS.Common.Configs.ClientConfig.URF.Label")]
    [Tooltip("$Mods.QOS.Common.Configs.ClientConfig.URF.Tooltip")]
    [BackgroundColor(200, 55, 55, 200)]
    [DefaultValue(false), ReloadRequired]
    public bool URF = false;

    [Header("$Mods.QOS.Common.Configs.ClientConfig.VisualHeader")]
    [Label("$Mods.QOS.Common.Configs.ClientConfig.DustAlpha.Label")]
    [Tooltip("$Mods.QOS.Common.Configs.ClientConfig.DustAlpha.Tooltip")]
    [Slider, Range(0, 1), Increment(0.05f), DefaultValue(1)]
    public float DustAlpha = 1;

    [Label("$Mods.QOS.Common.Configs.ClientConfig.GoreAlpha.Label")]
    [Tooltip("$Mods.QOS.Common.Configs.ClientConfig.GoreAlpha.Tooltip")]
    [Slider, Range(0, 1), Increment(0.05f), DefaultValue(1)]
    public float GoreAlpha = 1;

    [Label("$Mods.QOS.Common.Configs.ClientConfig.OwnProjectileAlpha.Label")]
    [Tooltip("$Mods.QOS.Common.Configs.ClientConfig.OwnProjectileAlpha.Tooltip")]
    [Slider, Range(0, 1), Increment(0.05f), DefaultValue(1)]
    public float OwnProjectileAlpha = 1;

    [Label("$Mods.QOS.Common.Configs.ClientConfig.FriendlyProjectileAlpha.Label")]
    [Tooltip("$Mods.QOS.Common.Configs.ClientConfig.FriendlyProjectileAlpha.Tooltip")]
    [Slider, Range(0, 1), Increment(0.05f), DefaultValue(1)]
    public float FriendlyProjectileAlpha = 1;

    [Label("$Mods.QOS.Common.Configs.ClientConfig.HostileProjectileAlpha.Label")]
    [Tooltip("$Mods.QOS.Common.Configs.ClientConfig.HostileProjectileAlpha.Tooltip")]
    [Slider, Range(0, 1), Increment(0.05f), DefaultValue(1)]
    public float HostileProjectileAlpha = 1;
}