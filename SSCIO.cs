// using System;
// using System.IO;
// using System.Linq;
// using Microsoft.Xna.Framework;
// using Terraria;
// using Terraria.GameContent.Creative;
// using Terraria.ModLoader.IO;
//
// namespace SSC
// {
//     public static class SSCIO
//     {
//         public static void Reset(Player player)
//         {
//             player.hair = 0;
//             player.hairDye = 0;
//             player.hideVisibleAccessory.Initialize();
//             player.hideMisc = 0;
//             player.skinVariant = 0;
//             player.statLife = player.statLifeMax = SSC.SSCConfig.StartingLife;
//             player.statMana = player.statManaMax = SSC.SSCConfig.StartingMana;
//             player.extraAccessory = false;
//             player.unlockedBiomeTorches = false;
//             player.UsingBiomeTorches = false;
//             player.downedDD2EventAnyDifficulty = false;
//             player.taxMoney = 0;
//             player.hairColor = new Color(215, 90, 55);
//             player.skinColor = new Color(255, 125, 90);
//             player.eyeColor = new Color(105, 90, 75);
//             player.shirtColor = new Color(175, 165, 140);
//             player.underShirtColor = new Color(160, 180, 215);
//             player.pantsColor = new Color(255, 230, 175);
//             player.shoeColor = new Color(160, 105, 60);
//             Array.ForEach(player.armor, item => item.SetDefaults());
//             Array.ForEach(player.dye, item => item.SetDefaults());
//             Array.ForEach(player.miscEquips, item => item.SetDefaults());
//             Array.ForEach(player.miscDyes, item => item.SetDefaults());
//             Array.ForEach(player.inventory, item => item.SetDefaults());
//             for (var i = 0; i < SSC.SSCConfig.StartingInventory.Count; i++)
//                 player.inventory[i].SetDefaults(SSC.SSCConfig.StartingInventory[i].Type);
//             Array.ForEach(player.bank.item, item => item.SetDefaults());
//             Array.ForEach(player.bank2.item, item => item.SetDefaults());
//             Array.ForEach(player.bank3.item, item => item.SetDefaults());
//             Array.ForEach(player.bank4.item, item => item.SetDefaults());
//             player.voidVaultInfo = 0;
//             player.SpawnX = -1;
//             player.SpawnY = -1;
//             player.hbLocked = false;
//             player.hideInfo.Initialize();
//             player.anglerQuestsFinished = 0;
//             player.DpadRadial.Bindings.Initialize();
//             player.builderAccStatus.Initialize();
//             player.bartenderQuestLog = 0;
//             player.dead = false;
//             player.respawnTimer = 0;
//             player.lastTimePlayerWasSaved = 0;
//             player.golferScoreAccumulated = 0;
//             player.creativeTracker.Reset();
//             Player.ClearPlayerTempInfo();
//             CreativePowerManager.Instance.ResetDataForNewPlayer(player);
//             PlayerIO.LoadModData(player, PlayerIO.SaveModData(Main.player[byte.MaxValue]));
//             PlayerIO.LoadModBuffs(player, PlayerIO.SaveModBuffs(Main.player[byte.MaxValue]));
//             PlayerIO.LoadInfoDisplays(player, PlayerIO.SaveInfoDisplays(Main.player[byte.MaxValue]));
//             PlayerIO.LoadUsedMods(player, PlayerIO.SaveUsedMods(Main.player[byte.MaxValue]));
//         }
//
//         public static void Load(Player player, TagCompound tagCompound)
//         {
//             if (tagCompound.ContainsKey("Hair"))
//                 player.hair = tagCompound.GetInt("Hair");
//             if (tagCompound.ContainsKey("HairDye"))
//                 player.hairDye = tagCompound.GetByte("HairDye");
//             if (tagCompound.ContainsKey("HideVisibleAccessory"))
//                 player.hideVisibleAccessory = tagCompound.GetList<bool>("HideVisibleAccessory").ToArray();
//             if (tagCompound.ContainsKey("HideMisc"))
//                 player.hideMisc = tagCompound.GetByte("HideMisc");
//             if (tagCompound.ContainsKey("SkinVariant"))
//                 player.skinVariant = tagCompound.GetInt("skinVariant");
//             if (tagCompound.ContainsKey("StatLife"))
//                 player.statLife = tagCompound.GetInt("StatLife");
//             if (tagCompound.ContainsKey("StatLifeMax"))
//                 player.statLifeMax = tagCompound.GetInt("StatLifeMax");
//             if (tagCompound.ContainsKey("StatMana"))
//                 player.statMana = tagCompound.GetInt("StatMana");
//             if (tagCompound.ContainsKey("StatManaMax"))
//                 player.statManaMax = tagCompound.GetInt("StatManaMax");
//             if (tagCompound.ContainsKey("ExtraAccessory"))
//                 player.extraAccessory = tagCompound.GetBool("ExtraAccessory");
//             if (tagCompound.ContainsKey("UnlockedBiomeTorches"))
//                 player.unlockedBiomeTorches = tagCompound.GetBool("UnlockedBiomeTorches");
//             if (tagCompound.ContainsKey("UsingBiomeTorches"))
//                 player.UsingBiomeTorches = tagCompound.GetBool("UsingBiomeTorches");
//             if (tagCompound.ContainsKey("DownedDD2EventAnyDifficulty"))
//                 player.downedDD2EventAnyDifficulty = tagCompound.GetBool("DownedDD2EventAnyDifficulty");
//             if (tagCompound.ContainsKey("TaxMoney"))
//                 player.taxMoney = tagCompound.GetInt("TaxMoney");
//             if (tagCompound.ContainsKey("HairColor"))
//                 player.hairColor = tagCompound.Get<Color>("HairColor");
//             if (tagCompound.ContainsKey("SkinColor"))
//                 player.skinColor = tagCompound.Get<Color>("SkinColor");
//             if (tagCompound.ContainsKey("EyeColor"))
//                 player.eyeColor = tagCompound.Get<Color>("EyeColor");
//             if (tagCompound.ContainsKey("ShirtColor"))
//                 player.shirtColor = tagCompound.Get<Color>("ShirtColor");
//             if (tagCompound.ContainsKey("UnderShirtColor"))
//                 player.underShirtColor = tagCompound.Get<Color>("UnderShirtColor");
//             if (tagCompound.ContainsKey("PantsColor"))
//                 player.pantsColor = tagCompound.Get<Color>("PantsColor");
//             if (tagCompound.ContainsKey("ShoeColor"))
//                 player.shoeColor = tagCompound.Get<Color>("ShoeColor");
//             if (tagCompound.ContainsKey("Armor"))
//                 PlayerIO.LoadInventory(player.armor, tagCompound.GetList<TagCompound>("Armor"));
//             if (tagCompound.ContainsKey("Dye"))
//                 PlayerIO.LoadInventory(player.dye, tagCompound.GetList<TagCompound>("Dye"));
//             if (tagCompound.ContainsKey("MiscEquips"))
//                 PlayerIO.LoadInventory(player.miscEquips, tagCompound.GetList<TagCompound>("MiscEquips"));
//             if (tagCompound.ContainsKey("MiscDyes"))
//                 PlayerIO.LoadInventory(player.miscDyes, tagCompound.GetList<TagCompound>("MiscDyes"));
//             if (tagCompound.ContainsKey("Inventory"))
//                 PlayerIO.LoadInventory(player.inventory, tagCompound.GetList<TagCompound>("Inventory"));
//             if (tagCompound.ContainsKey("Bank"))
//                 PlayerIO.LoadInventory(player.bank.item, tagCompound.GetList<TagCompound>("Bank"));
//             if (tagCompound.ContainsKey("Bank2"))
//                 PlayerIO.LoadInventory(player.bank2.item, tagCompound.GetList<TagCompound>("Bank2"));
//             if (tagCompound.ContainsKey("Bank3"))
//                 PlayerIO.LoadInventory(player.bank3.item, tagCompound.GetList<TagCompound>("Bank3"));
//             if (tagCompound.ContainsKey("Bank4"))
//                 PlayerIO.LoadInventory(player.bank4.item, tagCompound.GetList<TagCompound>("Bank4"));
//             if (tagCompound.ContainsKey("VoidVaultInfo"))
//                 player.voidVaultInfo = tagCompound.GetByte("VoidVaultInfo");
//             if (tagCompound.ContainsKey("SpawnX ="))
//                 player.SpawnX = tagCompound.GetInt("SpawnX =");
//             if (tagCompound.ContainsKey("SpawnY"))
//                 player.SpawnY = tagCompound.GetInt("SpawnY");
//             if (tagCompound.ContainsKey("HbLocked"))
//                 player.hbLocked = tagCompound.GetBool("HbLocked");
//             if (tagCompound.ContainsKey("HideInfo"))
//                 player.hideInfo = tagCompound.GetList<bool>("HideInfo").ToArray();
//             if (tagCompound.ContainsKey("AnglerQuestsFinished"))
//                 player.anglerQuestsFinished = tagCompound.GetInt("AnglerQuestsFinished");
//             if (tagCompound.ContainsKey("DpadRadial"))
//                 player.DpadRadial.Bindings = tagCompound.GetIntArray("DpadRadial");
//             if (tagCompound.ContainsKey("BuilderAccStatus"))
//                 player.builderAccStatus = tagCompound.GetIntArray("BuilderAccStatus");
//             if (tagCompound.ContainsKey("BartenderQuestLog"))
//                 player.bartenderQuestLog = tagCompound.GetInt("BartenderQuestLog");
//             if (tagCompound.ContainsKey("Dead"))
//                 player.dead = tagCompound.GetBool("Dead");
//             if (tagCompound.ContainsKey("RespawnTimer"))
//                 player.respawnTimer = tagCompound.GetInt("RespawnTimer");
//             if (tagCompound.ContainsKey("GolferScoreAccumulated"))
//                 player.golferScoreAccumulated = tagCompound.GetInt("GolferScoreAccumulated");
//             if (tagCompound.ContainsKey("CreativeTracker"))
//                 player.creativeTracker.Load(new BinaryReader(new MemoryStream(tagCompound.GetByteArray("CreativeTracker"))), 0);
//             if (tagCompound.ContainsKey("MouseItem"))
//                 ItemIO.Load(Main.mouseItem, tagCompound.GetCompound("MouseItem"));
//             if (tagCompound.ContainsKey("GuideItem"))
//                 ItemIO.Load(Main.guideItem, tagCompound.GetCompound("GuideItem"));
//             if (tagCompound.ContainsKey("ReforgeItem"))
//                 ItemIO.Load(Main.reforgeItem, tagCompound.GetCompound("ReforgeItem"));
//             if (tagCompound.ContainsKey("CreativeMenu"))
//                 Main.CreativeMenu.SetItembyIndex(ItemIO.Load(tagCompound.GetCompound("CreativeMenu")), 0);
//             if (tagCompound.ContainsKey("CreativePowerManager"))
//                 CreativePowerManager.Instance.LoadToPlayer(player, new BinaryReader(new MemoryStream(tagCompound.GetByteArray("CreativePowerManager"))), 0);
//             if (tagCompound.ContainsKey("HairDyeItemName"))
//                 PlayerIO.LoadHairDye(player, tagCompound.GetString("HairDyeItemName"));
//             if (tagCompound.ContainsKey("Research"))
//                 PlayerIO.LoadResearch(player, tagCompound.GetList<TagCompound>("Research"));
//             if (tagCompound.ContainsKey("ModData"))
//                 PlayerIO.LoadModData(player, tagCompound.GetList<TagCompound>("ModData"));
//             if (tagCompound.ContainsKey("ModBuffs"))
//                 PlayerIO.LoadModBuffs(player, tagCompound.GetList<TagCompound>("ModBuffs"));
//             if (tagCompound.ContainsKey("InfoDisplays"))
//                 PlayerIO.LoadInfoDisplays(player, tagCompound.GetList<string>("InfoDisplays"));
//             if (tagCompound.ContainsKey("UsedMods"))
//                 PlayerIO.LoadUsedMods(player, tagCompound.GetList<string>("UsedMods"));
//         }
//     }
// }