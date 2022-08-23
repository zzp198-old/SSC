using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace SSC;

[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public class SSCView : UIState
{
    public UITextPanel<LocalizedText> CreateButton;
    public UIPanel CreateView;
    public bool InputState;
    public UISearchBar NameInput;
    public UICharacterNameButton NameView;
    public Player Unknown;
    public UIPanel ViewGroup;
    public UIList ViewList;

    public override void OnActivate()
    {
        ViewGroup = new UIKit.UIDockPanel
        {
            Width = new StyleDimension(Main.screenWidth, 0),
            Height = new StyleDimension(Main.screenHeight, 0),
            MaxWidth = new StyleDimension(370, 0),
            MaxHeight = new StyleDimension(600, 0),
            HAlign = 0.5f, VAlign = 0.5f,
            PaddingRight = 10,
            BackgroundColor = new Color(33, 43, 79) * 0.8f
        };
        Append(ViewGroup);

        var scrollbar = new UIScrollbar
        {
            Height = new StyleDimension(-10, 1),
            HAlign = 1, VAlign = 0.5f
        };
        ViewGroup.Append(scrollbar);

        ViewList = new UIList
        {
            Width = new StyleDimension(-25, 1),
            Height = new StyleDimension(0, 1)
        };
        ViewList.SetScrollbar(scrollbar);
        ViewGroup.Append(ViewList);

        Unknown = new Player();
        CreateView = new UIPanel
        {
            Width = new StyleDimension(0, 1),
            Height = new StyleDimension(180, 0)
        };
        CreateView.SetPadding(10);
        ViewList.Add(CreateView);

        NameView = new UICharacterNameButton(Language.GetText("UI.WorldCreationName"), LocalizedText.Empty)
        {
            Width = new StyleDimension(0, 1),
            Height = new StyleDimension(40, 0)
        };
        NameView.OnUpdate += _ =>
        {
            if (NameView.IsMouseHovering)
            {
                if (InputState || !Main.mouseLeft) return;
                NameInput.ToggleTakingText();
                InputState = true;
            }
            else if (InputState && Main.mouseLeft)
            {
                NameInput.ToggleTakingText();
                InputState = false;
            }
        };
        CreateView.Append(NameView);

        NameInput = new UISearchBar(LocalizedText.Empty, 1)
        {
            Width = new StyleDimension(-50, 1),
            Height = new StyleDimension(40, 0),
            Left = new StyleDimension(50, 0)
        };
        NameInput.OnMouseOver += (evt, _) => NameView.MouseOver(evt); // 影响nameButton.IsMouseHovering
        NameInput.OnMouseOut += (evt, _) => NameView.MouseOut(evt);
        NameInput.OnContentsChanged += name => Unknown.name = name;
        CreateView.Append(NameInput);

        CreateView.Append(new UIDifficultyButton(Unknown, Lang.menu[26], null, PlayerDifficultyID.SoftCore, Color.Cyan)
        {
            Width = new StyleDimension(-5, 0.5f),
            Height = new StyleDimension(26, 0),
            Top = new StyleDimension(50, 0)
        });
        CreateView.Append(new UIDifficultyButton(Unknown, Lang.menu[25], null, PlayerDifficultyID.MediumCore, Main.mcColor)
        {
            Width = new StyleDimension(-5, 0.5f),
            Height = new StyleDimension(26, 0),
            Top = new StyleDimension(50, 0),
            Left = new StyleDimension(5, 0.5f)
        });
        CreateView.Append(new UIDifficultyButton(Unknown, Lang.menu[24], null, PlayerDifficultyID.Hardcore, Main.hcColor)
        {
            Width = new StyleDimension(-5, 0.5f),
            Height = new StyleDimension(26, 0),
            Top = new StyleDimension(80, 0)
        });
        CreateView.Append(new UIDifficultyButton(Unknown, Language.GetText("UI.Creative"), null, PlayerDifficultyID.Creative,
            Main.creativeModeColor)
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
            var mp = SSC.Mod.GetPacket();
            mp.Write((byte)SSC.ID.CreateSSC);
            mp.Write(SSC.Sid);
            mp.Write(Unknown.name);
            mp.Write(Unknown.difficulty);
            mp.Send();

            NameInput.SetContents("");
            Unknown.difficulty = 0;
        };
        CreateView.Append(CreateButton);
    }

    public void Refresh()
    {
        ViewList.Clear();
        if (!ModContent.GetInstance<SSCSyS>().Database.ContainsKey(SSC.Sid.ToString())) return;

        foreach (var compound in ModContent.GetInstance<SSCSyS>().Database.Get<List<TagCompound>>(SSC.Sid.ToString()))
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
            ViewList.Add(item);

            item.Append(new UIText(compound.GetString("name"))
            {
                Height = new StyleDimension(30, 0),
                TextColor = compound.GetByte("difficulty") switch
                {
                    1 => Main.mcColor, 2 => Main.hcColor,
                    3 => Main.creativeModeColor, _ => Color.White
                }
            });

            item.Append(new UIText(Language.GetTextValue(compound.GetByte("difficulty") switch
            {
                0 => "UI.Softcore", 1 => "UI.Mediumcore",
                2 => "UI.Hardcore", 3 => "UI.Creative",
                _ => "Unknown"
            }))
            {
                Height = new StyleDimension(30, 0),
                HAlign = 1,
                TextColor = compound.GetByte("difficulty") switch
                {
                    1 => Main.mcColor, 2 => Main.hcColor,
                    3 => Main.creativeModeColor, _ => Color.White
                }
            });

            item.Append(new UIImage(Main.Assets.Request<Texture2D>("Images/UI/Divider"))
            {
                Width = new StyleDimension(0, 1),
                HAlign = 0.5f, VAlign = 0.5f,
                ScaleToFit = true
            });

            var chooseView = new UIImageButton(Main.Assets.Request<Texture2D>("Images/UI/ButtonPlay"))
            {
                VAlign = 1
            };
            chooseView.OnClick += (_, _) =>
            {
                var mp = SSC.Mod.GetPacket();
                mp.Write((byte)SSC.ID.SelectSSC);
                mp.Write(SSC.Sid);
                mp.Write(compound.Get<string>("name"));
                mp.Send();
            };
            chooseView.OnUpdate += _ =>
            {
                if (chooseView.IsMouseHovering) Main.instance.MouseText(Language.GetTextValue("UI.Play"));
            };
            item.Append(chooseView);

            var removeView = new UIImageButton(Main.Assets.Request<Texture2D>("Images/UI/ButtonDelete"))
            {
                HAlign = 1, VAlign = 1
            };
            removeView.OnRightClick += (_, _) =>
            {
                var mp = SSC.Mod.GetPacket();
                mp.Write((byte)SSC.ID.RemoveSSC);
                mp.Write(SSC.Sid);
                mp.Write(compound.Get<string>("name"));
                mp.Send();
            };
            removeView.OnUpdate += _ =>
            {
                if (removeView.IsMouseHovering) Main.instance.MouseText($"{Language.GetTextValue("UI.Delete")} (Right-Click)");
            };
            item.Append(removeView);
        }

        ViewList.Add(CreateView);
    }
}