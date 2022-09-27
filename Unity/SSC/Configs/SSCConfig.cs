using Terraria.ModLoader.Config;

namespace QOS.Unity.SSC.Configs;

public class SSCConfig : ModConfig
{
    public static SSCConfig Instance;
    public override ConfigScope Mode => ConfigScope.ServerSide;

    public override void OnLoaded()
    {
        Mod.AddContent<Systems.HookSystem>();
    }
}