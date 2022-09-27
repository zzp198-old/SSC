// using System.Collections.Generic;
// using System.Linq;
// using Terraria;
// using Terraria.ModLoader;
// using Terraria.ModLoader.Config;
//
// namespace QOS.Common.Players;
//
// public class StartItemPlayer : ModPlayer
// {
//     public override IEnumerable<Item> AddStartingItems(bool mediumCoreDeath)
//     {
//         var items = QOS.SC.StartItemConfig.StartItems.Where(item => !item.Key.IsUnloaded && item.Value > 0);
//         return items.Select(item => new Item(item.Key.Type, item.Value));
//     }
//
//     public override void ModifyStartingInventory(IReadOnlyDictionary<string, List<Item>> itemsByMod, bool mediumCoreDeath)
//     {
//         foreach (var items in itemsByMod.Values)
//         {
//             foreach (var item in items.Where(item => QOS.SC.StartItemConfig.RemoveItems.Contains(new ItemDefinition(item.type))))
//             {
//                 item.TurnToAir();
//             }
//         }
//     }
// }

