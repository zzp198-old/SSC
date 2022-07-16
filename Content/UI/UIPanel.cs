using Microsoft.Xna.Framework;
using Terraria;
using Terraria.UI;

namespace SSC.Content.UI;

public class UIPanel : Terraria.GameContent.UI.Elements.UIPanel
{
    Vector2 moveV2;
    bool moving;

    public override void MouseDown(UIMouseEvent evt)
    {
        moving = true;
        moveV2 = new Vector2(evt.MousePosition.X - Left.Pixels, evt.MousePosition.Y - Top.Pixels);
    }

    public override void MouseUp(UIMouseEvent evt)
    {
        moving = false;
        Left.Set(evt.MousePosition.X - moveV2.X, 0f);
        Top.Set(evt.MousePosition.Y - moveV2.Y, 0f);
        Recalculate();
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime); // don't remove.
        if (ContainsPoint(Main.MouseScreen))
        {
            Main.LocalPlayer.mouseInterface = true;
        }

        if (moving)
        {
            Left.Set(Main.mouseX - moveV2.X, 0f);
            Top.Set(Main.mouseY - moveV2.Y, 0f);
            Recalculate();
        }

        var parentSpace = Parent.GetDimensions().ToRectangle();
        if (!GetDimensions().ToRectangle().Intersects(parentSpace))
        {
            Left.Pixels = Utils.Clamp(Left.Pixels, 0, parentSpace.Right - Width.Pixels);
            Top.Pixels = Utils.Clamp(Top.Pixels, 0, parentSpace.Bottom - Height.Pixels);
            Recalculate();
        }
    }
}