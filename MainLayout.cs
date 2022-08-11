using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Steamworks;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace SSC;

public class MainLayout : UIState
{
    internal static UIList UIList;

    internal static Player Unknown;
    internal static UIPanel CreatePanel;
    internal static bool Focus;

    public override void OnInitialize()
    {
        var panel = new UIPanel();
        panel.Width.Set(400, 0);
        panel.Height.Set(500, 0);
        panel.HAlign = panel.VAlign = 0.5f;
        Append(panel);

        UIList = new UIList();
        UIList.Width.Set(-20, 1);
        UIList.Height.Set(0, 1);
        panel.Append(UIList);

        var scrollbar = new UIScrollbar();
        scrollbar.Height.Set(0, 1);
        scrollbar.Left.Set(-15, 1);
        panel.Append(scrollbar);

        UIList.SetScrollbar(scrollbar);

        MakeCreatePanel();
        ResetListItem();
    }

    public override void OnActivate()
    {
        var p = ModContent.GetInstance<SSC>().GetPacket();
        p.Write((byte)SSC.ID.AskList);
        p.Write(SteamUser.GetSteamID().m_SteamID);
        p.Send();
    }

    private static void MakeCreatePanel()
    {
        Unknown = new Player();

        CreatePanel = new UIPanel();
        CreatePanel.Width.Set(0, 1);
        CreatePanel.Height.Set(180, 0);
        CreatePanel.SetPadding(10);

        var nameButton = new UICharacterNameButton(Language.GetText("UI.WorldCreationName"), LocalizedText.Empty);
        var nameSearchBar = new UISearchBar(LocalizedText.Empty, 1);

        nameButton.Width.Set(0, 1);
        nameButton.Height.Set(40, 0);
        nameButton.OnUpdate += _ =>
        {
            if (nameButton.IsMouseHovering)
            {
                if (!Focus && Main.mouseLeft) // 被选中且无焦点的情况下被点击
                {
                    nameSearchBar.ToggleTakingText();
                    Focus = true;
                }
            }
            else if (Focus && Main.mouseLeft) // 有焦点但未被选中的情况下点击
            {
                nameSearchBar.ToggleTakingText();
                Focus = false;
            }
        };
        CreatePanel.Append(nameButton);

        nameSearchBar.Width.Set(-50, 1);
        nameSearchBar.Height.Set(40, 0);
        nameSearchBar.Left.Set(50, 0);
        nameSearchBar.OnMouseOver += (evt, _) => nameButton.MouseOver(evt); // 影响nameButton.IsMouseHovering
        nameSearchBar.OnMouseOut += (evt, _) => nameButton.MouseOut(evt);
        nameSearchBar.OnContentsChanged += i => Unknown.name = i;
        CreatePanel.Append(nameSearchBar);

        var modeButton1 = new UIDifficultyButton(Unknown, Lang.menu[26], null, 0, Color.Cyan);
        modeButton1.Width.Set(-5, 0.5f);
        modeButton1.Height.Set(26, 0);
        modeButton1.Top.Set(50, 0);
        CreatePanel.Append(modeButton1);

        var modeButton2 = new UIDifficultyButton(Unknown, Lang.menu[25], null, 1, Main.mcColor);
        modeButton2.Width.Set(-5, 0.5f);
        modeButton2.Height.Set(26, 0);
        modeButton2.Top.Set(50, 0);
        modeButton2.Left.Set(5, 0.5f);
        CreatePanel.Append(modeButton2);

        var modeButton3 = new UIDifficultyButton(Unknown, Lang.menu[24], null, 2, Main.hcColor);
        modeButton3.Width.Set(-5, 0.5f);
        modeButton3.Height.Set(26, 0);
        modeButton3.Top.Set(80, 0);
        CreatePanel.Append(modeButton3);

        var modeButton4 = new UIDifficultyButton(Unknown, Language.GetText("UI.Creative"), null, 3,
            Main.creativeModeColor);
        modeButton4.Width.Set(-5, 0.5f);
        modeButton4.Height.Set(26, 0);
        modeButton4.Top.Set(80, 0);
        modeButton4.Left.Set(5, 0.5f);
        CreatePanel.Append(modeButton4);

        var createButton = new UITextPanel<LocalizedText>(Language.GetText("UI.Create"), 0.7f, true);
        createButton.Width.Set(0, 1);
        createButton.Height.Set(30, 0);
        createButton.Top.Set(115, 0);
        createButton.HAlign = 0.5f;
        createButton.OnMouseOver += (_, _) =>
        {
            createButton.BackgroundColor = new Color(73, 94, 171);
            createButton.BorderColor = Colors.FancyUIFatButtonMouseOver;
        };
        createButton.OnMouseOut += (_, _) =>
        {
            createButton.BackgroundColor = new Color(63, 82, 151) * 0.8f;
            createButton.BorderColor = Color.Black;
        };
        createButton.OnClick += (_, _) =>
        {
            var p = ModContent.GetInstance<SSC>().GetPacket();
            p.Write((byte)SSC.ID.AskCreate);
            p.Write(SteamUser.GetSteamID().m_SteamID);
            p.Write(Unknown.name);
            p.Write(Unknown.difficulty);
            p.Send();
            nameSearchBar.SetContents("");
            Unknown.difficulty = 0;
        };
        CreatePanel.Append(createButton);
    }

    public static void ResetListItem(List<(string name, byte difficulty, long tick)> items = null)
    {
        UIList.Clear();
        if (items == null || items.Count == 0)
        {
            var panel = new UITextPanel<string>("No data!", 1, true);
            panel.Width.Set(0, 1);
            panel.Height.Set(50, 0);
            UIList.Add(panel);
        }
        else
        {
            foreach (var (name, difficulty, tick) in items)
            {
                var data = new PlayerFileData
                {
                    Metadata = FileMetadata.FromCurrentSettings(FileType.Player),
                    Player = new Player
                    {
                        name = name, difficulty = difficulty
                    }
                };
                data.SetPlayTime(new TimeSpan(tick));
                UIList.Add(new ListItem(data));
            }
        }

        UIList.Add(CreatePanel);
    }
}