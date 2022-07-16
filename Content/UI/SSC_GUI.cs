using System.IO;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI.Elements;
using Terraria.UI;

namespace SSC.Content.UI;

public class SSC_GUI : UIState
{
    UIPanel UIPanel;
    UIGrid UIList;
    UIScrollbar UIScrollbar;

    public override void OnInitialize()
    {
        UIPanel = new UIPanel();
        UIPanel.Width.Set(400, 0);
        UIPanel.Height.Set(600, 0);
        UIPanel.BackgroundColor = new Color(33, 43, 79) * 0.8f;
        UIPanel.SetPadding(10);
        Append(UIPanel);

        UIList = new UIGrid();
        UIList.Width.Set(0, 1);
        UIList.Height.Set(0, 0.9f);
        UIList.Top.Set(60, 0);
        UIPanel.Append(UIList);

        UIScrollbar = new UIScrollbar();
        UIScrollbar.Height.Set(0, 1);
        UIScrollbar.HAlign = 0.95f;
        UIList.SetScrollbar(UIScrollbar);

        CalcUIList();
    }

    void CalcUIList()
    {
        UIList.Clear();
        var players = Directory.GetFiles(Main.PlayerPath, "*.plr");
        for (var i = 0; i < 20; i++)
        {
            var item = new UIListItem(Player.LoadPlayer(players[i % players.Length], false));
            item.Top.Set(i * 100, 0);
            UIList.Add(item);
        }

        UIList.Recalculate();
    }
}