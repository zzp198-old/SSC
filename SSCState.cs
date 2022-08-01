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

public class SSCState : UIState
{
    readonly Player Player = new();
    internal readonly static List<PlayerFileData> PlayerList = new();

    UITextPanel<LocalizedText> Content;

    static UIList ItemList;
    UIScrollbar Scrollbar;

    static UIPanel CreatePanel;
    UICharacterNameButton NameButton;
    UISearchBar NameSearchBar;
    UIDifficultyButton SoftCoreButton;
    UIDifficultyButton MediumCoreButton;
    UIDifficultyButton HardcoreButton;
    UIDifficultyButton CreativeButton;
    UITextPanel<LocalizedText> CreateButton;

    public override void OnInitialize()
    {
        Content = new UITextPanel<LocalizedText>(Language.GetText("SSC"), 1, true);
        Content.Width.Set(470, 0);
        Content.Height.Set(650, 0);
        Content.SetPadding(10);
        Content.HAlign = Content.VAlign = 0.5f;
        Append(Content);

        ItemList = new UIList();
        ItemList.Width.Set(450, 0);
        ItemList.Height.Set(-40, 1);
        ItemList.Top.Set(40, 0);
        Content.Append(ItemList);

        Scrollbar = new UIScrollbar();
        Scrollbar.Height.Set(0, 1);
        Scrollbar.HAlign = 1;
        ItemList.SetScrollbar(Scrollbar);
        Content.Append(Scrollbar);

        CreatePanel = new UIPanel();
        CreatePanel.Width.Set(0, 1);
        CreatePanel.Height.Set(180, 0);
        CreatePanel.SetPadding(10);

        NameButton = new UICharacterNameButton(Language.GetText("UI.WorldCreationName"), LocalizedText.Empty);
        NameButton.Width.Set(0, 1);
        NameButton.Height.Set(40, 0);
        NameButton.OnMouseOver += (evt, _) => NameSearchBar.ToggleTakingText();
        NameButton.OnMouseOut += (evt, _) => NameSearchBar.ToggleTakingText();
        CreatePanel.Append(NameButton);

        NameSearchBar = new UISearchBar(LocalizedText.Empty, 1);
        NameSearchBar.Width.Set(-50, 1);
        NameSearchBar.Height.Set(40, 0);
        NameSearchBar.Left.Set(50, 0);
        NameSearchBar.OnMouseOver += (evt, _) => NameButton.MouseOver(evt);
        NameSearchBar.OnMouseOut += (evt, _) => NameButton.MouseOut(evt);
        NameSearchBar.OnContentsChanged += i => Player.name = i;
        CreatePanel.Append(NameSearchBar);

        SoftCoreButton = new UIDifficultyButton(Player, Lang.menu[26], null, PlayerDifficultyID.SoftCore, Color.Cyan);
        SoftCoreButton.Width.Set(200, 0);
        SoftCoreButton.Height.Set(26, 0);
        SoftCoreButton.Top.Set(50, 0);
        CreatePanel.Append(SoftCoreButton);

        MediumCoreButton = new UIDifficultyButton(Player, Lang.menu[25], null, PlayerDifficultyID.MediumCore, Main.mcColor);
        MediumCoreButton.Width.Set(200, 0);
        MediumCoreButton.Height.Set(26, 0);
        MediumCoreButton.Top.Set(50, 0);
        MediumCoreButton.Left.Set(210, 0);
        CreatePanel.Append(MediumCoreButton);

        HardcoreButton = new UIDifficultyButton(Player, Lang.menu[24], null, PlayerDifficultyID.Hardcore, Main.hcColor);
        HardcoreButton.Width.Set(200, 0);
        HardcoreButton.Height.Set(26, 0);
        HardcoreButton.Top.Set(80, 0);
        CreatePanel.Append(HardcoreButton);

        CreativeButton = new UIDifficultyButton(Player, Language.GetText("UI.Creative"), null, PlayerDifficultyID.Creative,
            Main.creativeModeColor);
        CreativeButton.Width.Set(200, 0);
        CreativeButton.Height.Set(26, 0);
        CreativeButton.Top.Set(80, 0);
        CreativeButton.Left.Set(210, 0);
        CreatePanel.Append(CreativeButton);

        CreateButton = new UITextPanel<LocalizedText>(Language.GetText("UI.Create"), 0.7f, true);
        CreateButton.Width.Set(0, 1);
        CreateButton.Height.Set(30, 0);
        CreateButton.Top.Set(115, 0);
        CreateButton.HAlign = 0.5f;
        CreateButton.OnMouseOver += (_, _) =>
        {
            CreateButton.BackgroundColor = new Color(73, 94, 171);
            CreateButton.BorderColor = Colors.FancyUIFatButtonMouseOver;
        };
        CreateButton.OnMouseOut += (_, _) =>
        {
            CreateButton.BackgroundColor = new Color(63, 82, 151) * 0.8f;
            CreateButton.BorderColor = Color.Black;
        };
        CreateButton.OnClick += (_, _) =>
        {
            var p = ModContent.GetInstance<SSC>().GetPacket();
            p.Write((byte)PID.CreatePLR);
            p.Write(SteamUser.GetSteamID().m_SteamID);
            p.Write(Player.name);
            p.Write(Player.difficulty);
            p.Send();
            p = ModContent.GetInstance<SSC>().GetPacket();
            p.Write((byte)PID.PLRList);
            p.Write(SteamUser.GetSteamID().m_SteamID);
            p.Send();
            NameSearchBar.SetContents("");
            Player.difficulty = 0;
        };
        CreatePanel.Append(CreateButton);
    }

    public static void Refresh()
    {
        ItemList.Clear();
        PlayerList.Sort((x, y) => string.CompareOrdinal(x.Player.name, y.Player.name));
        PlayerList.ForEach(i =>
        {
            var item = new UIPlayerListItem(i);
            ItemList.Add(item);
        });
        ItemList.Add(CreatePanel);
    }
}