using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace QOS.Unity.Visual.Systems;

public class URFSystem : ModSystem
{
    public override bool IsLoadingEnabled(Mod mod)
    {
        return Common.QOS.CC.URF;
    }

    public override void Load()
    {
        On.Terraria.CombatText.NewText_Rectangle_Color_string_bool_bool += On_CombatText_NewText;
    }

    private static int On_CombatText_NewText(On.Terraria.CombatText.orig_NewText_Rectangle_Color_string_bool_bool invoke,
        Rectangle location, Color color, string text, bool dramatic, bool dot)
    {
        if (int.TryParse(text, out var num))
        {
            text = ((long)num * 1000).ToString();
        }

        return invoke(location, color, text, dramatic, dot);
    }
}