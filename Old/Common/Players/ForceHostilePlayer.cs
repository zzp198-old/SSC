// using Terraria;
// using Terraria.ID;
// using Terraria.ModLoader;
//
// namespace QOS.Common.Players;
//
// public class ForceHostilePlayer : ModPlayer
// {
//     public override void ResetEffects()
//     {
//         if (Player.whoAmI != Main.myPlayer || !QOS.SC.ForceHostile)
//         {
//             return;
//         }
//
//         if (Player.hostile)
//         {
//             return;
//         }
//
//         Player.hostile = true;
//         NetMessage.SendData(MessageID.TogglePVP, number: Player.whoAmI);
//     }
// }

