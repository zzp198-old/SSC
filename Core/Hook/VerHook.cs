using System;
using MonoMod.Cil;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SSC.Core.Hook;

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

    private static void Hook(ILContext il)
    {
        var c = new ILCursor(il);
        c.GotoNext(MoveType.After,
            i => i.MatchLdstr("GameUI.SettingsMenu"),
            i => i.MatchCall(typeof(Language), nameof(Language.GetTextValue))
        );
        c.EmitDelegate<Func<string, string>>(i => $"{i} (SSC Ver.{SSC.Mod.Version.ToString()})");
    }
}