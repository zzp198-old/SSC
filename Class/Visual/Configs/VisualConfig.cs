using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Terraria.ModLoader.Config;

namespace QOS.Class.Visual.Configs;

public class VisualConfig : ModConfig
{
    [SuppressMessage("ReSharper", "UnassignedField.Global")]
    public static VisualConfig Instance;

    [Label("华文圆体")] [Tooltip("123")] [DefaultValue(false)] [ReloadRequired]
    public bool HuaWenYuanTi = false;

    [Label("无限火力")] [Tooltip("123")] [DefaultValue(false)] [ReloadRequired]
    public bool UltraRapidFire = false;

    public override ConfigScope Mode => ConfigScope.ClientSide;
}