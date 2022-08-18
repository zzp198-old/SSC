using System;
using MonoMod.Cil;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SSC.Common;

public class VerHook : ILoadable
{
    public void Load(Mod mod)
    {
        IL.Terraria.IngameOptions.Draw += Hook;
    }

    public void Unload()
    {
        IL.Terraria.IngameOptions.Draw -= Hook;
    }

    private void Hook(ILContext il)
    {
        var c = new ILCursor(il);
        c.GotoNext(MoveType.After,
            i => i.MatchLdstr("GameUI.SettingsMenu"),
            i => i.MatchCall(typeof(Language), nameof(Language.GetTextValue))
        );
        c.EmitDelegate<Func<string, string>>(i => $"{i} (SSC Ver.{SSC.Mod.Version.ToString()})");
    }
}