using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using QOS.Class.SSC.Configs;
using QOS.Common.ViewKits;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.GameContent.UI.States;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader.IO;
using Terraria.ModLoader.UI.Elements;
using Terraria.UI;

namespace QOS.Class.SSC.Views;

public class SSCView : UIState
{
    internal Player Character;
    internal UICharacterCreation CharacterCreation;
    internal UIGrid CharacterGrid;
    internal UIPanel CreationPanel;

    public override void OnActivate()
    {
        CharacterCreation = new UICharacterCreation(Character = new Player());

        var element = new UIFixedPanel
        {
            Width = new StyleDimension(370, 0),
            Height = new StyleDimension(600, 0),
            HAlign = 0.5f, VAlign = 0.5f,
            PaddingRight = 10,
            BackgroundColor = new Color(33, 43, 79) * 0.8f
        };
        Append(element);

        var scrollbar = new UIScrollbar
        {
            Height = new StyleDimension(-10, 1),
            HAlign = 1, VAlign = 0.5f
        };
        element.Append(scrollbar);

        CharacterGrid = new UIGrid
        {
            Width = new StyleDimension(-25, 1),
            Height = new StyleDimension(0, 1)
        };
        CharacterGrid.SetScrollbar(scrollbar);
        element.Append(CharacterGrid);

        CreationPanel = new UIPanel
        {
            Width = new StyleDimension(0, 1),
            Height = new StyleDimension(180, 0)
        };
        CreationPanel.SetPadding(10);
        CharacterGrid.Append(CreationPanel);

        var nameButton = new UICharacterNameButton(Language.GetText("UI.WorldCreationName"), LocalizedText.Empty)
        {
            Width = new StyleDimension(0, 1),
            Height = new StyleDimension(40, 0)
        };
        var nameSearchBar = new UISearchBar(LocalizedText.Empty, 1)
        {
            Width = new StyleDimension(-50, 1),
            Height = new StyleDimension(40, 0),
            Left = new StyleDimension(50, 0)
        };
        nameButton.OnUpdate += _ =>
        {
            if (!Main.mouseLeft) return;

            switch (nameButton.IsMouseHovering)
            {
                case true when !nameSearchBar.IsWritingText:
                case false when nameSearchBar.IsWritingText:
                {
                    nameSearchBar.ToggleTakingText();
                    break;
                }
            }
        };
        nameSearchBar.OnMouseOver += (evt, _) => nameButton.MouseOver(evt);
        nameSearchBar.OnMouseOut += (evt, _) => nameButton.MouseOut(evt);
        nameSearchBar.OnContentsChanged += name => Character.name = name;
        CreationPanel.Append(nameButton);
        CreationPanel.Append(nameSearchBar);

        CreationPanel.Append(new UIDifficultyButton(Character, Lang.menu[26], null, PlayerDifficultyID.SoftCore, Color.Cyan)
        {
            Width = new StyleDimension(-5, 0.5f),
            Height = new StyleDimension(26, 0),
            Top = new StyleDimension(50, 0)
        });
        CreationPanel.Append(new UIDifficultyButton(Character, Lang.menu[25], null, PlayerDifficultyID.MediumCore, Main.mcColor)
        {
            Width = new StyleDimension(-5, 0.5f),
            Height = new StyleDimension(26, 0),
            Top = new StyleDimension(50, 0),
            Left = new StyleDimension(5, 0.5f)
        });
        CreationPanel.Append(new UIDifficultyButton(Character, Lang.menu[24], null, PlayerDifficultyID.Hardcore, Main.hcColor)
        {
            Width = new StyleDimension(-5, 0.5f),
            Height = new StyleDimension(26, 0),
            Top = new StyleDimension(80, 0)
        });
        CreationPanel.Append(new UIDifficultyButton(Character, Language.GetText("UI.Creative"), null, PlayerDifficultyID.Creative,
            Main.creativeModeColor)
        {
            Width = new StyleDimension(-5, 0.5f),
            Height = new StyleDimension(26, 0),
            Top = new StyleDimension(80, 0),
            Left = new StyleDimension(5, 0.5f)
        });
        var dummyCreateButton = new UITextPanel<LocalizedText>(Language.GetText("UI.Create"), 0.7f, true)
        {
            Width = new StyleDimension(0, 1),
            Height = new StyleDimension(30, 0),
            Top = new StyleDimension(115, 0),
            HAlign = 0.5f
        };
        dummyCreateButton.OnMouseOver += (_, _) =>
        {
            dummyCreateButton.BackgroundColor = new Color(73, 94, 171);
            dummyCreateButton.BorderColor = Colors.FancyUIFatButtonMouseOver;
        };
        dummyCreateButton.OnMouseOut += (_, _) =>
        {
            dummyCreateButton.BackgroundColor = new Color(63, 82, 151) * 0.8f;
            dummyCreateButton.BorderColor = Color.Black;
        };
        dummyCreateButton.OnClick += (_, _) =>
        {
            var invoke = typeof(UICharacterCreation).GetMethod("SetupPlayerStatsAndInventoryBasedOnDifficulty", (BindingFlags)36);
            invoke.Invoke(CharacterCreation, Array.Empty<object>());
            Character.statLife = Character.statLifeMax = SSCConfig.Instance.StartLife;
            Character.statMana = Character.statManaMax = SSCConfig.Instance.StartMana;
            Player.SavePlayer(new PlayerFileData(Path.Combine(Main.PlayerPath, $"{QOS.ClientID}.SSC"), false)
            {
                Metadata = FileMetadata.FromCurrentSettings(FileType.Player), Player = Character
            });

            nameSearchBar.SetContents("");
            Character.difficulty = PlayerDifficultyID.Creative;
        };
        CreationPanel.Append(dummyCreateButton);
    }

    public void Redraw(TagCompound obj)
    {
        CharacterGrid.Clear();
        foreach (var tag in obj.Get<List<TagCompound>>(QOS.ClientID.ToString()))
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

            // item.Append(new UIText(QOSKit.DifficultyTextValue(tag.GetByte("difficulty")))
            item.Append(new UIText("123")
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
                var mp = QOS.Mod.GetPacket();
                mp.Write((byte)QOS.PID.ChooseSSC);
                mp.Write(QOS.ClientID);
                mp.Write(tag.GetString("name"));
                mp.Send();
            };
            itemPlayButton.OnUpdate += _ =>
            {
                if (itemPlayButton.IsMouseHovering) Main.instance.MouseText(Language.GetTextValue("UI.Play"));
            };
            item.Append(itemPlayButton);

            var itemDeleteButton = new UIImageButton(Main.Assets.Request<Texture2D>("Images/UI/ButtonDelete"))
            {
                HAlign = 1, VAlign = 1
            };
            itemDeleteButton.OnRightDoubleClick += (_, _) =>
            {
                var mp = QOS.Mod.GetPacket();
                mp.Write((byte)QOS.PID.RemoveSSC);
                mp.Write(QOS.ClientID);
                mp.Write(tag.Get<string>("name"));
                mp.Send();
            };
            itemDeleteButton.OnUpdate += _ =>
            {
                if (itemDeleteButton.IsMouseHovering)
                    Main.instance.MouseText($"{Language.GetTextValue("UI.Delete")} (Right-Double-Click/右键双击)");
            };
            item.Append(itemDeleteButton);
        }

        CharacterGrid.Add(CreationPanel);
    }
}