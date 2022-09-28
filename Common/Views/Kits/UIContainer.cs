using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace QOS.Common.Views.Kits;

internal class UIContainer : UIPanel
{
    internal bool Move;
    internal Vector2 MoveVec2;

    public override void MouseDown(UIMouseEvent evt)
    {
        MoveVec2 = new Vector2(evt.MousePosition.X - Left.Pixels, evt.MousePosition.Y - Top.Pixels);
        Move = true;
    }

    public override void MouseUp(UIMouseEvent evt)
    {
        Move = false;
    }

    public override void Update(GameTime gameTime)
    {
        if (ContainsPoint(Main.MouseScreen))
        {
            Main.LocalPlayer.mouseInterface = true;
        }

        if (Move)
        {
            Left.Set(Main.mouseX - MoveVec2.X, 0f);
            Top.Set(Main.mouseY - MoveVec2.Y, 0f);
            Recalculate();
        }

        var pa = Parent.GetViewCullingArea();
        var a = GetViewCullingArea();
        if (!pa.Contains(a))
        {
            if (a.X < pa.X) Left.Pixels += pa.X - a.X;
            if (a.X + a.Width > pa.Width) Left.Pixels -= a.X + a.Width - pa.Width;
            if (a.Y < pa.Y) Top.Pixels += pa.Y - a.Y;
            if (a.Y + a.Height > pa.Height) Top.Pixels -= a.Y + a.Height - pa.Height;
            Recalculate();
        }

        base.Update(gameTime); // don't remove.
    }
}