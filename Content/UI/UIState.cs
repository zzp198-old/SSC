using System.IO;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.UI.Elements;

namespace SSC.Content.UI;

public class UIState : Terraria.UI.UIState
{
    UIPanel UIPanel;
    UIList UIList;
    UIScrollbar UIScrollbar;

    public override void OnInitialize()
    {
        UIPanel = new UIPanel();
        UIPanel.Width.Set(450, 0);
        UIPanel.Height.Set(640, 0);
        UIPanel.HAlign = UIPanel.VAlign = 0.5f;
        UIPanel.BackgroundColor = new Color(33, 43, 79);

        Append(UIPanel);

        UIList = new UIList();
        UIList.Width.Set(0, 1);
        UIList.Height.Set(0, 1);
        UIPanel.Append(UIList);

        UIScrollbar = new UIScrollbar();
        UIScrollbar.SetView(100, 1000);
        UIScrollbar.Height.Set(0, 1);
        UIScrollbar.HAlign = 1;
        UIList.SetScrollbar(UIScrollbar);
    }

    public override void OnActivate()
    {
        UIList.Clear();
        foreach (var path in Directory.GetFiles(Path.Combine(SSC.SavePath, "Cache"), "*.plr"))
        {
            var item = new UIListView(Player.LoadPlayer(path, false));
            UIList.Add(item);
        }
    }
}