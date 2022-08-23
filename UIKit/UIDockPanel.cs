using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace SSC.UIKit;

[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public class UIDockPanel : UIPanel
{
    public bool Move;
    public Vector2 MoveVec2;

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
        base.Update(gameTime); // don't remove.

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

        var pr = Parent.GetViewCullingArea();
        var r = GetViewCullingArea();
        if (!pr.Contains(r))
        {
            if (r.X < pr.X) Left.Pixels += pr.X - r.X;
            if (r.X + r.Width > pr.Width) Left.Pixels -= r.X + r.Width - pr.Width;
            if (r.Y < pr.Y) Top.Pixels += pr.Y - r.Y;
            if (r.Y + r.Height > pr.Height) Top.Pixels -= r.Y + r.Height - pr.Height;
            Recalculate();
        }
    }
}