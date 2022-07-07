using System.Collections.Generic;
using System.ComponentModel;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace ServerMod
{
    public class SSCConfig : ModConfig
    {
        [Label("StartingInventory")] public List<ItemDefinition> StartingInventory = new()
        {
            new ItemDefinition(3507),
            new ItemDefinition(3509),
            new ItemDefinition(3506),
        };

        public override ConfigScope Mode => ConfigScope.ServerSide;

        [Label("StartingLife")]
        [Tooltip("StartingLife")]
        [Range(20, 400)]
        [Increment(20)]
        [Slider]
        [DefaultValue(100)]
        public int StartingLife { get; set; }

        [Label("StartingMana")]
        [Tooltip("StartingMana")]
        [Range(0, 200)]
        [Increment(20)]
        [Slider]
        [DefaultValue(20)]
        public int StartingMana { get; set; }
    }
}