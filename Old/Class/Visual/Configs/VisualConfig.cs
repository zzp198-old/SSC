using System.ComponentModel;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace QOS.Class.Visual.Configs;

[Label("$Mods.QOS.Configs.VisualConfig.Label")]
public class VisualConfig : ModConfig
{
    // ReSharper disable FieldCanBeMadeReadOnly.Global
    public static VisualConfig Instance = null;

    [Label("$Mods.QOS.Configs.VisualConfig.UltraRapidFire.Label")]
    [Tooltip("$Mods.QOS.Configs.VisualConfig.UltraRapidFire.Tooltip")]
    [BackgroundColor(200, 55, 55, 200)]
    [DefaultValue(false), ReloadRequired]
    public bool UltraRapidFire = false;

    [Label("$Mods.QOS.Configs.VisualConfig.HuaWenYuanTi.Label")]
    [Tooltip("$Mods.QOS.Configs.VisualConfig.HuaWenYuanTi.Tooltip")]
    [BackgroundColor(200, 55, 55, 200)]
    [DefaultValue(false), ReloadRequired]
    public bool HuaWenYuanTi = false;

    public override ConfigScope Mode => ConfigScope.ClientSide;

    public override void OnLoaded()
    {
    
    }
}