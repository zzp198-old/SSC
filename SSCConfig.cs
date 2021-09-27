using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace SSC
{
    public class SSCConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;
    }
}