using Terraria.ModLoader.Config;

namespace SSC;

public class MainConfig : ModConfig
{
    public override ConfigScope Mode => ConfigScope.ServerSide;

    public bool CollectorsEdition = true;

    public bool RepeatConnect = false;

    public int SaveTime = 600;
}