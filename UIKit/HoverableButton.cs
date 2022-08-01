using Microsoft.Xna.Framework;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.UI;

namespace SSC.UIKit;

public class HoverableButton : UITextPanel<string>
{
    public HoverableButton(string text, float textScale = 1, bool large = false) : base(text, textScale, large)
    {
    }

    public override void MouseOver(UIMouseEvent evt)
    {
        BackgroundColor = new Color(73, 94, 171);
        BorderColor = Colors.FancyUIFatButtonMouseOver;
    }

    public override void MouseOut(UIMouseEvent evt)
    {
        BackgroundColor = new Color(63, 82, 151) * 0.8f;
        BorderColor = Color.Black;
    }
}