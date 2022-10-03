using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.GameContent.UI.States;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ModLoader.UI.Elements;
using Terraria.UI;
using Terraria.Utilities;

namespace SSC.Common.Views;

public class SSCView : UIState
{
    internal Kits.UIContainer Container;
    internal UIGrid CharacterGrid;
    internal Player Character;
    internal UICharacterCreation CharacterCreation;
    internal UIPanel CharacterCreationPanel;
    internal UICharacterNameButton NameButton;
    internal UISearchBar NameSearchBar;
    internal UITextPanel<LocalizedText> CreateButton;

    public override void OnActivate()
    {
        Append(Container = new Kits.UIContainer
        {
            Width = new StyleDimension(370, 0),
            Height = new StyleDimension(600, 0),
            HAlign = 0.5f, VAlign = 0.5f,
            PaddingRight = 10,
            BackgroundColor = new Color(33, 43, 79) * 0.8f
        });

        var scrollbar = new UIScrollbar
        {
            Height = new StyleDimension(-10, 1),
            HAlign = 1, VAlign = 0.5f
        };
        Container.Append(scrollbar);

        Container.Append(CharacterGrid = new UIGrid
        {
            Width = new StyleDimension(-25, 1),
            Height = new StyleDimension(0, 1)
        });
        CharacterGrid.SetScrollbar(scrollbar);

        CharacterCreation = new UICharacterCreation(Character = new Player());

        CharacterCreationPanel = new UIPanel
        {
            Width = new StyleDimension(0, 1),
            Height = new StyleDimension(180, 0)
        };
        CharacterCreationPanel.SetPadding(10);
        CharacterGrid.Append(CharacterCreationPanel);

        NameButton = new UICharacterNameButton(Language.GetText("UI.WorldCreationName"), LocalizedText.Empty)
        {
            Width = new StyleDimension(0, 1),
            Height = new StyleDimension(40, 0)
        };
        NameButton.OnUpdate += _ =>
        {
            if (!Main.mouseLeft)
            {
                return;
            }

            switch (NameButton.IsMouseHovering)
            {
                case true when !NameSearchBar.IsWritingText:
                case false when NameSearchBar.IsWritingText:
                {
                    NameSearchBar.ToggleTakingText();
                    break;
                }
            }
        };
        CharacterCreationPanel.Append(NameButton);

        NameSearchBar = new UISearchBar(LocalizedText.Empty, 1)
        {
            Width = new StyleDimension(-50, 1),
            Height = new StyleDimension(40, 0),
            Left = new StyleDimension(50, 0)
        };
        NameSearchBar.OnMouseOver += (evt, _) => NameButton.MouseOver(evt);
        NameSearchBar.OnMouseOut += (evt, _) => NameButton.MouseOut(evt);
        NameSearchBar.OnContentsChanged += name => Character.name = name;
        CharacterCreationPanel.Append(NameSearchBar);

        CharacterCreationPanel.Append(new UIDifficultyButton(Character, Lang.menu[26], null, PlayerDifficultyID.SoftCore, Color.Cyan)
        {
            Width = new StyleDimension(-5, 0.5f),
            Height = new StyleDimension(26, 0),
            Top = new StyleDimension(50, 0)
        });
        CharacterCreationPanel.Append(new UIDifficultyButton(Character, Lang.menu[25], null, PlayerDifficultyID.MediumCore, Main.mcColor)
        {
            Width = new StyleDimension(-5, 0.5f),
            Height = new StyleDimension(26, 0),
            Top = new StyleDimension(50, 0),
            Left = new StyleDimension(5, 0.5f)
        });
        CharacterCreationPanel.Append(new UIDifficultyButton(Character, Lang.menu[24], null, PlayerDifficultyID.Hardcore, Main.hcColor)
        {
            Width = new StyleDimension(-5, 0.5f),
            Height = new StyleDimension(26, 0),
            Top = new StyleDimension(80, 0)
        });
        CharacterCreationPanel.Append(new UIDifficultyButton(Character, Language.GetText("UI.Creative"), null, PlayerDifficultyID.Creative, Main.creativeModeColor)
        {
            Width = new StyleDimension(-5, 0.5f),
            Height = new StyleDimension(26, 0),
            Top = new StyleDimension(80, 0),
            Left = new StyleDimension(5, 0.5f)
        });

        CreateButton = new UITextPanel<LocalizedText>(Language.GetText("UI.Create"), 0.7f, true)
        {
            Width = new StyleDimension(0, 1),
            Height = new StyleDimension(30, 0),
            Top = new StyleDimension(115, 0),
            HAlign = 0.5f
        };
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
            var invoke = typeof(UICharacterCreation).GetMethod("SetupPlayerStatsAndInventoryBasedOnDifficulty", (BindingFlags)36);
            invoke?.Invoke(CharacterCreation, Array.Empty<object>());
            Character.statLife = Character.statLifeMax = ModContent.GetInstance<Configs.UnityConfig>().StartingHealth;
            Character.statMana = Character.statManaMax = ModContent.GetInstance<Configs.UnityConfig>().StartingMana;
            var data = new PlayerFileData(Path.Combine(Main.PlayerPath, "SSC.SSC"), false)
            {
                Metadata = FileMetadata.FromCurrentSettings(FileType.Player), Player = Character
            };
            data.MarkAsServerSide();
            SSCKit.InternalSavePlayerFile(data);

            NameSearchBar.SetContents("");
            Character.difficulty = PlayerDifficultyID.Creative;
        };
        CharacterCreationPanel.Append(CreateButton);
    }

    public void Calc(TagCompound obj)
    {
        CharacterGrid.Clear();
        foreach (var tag in obj.Get<List<TagCompound>>(SSCKit.ClientID.ToString()))
        {
            var item = new UIPanel
            {
                Width = new StyleDimension(0, 1),
                Height = new StyleDimension(80, 0),
                PaddingBottom = 10
            };
            item.OnMouseOver += (_, _) =>
            {
                item.BackgroundColor = new Color(73, 94, 171);
                item.BorderColor = new Color(89, 116, 213);
            };
            item.OnMouseOut += (_, _) =>
            {
                item.BackgroundColor = new Color(63, 82, 151) * 0.7f;
                item.BorderColor = new Color(89, 116, 213) * 0.7f;
            };
            CharacterGrid.Add(item);

            item.Append(new UIText(tag.GetString("name"))
            {
                Height = new StyleDimension(30, 0),
                TextColor = SSCKit.DifficultyTextColor(tag.GetByte("difficulty"))
            });

            item.Append(new UIText(SSCKit.DifficultyTextValue(tag.GetByte("difficulty")))
            {
                Height = new StyleDimension(30, 0),
                HAlign = 1,
                TextColor = SSCKit.DifficultyTextColor(tag.GetByte("difficulty"))
            });

            item.Append(new UIImage(Main.Assets.Request<Texture2D>("Images/UI/Divider"))
            {
                Width = new StyleDimension(0, 1),
                HAlign = 0.5f, VAlign = 0.5f,
                ScaleToFit = true
            });

            var itemPlayButton = new UIImageButton(Main.Assets.Request<Texture2D>("Images/UI/ButtonPlay"))
            {
                VAlign = 1
            };
            itemPlayButton.OnClick += (_, _) =>
            {
                var mp = ModContent.GetInstance<SSC>().GetPacket();
                mp.Write((byte)SSC.PID.ChooseSSC);
                mp.Write(SSCKit.ClientID);
                mp.Write(tag.GetString("name"));
                mp.Send();
            };
            itemPlayButton.OnUpdate += _ =>
            {
                if (itemPlayButton.IsMouseHovering)
                {
                    Main.instance.MouseText(Language.GetTextValue("UI.Play"));
                }
            };
            item.Append(itemPlayButton);

            var itemDeleteButton = new UIImageButton(Main.Assets.Request<Texture2D>("Images/UI/ButtonDelete"))
            {
                HAlign = 1, VAlign = 1
            };
            itemDeleteButton.OnRightDoubleClick += (_, _) =>
            {
                var mp = ModContent.GetInstance<SSC>().GetPacket();
                mp.Write((byte)SSC.PID.RemoveSSC);
                mp.Write(SSCKit.ClientID);
                mp.Write(tag.Get<string>("name"));
                mp.Send();
            };
            itemDeleteButton.OnUpdate += _ =>
            {
                if (itemDeleteButton.IsMouseHovering)
                {
                    // TODO
                    Main.instance.MouseText($"{Language.GetTextValue("UI.Delete")} (Right-Double-Click/右键双击)");
                }
            };
            item.Append(itemDeleteButton);
        }

        CharacterGrid.Add(CharacterCreationPanel);
    }
}