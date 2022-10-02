using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace SSC.Common.Views.Kits;

public class UIContainer : UIPanel
{
    public new bool Click;
    public Vector2 ClickV2;

    public override void MouseDown(UIMouseEvent evt)
    {
        Click = true;
        ClickV2 = new Vector2(evt.MousePosition.X - Left.Pixels, evt.MousePosition.Y - Top.Pixels);
    }

    public override void MouseUp(UIMouseEvent evt)
    {
        Click = false;
    }

    public override void Update(GameTime gameTime)
    {
        if (ContainsPoint(Main.MouseScreen))
        {
            Main.LocalPlayer.mouseInterface = true;
        }

        if (Click)
        {
            Left.Set(Main.mouseX - ClickV2.X, 0f);
            Top.Set(Main.mouseY - ClickV2.Y, 0f);
        }

        var space = Parent.GetViewCullingArea();
        var area = GetViewCullingArea();
        if (!space.Contains(area))
        {
            if (area.X < space.X)
            {
                Left.Pixels += space.X - area.X;
            }

            if (area.X + area.Width > space.Width)
            {
                Left.Pixels -= area.X + area.Width - space.Width;
            }

            if (area.Y < space.Y)
            {
                Top.Pixels += space.Y - area.Y;
            }

            if (area.Y + area.Height > space.Height)
            {
                Top.Pixels -= area.Y + area.Height - space.Height;
            }
        }

        Recalculate();
        base.Update(gameTime); // don't remove.
    }
}