using System.Collections.Generic;
using System.Linq;
using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;

namespace QOS.Common.Systems;

public class FlagSystem : ModSystem
{
    internal static List<NPC> ActiveBoss = new();
    internal static bool AnyActiveBoss => ActiveBoss.Count != 0; // AnyActiveBossNPC > AnyDanger > AnyActiveBoss

    public override void Load()
    {
        IL.Terraria.Main.DoUpdateInWorld += IL_Main_DoUpdateInWorld;
    }

    private static void IL_Main_DoUpdateInWorld(ILContext il)
    {
        var ilCursor = new ILCursor(il);
        ilCursor.GotoNext(MoveType.After,
            i => i.MatchLdloc(2),
            i => i.MatchStsfld(typeof(Main.CurrentFrameFlags), nameof(Main.CurrentFrameFlags.AnyActiveBossNPC))
        );
        ilCursor.EmitDelegate(() =>
        {
            ActiveBoss.Clear();
            foreach (var npc in Main.npc.Where(npc => npc.active && npc.boss))
            {
                ActiveBoss.Add(npc);
            }
        });
    }
}