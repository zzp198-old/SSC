using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Terraria.ModLoader.Config;

namespace QOS.Class.Visual.Configs;

public class VisualConfig : ModConfig
{
    [SuppressMessage("ReSharper", "UnassignedField.Global")]
    public static VisualConfig Instance;

    public override ConfigScope Mode => ConfigScope.ClientSide;

    [Label("无限火力")] [Tooltip("123")] [DefaultValue(false)] [ReloadRequired]
    public bool UltraRapidFire = false;

    [Label("华文圆体")] [Tooltip("123")] [DefaultValue(false)] [ReloadRequired]
    public bool HuaWenYuanTi = false;
}