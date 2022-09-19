using System.ComponentModel;
using Terraria.Localization;
using Terraria.ModLoader.Config;

namespace QOS.Common.Configs;

public class SSCConfig
{
    [Label("$Mods.QOS.Config.SSCConfig.SSC.Label"),
     Tooltip("$Mods.QOS.Config.SSCConfig.SSC.Tooltip")]
    [BackgroundColor(255, 0, 0)]
    [DefaultValue(false)]
    public bool SSC = false;

    [Label("$Mods.QOS.Config.SSCConfig.CollectorsEdition.Label"),
     Tooltip("$Mods.QOS.Config.SSCConfig.CollectorsEdition.Tooltip")]
    [DefaultValue(false)]
    public bool CollectorsEdition = false;

    [Header("")]
    [Label("$Mods.QOS.Config.SSCConfig.StartLife.Label"),
     Tooltip("$Mods.QOS.Config.SSCConfig.StartLife.Tooltip")]
    [Range(1, 500), DefaultValue(100)]
    public int StartLife = 100;

    [Label("$Mods.QOS.Config.SSCConfig.StartMana.Label"),
     Tooltip("$Mods.QOS.Config.SSCConfig.StartMana.Tooltip")]
    [Range(0, 200), DefaultValue(20)]
    public int StartMana = 20;

    public override string ToString()
    {
        return Language.GetTextValue($"Mods.QOS.{(SSC ? "Enable" : "Disable")}");
    }
}