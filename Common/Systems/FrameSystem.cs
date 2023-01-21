using System.Collections.Generic;
using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;

namespace SSC.Common.Systems;

public class FrameSystem : ModSystem
{
    public readonly static List<NPC> ActiveBoss = new();
    public static bool AnyActiveBoss => ActiveBoss.Count != 0; // AnyActiveBossNPC > AnyDanger > AnyActiveBoss

    public override void Load()
    {
        IL.Terraria.Main.DoUpdateInWorld += Hook1;
    }

    private static void Hook1(ILContext il)
    {
        var ilCursor = new ILCursor(il);
        ilCursor.GotoNext(i => i.MatchStsfld(typeof(Main.CurrentFrameFlags), nameof(Main.CurrentFrameFlags.AnyActiveBossNPC)));
        ilCursor.EmitDelegate(() =>
        {
            ActiveBoss.Clear();
            foreach (var npc in Main.npc)
            {
                if (npc.active && npc.boss)
                {
                    ActiveBoss.Add(npc);
                }
            }
        });
    }
}