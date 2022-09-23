using System;
using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace QOS.Common.Configs;

public class ClientConfig : ModConfig
{
    public override ConfigScope Mode => ConfigScope.ClientSide;

    internal float DustTransparency;

    [Slider, Range(0, 1), Increment(0.05f), DefaultValue(1)]
    public float DustTransparencyProperty
    {
        get => DustTransparency;
        set => DustTransparency = MathF.Round(value, 2, MidpointRounding.AwayFromZero);
    }

    internal float GoreTransparency;

    [Slider, Range(0, 1), Increment(0.05f), DefaultValue(1)]
    public float GoreTransparencyProperty
    {
        get => GoreTransparency;
        set => GoreTransparency = MathF.Round(value, 2, MidpointRounding.AwayFromZero);
    }

    internal float ProjectileTransparency;

    [Slider, Range(0, 1), Increment(0.05f), DefaultValue(1)]
    public float ProjectileTransparencyProperty
    {
        get => ProjectileTransparency;
        set => ProjectileTransparency = MathF.Round(value, 2, MidpointRounding.AwayFromZero);
    }
}