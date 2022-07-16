using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.UI;

namespace SSC.Content.UI;

public class UIListItem : UIPanel
{
    PlayerFileData Data;

    UICharacter UICharacter;
    UIImageButton PlayButton;
    UIImageButton RenameButton;
    UIImageButton DeleteButton;


    public UIListItem(PlayerFileData data)
    {
        BorderColor = new Color(89, 116, 213) * 0.7f;
        Width.Set(0, 1);
        Height.Set(96, 0);
        SetPadding(6);

        Data = data;

        UICharacter = new UICharacter(Data.Player);
        UICharacter.Left.Set(4, 0);
        UICharacter.OnDoubleClick += delegate(UIMouseEvent evt, UIElement element) { };
        Append(UICharacter);

        var pixel = 4f;
        PlayButton = new UIImageButton(Main.Assets.Request<Texture2D>("Images/UI/ButtonPlay"));
        PlayButton.VAlign = 1;
        PlayButton.Left.Set(pixel, 0);
        PlayButton.OnClick += delegate(UIMouseEvent evt, UIElement element) { };
        Append(PlayButton);

        pixel += 24;
        RenameButton = new UIImageButton(Main.Assets.Request<Texture2D>("Images/UI/ButtonRename"));
        RenameButton.VAlign = 1;
        RenameButton.Left.Set(pixel, 0);
        RenameButton.OnClick += delegate(UIMouseEvent evt, UIElement element) { };
        Append(RenameButton);

        pixel += 24;
        DeleteButton = new UIImageButton(Main.Assets.Request<Texture2D>("Images/UI/ButtonDelete"));
        DeleteButton.HAlign = 1;
        DeleteButton.VAlign = 1;
        DeleteButton.Left.Set(pixel, 0);
        DeleteButton.OnClick += delegate(UIMouseEvent evt, UIElement element) { };
        Append(DeleteButton);
    }

    private void DrawPanel(SpriteBatch spriteBatch, Vector2 position, float width)
    {
        spriteBatch.Draw(Main.Assets.Request<Texture2D>("Images/UI/InnerPanelBackground").Value, position,
            new Rectangle?(new Rectangle(0, 0, 8,
                Main.Assets.Request<Texture2D>("Images/UI/InnerPanelBackground").Height())), Color.White);
        spriteBatch.Draw(Main.Assets.Request<Texture2D>("Images/UI/InnerPanelBackground").Value,
            new Vector2(position.X + 8f, position.Y),
            new Rectangle?(new Rectangle(8, 0, 8,
                Main.Assets.Request<Texture2D>("Images/UI/InnerPanelBackground").Height())), Color.White, 0.0f,
            Vector2.Zero, new Vector2((float)(((double)width - 16.0) / 8.0), 1f), SpriteEffects.None, 0.0f);
        spriteBatch.Draw(Main.Assets.Request<Texture2D>("Images/UI/InnerPanelBackground").Value,
            new Vector2((float)((double)position.X + (double)width - 8.0), position.Y),
            new Rectangle?(new Rectangle(16, 0, 8,
                Main.Assets.Request<Texture2D>("Images/UI/InnerPanelBackground").Height())), Color.White);
    }


    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);
        CalculatedStyle innerDimensions = GetInnerDimensions();
        CalculatedStyle dimensions = UICharacter.GetDimensions();
        float x = dimensions.X + dimensions.Width;
        Color color1 = Color.White;
        string text1 = Data.Name;
        if (Data.Player.loadStatus != 0)
        {
            color1 = Color.Gray;
            text1 = "(" + StatusID.Search.GetName(Data.Player.loadStatus) + ") " + text1;
        }

        Utils.DrawBorderString(spriteBatch, text1, new Vector2(x + 6f, dimensions.Y - 2f), color1);
        spriteBatch.Draw(Main.Assets.Request<Texture2D>("Images/UI/Divider").Value,
            new Vector2(x, innerDimensions.Y + 21f), new Rectangle?(),
            Color.White, 0.0f, Vector2.Zero,
            new Vector2(
                (float)(((double)this.GetDimensions().X + (double)this.GetDimensions().Width - (double)x) / 8.0), 1f),
            SpriteEffects.None, 0.0f);
        Vector2 vector2 = new Vector2(x + 6f, innerDimensions.Y + 29f);
        float width1 = 200f;
        Vector2 position1 = vector2;
        this.DrawPanel(spriteBatch, position1, width1);
        spriteBatch.Draw(TextureAssets.Heart.Value, position1 + new Vector2(5f, 2f), Color.White);
        position1.X += 10f + (float)TextureAssets.Heart.Width();
        Utils.DrawBorderString(spriteBatch,
            Data.Player.statLifeMax2.ToString() + Language.GetTextValue("GameUI.PlayerLifeMax"),
            position1 + new Vector2(0.0f, 3f), Color.White);
        position1.X += 65f;
        spriteBatch.Draw(TextureAssets.Mana.Value, position1 + new Vector2(5f, 2f), Color.White);
        position1.X += 10f + (float)TextureAssets.Mana.Width();
        Utils.DrawBorderString(spriteBatch,
            Data.Player.statManaMax2.ToString() + Language.GetTextValue("GameUI.PlayerManaMax"),
            position1 + new Vector2(0.0f, 3f), Color.White);
        vector2.X += width1 + 5f;
        Vector2 position2 = vector2;
        float width2 = 140f;
        if (GameCulture.FromCultureName(GameCulture.CultureName.Russian).IsActive)
            width2 = 180f;
        this.DrawPanel(spriteBatch, position2, width2);
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
                       new Vector2(
                           (float)((double)width2 * 0.5 -
                                   (double)FontAssets.MouseText.Value.MeasureString(text2).X * 0.5), 3f);
        Utils.DrawBorderString(spriteBatch, text2, pos1, color2);
        vector2.X += width2 + 5f;
        Vector2 position3 = vector2;
        float width3 = innerDimensions.X + innerDimensions.Width - position3.X;
        this.DrawPanel(spriteBatch, position3, width3);
        TimeSpan playTime = Data.GetPlayTime();
        int num = playTime.Days * 24 + playTime.Hours;
        string text3 = (num < 10 ? "0" : "") + num.ToString() + playTime.ToString("\\:mm\\:ss");
        Vector2 pos2 = position3 +
                       new Vector2(
                           (float)((double)width3 * 0.5 -
                                   (double)FontAssets.MouseText.Value.MeasureString(text3).X * 0.5), 3f);
        Utils.DrawBorderString(spriteBatch, text3, pos2, Color.White);
    }
}