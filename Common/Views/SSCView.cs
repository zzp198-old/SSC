using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace QOS.Common.Views;

public class SSCView : UIState
{
    internal UIList UiList;
    internal UIPanel UiPanel;
    internal Player Dummy;

    public override void OnActivate()
    {
        var panel = new Kits.UIFixedPanel
        {
            Width = new StyleDimension(370, 0),
            Height = new StyleDimension(600, 0),
            HAlign = 0.5f, VAlign = 0.5f,
            PaddingRight = 10,
            BackgroundColor = new Color(33, 43, 79) * 0.8f
        };
        Append(panel);

        var scrollbar = new UIScrollbar
        {
            Height = new StyleDimension(-10, 1),
            HAlign = 1, VAlign = 0.5f
        };
        panel.Append(scrollbar);

        UiList = new UIList
        {
            Width = new StyleDimension(-25, 1),
            Height = new StyleDimension(0, 1),
        };
        UiList.SetScrollbar(scrollbar);
        panel.Append(UiList);

        UiPanel = new UIPanel
        {
            Width = new StyleDimension(0, 1),
            Height = new StyleDimension(180, 0)
        };
        UiPanel.SetPadding(10);
        UiList.Append(UiPanel);

        Dummy = new Player();
        var dummyNameButton = new UICharacterNameButton(Language.GetText("UI.WorldCreationName"), LocalizedText.Empty)
        {
            Width = new StyleDimension(0, 1),
            Height = new StyleDimension(40, 0)
        };
        var dummyNameSearchBar = new UISearchBar(LocalizedText.Empty, 1)
        {
            Width = new StyleDimension(-50, 1),
            Height = new StyleDimension(40, 0),
            Left = new StyleDimension(50, 0)
        };
        dummyNameButton.OnUpdate += _ =>
        {
            if (Main.mouseLeft)
            {
                switch (dummyNameButton.IsMouseHovering)
                {
                    case true when !dummyNameSearchBar.IsWritingText:
                    case false when dummyNameSearchBar.IsWritingText:
                    {
                        dummyNameSearchBar.ToggleTakingText();
                        break;
                    }
                }
            }
        };
        dummyNameSearchBar.OnMouseOver += (evt, _) => dummyNameButton.MouseOver(evt);
        dummyNameSearchBar.OnMouseOut += (evt, _) => dummyNameButton.MouseOut(evt);
        dummyNameSearchBar.OnContentsChanged += name => Dummy.name = name;
        UiPanel.Append(dummyNameButton);
        UiPanel.Append(dummyNameSearchBar);

        UiPanel.Append(new UIDifficultyButton(Dummy, Lang.menu[26], null, PlayerDifficultyID.SoftCore, Color.Cyan)
        {
            Width = new StyleDimension(-5, 0.5f),
            Height = new StyleDimension(26, 0),
            Top = new StyleDimension(50, 0)
        });
        UiPanel.Append(new UIDifficultyButton(Dummy, Lang.menu[25], null, PlayerDifficultyID.MediumCore, Main.mcColor)
        {
            Width = new StyleDimension(-5, 0.5f),
            Height = new StyleDimension(26, 0),
            Top = new StyleDimension(50, 0),
            Left = new StyleDimension(5, 0.5f)
        });
        UiPanel.Append(new UIDifficultyButton(Dummy, Lang.menu[24], null, PlayerDifficultyID.Hardcore, Main.hcColor)
        {
            Width = new StyleDimension(-5, 0.5f),
            Height = new StyleDimension(26, 0),
            Top = new StyleDimension(80, 0)
        });
        UiPanel.Append(new UIDifficultyButton(Dummy, Language.GetText("UI.Creative"), null, PlayerDifficultyID.Creative, Main.creativeModeColor)
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
            var mp = QOS.Mod.GetPacket();
            mp.Write((byte)QOS.ID.CreateSSC);
            mp.Write(QOS.ClientID);
            mp.Write(Dummy.name);
            mp.Write(Dummy.difficulty);
            mp.Send();

            dummyNameSearchBar.SetContents("");
            Dummy.difficulty = 0;
        };
        UiPanel.Append(dummyCreateButton);
    }

    public void FlushList(TagCompound bin)
    {
        UiList.Clear();
        foreach (var tag in bin.Get<List<TagCompound>>(QOS.ClientID.ToString()))
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
            UiList.Add(item);

            item.Append(new UIText(tag.GetString("name"))
            {
                Height = new StyleDimension(30, 0),
                TextColor = QOSKit.DifficultyColor(tag.GetByte("difficulty"))
            });

            item.Append(new UIText(QOSKit.DifficultyTextValue(tag.GetByte("difficulty")))
            {
                Height = new StyleDimension(30, 0),
                HAlign = 1,
                TextColor = QOSKit.DifficultyColor(tag.GetByte("difficulty"))
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
                mp.Write((byte)QOS.ID.ChooseSSC);
                mp.Write(QOS.ClientID);
                mp.Write(tag.Get<string>("name"));
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
                var mp = QOS.Mod.GetPacket();
                mp.Write((byte)QOS.ID.RemoveSSC);
                mp.Write(QOS.ClientID);
                mp.Write(tag.Get<string>("name"));
                mp.Send();
            };
            itemDeleteButton.OnUpdate += _ =>
            {
                if (itemDeleteButton.IsMouseHovering)
                {
                    Main.instance.MouseText($"{Language.GetTextValue("UI.Delete")} (Right-Double-Click/右键双击)");
                }
            };
            item.Append(itemDeleteButton);
        }

        UiList.Add(UiPanel);
    }
}