using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.IO;
using Terraria.Localization;
using Terraria.UI;

namespace SSC.UIKit;

public class UICharacterListItem : UIPanel
{
    PlayerFileData _data;

    UICharacter _playerPanel;

    UIImageButton _playButton;
    UIText _buttonLabel;

    UIImageButton _deleteButton;


    public UICharacterListItem(PlayerFileData data)
    {
        BorderColor = new Color(89, 116, 213) * 0.7f;
        Height.Set(96, 0);
        Width.Set(0, 1);
        SetPadding(6);
        _data = data;

        _playerPanel = new UICharacter(data.Player);
        _playerPanel.Left.Set(4f, 0.0f);
        Append(_playerPanel);

        _playButton = new UIImageButton(Main.Assets.Request<Texture2D>("Images/UI/ButtonPlay"));
        _playButton.VAlign = 1;
        _playButton.Left.Set(4, 0);
        // TODO
        // _playButton.OnClick +=;
        _playButton.OnMouseOver += (_, _) => { _buttonLabel.SetText(Language.GetTextValue("UI.Play")); };
        _playButton.OnMouseOut += (_, _) => { _buttonLabel.SetText(""); };


//       element1.OnMouseOver += new UIElement.MouseEvent(this.PlayMouseOver);
//       element1.OnMouseOut += new UIElement.MouseEvent(this.ButtonMouseOut);
//       this.Append((UIElement) element1);
    }
}

//     private UIText _buttonLabel;
//     private UIText _deleteButtonLabel;
//     private ulong _fileSize;
//       this._dividerTexture = Main.Assets.Request<Texture2D>("Images/UI/Divider");
//       this._innerPanelTexture = Main.Assets.Request<Texture2D>("Images/UI/InnerPanelBackground");
//       this._buttonCloudActiveTexture = Main.Assets.Request<Texture2D>("Images/UI/ButtonCloudActive");
//       this._buttonCloudInactiveTexture = Main.Assets.Request<Texture2D>("Images/UI/ButtonCloudInactive");
//       this._buttonFavoriteActiveTexture = Main.Assets.Request<Texture2D>("Images/UI/ButtonFavoriteActive");
//       this._buttonFavoriteInactiveTexture = Main.Assets.Request<Texture2D>("Images/UI/ButtonFavoriteInactive");
//       this._buttonPlayTexture = Main.Assets.Request<Texture2D>("Images/UI/ButtonPlay");
//       this._buttonRenameTexture = Main.Assets.Request<Texture2D>("Images/UI/ButtonRename");
//       this._buttonDeleteTexture = Main.Assets.Request<Texture2D>("Images/UI/ButtonDelete");
//       this._errorTexture = UICommon.ButtonErrorTexture;
//       this._configTexture = UICommon.ButtonConfigTexture;
//       this._fileSize = (ulong) FileUtilities.GetFileSize(data.Path, data.IsCloudSave);

//       float pixels1 = 4f;
//       UIImageButton element1 = new UIImageButton(this._buttonPlayTexture);
//       element1.VAlign = 1f;
//       element1.Left.Set(pixels1, 0.0f);
//       element1.OnClick += new UIElement.MouseEvent(this.PlayGame);
//       element1.OnMouseOver += new UIElement.MouseEvent(this.PlayMouseOver);
//       element1.OnMouseOut += new UIElement.MouseEvent(this.ButtonMouseOut);
//       this.Append((UIElement) element1);
//       float pixels2 = pixels1 + 24f;
//       UIImageButton element2 = new UIImageButton(this._data.IsFavorite ? this._buttonFavoriteActiveTexture : this._buttonFavoriteInactiveTexture);
//       element2.VAlign = 1f;
//       element2.Left.Set(pixels2, 0.0f);
//       element2.OnClick += new UIElement.MouseEvent(this.FavoriteButtonClick);
//       element2.OnMouseOver += new UIElement.MouseEvent(this.FavoriteMouseOver);
//       element2.OnMouseOut += new UIElement.MouseEvent(this.ButtonMouseOut);
//       element2.SetVisibility(1f, this._data.IsFavorite ? 0.8f : 0.4f);
//       this.Append((UIElement) element2);
//       float pixels3 = pixels2 + 24f;
//       if (SocialAPI.Cloud != null)
//       {
//         UIImageButton element3 = new UIImageButton(this._data.IsCloudSave ? this._buttonCloudActiveTexture : this._buttonCloudInactiveTexture);
//         element3.VAlign = 1f;
//         element3.Left.Set(pixels3, 0.0f);
//         element3.OnClick += new UIElement.MouseEvent(this.CloudButtonClick);
//         element3.OnMouseOver += new UIElement.MouseEvent(this.CloudMouseOver);
//         element3.OnMouseOut += new UIElement.MouseEvent(this.ButtonMouseOut);
//         this.Append((UIElement) element3);
//         element3.SetSnapPoint("Cloud", snapPointIndex);
//         pixels3 += 24f;
//       }
//       UIImageButton element4 = new UIImageButton(this._buttonRenameTexture);
//       element4.VAlign = 1f;
//       element4.Left.Set(pixels3, 0.0f);
//       element4.OnClick += new UIElement.MouseEvent(this.RenameButtonClick);
//       element4.OnMouseOver += new UIElement.MouseEvent(this.RenameMouseOver);
//       element4.OnMouseOut += new UIElement.MouseEvent(this.ButtonMouseOut);
//       this.Append((UIElement) element4);
//       float num = pixels3 + 24f;
//       UIImageButton uiImageButton1 = new UIImageButton(this._buttonDeleteTexture);
//       uiImageButton1.VAlign = 1f;
//       uiImageButton1.HAlign = 1f;
//       UIImageButton element5 = uiImageButton1;
//       if (!this._data.IsFavorite)
//         element5.OnClick += new UIElement.MouseEvent(this.DeleteButtonClick);
//       element5.OnMouseOver += new UIElement.MouseEvent(this.DeleteMouseOver);
//       element5.OnMouseOut += new UIElement.MouseEvent(this.DeleteMouseOut);
//       this._deleteButton = element5;
//       this.Append((UIElement) element5);
//       float pixels4 = num + 4f;
//       if (data.customDataFail != null)
//       {
//         UIImageButton element6 = new UIImageButton(this._errorTexture);
//         element6.VAlign = 1f;
//         element6.HAlign = 1f;
//         element6.Left.Set(-24f, 0.0f);
//         element6.OnClick += new UIElement.MouseEvent(this.ErrorButtonClick);
//         element6.OnMouseOver += new UIElement.MouseEvent(this.ErrorMouseOver);
//         element6.OnMouseOut += new UIElement.MouseEvent(this.DeleteMouseOut);
//         this.Append((UIElement) element6);
//       }
//       if (data.Player.usedMods != null)
//       {
//         string[] array = ((IEnumerable<Mod>) Terraria.ModLoader.ModLoader.Mods).Select<Mod, string>((Func<Mod, string>) (m => m.Name)).ToArray<string>();
//         List<string> list1 = data.Player.usedMods.Except<string>((IEnumerable<string>) array).ToList<string>();
//         List<string> list2 = ((IEnumerable<string>) array).Except<string>((IEnumerable<string>) new string[1]
//         {
//           "ModLoader"
//         }).Except<string>((IEnumerable<string>) data.Player.usedMods).ToList<string>();
//         bool flag = Path.GetFileNameWithoutExtension(ModOrganizer.ModPackActive) != data.Player.modPack;
//         if (flag || list1.Count > 0 || list2.Count > 0)
//         {
//           UIText uiText = new UIText("");
//           uiText.VAlign = 0.0f;
//           uiText.HAlign = 1f;
//           UIText warningLabel = uiText;
//           warningLabel.Left.Set(-30f, 0.0f);
//           warningLabel.Top.Set(3f, 0.0f);
//           this.Append((UIElement) warningLabel);
//           UIImageButton uiImageButton2 = new UIImageButton(this._errorTexture);
//           uiImageButton2.VAlign = 0.0f;
//           uiImageButton2.HAlign = 1f;
//           UIImageButton element7 = uiImageButton2;
//           element7.Top.Set(-2f, 0.0f);
//           StringBuilder stringBuilder1 = new StringBuilder(Language.GetTextValue("tModLoader.ModsDifferentSinceLastPlay"));
//           StringBuilder stringBuilder2 = new StringBuilder();
//           if (flag)
//           {
//             string str = data.Player.modPack;
//             if (string.IsNullOrEmpty(str))
//               str = "None";
//             stringBuilder2.Append(Language.GetTextValue("tModLoader.ModPackMismatch", (object) str));
//             stringBuilder1.Append("\n" + Language.GetTextValue("tModLoader.ModPackMismatch", (object) str));
//           }
//           if (list1.Count > 0)
//           {
//             stringBuilder2.Append(list1.Count > 1 ? Language.GetTextValue("tModLoader.MissingXMods", (object) list1.Count) : Language.GetTextValue("tModLoader.Missing1Mod"));
//             stringBuilder1.Append("\n" + Language.GetTextValue("tModLoader.MissingModsListing", (object) string.Join("\n", list1.Select<string, string>((Func<string, string>) (x => "- " + x)))));
//           }
//           if (list2.Count > 0)
//           {
//             stringBuilder2.Append(" " + (list2.Count > 1 ? Language.GetTextValue("tModLoader.NewXMods", (object) list2.Count) : Language.GetTextValue("tModLoader.New1Mod")));
//             stringBuilder1.Append("\n" + Language.GetTextValue("tModLoader.NewModsListing", (object) string.Join("\n", list2.Select<string, string>((Func<string, string>) (x => "- " + x)))));
//           }
//           string warning = stringBuilder2.ToString();
//           string fullWarning = stringBuilder1.ToString();
//           element7.OnMouseOver += (UIElement.MouseEvent) ((a, b) => warningLabel.SetText(warning));
//           element7.OnMouseOut += (UIElement.MouseEvent) ((a, b) => warningLabel.SetText(""));
//           element7.OnClick += (UIElement.MouseEvent) ((a, b) => Interface.infoMessage.Show(fullWarning, 888, (UIState) Main._characterSelectMenu));
//           this.Append((UIElement) element7);
//         }
//       }
//       this._buttonLabel = new UIText("");
//       this._buttonLabel.VAlign = 1f;
//       this._buttonLabel.Left.Set(pixels4, 0.0f);
//       this._buttonLabel.Top.Set(-3f, 0.0f);
//       this.Append((UIElement) this._buttonLabel);
//       this._deleteButtonLabel = new UIText("");
//       this._deleteButtonLabel.VAlign = 1f;
//       this._deleteButtonLabel.HAlign = 1f;
//       this._deleteButtonLabel.Left.Set(data.customDataFail == null ? -30f : -54f, 0.0f);
//       this._deleteButtonLabel.Top.Set(-3f, 0.0f);
//       this.Append((UIElement) this._deleteButtonLabel);
//       element1.SetSnapPoint("Play", snapPointIndex);
//       element2.SetSnapPoint("Favorite", snapPointIndex);
//       element4.SetSnapPoint("Rename", snapPointIndex);
//       element5.SetSnapPoint("Delete", snapPointIndex);
//     }
//
//     private void RenameMouseOver(UIMouseEvent evt, UIElement listeningElement) => this._buttonLabel.SetText(Language.GetTextValue("UI.Rename"));
//
//     private void FavoriteMouseOver(UIMouseEvent evt, UIElement listeningElement)
//     {
//       if (this._data.IsFavorite)
//         this._buttonLabel.SetText(Language.GetTextValue("UI.Unfavorite"));
//       else
//         this._buttonLabel.SetText(Language.GetTextValue("UI.Favorite"));
//     }
//
//     private void CloudMouseOver(UIMouseEvent evt, UIElement listeningElement)
//     {
//       if (this._data.IsCloudSave)
//         this._buttonLabel.SetText(Language.GetTextValue("UI.MoveOffCloud"));
//       else if (!Steam.CheckSteamCloudStorageSufficient(this._fileSize))
//         this._buttonLabel.SetText(Language.GetTextValue("tModLoader.CloudWarning"));
//       else
//         this._buttonLabel.SetText(Language.GetTextValue("UI.MoveToCloud"));
//     }

//     private void DeleteMouseOver(UIMouseEvent evt, UIElement listeningElement)
//     {
//       if (this._data.IsFavorite)
//         this._deleteButtonLabel.SetText(Language.GetTextValue("UI.CannotDeleteFavorited"));
//       else
//         this._deleteButtonLabel.SetText(Language.GetTextValue("UI.Delete"));
//     }
//
//     private void ErrorMouseOver(UIMouseEvent evt, UIElement listeningElement) => this._deleteButtonLabel.SetText(this._data.customDataFail.modName + " Error");
//
//     private void ConfigMouseOver(UIMouseEvent evt, UIElement listeningElement) => this._buttonLabel.SetText("Edit Player Config");
//
//     private void DeleteMouseOut(UIMouseEvent evt, UIElement listeningElement) => this._deleteButtonLabel.SetText("");
//
//     private void ButtonMouseOut(UIMouseEvent evt, UIElement listeningElement) => this.;
//
//     private void RenameButtonClick(UIMouseEvent evt, UIElement listeningElement)
//     {
//       SoundEngine.PlaySound(10);
//       Main.clrInput();
//       UIVirtualKeyboard state = new UIVirtualKeyboard(Lang.menu[45].Value, "", new UIVirtualKeyboard.KeyboardSubmitEvent(this.OnFinishedSettingName), new Action(this.GoBackHere), allowEmpty: true);
//       state.SetMaxInputLength(20);
//       Main.MenuUI.SetState((UIState) state);
//       if (!(this.Parent.Parent is UIList parent))
//         return;
//       parent.UpdateOrder();
//     }
//
//     private void OnFinishedSettingName(string name)
//     {
//       string newName = name.Trim();
//       Main.menuMode = 10;
//       this._data.Rename(newName);
//       Main.OpenCharacterSelectUI();
//     }
//
//     private void GoBackHere() => Main.OpenCharacterSelectUI();
//
//     private void CloudButtonClick(UIMouseEvent evt, UIElement listeningElement)
//     {
//       if (this._data.IsCloudSave)
//       {
//         this._data.MoveToLocal();
//       }
//       else
//       {
//         Steam.RecalculateAvailableSteamCloudStorage();
//         if (!Steam.CheckSteamCloudStorageSufficient(this._fileSize))
//           return;
//         this._data.MoveToCloud();
//       }
//       ((UIImageButton) evt.Target).SetImage(this._data.IsCloudSave ? this._buttonCloudActiveTexture : this._buttonCloudInactiveTexture);
//       if (this._data.IsCloudSave)
//         this._buttonLabel.SetText(Language.GetTextValue("UI.MoveOffCloud"));
//       else
//         this._buttonLabel.SetText(Language.GetTextValue("UI.MoveToCloud"));
//     }
//
//     private void DeleteButtonClick(UIMouseEvent evt, UIElement listeningElement)
//     {
//       for (int index = 0; index < Main.PlayerList.Count; ++index)
//       {
//         if (Main.PlayerList[index] == this._data)
//         {
//           SoundEngine.PlaySound(10);
//           Main.selectedPlayer = index;
//           Main.menuMode = 5;
//           break;
//         }
//       }
//     }
//
//     private void PlayGame(UIMouseEvent evt, UIElement listeningElement)
//     {
//       if (listeningElement != evt.Target || this._data.Player.loadStatus != 0)
//         return;
//       Main.SelectPlayer(this._data);
//     }
//
//     private void FavoriteButtonClick(UIMouseEvent evt, UIElement listeningElement)
//     {
//       this._data.ToggleFavorite();
//       ((UIImageButton) evt.Target).SetImage(this._data.IsFavorite ? this._buttonFavoriteActiveTexture : this._buttonFavoriteInactiveTexture);
//       ((UIImageButton) evt.Target).SetVisibility(1f, this._data.IsFavorite ? 0.8f : 0.4f);
//       if (this._data.IsFavorite)
//       {
//         this._buttonLabel.SetText(Language.GetTextValue("UI.Unfavorite"));
//         this._deleteButton.OnClick -= new UIElement.MouseEvent(this.DeleteButtonClick);
//       }
//       else
//       {
//         this._buttonLabel.SetText(Language.GetTextValue("UI.Favorite"));
//         this._deleteButton.OnClick += new UIElement.MouseEvent(this.DeleteButtonClick);
//       }
//       if (!(this.Parent.Parent is UIList parent))
//         return;
//       parent.UpdateOrder();
//     }
//
//     private void ErrorButtonClick(UIMouseEvent evt, UIElement listeningElement) => Logging.Terraria.Error((object) Language.GetTextValue("tModLoader.PlayerCustomDataFail"), this._data.customDataFail.InnerException);
//
//     private void ConfigButtonClick(UIMouseEvent evt, UIElement listeningElement)
//     {
//     }
//
//     public override int CompareTo(object obj)
//     {
//       if (!(obj is UICharacterListItem characterListItem))
//         return base.CompareTo(obj);
//       if (this.IsFavorite && !characterListItem.IsFavorite)
//         return -1;
//       if (!this.IsFavorite && characterListItem.IsFavorite)
//         return 1;
//       return this._data.Name.CompareTo(characterListItem._data.Name) != 0 ? this._data.Name.CompareTo(characterListItem._data.Name) : this._data.GetFileName().CompareTo(characterListItem._data.GetFileName());
//     }
//
//     public override void MouseOver(UIMouseEvent evt)
//     {
//       base.MouseOver(evt);
//       this.BackgroundColor = new Color(73, 94, 171);
//       this.BorderColor = new Color(89, 116, 213);
//       this._playerPanel.SetAnimated(true);
//     }
//
//     public override void MouseOut(UIMouseEvent evt)
//     {
//       base.MouseOut(evt);
//       this.BackgroundColor = new Color(63, 82, 151) * 0.7f;
//       this.BorderColor = new Color(89, 116, 213) * 0.7f;
//       this._playerPanel.SetAnimated(false);
//     }
//
//     private void DrawPanel(SpriteBatch spriteBatch, Vector2 position, float width)
//     {
//       spriteBatch.Draw(this._innerPanelTexture.Value, position, new Rectangle?(new Rectangle(0, 0, 8, this._innerPanelTexture.Height())), Color.White);
//       spriteBatch.Draw(this._innerPanelTexture.Value, new Vector2(position.X + 8f, position.Y), new Rectangle?(new Rectangle(8, 0, 8, this._innerPanelTexture.Height())), Color.White, 0.0f, Vector2.Zero, new Vector2((float) (((double) width - 16.0) / 8.0), 1f), SpriteEffects.None, 0.0f);
//       spriteBatch.Draw(this._innerPanelTexture.Value, new Vector2((float) ((double) position.X + (double) width - 8.0), position.Y), new Rectangle?(new Rectangle(16, 0, 8, this._innerPanelTexture.Height())), Color.White);
//     }
//
//     protected override void DrawSelf(SpriteBatch spriteBatch)
//     {
//       base.DrawSelf(spriteBatch);
//       CalculatedStyle innerDimensions = this.GetInnerDimensions();
//       CalculatedStyle dimensions = this._playerPanel.GetDimensions();
//       float x = dimensions.X + dimensions.Width;
//       Color color1 = Color.White;
//       string text1 = this._data.Name;
//       if (this._data.Player.loadStatus != 0)
//       {
//         color1 = Color.Gray;
//         text1 = "(" + StatusID.Search.GetName(this._data.Player.loadStatus) + ") " + text1;
//       }
//       Utils.DrawBorderString(spriteBatch, text1, new Vector2(x + 6f, dimensions.Y - 2f), color1);
//       spriteBatch.Draw(this._dividerTexture.Value, new Vector2(x, innerDimensions.Y + 21f), new Rectangle?(), Color.White, 0.0f, Vector2.Zero, new Vector2((float) (((double) this.GetDimensions().X + (double) this.GetDimensions().Width - (double) x) / 8.0), 1f), SpriteEffects.None, 0.0f);
//       Vector2 vector2 = new Vector2(x + 6f, innerDimensions.Y + 29f);
//       float width1 = 200f;
//       Vector2 position1 = vector2;
//       this.DrawPanel(spriteBatch, position1, width1);
//       spriteBatch.Draw(TextureAssets.Heart.Value, position1 + new Vector2(5f, 2f), Color.White);
//       position1.X += 10f + (float) TextureAssets.Heart.Width();
//       Utils.DrawBorderString(spriteBatch, this._data.Player.statLifeMax2.ToString() + Language.GetTextValue("GameUI.PlayerLifeMax"), position1 + new Vector2(0.0f, 3f), Color.White);
//       position1.X += 65f;
//       spriteBatch.Draw(TextureAssets.Mana.Value, position1 + new Vector2(5f, 2f), Color.White);
//       position1.X += 10f + (float) TextureAssets.Mana.Width();
//       Utils.DrawBorderString(spriteBatch, this._data.Player.statManaMax2.ToString() + Language.GetTextValue("GameUI.PlayerManaMax"), position1 + new Vector2(0.0f, 3f), Color.White);
//       vector2.X += width1 + 5f;
//       Vector2 position2 = vector2;
//       float width2 = 140f;
//       if (GameCulture.FromCultureName(GameCulture.CultureName.Russian).IsActive)
//         width2 = 180f;
//       this.DrawPanel(spriteBatch, position2, width2);
//       string text2 = "";
//       Color color2 = Color.White;
//       switch (this._data.Player.difficulty)
//       {
//         case 0:
//           text2 = Language.GetTextValue("UI.Softcore");
//           break;
//         case 1:
//           text2 = Language.GetTextValue("UI.Mediumcore");
//           color2 = Main.mcColor;
//           break;
//         case 2:
//           text2 = Language.GetTextValue("UI.Hardcore");
//           color2 = Main.hcColor;
//           break;
//         case 3:
//           text2 = Language.GetTextValue("UI.Creative");
//           color2 = Main.creativeModeColor;
//           break;
//       }
//       Vector2 pos1 = position2 + new Vector2((float) ((double) width2 * 0.5 - (double) FontAssets.MouseText.Value.MeasureString(text2).X * 0.5), 3f);
//       Utils.DrawBorderString(spriteBatch, text2, pos1, color2);
//       vector2.X += width2 + 5f;
//       Vector2 position3 = vector2;
//       float width3 = innerDimensions.X + innerDimensions.Width - position3.X;
//       this.DrawPanel(spriteBatch, position3, width3);
//       TimeSpan playTime = this._data.GetPlayTime();
//       int num = playTime.Days * 24 + playTime.Hours;
//       string text3 = (num < 10 ? "0" : "") + num.ToString() + playTime.ToString("\\:mm\\:ss");
//       Vector2 pos2 = position3 + new Vector2((float) ((double) width3 * 0.5 - (double) FontAssets.MouseText.Value.MeasureString(text3).X * 0.5), 3f);
//       Utils.DrawBorderString(spriteBatch, text3, pos2, Color.White);
//     }
//   }
// }