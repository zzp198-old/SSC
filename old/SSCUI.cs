// using System.Collections.Generic;
// using Microsoft.Xna.Framework;
// using Microsoft.Xna.Framework.Graphics;
// using Terraria;
// using Terraria.Chat;
// using Terraria.GameContent.UI.Elements;
// using Terraria.ID;
// using Terraria.Localization;
// using Terraria.UI;
//
// namespace SSC;
//
// public class SSCUI : UIState
// {
//     internal static Player Unknown;
//     internal static UIList UIList;
//
//     internal static UIPanel CreatePanel;
//     internal static bool NameFocus;
//
//     public override void OnInitialize()
//     {
//         // 基础布局
//         var panel = new UIPanel();
//         panel.BorderColor = new Color(89, 116, 213);
//         panel.Width.Set(400, 0);
//         panel.Height.Set(500, 0);
//         panel.HAlign = panel.VAlign = 0.5f;
//         Append(panel);
//
//         UIList = new UIList();
//         UIList.Width.Set(-20, 1);
//         UIList.Height.Set(0, 1);
//         panel.Append(UIList);
//
//         var scrollbar = new UIScrollbar();
//         scrollbar.Height.Set(0, 1);
//         scrollbar.Left.Set(-15, 1);
//         panel.Append(scrollbar);
//
//         UIList.SetScrollbar(scrollbar);
//
//         // 角色创建界面
//         Unknown = new Player();
//
//         CreatePanel = new UIPanel();
//         CreatePanel.Width.Set(0, 1);
//         CreatePanel.Height.Set(180, 0);
//         CreatePanel.SetPadding(10);
//
//         var nameButton = new UICharacterNameButton(Language.GetText("UI.WorldCreationName"), LocalizedText.Empty);
//         var nameSearchBar = new UISearchBar(LocalizedText.Empty, 1);
//
//         nameButton.Width.Set(0, 1);
//         nameButton.Height.Set(40, 0);
//         nameButton.OnUpdate += _ =>
//         {
//             if (nameButton.IsMouseHovering)
//             {
//                 if (!NameFocus && Main.mouseLeft) // 被选中且无焦点的情况下被点击
//                 {
//                     nameSearchBar.ToggleTakingText();
//                     NameFocus = true;
//                 }
//             }
//             else if (NameFocus && Main.mouseLeft) // 有焦点但未被选中的情况下点击
//             {
//                 nameSearchBar.ToggleTakingText();
//                 NameFocus = false;
//             }
//         };
//         CreatePanel.Append(nameButton);
//
//         nameSearchBar.Width.Set(-50, 1);
//         nameSearchBar.Height.Set(40, 0);
//         nameSearchBar.Left.Set(50, 0);
//         nameSearchBar.OnMouseOver += (evt, _) => nameButton.MouseOver(evt); // 影响nameButton.IsMouseHovering
//         nameSearchBar.OnMouseOut += (evt, _) => nameButton.MouseOut(evt);
//         nameSearchBar.OnContentsChanged += i => Unknown.name = i;
//         CreatePanel.Append(nameSearchBar);
//
//         var difficultyButton0 = new UIDifficultyButton(Unknown, Lang.menu[26], null, PlayerDifficultyID.SoftCore, Color.Cyan);
//         difficultyButton0.Width.Set(-5, 0.5f);
//         difficultyButton0.Height.Set(26, 0);
//         difficultyButton0.Top.Set(50, 0);
//         CreatePanel.Append(difficultyButton0);
//
//         var difficultyButton1 = new UIDifficultyButton(Unknown, Lang.menu[25], null, PlayerDifficultyID.MediumCore, Main.mcColor);
//         difficultyButton1.Width.Set(-5, 0.5f);
//         difficultyButton1.Height.Set(26, 0);
//         difficultyButton1.Top.Set(50, 0);
//         difficultyButton1.Left.Set(5, 0.5f);
//         CreatePanel.Append(difficultyButton1);
//
//         var difficultyButton2 = new UIDifficultyButton(Unknown, Lang.menu[24], null, PlayerDifficultyID.Hardcore, Main.hcColor);
//         difficultyButton2.Width.Set(-5, 0.5f);
//         difficultyButton2.Height.Set(26, 0);
//         difficultyButton2.Top.Set(80, 0);
//         CreatePanel.Append(difficultyButton2);
//
//         var difficultyButton3 = new UIDifficultyButton(Unknown, Language.GetText("UI.Creative"), null, PlayerDifficultyID.Creative,
//             Main.creativeModeColor);
//         difficultyButton3.Width.Set(-5, 0.5f);
//         difficultyButton3.Height.Set(26, 0);
//         difficultyButton3.Top.Set(80, 0);
//         difficultyButton3.Left.Set(5, 0.5f);
//         CreatePanel.Append(difficultyButton3);
//
//         var createButton = new UITextPanel<LocalizedText>(Language.GetText("UI.Create"), 0.7f, true);
//         createButton.Width.Set(0, 1);
//         createButton.Height.Set(30, 0);
//         createButton.Top.Set(115, 0);
//         createButton.HAlign = 0.5f;
//         createButton.OnMouseOver += (_, _) =>
//         {
//             createButton.BackgroundColor = new Color(73, 94, 171);
//             createButton.BorderColor = Colors.FancyUIFatButtonMouseOver;
//         };
//         createButton.OnMouseOut += (_, _) =>
//         {
//             createButton.BackgroundColor = new Color(63, 82, 151) * 0.8f;
//             createButton.BorderColor = Color.Black;
//         };
//         createButton.OnClick += (_, _) =>
//         {
//             var mp = SSC.GetPacket(SSC.ID.CreateSSC);
//             mp.Write(SSC.SteamID);
//             mp.Write(Unknown.name);
//             mp.Write(Unknown.difficulty);
//             mp.Send();
//
//             nameSearchBar.SetContents("");
//             Unknown.difficulty = 0;
//         };
//         CreatePanel.Append(createButton);
//     }
//
//     public static void Refresh(List<(string name, byte difficulty)> items)
//     {
//         UIList.Clear();
//         if (items.Count == 0)
//         {
//             var panel = new UITextPanel<string>("No data!", 1, true);
//             panel.Width.Set(0, 1);
//             panel.Height.Set(50, 0);
//             UIList.Add(panel);
//         }
//         else
//         {
//             foreach (var (name, difficulty) in items)
//             {
//                 var panel = new UIPanel();
//                 panel.Width.Set(0, 1);
//                 panel.Height.Set(35, 0);
//                 panel.SetPadding(0);
//                 panel.PaddingTop = 7; // TODO
//                 UIList.Add(panel);
//
//                 var nameText = new UIText(name);
//                 nameText.Width.Set(0, 0.5f);
//                 nameText.Height.Set(0, 1);
//                 nameText.VAlign = 0.5f;
//                 panel.Append(nameText);
//
//                 var difficultyValue = "Unknown";
//                 var difficultyColor = Color.White;
//                 switch (difficulty)
//                 {
//                     case 0:
//                         difficultyValue = Language.GetTextValue("UI.Softcore");
//                         break;
//                     case 1:
//                         difficultyValue = Language.GetTextValue("UI.Mediumcore");
//                         difficultyColor = Main.mcColor;
//                         break;
//                     case 2:
//                         difficultyValue = Language.GetTextValue("UI.Hardcore");
//                         difficultyColor = Main.hcColor;
//                         break;
//                     case 3:
//                         difficultyValue = Language.GetTextValue("UI.Creative");
//                         difficultyColor = Main.creativeModeColor;
//                         break;
//                 }
//
//                 var difficultyText = new UIText(difficultyValue);
//                 difficultyText.Width.Set(0, 0.3f);
//                 difficultyText.Height.Set(0, 1);
//                 difficultyText.Left.Set(0, 0.5f);
//                 difficultyText.VAlign = 0.5f;
//                 difficultyText.TextColor = difficultyColor;
//                 panel.Append(difficultyText);
//
//                 var playButton = new UIImageButton(Main.Assets.Request<Texture2D>("Images/UI/ButtonPlay"));
//                 playButton.Width.Set(0, 0.1f);
//                 playButton.Height.Set(0, 1);
//                 playButton.Left.Set(0, 0.8f);
//                 playButton.VAlign = 0.5f;
//                 playButton.OnClick += (_, _) =>
//                 {
//                     var mp = SSC.GetPacket(SSC.ID.SelectSSC);
//                     mp.Write(SSC.SteamID);
//                     mp.Write(name);
//                     mp.Send();
//                 };
//                 panel.Append(playButton);
//
//                 var deleteButton = new UIImageButton(Main.Assets.Request<Texture2D>("Images/UI/ButtonDelete"));
//                 deleteButton.Width.Set(0, 0.1f);
//                 deleteButton.Height.Set(0, 1);
//                 deleteButton.Left.Set(0, 0.9f);
//                 deleteButton.VAlign = 0.5f;
//                 deleteButton.OnDoubleClick += (_, _) =>
//                 {
//                     var mp = SSC.GetPacket(SSC.ID.RemoveSSC);
//                     mp.Write(SSC.SteamID);
//                     mp.Write(name);
//                     mp.Send();
//                 };
//                 panel.Append(deleteButton);
//             }
//         }
//
//         UIList.Add(CreatePanel);
//     }
// }