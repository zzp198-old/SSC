using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Terraria;
using Terraria.ModLoader.Config;

namespace QOS.Class.SSC.Configs;

public class SSCConfig : ModConfig
{
    [SuppressMessage("ReSharper", "UnassignedField.Global")]
    public static SSCConfig Instance;

    public override ConfigScope Mode => ConfigScope.ServerSide;
    public override bool Autoload(ref string name) => false;

    public override void OnLoaded()
    {
        if (Main.dedServ)
        {
            Utils.TryCreatingDirectory(Path.Combine(QOS.SavePath, "SSC"));
        }
    }

    // TODO
    [Label("StartLife")] //
    [Tooltip("需要重新加载")]
    [Slider, SliderColor(0, 255, 0), Range(1, 500), DefaultValue(100)]
    [ReloadRequired]
    // 避免误操作导致乱档
    public int StartLife = 100;

    [Label("StartMana")] //
    [Tooltip("需要重新加载")]
    [Slider, SliderColor(0, 0, 255), Range(0, 200), DefaultValue(20)]
    [ReloadRequired]
    // 避免误操作导致乱档
    public int StartMana = 20;

    [Label("DEBUG")] //
    [Tooltip("abab")]
    public bool DEBUG = false;
}