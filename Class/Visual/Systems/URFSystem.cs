using Microsoft.Xna.Framework;
using On.Terraria;
using QOS.Class.Visual.Configs;
using Terraria.ModLoader;

namespace QOS.Class.Visual.Systems;

public class URFSystem : ModSystem
{
    public override bool IsLoadingEnabled(Mod mod)
    {
        return VisualConfig.Instance.UltraRapidFire;
    }

    public override void Load()
    {
        CombatText.NewText_Rectangle_Color_string_bool_bool += On_NewText;
    }

    public override void Unload()
    {
        CombatText.NewText_Rectangle_Color_string_bool_bool -= On_NewText;
    }

    private static int On_NewText(CombatText.orig_NewText_Rectangle_Color_string_bool_bool invoke, Rectangle location,
        Color color, string text, bool dramatic, bool dot)
    {
        return invoke(location, color, int.TryParse(text, out var num) ? ((long)num * 1000).ToString() : text, dramatic, dot);
    }
}