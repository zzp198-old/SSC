// using Terraria;
// using Terraria.Enums;
// using Terraria.ID;
// using Terraria.ModLoader;
//
// namespace QOS.Common.Players;
//
// public class AutoJoinTeamPlayer : ModPlayer
// {
//     public bool Joined;
//
//     public override void PreUpdate()
//     {
//         if (Player.whoAmI != Main.myPlayer || Joined)
//         {
//             return;
//         }
//
//         Joined = true;
//         if (Main.netMode != NetmodeID.MultiplayerClient || QOS.SC.AutoJoinTeam == Team.None)
//         {
//             return;
//         }
//
//         Player.team = (byte)QOS.SC.AutoJoinTeam;
//         NetMessage.SendData(MessageID.PlayerTeam, number: Player.whoAmI);
//     }
// }

