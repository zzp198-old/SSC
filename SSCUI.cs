using System.IO;
using Microsoft.Xna.Framework;
using SSC.UI;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace SSC;

public class SSCUI : UIState
{
    UIPanel UIPanel;
    UIList UIList;
    UIScrollbar UIScrollbar;

    public override void OnInitialize()
    {
        UIPanel = new UIPanel();
        UIPanel.Width.Set(450, 0);
        UIPanel.Height.Set(840, 0);
        UIPanel.HAlign = UIPanel.VAlign = 0.5f;
        UIPanel.BackgroundColor = new Color(33, 43, 79);
        Append(UIPanel);

        UIList = new UIList();
        UIList.Width.Set(0, 1);
        UIList.Height.Set(0, 0.8f);
        UIPanel.Append(UIList);

        UIScrollbar = new UIScrollbar();
        UIScrollbar.SetView(100, 1000);
        UIScrollbar.Height.Set(0, 1);
        UIScrollbar.HAlign = 1;
        UIList.SetScrollbar(UIScrollbar);
        UIPanel.Append(UIScrollbar);
        
        
    }

    public override void OnActivate()
    {
        UIList.Clear();
        var files = Directory.GetFiles(SSC.CachePath, "*.plr");
        foreach (var file in files)
        {
            var fileData = Player.GetFileData(file, false);
            if (fileData != null)
            {
                UIList.Add(new UIListView(fileData));
            }
        }
    }
}