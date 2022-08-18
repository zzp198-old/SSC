using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.Chat;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SSC.Common;

public class GreetHook : ILoadable
{
    public void Load(Mod mod) => IL.Terraria.NetMessage.greetPlayer += Hook;
    public void Unload() => IL.Terraria.NetMessage.greetPlayer -= Hook;

    private static void Hook(ILContext il)
    {
        var c = new ILCursor(il);
        c.Emit(OpCodes.Ldarg_0);
        c.EmitDelegate<Action<int>>(whoAmI =>
        {
            ChatHelper.SendChatMessageToClient(NetworkText.FromLiteral(Main.motd switch
            {
                "" => $"Welcome to {Main.worldName} with SSC",
                _ => Main.motd
            }), new Color(byte.MaxValue, 240, 20), whoAmI);

            ChatHelper.SendChatMessageToClient(NetworkText.FromKey(
                "Game.JoinGreeting", string.Join(", ", from i in Main.player.Where(i => i.active) select i.name)
            ), new Color(byte.MaxValue, 240, 20), whoAmI);
        });
        c.Emit(OpCodes.Ret);
    }
}