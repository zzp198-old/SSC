using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace QOS.Unity.Visual.Configs;

[Label("$Mods.QOS.Common.Configs.VisualConfig.Label")]
public class VisualConfig : ModConfig
{
    public override ConfigScope Mode => ConfigScope.ClientSide;
    public override bool Autoload(ref string name) => true;

    [Label("$Mods.QOS.Common.Configs.VisualConfig.HuaWenYuanTi.Label")]
    [Tooltip("$Mods.QOS.Common.Configs.VisualConfig.HuaWenYuanTi.Tooltip")]
    [DefaultValue(false), ReloadRequired]
    public bool HuaWenYuanTi = false;

    public override void OnLoaded()
    {
        if (HuaWenYuanTi)
        {
            Mod.AddContent<Systems.FontSystem>();
        }
    }
}