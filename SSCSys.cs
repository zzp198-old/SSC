// using Microsoft.Xna.Framework;
// using Steamworks;
// using Terraria;
// using Terraria.ID;
// using Terraria.ModLoader;
// using Terraria.UI;
//
// namespace SSC;
//
// public class UISystem : ModSystem
// {
//     internal UserInterface UI;
//     internal GameTime GameTime;
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
//         GameTime = time;
//         if (UI?.CurrentState != null)
//         {
//             UI.Update(time);
//         }
//     }
// }