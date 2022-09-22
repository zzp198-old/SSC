// using Microsoft.Xna.Framework;
// using Terraria;
// using Terraria.ModLoader;
//
// namespace QOS.Common.Systems;
//
// public class VisualSystem : ModSystem
// {
//     public override void Load()
//     {
//         On.Terraria.Dust.GetAlpha += On_Dust_GetAlpha;
//         On.Terraria.Gore.GetAlpha += On_Gore_GetAlpha;
//         On.Terraria.Projectile.GetAlpha += On_Projectile_GetAlpha;
//     }
//
//     public override void Unload()
//     {
//         On.Terraria.Dust.GetAlpha -= On_Dust_GetAlpha;
//         On.Terraria.Gore.GetAlpha -= On_Gore_GetAlpha;
//         On.Terraria.Projectile.GetAlpha -= On_Projectile_GetAlpha;
//     }
//
//     private Color On_Dust_GetAlpha(On.Terraria.Dust.orig_GetAlpha invoke, Dust self, Color newColor)
//     {
//         return invoke(self, newColor) * QOS.CC.VisualConfig.DustTransparency;
//     }
//
//     private Color On_Gore_GetAlpha(On.Terraria.Gore.orig_GetAlpha invoke, Gore self, Color newColor)
//     {
//         return invoke(self, newColor) * QOS.CC.VisualConfig.GoreTransparency;
//     }
//
//     private Color On_Projectile_GetAlpha(On.Terraria.Projectile.orig_GetAlpha invoke, Projectile self, Color newColor)
//     {
//         if (self.friendly && !self.hostile)
//         {
//             if (self.owner == Main.myPlayer)
//             {
//                 return invoke(self, newColor) * QOS.CC.VisualConfig.PlayerProjTransparency;
//             }
//
//             if (self.owner != byte.MaxValue)
//             {
//                 return invoke(self, newColor) * QOS.CC.VisualConfig.TeammateProjTransparency;
//             }
//         }
//
//         return invoke(self, newColor) * QOS.CC.VisualConfig.UnfriendlyProjTransparency;
//     }
// }