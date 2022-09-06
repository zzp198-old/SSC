using System.Collections.Generic;
using Terraria.ModLoader.Config;

namespace SSC;

public class SSCSet : ModConfig
{
    public override ConfigScope Mode => ConfigScope.ServerSide;

    public bool NoSpawnWhenBossFight;

    public bool SpectatorMode;

    public List<string> LocalModWhiteList = new()
    {
        "Smoother",
    };

    public override bool AcceptClientChanges(ModConfig pendingConfig, int whoAmI, ref string message) => false;
}