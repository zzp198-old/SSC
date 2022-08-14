using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.UI;

namespace SSC;

public class SSCView : UIState
{
    internal static UIPanel ViewGroup;
    internal static UIList ViewList;
    internal static UIPanel CreateView;

    public override void OnActivate()
    {
        ViewGroup = new UIPanel
        {
            Width = new StyleDimension(309, 0),
            Height = new StyleDimension(500, 0),
            HAlign = 0.5f, VAlign = 0.5f,
            PaddingRight = 10,
        };
        Append(ViewGroup);

        var scrollbar = new UIScrollbar
        {
            Height = new StyleDimension(-10, 1),
            HAlign = 1, VAlign = 0.5f
        };
        ViewGroup.Append(scrollbar);

        ViewList = new UIList
        {
            Width = new StyleDimension(-25, 1),
            Height = new StyleDimension(0, 1)
        };
        ViewList.SetScrollbar(scrollbar);
        ViewGroup.Append(ViewList);

        RedrawList(new List<(string, byte)>
        {
            ("zzp198", 0),
            ("zzp198", 1),
            ("zzp198", 2),
            ("zzp198", 3),
            ("zzp198", 4),
            ("Sam Javid Pack", 4),
        });
    }

    public override void OnDeactivate()
    {
        CreateView = null;
        ViewList = null;
        ViewGroup = null;
    }

    public static void RedrawList(List<(string, byte)> data)
    {
        ViewList.Clear();
        foreach (var (name, mode) in data)
        {
            var baseColor = mode switch
            {
                1 => Main.mcColor, 2 => Main.hcColor,
                3 => Main.creativeModeColor, _ => Color.White,
            };

            var item = new UIPanel
            {
                Width = new StyleDimension(0, 1),
                Height = new StyleDimension(80, 0),
            };
            ViewList.Add(item);

            item.Append(new UIText(name)
            {
                TextColor = baseColor,
                Height = new StyleDimension(30, 0)
            });

            item.Append(new UIText(Language.GetTextValue(mode switch
            {
                0 => "UI.Softcore", 1 => "UI.Mediumcore",
                2 => "UI.Hardcore", 3 => "UI.Creative",
                _ => "Unknown"
            }))
            {
                TextColor = baseColor,
                Height = new StyleDimension(30, 0),
                HAlign = 1,
            });

            item.Append(new UIImage(Main.Assets.Request<Texture2D>("Images/UI/Divider"))
            {
                Width = new StyleDimension(0, 1),
                HAlign = 0.5f, VAlign = 0.5f,
                ScaleToFit = true
            });

            var PlayButton = new UIImageButton(Main.Assets.Request<Texture2D>("Images/UI/ButtonPlay"))
            {
                VAlign = 1
            };
            PlayButton.OnClick += (_, _) => { };
            PlayButton.OnUpdate += _ =>
            {
                if (PlayButton.IsMouseHovering)
                {
                    Main.instance.MouseText("开始游戏");
                }
            };
            item.Append(PlayButton);

            var DeleteButton = new UIImageButton(Main.Assets.Request<Texture2D>("Images/UI/ButtonDelete"))
            {
                HAlign = 1, VAlign = 1
            };
            DeleteButton.OnClick += (_, _) => { };
            DeleteButton.OnUpdate += _ =>
            {
                if (DeleteButton.IsMouseHovering)
                {
                    Main.instance.MouseText("双击删除");
                }
            };
            item.Append(DeleteButton);
        }
    }
}