using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Terraria.ModLoader.Config;

namespace QOS.Class.SSC.Configs;

public class SSCConfig : ModConfig
{
    [SuppressMessage("ReSharper", "UnassignedField.Global")]
    public static SSCConfig Instance;

    public override ConfigScope Mode => ConfigScope.ServerSide;
    public override bool Autoload(ref string name) => false;

    [Label("every world")] [Tooltip("abab")] [Slider, Range(1, 500), DefaultValue(100)]
    public int StartLife = 100;

    [Label("every world")] [Tooltip("abab")] [Slider, Range(0, 200), DefaultValue(20)]
    public int StartMana = 20;

    [Label("every world")] [Tooltip("abab")] [DefaultValue(false), ReloadRequired]
    public bool EveryWorld = false;
}