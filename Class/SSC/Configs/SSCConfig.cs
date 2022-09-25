using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace QOS.Class.SSC.Configs;

public class SSCConfig : ModConfig
{
    [Label("DEBUG")] //
    [Tooltip("abab")]
    public bool DEBUG = false;

    [Label("every world")]
    [Tooltip("需要重新加载")] //
    [DefaultValue(false)]
    [ReloadRequired]
    // 避免误操作导致乱档
    public bool EveryWorld = false;

    // TODO
    [Label("StartLife")] //
    [Tooltip("需要重新加载")]
    [SliderColor(0, 255, 0)]
    [Slider]
    [Range(1, 500)]
    [DefaultValue(100)]
    [ReloadRequired]
    // 避免误操作导致乱档
    public int StartLife = 100;

    [Label("StartMana")] //
    [Tooltip("需要重新加载")]
    [SliderColor(0, 0, 255)]
    [Slider]
    [Range(0, 200)]
    [DefaultValue(20)]
    [ReloadRequired]
    // 避免误操作导致乱档
    public int StartMana = 20;

    [SuppressMessage("ReSharper", "UnassignedField.Global")]
    public static SSCConfig Instance => ModContent.GetInstance<SSCConfig>();

    public override ConfigScope Mode => ConfigScope.ServerSide;

    public override bool Autoload(ref string name)
    {
        return true;
    }
}