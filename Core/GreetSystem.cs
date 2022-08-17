using System;
using System.Collections.Generic;
using log4net.Repository.Hierarchy;
using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.Chat;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SSC.Core;

public class GreetSystem : ModSystem
{
    public override void Load()
    {
        IL.Terraria.NetMessage.greetPlayer += NewGreet;
    }

    private void NewGreet(ILContext il)
    {
        var c = new ILCursor(il);
        c.Emit(OpCodes.Ldarg_0);
        c.EmitDelegate<Action<int>>(whoAmI =>
        {
            Mod.Logger.Debug("New Greet Delegate");

            ChatHelper.SendChatMessageToClient(NetworkText.FromLiteral(Main.motd switch
            {
                "" => "Welcome server by TML with SSC",
                _ => Main.motd
            }), new Color(byte.MaxValue, 240, 20), whoAmI);

            var names = new List<string>();
            for (var i = 0; i < byte.MaxValue; i++)
            {
                if (!Main.player[i].active) continue;
                names.Add(Main.player[i].name);
            }

            ChatHelper.SendChatMessageToClient(NetworkText.FromKey(
                "Game.JoinGreeting", string.Join(", ", names)
            ), new Color(byte.MaxValue, 240, 20), whoAmI);
        });
        c.Emit(OpCodes.Ret);
    }

    public override void Unload()
    {
        IL.Terraria.NetMessage.greetPlayer -= NewGreet;
    }
}