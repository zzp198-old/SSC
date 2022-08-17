using System;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria.ModLoader;

namespace SSC;

public class SSC : Mod
{
    public override void Load()
    {
        IL.Terraria.NetMessage.greetPlayer += NewGreet;
    }

    private static void NewGreet(ILContext il)
    {
        var c = new ILCursor(il);
        c.Emit(OpCodes.Ldarg_0);
        c.EmitDelegate<RuntimeILReferenceBag.FastDelegateInvokers.Action<int>>(whoAmI =>
        {
            Console.WriteLine(whoAmI);
            Console.WriteLine("Only console");
        });
        c.Emit(OpCodes.Ret);
    }


    public override void Unload()
    {
        IL.Terraria.NetMessage.greetPlayer -= NewGreet;
    }
}