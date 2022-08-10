using System.Collections.Generic;
using Terraria.ModLoader.Config;

namespace SSC;

public class SSCConfig : ModConfig
{
    public override ConfigScope Mode => ConfigScope.ServerSide;

    public bool RepeatConnect = false;

    public int SaveTime = 600;

    public bool Compress = true;
}