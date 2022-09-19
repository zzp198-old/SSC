using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace QOS.Common.Configs;

public class VisualConfig
{
    [Label("$Mods.QOS.Config.VisualConfig.DustTransparency.Label"),
     Tooltip("$Mods.QOS.Config.VisualConfig.DustTransparency.Tooltip")]
    [Range(0, 1), Increment(0.05f), DefaultValue(1)]
    public float DustTransparency = 1;

    [Label("$Mods.QOS.Config.VisualConfig.GoreTransparency.Label"),
     Tooltip("$Mods.QOS.Config.VisualConfig.GoreTransparency.Tooltip")]
    [Range(0, 1), Increment(0.05f), DefaultValue(1)]
    public float GoreTransparency = 1;

    [Label("$Mods.QOS.Config.VisualConfig.PlayerProjTransparency.Label"),
     Tooltip("$Mods.QOS.Config.VisualConfig.PlayerProjTransparency.Tooltip")]
    [Range(0, 1), Increment(0.05f), DefaultValue(1)]
    public float PlayerProjTransparency = 1;

    [Label("$Mods.QOS.Config.VisualConfig.TeammateProjTransparency.Label"),
     Tooltip("$Mods.QOS.Config.VisualConfig.TeammateProjTransparency.Tooltip")]
    [Range(0, 1), Increment(0.05f), DefaultValue(1)]
    public float TeammateProjTransparency = 1;

    [Label("$Mods.QOS.Config.VisualConfig.UnfriendlyProjTransparency.Label"),
     Tooltip("$Mods.QOS.Config.VisualConfig.UnfriendlyProjTransparency.Tooltip")]
    [Range(0, 1), Increment(0.05f), DefaultValue(1)]
    public float UnfriendlyProjTransparency = 1;
}