using System;
using System.Collections.Generic;
using Terraria.ModLoader.Config;

namespace QOS.Common.Configs;

public class StartItemConfig
{
    [Label("$Mods.QOS.Config.StartItemConfig.StartItems.Label"),
     Tooltip("$Mods.QOS.Config.StartItemConfig.StartItems.Tooltip")]
    [Range(0, 9999)]
    public Dictionary<ItemDefinition, int> StartItems = new();

    [Label("$Mods.QOS.Config.StartItemConfig.RemoveItems.Label"),
     Tooltip("$Mods.QOS.Config.StartItemConfig.RemoveItems.Tooltip")]
    public HashSet<ItemDefinition> RemoveItems = new();
}