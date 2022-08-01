using IL.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Steamworks;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.IO;

namespace SSC.UIKit;

public class UIPlayerListItem : UIPanel
{
    PlayerFileData Data;
    UICharacter PlayerPanel;

    UIImageButton DeleteButton;


    public UIPlayerListItem(PlayerFileData data)
    {
        Data = data;
    }

    public override void OnInitialize()
    {
        BorderColor = new Color(89, 116, 213) * 0.7f;
        Height.Set(96, 0);
        Width.Set(0, 1);
        SetPadding(6);

        PlayerPanel = new UICharacter(Data.Player);
        PlayerPanel.Left.Set(4, 0);
        Append(PlayerPanel);

        DeleteButton = new UIImageButton(Main.Assets.Request<Texture2D>("Images/UI/ButtonDelete"));
        DeleteButton.VAlign = 1f;
        DeleteButton.HAlign = 1f;
        DeleteButton.OnClick += (_, _) =>
        {
            NETCore.C_DeletePLR(SteamUser.GetSteamID().m_SteamID, Data.Player.name);
            NETCore.C_ObtainPLR(SteamUser.GetSteamID().m_SteamID);
        };
        Append(DeleteButton);
    }
}