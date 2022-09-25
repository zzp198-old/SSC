using System.Collections.Generic;
using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;

namespace QOS.Common.Systems;

public class FlagSystem : ModSystem
{
    internal List<NPC> ActiveBoss = new();
    internal bool AnyActiveBoss => ActiveBoss.Count != 0; // AnyActiveBossNPC > AnyDanger > AnyActiveBoss

    public override void Load()
    {
        IL.Terraria.Main.DoUpdateInWorld += IL_DoUpdateInWorld;
    }

    public override void Unload()
    {
        IL.Terraria.Main.DoUpdateInWorld -= IL_DoUpdateInWorld;
    }

    private void IL_DoUpdateInWorld(ILContext il)
    {
        var ilCursor = new ILCursor(il);
        ilCursor.GotoNext(MoveType.After,
            i => i.MatchLdloc(2),
            i => i.MatchStsfld(typeof(Main.CurrentFrameFlags), nameof(Main.CurrentFrameFlags.AnyActiveBossNPC))
        );
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