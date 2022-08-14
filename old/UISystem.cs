// using System.Collections.Generic;
// using Microsoft.Xna.Framework;
// using Terraria;
// using Terraria.ModLoader;
// using Terraria.UI;
//
// namespace SSC;
//
// public class UISystem : ModSystem
// {
//     internal static UserInterface UI;
//     internal static GameTime Time;
//
//     public override void Load()
//     {
//         if (!Main.dedServ)
//         {
//             UI = new UserInterface();
//         }
//     }
//
//     public override void Unload()
//     {
//         UI = null;
//     }
//
//     public override void UpdateUI(GameTime time)
//     {
//         Time = time;
//         if (UI?.CurrentState != null)
//         {
//             UI.Update(time);
//         }
//     }
//
//     public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
//     {
//         var index = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Cursor"));
//         if (index != -1)
//         {
//             layers.Insert(index, new LegacyGameInterfaceLayer("SSC: UISystem", () =>
//             {
//                 if (Time != null && UI?.CurrentState != null)
//                 {
//                     UI.Draw(Main.spriteBatch, Time);
//                 }
//
//                 return true;
//             }, InterfaceScaleType.UI));
//         }
//     }
// }