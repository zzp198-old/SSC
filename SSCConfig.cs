using System.Collections.Generic;
using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace SSC
{
    public class SSCConfig : ModConfig
    {
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

        [Label("StartingInventory")]
        [Tooltip("StartingInventory")]
        [Range(20, 400)]
        [Slider]
        public List<ItemDefinition> StartingInventory = new();
    }
}