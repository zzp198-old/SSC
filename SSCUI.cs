// using System.IO;
// using Microsoft.Xna.Framework;
// using SSC.UI;
// using Terraria;
// using Terraria.GameContent.UI.Elements;
// using Terraria.ID;
// using Terraria.IO;
// using Terraria.Localization;
// using Terraria.ModLoader;
// using Terraria.ModLoader.IO;
// using Terraria.UI;
//
// namespace SSC;
//
// public class SSCUI : UIState
// {
//     UIPanel UIPanel;
//     UIList UIList;
//     UIScrollbar UIScrollbar;
//
//     Player pendingPlayer;
//     UITextBox textBox;
//     UIDifficultyButton difficultyButton1;
//     UIDifficultyButton difficultyButton2;
//     UIDifficultyButton difficultyButton3;
//     UIDifficultyButton difficultyButton4;
//     UITextPanel<LocalizedText> CreateButton;
//
//     public override void OnInitialize()
//     {
//         UIPanel = new UIPanel();
//         UIPanel.Width.Set(450, 0);
//         UIPanel.Height.Set(1040, 0);
//         UIPanel.HAlign = UIPanel.VAlign = 0.5f;
//         UIPanel.BackgroundColor = new Color(33, 43, 79);
//         Append(UIPanel);
//
//         UIList = new UIList();
//         UIList.Width.Set(0, 1);
//         UIList.Height.Set(640, 0);
//         UIPanel.Append(UIList);
//
//         UIScrollbar = new UIScrollbar();
//         UIScrollbar.SetView(100, 1000);
//         UIScrollbar.Height.Set(0, 1);
//         UIScrollbar.HAlign = 1;
//         UIList.SetScrollbar(UIScrollbar);
//         UIPanel.Append(UIScrollbar);
//
//         pendingPlayer = new Player();
//         var difficultyButton1 =
//             new UIDifficultyButton(pendingPlayer, Lang.menu[26], Lang.menu[31], (byte)0, Color.Cyan);
//         difficultyButton1.Left.Set(0, 0);
//         difficultyButton1.Top.Set(640, 0);
//         difficultyButton1.Width.Set(200, 0);
//         difficultyButton1.Width.Set(50, 0);
//         var difficultyButton2 =
//             new UIDifficultyButton(pendingPlayer, Lang.menu[25], Lang.menu[30], (byte)1, Main.mcColor);
//         difficultyButton2.Left.Set(225, 0);
//         difficultyButton2.Top.Set(640, 0);
//         difficultyButton2.Width.Set(200, 0);
//         difficultyButton2.Width.Set(50, 0);
//         var difficultyButton3 =
//             new UIDifficultyButton(pendingPlayer, Lang.menu[24], Lang.menu[29], (byte)2, Main.hcColor);
//         difficultyButton3.Left.Set(0, 0);
//         difficultyButton3.Top.Set(740, 0);
//         difficultyButton3.Width.Set(200, 0);
//         difficultyButton3.Width.Set(50, 0);
//         var difficultyButton4 = new UIDifficultyButton(pendingPlayer, Language.GetText("UI.Creative"),
//             Language.GetText("UI.CreativeDescriptionPlayer"), (byte)3, Main.creativeModeColor);
//         difficultyButton4.Left.Set(225, 0);
//         difficultyButton4.Top.Set(740, 0);
//         difficultyButton4.Width.Set(200, 0);
//         difficultyButton4.Width.Set(50, 0);
//         UIPanel.Append(difficultyButton1);
//         UIPanel.Append(difficultyButton2);
//         UIPanel.Append(difficultyButton3);
//         UIPanel.Append(difficultyButton4);
//
//         textBox = new UITextBox("", 0.78f, true);
//         textBox.BackgroundColor = Color.Transparent;
//         textBox.BorderColor = Color.Transparent;
//         textBox.HAlign = 0.5f;
//         textBox.Width.Pixels = 225;
//         textBox.Top.Set(840, 0);
//         textBox.Height.Pixels = 37f;
//         Append(textBox);
//
//         var CreateButton = new UITextPanel<LocalizedText>(Language.GetText("UI.Create"), 0.7f, true);
//         CreateButton.Width = StyleDimension.FromPixelsAndPercent(-10f, 0.5f);
//         CreateButton.Height = StyleDimension.FromPixels(50f);
//         CreateButton.Top.Set(940, 0);
//         CreateButton.Left.Set(225, 0);
//         CreateButton.OnMouseOver += FadedMouseOver;
//         CreateButton.OnMouseOut += FadedMouseOut;
//         CreateButton.OnMouseDown += CreateButtonClick;
//         Append(CreateButton);
//     }
//
//     private void CreateButtonClick(UIMouseEvent evt, UIElement listeningelement)
//     {
//         var data = new PlayerFileData(Path.Combine(SSC.CachePath, textBox.Text + ".plr"), false);
//         data.Metadata = FileMetadata.FromCurrentSettings(FileType.Player);
//         pendingPlayer.name = textBox.Text;
//         data.Player = pendingPlayer;
//         Player.SavePlayer(data, true);
//         var P = ModContent.GetInstance<SSC>().GetPacket();
//         P.Write((int)SSC.PKG_ID.CreatePlayer);
//         var tag = new TagCompound();
//         tag.Add("plr", File.ReadAllBytes(Path.Combine(SSC.CachePath, textBox.Text + ".plr")));
//         if (File.Exists(Path.Combine(SSC.CachePath, textBox.Text + ".tplr")))
//         {
//             tag.Add("tplr", File.ReadAllBytes(Path.Combine(SSC.CachePath, textBox.Text + ".tplr")));
//         }
//
//         P.Write(Main.clientUUID);
//         TagIO.WriteTag(textBox.Text, tag, P);
//         P.Send();
//     }
//
//     private void FadedMouseOver(UIMouseEvent evt, UIElement listeningElement)
//     {
//         ((UIPanel)evt.Target).BackgroundColor = new Color(73, 94, 171);
//         ((UIPanel)evt.Target).BorderColor = Colors.FancyUIFatButtonMouseOver;
//     }
//
//     private void FadedMouseOut(UIMouseEvent evt, UIElement listeningElement)
//     {
//         ((UIPanel)evt.Target).BackgroundColor = new Color(63, 82, 151) * 0.7f;
//         ((UIPanel)evt.Target).BorderColor = Color.Black;
//     }
//
//     public override void OnActivate()
//     {
//         UIList.Clear();
//         var files = Directory.GetFiles(SSC.CachePath, "*.plr");
//         foreach (var file in files)
//         {
//             var fileData = Player.GetFileData(file, false);
//             if (fileData != null)
//             {
//                 UIList.Add(new UIListView(fileData));
//             }
//         }
//     }
// }