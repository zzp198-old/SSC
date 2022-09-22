// using System;
// using System.Linq;
// using Microsoft.Xna.Framework;
// using MonoMod.Cil;
// using Terraria;
// using Terraria.Localization;
// using Terraria.ModLoader;
// using Player = On.Terraria.Player;
//
// namespace QOS.Common.Players;
//
// public class ObservePlayer : ModPlayer
// {
//     internal const int CountdownMax = 150;
//     internal static int Countdown;
//     internal static float ShowCountdown;
//     internal static bool ObType;
//     internal static int ObIndex;
//
//     public override bool IsLoadingEnabled(Mod mod)
//     {
//         return QOS.SC.Observer;
//     }
//
//     public override void Load()
//     {
//         IL.Terraria.Main.DrawInterface_35_YouDied += IL_Main_DrawInterface_35_YouDied; // 字体位置下调
//         On.Terraria.Player.GetDeathAlpha += On_Player_GetDeathAlpha; // 隐藏字体
//     }
//
//     public override void Unload()
//     {
//         IL.Terraria.Main.DrawInterface_35_YouDied -= IL_Main_DrawInterface_35_YouDied;
//         On.Terraria.Player.GetDeathAlpha -= On_Player_GetDeathAlpha;
//     }
//
//     public override void ResetEffects()
//     {
//         if (Player.whoAmI != Main.myPlayer)
//         {
//             return;
//         }
//
//         Countdown = 0;
//         ShowCountdown = 0;
//     }
//
//     public override void UpdateDead()
//     {
//         if (Player.whoAmI != Main.myPlayer)
//         {
//             return;
//         }
//
//         if (Countdown < CountdownMax)
//         {
//             Countdown++;
//             return;
//         }
//
//         if (QOS.SC.ReviveSeal && Player.GetModPlayer<ReviveSealPlayer>().Sealers.Count > 0)
//         {
//             ShowCountdown = Utils.Clamp(ShowCountdown + 1, 0, 100);
//         }
//         else
//         {
//             ShowCountdown = Utils.Clamp(ShowCountdown - 1, 0, 100);
//         }
//
//         var npcList = Main.npc.Where(npc => npc.active && npc.boss).ToList();
//         if (npcList.Count == 0)
//         {
//             return;
//         }
//
//         if (Main.mouseLeft && Main.mouseLeftRelease)
//         {
//             ObIndex++;
//         }
//
//         if (Main.mouseRight && Main.mouseRightRelease)
//         {
//             ObType = !ObType;
//         }
//
//         if (ObIndex >= npcList.Count)
//         {
//             ObIndex = 0;
//         }
//
//         var npc = npcList[ObIndex]; // 筛选出要观战的BOSS
//         var targetCenter = npc.Center;
//
//         if (ObType && npc.HasValidTarget) // 选择观战Boss的目标玩家
//         {
//             targetCenter = npc.GetTargetData().Center;
//         }
//
//         var coordinate = targetCenter.ToTileCoordinates();
//         if (!WorldGen.InWorld(coordinate.X, coordinate.Y))
//         {
//             return;
//         }
//
//         if (Vector2.Distance(Player.Center, targetCenter) < 10)
//         {
//             targetCenter = Player.Center;
//         }
//         else if (Vector2.Distance(Player.Center, targetCenter) < 100) // 平滑处理
//         {
//             targetCenter = Vector2.Lerp(Player.Center, targetCenter, 0.05f);
//         }
//         else if (Vector2.Distance(Player.Center, targetCenter) < Main.ScreenSize.ToVector2().Length())
//         {
//             targetCenter = Vector2.Lerp(Player.Center, targetCenter, 0.1f);
//         }
//
//         Player.Center = targetCenter;
//     }
//
//     private void IL_Main_DrawInterface_35_YouDied(ILContext il)
//     {
//         var c = new ILCursor(il);
//         c.GotoNext(MoveType.After, i => i.MatchLdcR4(-60));
//         c.EmitDelegate<Func<float, float>>(_ => 60);
//     }
//
//     private Color On_Player_GetDeathAlpha(Player.orig_GetDeathAlpha invoke, Terraria.Player self, Color newColor)
//     {
//         return invoke(self, newColor) * (1 - ShowCountdown / 100);
//     }
// }