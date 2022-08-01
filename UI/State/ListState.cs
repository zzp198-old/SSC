using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Steamworks;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace SSC.UI.State;

public class ListState : UIState
{
    int Countdown;
    ulong SteamID;

    UITextPanel<string> MainPanel;

    List<PlayerFileData> PlayerList;
    UIList ItemList;

    Player Player;
    UIPanel CreatePanel;
    UISearchBar NameSearchBar;
    UIDifficultyButton SoftCoreButton;
    UIDifficultyButton MediumCoreButton;
    UIDifficultyButton HardcoreButton;
    UIDifficultyButton CreativeButton;
    UITextPanel<LocalizedText> CreateButton;

    public override void OnInitialize()
    {
        MainPanel = new UITextPanel<string>("SSC", 1, true);
        MainPanel.Width.Set(450, 0);
        MainPanel.Height.Set(650, 0);
        MainPanel.SetPadding(10);
        MainPanel.HAlign = MainPanel.VAlign = 0.5f;
        Append(MainPanel);

        ItemList = new UIList();
        ItemList.Width.Set(0, 1);
        ItemList.Height.Set(0, 1);
        ItemList.Top.Set(40, 0);
        MainPanel.Append(ItemList);

        Player = new Player();
        CreatePanel = new UIPanel();
        CreatePanel.Width.Set(0, 1); // 430
        CreatePanel.Height.Set(300, 0);
        CreatePanel.SetPadding(10);
        ItemList.Add(CreatePanel);

        NameSearchBar = new UISearchBar(Language.GetText("UI.PlayerNameSlot"), 1);
        NameSearchBar.Width.Set(0, 1);
        NameSearchBar.Height.Set(60, 0);
        CreatePanel.Append(NameSearchBar);

        SoftCoreButton = new UIDifficultyButton(Player, Lang.menu[26], null, 0, Color.Cyan);
        SoftCoreButton.Width.Set(170, 0);
        SoftCoreButton.Height.Set(26, 0);
        SoftCoreButton.Top.Set(70, 0);
        SoftCoreButton.Left.Set(20, 0);
        CreatePanel.Append(SoftCoreButton);

        MediumCoreButton = new UIDifficultyButton(Player, Lang.menu[25], null, 1, Main.mcColor);
        MediumCoreButton.Width.Set(170, 0);
        MediumCoreButton.Height.Set(26, 0);
        MediumCoreButton.Top.Set(70, 0);
        MediumCoreButton.Left.Set(220, 0);
        CreatePanel.Append(MediumCoreButton);

        HardcoreButton = new UIDifficultyButton(Player, Lang.menu[24], null, 2, Main.hcColor);
        HardcoreButton.Width.Set(170, 0);
        HardcoreButton.Height.Set(26, 0);
        HardcoreButton.Top.Set(100, 0);
        HardcoreButton.Left.Set(20, 0);
        CreatePanel.Append(HardcoreButton);

        CreativeButton =
            new UIDifficultyButton(Player, Language.GetText("UI.Creative"), null, 3, Main.creativeModeColor);
        CreativeButton.Width.Set(170, 0);
        CreativeButton.Height.Set(26, 0);
        CreativeButton.Top.Set(100, 0);
        CreativeButton.Left.Set(220, 0);
        CreatePanel.Append(CreativeButton);

        CreateButton = new UITextPanel<LocalizedText>(Language.GetText("UI.Create"), 0.7f, true);
        CreateButton.Width.Set(0, 1);
        CreateButton.Height.Set(50, 0);
        CreativeButton.Top.Set(130, 0);
        CreatePanel.Append(CreateButton);
    }

    public override void OnActivate()
    {
        SteamID = SteamUser.GetSteamID().m_SteamID;
        if (Main.netMode == NetmodeID.MultiplayerClient)
        {
            NETCore.C_ObtainPLR(SteamID);
        }
    }

    public override void Update(GameTime gameTime)
    {
        Countdown++;
        if (Countdown < 60) return;
        Countdown = 0;
        ItemList.Clear();
        PlayerList = Utils.GetPlayerList(SteamID, "*.plr").Select(i => Player.LoadPlayer(i, false)).ToList();
        PlayerList.Sort((x, y) => x.IsFavorite switch
        {
            true when !y.IsFavorite => -1, false when y.IsFavorite => 1,
            _ => string.CompareOrdinal(x.GetFileName(), y.GetFileName())
        });
        PlayerList.ForEach(i =>
        {
            var item = new UICharacterListItem(i, 0);
            ItemList.Add(item);
        });
        ItemList.Add(CreatePanel);
    }
}