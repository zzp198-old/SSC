using System;
using MonoMod.Cil;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace QOS.Unity.Visual.Systems;

public class CaiSystem : ModSystem
{
    public override bool IsLoadingEnabled(Mod mod)
    {
        return Common.QOS.CC.Cai;
    }

    public override void Load()
    {
        IL.Terraria.Main.DrawInterface_35_YouDied += IL_Main_DrawInterface_35_YouDied;
    }

    private static void IL_Main_DrawInterface_35_YouDied(ILContext il)
    {
        var ilCursor = new ILCursor(il);
        ilCursor.GotoNext(MoveType.After,
            i => i.MatchLdsfld(typeof(Lang), nameof(Lang.inter)),
            i => i.MatchLdcI4(38),
            i => i.MatchLdelemRef(),
            i => i.MatchCallvirt(typeof(LocalizedText), "get_Value")
        );
        ilCursor.EmitDelegate<Func<string, string>>(_ => "菜");
    }
}