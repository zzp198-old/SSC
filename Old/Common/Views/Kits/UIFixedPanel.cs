// using Microsoft.Xna.Framework;
// using Terraria;
// using Terraria.GameContent.UI.Elements;
// using Terraria.UI;
//
// namespace QOS.Common.Views.Kits;
//
// public class UIFixedPanel : UIPanel
// {
//     private bool _move;
//     private Vector2 _moveVec2;
//
//     public override void MouseDown(UIMouseEvent evt)
//     {
//         _moveVec2 = new Vector2(evt.MousePosition.X - Left.Pixels, evt.MousePosition.Y - Top.Pixels);
//         _move = true;
//     }
//
//     public override void MouseUp(UIMouseEvent evt)
//     {
//         _move = false;
//     }
//
//     public override void Update(GameTime gameTime)
//     {
//         if (ContainsPoint(Main.MouseScreen))
//         {
//             Main.LocalPlayer.mouseInterface = true;
//         }
//
//         if (_move)
//         {
//             Left.Set(Main.mouseX - _moveVec2.X, 0f);
//             Top.Set(Main.mouseY - _moveVec2.Y, 0f);
//             Recalculate();
//         }
//
//         var pa = Parent.GetViewCullingArea();
//         var a = GetViewCullingArea();
//         if (!pa.Contains(a))
//         {
//             if (a.X < pa.X) Left.Pixels += pa.X - a.X;
//             if (a.X + a.Width > pa.Width) Left.Pixels -= a.X + a.Width - pa.Width;
//             if (a.Y < pa.Y) Top.Pixels += pa.Y - a.Y;
//             if (a.Y + a.Height > pa.Height) Top.Pixels -= a.Y + a.Height - pa.Height;
//             Recalculate();
//         }
//
//         base.Update(gameTime); // don't remove.
//     }
// }