using Microsoft.Xna.Framework;
using Steamworks;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.UI;

namespace SSC.Kit;

public class CreateView : UIPanel
{
    private bool _inputState;

    public override void OnActivate()
    {
        var dummy = new Player();

        var nameView = new UICharacterNameButton(Language.GetText("UI.WorldCreationName"), LocalizedText.Empty)
        {
            Width = new StyleDimension(0, 1),
            Height = new StyleDimension(40, 0)
        };
        var nameInput = new UISearchBar(LocalizedText.Empty, 1)
        {
            Width = new StyleDimension(-50, 1),
            Height = new StyleDimension(40, 0),
            Left = new StyleDimension(50, 0)
        };

        nameView.OnUpdate += _ =>
        {
            if (nameView.IsMouseHovering)
            {
                if (!_inputState && Main.mouseLeft)
                {
                    nameInput.ToggleTakingText();
                    _inputState = true;
                }
            }
            else if (_inputState && Main.mouseLeft)
            {
                nameInput.ToggleTakingText();
                _inputState = false;
            }
        };
        nameInput.OnMouseOver += (evt, _) => nameView.MouseOver(evt);
        nameInput.OnMouseOut += (evt, _) => nameView.MouseOut(evt);
        nameInput.OnContentsChanged += name => dummy.name = name;

        Append(nameView);
        Append(nameInput);

        Append(new UIDifficultyButton(dummy, Lang.menu[26], null, PlayerDifficultyID.SoftCore, Color.Cyan)
        {
            Width = new StyleDimension(-5, 0.5f),
            Height = new StyleDimension(26, 0),
            Top = new StyleDimension(50, 0)
        });
        Append(new UIDifficultyButton(dummy, Lang.menu[25], null, PlayerDifficultyID.MediumCore, Main.mcColor)
        {
            Width = new StyleDimension(-5, 0.5f),
            Height = new StyleDimension(26, 0),
            Top = new StyleDimension(50, 0),
            Left = new StyleDimension(5, 0.5f)
        });
        Append(new UIDifficultyButton(dummy, Lang.menu[24], null, PlayerDifficultyID.Hardcore, Main.hcColor)
        {
            Width = new StyleDimension(-5, 0.5f),
            Height = new StyleDimension(26, 0),
            Top = new StyleDimension(80, 0)
        });
        Append(new UIDifficultyButton(dummy, Language.GetText("UI.Creative"), null, PlayerDifficultyID.Creative, Main.creativeModeColor)
        {
            Width = new StyleDimension(-5, 0.5f),
            Height = new StyleDimension(26, 0),
            Top = new StyleDimension(80, 0),
            Left = new StyleDimension(5, 0.5f)
        });

        var btn = new UITextPanel<LocalizedText>(Language.GetText("UI.Create"), 0.7f, true)
        {
            Width = new StyleDimension(0, 1),
            Height = new StyleDimension(30, 0),
            Top = new StyleDimension(115, 0),
            HAlign = 0.5f
        };
        btn.OnMouseOver += (_, _) =>
        {
            btn.BackgroundColor = new Color(73, 94, 171);
            btn.BorderColor = Colors.FancyUIFatButtonMouseOver;
        };
        btn.OnMouseOut += (_, _) =>
        {
            btn.BackgroundColor = new Color(63, 82, 151) * 0.8f;
            btn.BorderColor = Color.Black;
        };
        btn.OnClick += (_, _) =>
        {
            var mp = SSC.Mod.GetPacket();
            mp.Write((byte)SSC.ID.CreateSSC);
            mp.Write(SteamUser.GetSteamID().m_SteamID);
            mp.Write(dummy.name);
            mp.Write(dummy.difficulty);
            mp.Send();

            nameInput.SetContents("");
            dummy.difficulty = 0;
        };
        Append(btn);
    }
}