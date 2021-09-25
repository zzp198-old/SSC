using System.Collections.Generic;
using Terraria.ModLoader.Config;

namespace SSC
{
    public class SSCConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        public int StartingLife = 100;
        public int StartingMana = 20;
        public List<ItemDefinition> StartingInventory = new List<ItemDefinition>();
    }
}