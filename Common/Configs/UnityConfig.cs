using System;
using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace QOS.Common.Configs;

[Label("$Mods.QOS.Common.Configs.UnityConfig.Label")]
public class UnityConfig : ModConfig
{
    public static UnityConfig Instance;
    public override ConfigScope Mode => ConfigScope.ServerSide;
    public override bool Autoload(ref string name) => true;

    [Label("$Mods.QOS.Common.Configs.UnityConfig.SSC.Label")]
    [Tooltip("$Mods.QOS.Common.Configs.UnityConfig.SSC.Tooltip")]
    [DefaultValue(false), ReloadRequired]
    public bool SSC = false;

    public override void OnLoaded()
    {
        if (QOS.Mod == null)
        {
            return; // 延迟加载,避免出现对比乱序
        }

        if (SSC && Unity.SSC.Configs.SSCConfig.Instance == null)
        {
            var type = typeof(Unity.SSC.Configs.SSCConfig);
            Mod.AddConfig(type.Name, (ModConfig)Activator.CreateInstance(type));
        }
    }
}