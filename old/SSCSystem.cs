// using System.IO;
// using Microsoft.Xna.Framework;
// using Terraria;
// using Terraria.Chat;
// using Terraria.ID;
// using Terraria.IO;
// using Terraria.Localization;
// using Terraria.ModLoader;
//
// namespace SSC;
//
// public class SSCSystem : ModSystem
// {
//     internal static bool Selected;
//     internal static int Cooldown;
//
//     public override void OnWorldLoad()
//     {
//         if (Main.netMode == NetmodeID.MultiplayerClient)
//         {
//             var mp = SSC.GetPacket(SSC.ID.SSCInit);
//             mp.Write(SSC.SteamID);
//             mp.Send();
//
//             UISystem.UI.SetState(new SSCUI());
//             mp = SSC.GetPacket(SSC.ID.SSCList);
//             mp.Write(SSC.SteamID);
//             mp.Send();
//         }
//     }
//
//     public override void OnWorldUnload()
//     {
//         Selected = false;
//     }
//
//     public override void PostUpdatePlayers()
//     {
//         if (Selected)
//         {
//             Cooldown++;
//             if (Cooldown > 3600)
//             {
//                 SSC.SendSaveSSC(SSC.SteamID, Main.LocalPlayer);
//             }
//         }
//     }
// }