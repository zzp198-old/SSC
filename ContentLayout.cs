using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using SSC.UIKit;
using Steamworks;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.UI;

namespace SSC;

public class ContentLayout : UIState
{
    readonly Player Player;
    readonly FileSystemWatcher Watch;
    List<PlayerFileData> PlayerList;

    UITextPanel<LocalizedText> Contain;
    UIList ItemList;
    UIPanel CreatePanel;

    UICharacterNameButton NameButton;
    UISearchBar NameSearchBar;
    UIDifficultyButton SoftCoreButton;
    UIDifficultyButton MediumCoreButton;
    UIDifficultyButton HardcoreButton;
    UIDifficultyButton CreativeButton;
    HoverableButton CreateButton;

    public ContentLayout()
    {
        Player = new Player();
        Watch = new FileSystemWatcher();
        Watch.Path = Path.Combine(SSC.SavePath, SteamUser.GetSteamID().m_SteamID.ToString());
        Watch.Changed += Changed;
        Watch.EnableRaisingEvents = true;
    }

    public override void OnInitialize()
    {
        Contain = new UITextPanel<LocalizedText>(Language.GetText("SSC"));
        Contain.Width.Set(450, 0);
        Contain.Height.Set(650, 0);
        Contain.SetPadding(10);
        Contain.HAlign = Contain.VAlign = 0.5f;
        Append(Contain);

        ItemList = new UIList();
        ItemList.Width.Set(0, 1);
        ItemList.Height.Set(0, 1);
        ItemList.Top.Set(40, 0);
        Contain.Append(ItemList);

        CreatePanel = new UIPanel();
        CreatePanel.Width.Set(0, 1);
        CreatePanel.Height.Set(180, 0);
        CreatePanel.SetPadding(10);
        ItemList.Add(CreatePanel);

        NameButton = new UICharacterNameButton(Language.GetText("UI.WorldCreationName"), LocalizedText.Empty);
        NameButton.Width.Set(0, 1);
        NameButton.Height.Set(40, 0);
        NameButton.OnClick += (_, _) => { NameSearchBar.ToggleTakingText(); };
        CreatePanel.Append(NameButton);

        NameSearchBar = new UISearchBar(LocalizedText.Empty, 1);
        NameSearchBar.Width.Set(-50, 1);
        NameSearchBar.Height.Set(40, 0);
        NameSearchBar.Left.Set(50, 0);
        NameSearchBar.OnMouseOver += (evt, _) => NameButton.MouseOver(evt);
        NameSearchBar.OnMouseOut += (evt, _) => NameButton.MouseOut(evt);
        NameSearchBar.OnClick += (evt, _) => NameButton.Click(evt);
        NameSearchBar.OnContentsChanged += i => Player.name = i;
        CreatePanel.Append(NameSearchBar);

        SoftCoreButton = new UIDifficultyButton(Player, Lang.menu[26], null, PlayerDifficultyID.SoftCore, Color.Cyan);
        SoftCoreButton.Width.Set(200, 0);
        SoftCoreButton.Height.Set(26, 0);
        SoftCoreButton.Top.Set(50, 0);
        CreatePanel.Append(SoftCoreButton);

        MediumCoreButton = new UIDifficultyButton(Player, Lang.menu[25], null, PlayerDifficultyID.MediumCore,
            Main.mcColor);
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

        CreativeButton = new UIDifficultyButton(Player, Language.GetText("UI.Creative"), null,
            PlayerDifficultyID.Creative, Main.creativeModeColor);
        CreativeButton.Width.Set(200, 0);
        CreativeButton.Height.Set(26, 0);
        CreativeButton.Top.Set(80, 0);
        CreativeButton.Left.Set(210, 0);
        CreatePanel.Append(CreativeButton);

        CreateButton = new HoverableButton(Language.GetText("UI.Create").Value, 0.7f, true);
        CreateButton.Width.Set(0, 1);
        CreateButton.Height.Set(30, 0);
        CreateButton.Top.Set(115, 0);
        CreateButton.HAlign = 0.5f;
        CreateButton.OnClick += (_, _) =>
        {
            // TODO


            NameSearchBar.SetContents("");
            Player.difficulty = 0;
        };
        CreatePanel.Append(CreateButton);
    }

    void Changed(object sender, FileSystemEventArgs e)
    {
        ItemList.Clear();
        PlayerList = Directory.GetFiles(Watch.Path, "*.plr").Select(i => Player.LoadPlayer(i, false)).ToList();
        PlayerList.Sort((x, y) => x.IsFavorite switch
        {
            true when !y.IsFavorite => -1, false when y.IsFavorite => 1,
            _ => string.CompareOrdinal(x.GetFileName(), y.GetFileName())
        });
        PlayerList.ForEach(i =>
        {
            var item = new UIPlayerListItem(i);
            ItemList.Add(item);
        });
        ItemList.Add(CreatePanel);
    }
}