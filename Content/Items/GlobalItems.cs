using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace QOS.Content.Items;

public class GlobalItems : GlobalItem
{
    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
    {
        if (item.ModItem == null)
        {
            tooltips.Add(new TooltipLine(Mod, "Type", $"物品编号: {item.type}"));
        }
        else
        {
            tooltips.Add(new TooltipLine(Mod, "ModName", $"所属模组: {item.ModItem.Mod.Name}"));
            tooltips.Add(new TooltipLine(Mod, "ItemName", $"物品名称: {item.ModItem.Name}"));
        }
    }
}