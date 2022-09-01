using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Steamworks;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace SSC;

public class SSCView : UIState
{
    private UIList _listView;
    private UIPanel _createView;

    public override void OnActivate()
    {
        var panel = new Kit.AdsorbView
        {
            Width = new StyleDimension(Main.screenWidth, 0),
            Height = new StyleDimension(Main.screenHeight, 0),
            MaxWidth = new StyleDimension(370, 0),
            MaxHeight = new StyleDimension(600, 0),
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

        _listView = new UIList
        {
            Width = new StyleDimension(-25, 1),
            Height = new StyleDimension(0, 1),
        };
        _listView.SetScrollbar(scrollbar);
        panel.Append(_listView);

        _createView = new Kit.CreateView
        {
            Width = new StyleDimension(0, 1),
            Height = new StyleDimension(180, 0)
        };
        _createView.SetPadding(10);
        _listView.Append(_createView);
    }

    public void RedrawList(TagCompound binary)
    {
        _listView.Clear();
        foreach (var compound in binary.Get<List<TagCompound>>(SteamUser.GetSteamID().m_SteamID.ToString()))
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
            _listView.Add(item);

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
                mp.Write((byte)SSC.ID.ChooseSSC);
                mp.Write(SteamUser.GetSteamID().m_SteamID);
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
                mp.Write(SteamUser.GetSteamID().m_SteamID);
                mp.Write(compound.Get<string>("name"));
                mp.Send();
            };
            removeView.OnUpdate += _ =>
            {
                if (removeView.IsMouseHovering) Main.instance.MouseText($"{Language.GetTextValue("UI.Delete")} (Right-Click)");
            };
            item.Append(removeView);
        }

        _listView.Add(_createView);
    }
}