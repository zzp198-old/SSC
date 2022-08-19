using System;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace SSC.Core.Hook;

public class SaveHook : ILoadable
{
    public void Load(Mod mod)
    {
        IL.Terraria.Main.DoUpdate_AutoSave += Hook1;
        IL.Terraria.Player.InternalSavePlayerFile += Hook2;
    }

    public void Unload()
    {
        IL.Terraria.Main.DoUpdate_AutoSave -= Hook1;
        IL.Terraria.Player.InternalSavePlayerFile -= Hook2;
    }

    private static void Hook1(ILContext il)
    {
        var c = new ILCursor(il);
        c.GotoNext(MoveType.After, i => i.MatchLdcI4(300000));
        c.EmitDelegate<Func<long, long>>(_ => 30000);
    }

    private static void Hook2(ILContext il)
    {
        var c = new ILCursor(il);
        c.Emit(OpCodes.Ldarg_0);
        c.EmitDelegate<Action<PlayerFileData>>(data =>
        {
            if (data.ServerSideCharacter && data.Path.EndsWith("SSC"))
            {
                FileUtilities.ProtectedInvoke(() =>
                {
                    // TODO
                });
            }
        });
    }
}