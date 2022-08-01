using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Steamworks;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace SSC;

public class UIPlayerListItem : UIPanel
{
    Asset<Texture2D> _innerPanelTexture = Main.Assets.Request<Texture2D>("Images/UI/InnerPanelBackground");

    PlayerFileData Data;

    UICharacter PlayerPanel;
    UIImageButton PlayButton;
    UIImageButton DeleteButton;

    public UIPlayerListItem(PlayerFileData data)
    {
        Data = data;
        BorderColor = new Color(89, 116, 213) * 0.7f;
        Width.Set(0, 1);
        Height.Set(96, 0);
        SetPadding(6);

        PlayerPanel = new UICharacter(Data.Player);
        PlayerPanel.Left.Set(4, 0);
        Append(PlayerPanel);

        PlayButton = new UIImageButton(Main.Assets.Request<Texture2D>("Images/UI/ButtonPlay"));
        PlayButton.Left.Set(4, 0);
        PlayButton.VAlign = 1;
        PlayButton.OnClick += (_, _) =>
        {
            if (!Main.LocalPlayer.GetModPlayer<SSCPlayer>().Selected)
            {
                var p = ModContent.GetInstance<SSC>().GetPacket();
                p.Write((byte)PID.SelectPLR);
                p.Write(SteamUser.GetSteamID().m_SteamID);
                p.Write(Data.Player.name);
                p.Send();
                p = ModContent.GetInstance<SSC>().GetPacket();
                p.Write((byte)PID.PLRList);
                p.Write(SteamUser.GetSteamID().m_SteamID);
                p.Send();
            }
            else
            {
                Utils.Boot(Main.myPlayer, "Prohibit repeated login");
            }
        };
        Append(PlayButton);

        DeleteButton = new UIImageButton(Main.Assets.Request<Texture2D>("Images/UI/ButtonDelete"));
        DeleteButton.VAlign = 1f;
        DeleteButton.HAlign = 1f;
        DeleteButton.OnClick += (_, _) =>
        {
            var p = ModContent.GetInstance<SSC>().GetPacket();
            p.Write((byte)PID.DeletePLR);
            p.Write(SteamUser.GetSteamID().m_SteamID);
            p.Write(Data.Player.name);
            p.Send();
            p = ModContent.GetInstance<SSC>().GetPacket();
            p.Write((byte)PID.PLRList);
            p.Write(SteamUser.GetSteamID().m_SteamID);
            p.Send();
        };
        Append(DeleteButton);
    }

    public override void DoubleClick(UIMouseEvent evt)
    {
        PlayButton.Click(evt);
    }

    private void DrawPanel(SpriteBatch spriteBatch, Vector2 position, float width)
    {
        spriteBatch.Draw(_innerPanelTexture.Value, position, new Rectangle(0, 0, 8, _innerPanelTexture.Height()), Color.White);
        spriteBatch.Draw(_innerPanelTexture.Value, new Vector2(position.X + 8f, position.Y),
            new Rectangle(8, 0, 8, _innerPanelTexture.Height()), Color.White, 0.0f, Vector2.Zero,
            new Vector2((float)((width - 16.0) / 8.0), 1f), SpriteEffects.None, 0.0f);
        spriteBatch.Draw(_innerPanelTexture.Value, new Vector2((float)(position.X + (double)width - 8.0), position.Y),
            new Rectangle(16, 0, 8, _innerPanelTexture.Height()), Color.White);
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);
        CalculatedStyle innerDimensions = this.GetInnerDimensions();
        CalculatedStyle dimensions = PlayerPanel.GetDimensions();
        float x = dimensions.X + dimensions.Width;
        Color color1 = Color.White;
        string text1 = Data.Name;
        Terraria.Utils.DrawBorderString(spriteBatch, text1, new Vector2(x + 6f, dimensions.Y - 2f), color1);
        spriteBatch.Draw(Main.Assets.Request<Texture2D>("Images/UI/Divider").Value, new Vector2(x, innerDimensions.Y + 21f),
            new Rectangle?(), Color.White, 0.0f,
            Vector2.Zero,
            new Vector2((float)(((double)this.GetDimensions().X + (double)this.GetDimensions().Width - (double)x) / 8.0), 1f),
            SpriteEffects.None, 0.0f);
        Vector2 vector2 = new Vector2(x + 6f, innerDimensions.Y + 29f);
        float width1 = 0;
        Vector2 position1 = vector2;
        DrawPanel(spriteBatch, position1, width1);
        vector2.X += width1 + 5f;
        Vector2 position2 = vector2;
        float width2 = 140f;
        if (GameCulture.FromCultureName(GameCulture.CultureName.Russian).IsActive)
            width2 = 180f;
        DrawPanel(spriteBatch, position2, width2);
        string text2 = "";
        Color color2 = Color.White;
        switch (Data.Player.difficulty)
        {
            case 0:
                text2 = Language.GetTextValue("UI.Softcore");
                break;
            case 1:
                text2 = Language.GetTextValue("UI.Mediumcore");
                color2 = Main.mcColor;
                break;
            case 2:
                text2 = Language.GetTextValue("UI.Hardcore");
                color2 = Main.hcColor;
                break;
            case 3:
                text2 = Language.GetTextValue("UI.Creative");
                color2 = Main.creativeModeColor;
                break;
        }

        Vector2 pos1 = position2 +
                       new Vector2((float)(width2 * 0.5 - FontAssets.MouseText.Value.MeasureString(text2).X * 0.5), 3f);
        Terraria.Utils.DrawBorderString(spriteBatch, text2, pos1, color2);
        vector2.X += width2 + 5f;
        Vector2 position3 = vector2;
        float width3 = innerDimensions.X + innerDimensions.Width - position3.X;
        this.DrawPanel(spriteBatch, position3, width3);
        TimeSpan playTime = Data.GetPlayTime();
        int num = playTime.Days * 24 + playTime.Hours;
        var text3 = (num < 10 ? "0" : "") + num + playTime.ToString("\\:mm\\:ss");
        var pos2 = position3 + new Vector2((float)(width3 * 0.5 - FontAssets.MouseText.Value.MeasureString(text3).X * 0.5), 3f);
        Terraria.Utils.DrawBorderString(spriteBatch, text3, pos2, Color.White);
    }
}