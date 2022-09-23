using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace QOS.Content.Items;

public class NPCCensus : ModItem
{
    public override string Texture => $"Terraria/Images/Item_{ItemID.BlueFairyJar}";

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        var spawnRate = typeof(NPC).GetField("spawnRate", (BindingFlags)40)?.GetValue(null);
        var maxSpawns = typeof(NPC).GetField("maxSpawns", (BindingFlags)40)?.GetValue(null);
        tooltips.Add(new TooltipLine(Mod, "SpawnRate", $"Spawn Rate: {spawnRate}"));
        tooltips.Add(new TooltipLine(Mod, "MaxSpawns", $"Max Spawns: {maxSpawns}"));
    }
}