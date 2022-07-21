// using System;
// using Microsoft.Xna.Framework;
// using Microsoft.Xna.Framework.Graphics;
// using Terraria;
// using Terraria.GameContent;
// using Terraria.GameContent.UI.Elements;
// using Terraria.IO;
// using Terraria.Localization;
// using Terraria.ModLoader;
// using Terraria.UI;
//
// namespace SSC.UI;
//
// public class UIListView : UIPanel
// {
//     PlayerFileData Data;
//
//     UICharacter UICharacter;
//     UIImageButton PlayButton;
//     UIImageButton DeleteButton;
//     UIText LeftText;
//     UIText RightText;
//
//     public UIListView(PlayerFileData data)
//     {
//         Data = data;
//
//         BorderColor = new Color(89, 116, 213) * 0.7f;
//         Width.Set(0, 1);
//         Height.Set(96, 0);
//         SetPadding(6);
//
//         UICharacter = new UICharacter(Data.Player);
//         UICharacter.Left.Set(4, 0);
//         Append(UICharacter);
//
//         PlayButton = new UIImageButton(Main.Assets.Request<Texture2D>("Images/UI/ButtonPlay"));
//         PlayButton.VAlign = 1;
//         PlayButton.Left.Set(4, 0);
//         PlayButton.OnMouseOver += (_, _) => LeftText.SetText(Language.GetTextValue("UI.Play"));
//         PlayButton.OnMouseOut += (_, _) => LeftText.SetText("");
//         PlayButton.OnClick += PlayerButtonClick;
//         Append(PlayButton);
//
//         LeftText = new UIText("");
//         LeftText.VAlign = 1;
//         LeftText.Left.Set(52, 0);
//         LeftText.Top.Set(-3, 0);
//         Append(LeftText);
//
//         RightText = new UIText("");
//         RightText.VAlign = 1;
//         RightText.HAlign = 1;
//         RightText.Left.Set(-30, 0);
//         RightText.Top.Set(-3, 0);
//         Append(RightText);
//
//         DeleteButton = new UIImageButton(Main.Assets.Request<Texture2D>("Images/UI/ButtonDelete"));
//         DeleteButton.HAlign = 1;
//         DeleteButton.VAlign = 1;
//         DeleteButton.OnMouseOver += (_, _) => RightText.SetText(Language.GetTextValue("UI.Delete"));
//         DeleteButton.OnMouseOut += (_, _) => RightText.SetText("");
//         DeleteButton.OnClick += DeleteButtonClick;
//         Append(DeleteButton);
//
//         UIImage element4 = new UIImage(Main.Assets.Request<Texture2D>("Images/UI/InnerPanelBackground"));
//         element4.Left.Set(74f, 0.0f);
//         element4.Top.Set(35f, 0.0f);
//         element4.Width.Set(200, 0);
//         element4.SetPadding(0.0f);
//         Append(element4);
//         string text1 = "";
//         Color color = Color.White;
//         switch (data.Player.difficulty)
//         {
//             case 0:
//                 text1 = Language.GetTextValue("UI.Softcore");
//                 break;
//             case 1:
//                 text1 = Language.GetTextValue("UI.Mediumcore");
//                 color = Main.mcColor;
//                 break;
//             case 2:
//                 text1 = Language.GetTextValue("UI.Hardcore");
//                 color = Main.hcColor;
//                 break;
//             case 3:
//                 text1 = Language.GetTextValue("UI.Creative");
//                 color = Main.creativeModeColor;
//                 break;
//         }
//
//         Main.LocalPlayer.ChatColor();
//
//         UIText element5 = new UIText(text1);
//         element5.TextColor = color;
//         element5.Left.Set((float)(70.0 - (double)FontAssets.MouseText.Value.MeasureString(text1).X * 0.5), 0.0f);
//         element5.Top.Set(5f, 0.0f);
//         element4.Append((UIElement)element5);
//
//
//         UIImage element6 = new UIImage(Main.Assets.Request<Texture2D>("Images/UI/InnerPanelBackground"));
//         element6.Left.Set(219f, 0.0f);
//         element6.Top.Set(35f, 0.0f);
//         element6.Width.Set(GameCulture.FromCultureName(GameCulture.CultureName.Russian).IsActive ? 180 : 140, 0);
//         element6.SetPadding(0.0f);
//         this.Append((UIElement)element6);
//         TimeSpan playTime = data.GetPlayTime();
//         int num = playTime.Days * 24 + playTime.Hours;
//         string text2 = (num < 10 ? "0" : "") + num.ToString() + playTime.ToString("\\:mm\\:ss");
//         UIText element7 = new UIText(text2);
//         element7.Left.Set((float)(55.0 - (double)FontAssets.MouseText.Value.MeasureString(text2).X * 0.5), 0.0f);
//         element7.Top.Set(5f, 0.0f);
//         element6.Append((UIElement)element7);
//
//         UIImage element8 = new UIImage(Main.Assets.Request<Texture2D>("Images/UI/Divider"));
//         element8.Left.Set(UICharacter.Left.Pixels + UICharacter.Width.Pixels, 0.0f);
//         element8.Top.Set(25f, 0.0f);
//         this.Append((UIElement)element8);
//         UIText element9 = new UIText(data.Name);
//         element9.Left.Set(75f, 0.0f);
//         element9.Top.Set(7f, 0.0f);
//         this.Append((UIElement)element9);
//     }
//
//     private void DeleteButtonClick(UIMouseEvent evt, UIElement listeningelement)
//     {
//         var P = ModContent.GetInstance<SSC>().GetPacket();
//         P.Write((int)SSC.PKG_ID.DeletePlayer);
//         P.Write(Main.clientUUID);
//         P.Write(Data.Name);
//         P.Send();
//     }
//
//     private void PlayerButtonClick(UIMouseEvent evt, UIElement listeningelement)
//     {
//         var P = ModContent.GetInstance<SSC>().GetPacket();
//         P.Write((int)SSC.PKG_ID.PlayPlayer);
//         P.Write(Main.clientUUID);
//         P.Write(Data.Name);
//         P.Send();
//     }
//
//     public override void MouseOver(UIMouseEvent evt)
//     {
//         base.MouseOver(evt);
//         BackgroundColor = new Color(73, 94, 171);
//         BorderColor = new Color(89, 116, 213);
//         UICharacter.SetAnimated(true);
//     }
//
//     public override void MouseOut(UIMouseEvent evt)
//     {
//         base.MouseOut(evt);
//         BackgroundColor = new Color(63, 82, 151) * 0.7f;
//         BorderColor = new Color(89, 116, 213) * 0.7f;
//         UICharacter.SetAnimated(false);
//     }
// }