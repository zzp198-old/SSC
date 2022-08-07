using Terraria.ModLoader.Config;

namespace SSC;

public class SSCConfig : ModConfig
{
    public override ConfigScope Mode => ConfigScope.ServerSide;

    public bool MultipleOnline;
}