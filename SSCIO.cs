using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace ServerMod;

public static class SSCIO
{
    public static void Reset(Player player)
    {
        player.hair = 0;
        player.hairDye = 0;
        player.hideVisibleAccessory.Initialize();
        player.hideMisc = (BitsByte)(byte)0;
        player.skinVariant = 0;
        player.statLife = player.statLifeMax = 100;
        player.statMana = player.statManaMax = 20;
        player.extraAccessory = false;
        player.unlockedBiomeTorches = false;
        player.UsingBiomeTorches = false;
        player.downedDD2EventAnyDifficulty = false;
        player.taxMoney = 0;
        player.hairColor = new Color(215, 90, 55);
        player.skinColor = new Color((int)byte.MaxValue, 125, 90);
        player.eyeColor = new Color(105, 90, 75);
        player.shirtColor = new Color(175, 165, 140);
        player.underShirtColor = new Color(160, 180, 215);
        player.pantsColor = new Color((int)byte.MaxValue, 230, 175);
        player.shoeColor = new Color(160, 105, 60);
        Array.ForEach<Item>(player.armor, (Action<Item>)(item => item.SetDefaults()));
        Array.ForEach<Item>(player.dye, (Action<Item>)(item => item.SetDefaults()));
        Array.ForEach<Item>(player.miscEquips, (Action<Item>)(item => item.SetDefaults()));
        Array.ForEach<Item>(player.miscDyes, (Action<Item>)(item => item.SetDefaults()));
        Array.ForEach<Item>(player.inventory, (Action<Item>)(item => item.SetDefaults()));
        for (int index = 0; index < ServerMod.SSCConfig.StartingInventory.Count; ++index)
            player.inventory[index].SetDefaults(ServerMod.SSCConfig.StartingInventory[index].Type);
        Array.ForEach<Item>(player.bank.item, (Action<Item>)(item => item.SetDefaults()));
        Array.ForEach<Item>(player.bank2.item, (Action<Item>)(item => item.SetDefaults()));
        Array.ForEach<Item>(player.bank3.item, (Action<Item>)(item => item.SetDefaults()));
        Array.ForEach<Item>(player.bank4.item, (Action<Item>)(item => item.SetDefaults()));
        player.voidVaultInfo = (BitsByte)(byte)0;
        player.SpawnX = -1;
        player.SpawnY = -1;
        player.hbLocked = false;
        player.hideInfo.Initialize();
        player.anglerQuestsFinished = 0;
        player.DpadRadial.Bindings.Initialize();
        player.builderAccStatus.Initialize();
        player.bartenderQuestLog = 0;
        player.dead = false;
        player.respawnTimer = 0;
        player.lastTimePlayerWasSaved = 0L;
        player.golferScoreAccumulated = 0;
        player.creativeTracker.Reset();
        Player.ClearPlayerTempInfo();
        CreativePowerManager.Instance.ResetDataForNewPlayer(player);
        LoadModData(player,
            (IList<TagCompound>)SaveModData(Main.player[(int)byte.MaxValue]));
        LoadModBuffs(player,
            (IList<TagCompound>)SaveModBuffs(Main.player[(int)byte.MaxValue]));
        LoadInfoDisplays(player,
            (IList<string>)SaveInfoDisplays(Main.player[(int)byte.MaxValue]));
        LoadUsedMods(player,
            (IList<string>)SaveUsedMods(Main.player[(int)byte.MaxValue]));

        IList<Item> t = new List<Item>();
        foreach (var itemDefinition in ServerMod.SSCConfig.StartingInventory)
        {
            t.Add(new Item(itemDefinition.Type));
        }

        PlayerLoader.SetStartInventory(player,
            PlayerLoader.GetStartingItems(player, t, player.difficulty == PlayerDifficultyID.MediumCore));
    }

    public static void Load(Player player, TagCompound tagCompound)
    {
        if (tagCompound.ContainsKey("Hair"))
            player.hair = tagCompound.GetInt("Hair");
        if (tagCompound.ContainsKey("HairDye"))
            player.hairDye = (int)tagCompound.GetByte("HairDye");
        if (tagCompound.ContainsKey("HideVisibleAccessory"))
            player.hideVisibleAccessory = tagCompound.GetList<bool>("HideVisibleAccessory").ToArray<bool>();
        if (tagCompound.ContainsKey("HideMisc"))
            player.hideMisc = (BitsByte)tagCompound.GetByte("HideMisc");
        if (tagCompound.ContainsKey("SkinVariant"))
            player.skinVariant = tagCompound.GetInt("skinVariant");
        if (tagCompound.ContainsKey("StatLife"))
            player.statLife = tagCompound.GetInt("StatLife");
        if (tagCompound.ContainsKey("StatLifeMax"))
            player.statLifeMax = tagCompound.GetInt("StatLifeMax");
        if (tagCompound.ContainsKey("StatMana"))
            player.statMana = tagCompound.GetInt("StatMana");
        if (tagCompound.ContainsKey("StatManaMax"))
            player.statManaMax = tagCompound.GetInt("StatManaMax");
        if (tagCompound.ContainsKey("ExtraAccessory"))
            player.extraAccessory = tagCompound.GetBool("ExtraAccessory");
        if (tagCompound.ContainsKey("UnlockedBiomeTorches"))
            player.unlockedBiomeTorches = tagCompound.GetBool("UnlockedBiomeTorches");
        if (tagCompound.ContainsKey("UsingBiomeTorches"))
            player.UsingBiomeTorches = tagCompound.GetBool("UsingBiomeTorches");
        if (tagCompound.ContainsKey("DownedDD2EventAnyDifficulty"))
            player.downedDD2EventAnyDifficulty = tagCompound.GetBool("DownedDD2EventAnyDifficulty");
        if (tagCompound.ContainsKey("TaxMoney"))
            player.taxMoney = tagCompound.GetInt("TaxMoney");
        if (tagCompound.ContainsKey("HairColor"))
            player.hairColor = tagCompound.Get<Color>("HairColor");
        if (tagCompound.ContainsKey("SkinColor"))
            player.skinColor = tagCompound.Get<Color>("SkinColor");
        if (tagCompound.ContainsKey("EyeColor"))
            player.eyeColor = tagCompound.Get<Color>("EyeColor");
        if (tagCompound.ContainsKey("ShirtColor"))
            player.shirtColor = tagCompound.Get<Color>("ShirtColor");
        if (tagCompound.ContainsKey("UnderShirtColor"))
            player.underShirtColor = tagCompound.Get<Color>("UnderShirtColor");
        if (tagCompound.ContainsKey("PantsColor"))
            player.pantsColor = tagCompound.Get<Color>("PantsColor");
        if (tagCompound.ContainsKey("ShoeColor"))
            player.shoeColor = tagCompound.Get<Color>("ShoeColor");
        if (tagCompound.ContainsKey("Armor"))
            LoadInventory(player.armor, tagCompound.GetList<TagCompound>("Armor"));
        if (tagCompound.ContainsKey("Dye"))
            LoadInventory(player.dye, tagCompound.GetList<TagCompound>("Dye"));
        if (tagCompound.ContainsKey("MiscEquips"))
            LoadInventory(player.miscEquips, tagCompound.GetList<TagCompound>("MiscEquips"));
        if (tagCompound.ContainsKey("MiscDyes"))
            LoadInventory(player.miscDyes, tagCompound.GetList<TagCompound>("MiscDyes"));
        if (tagCompound.ContainsKey("Inventory"))
            LoadInventory(player.inventory, tagCompound.GetList<TagCompound>("Inventory"));
        if (tagCompound.ContainsKey("Bank"))
            LoadInventory(player.bank.item, tagCompound.GetList<TagCompound>("Bank"));
        if (tagCompound.ContainsKey("Bank2"))
            LoadInventory(player.bank2.item, tagCompound.GetList<TagCompound>("Bank2"));
        if (tagCompound.ContainsKey("Bank3"))
            LoadInventory(player.bank3.item, tagCompound.GetList<TagCompound>("Bank3"));
        if (tagCompound.ContainsKey("Bank4"))
            LoadInventory(player.bank4.item, tagCompound.GetList<TagCompound>("Bank4"));
        if (tagCompound.ContainsKey("VoidVaultInfo"))
            player.voidVaultInfo = (BitsByte)tagCompound.GetByte("VoidVaultInfo");
        if (tagCompound.ContainsKey("SpawnX ="))
            player.SpawnX = tagCompound.GetInt("SpawnX =");
        if (tagCompound.ContainsKey("SpawnY"))
            player.SpawnY = tagCompound.GetInt("SpawnY");
        if (tagCompound.ContainsKey("HbLocked"))
            player.hbLocked = tagCompound.GetBool("HbLocked");
        if (tagCompound.ContainsKey("HideInfo"))
            player.hideInfo = tagCompound.GetList<bool>("HideInfo").ToArray<bool>();
        if (tagCompound.ContainsKey("AnglerQuestsFinished"))
            player.anglerQuestsFinished = tagCompound.GetInt("AnglerQuestsFinished");
        if (tagCompound.ContainsKey("DpadRadial"))
            player.DpadRadial.Bindings = tagCompound.GetIntArray("DpadRadial");
        if (tagCompound.ContainsKey("BuilderAccStatus"))
            player.builderAccStatus = tagCompound.GetIntArray("BuilderAccStatus");
        if (tagCompound.ContainsKey("BartenderQuestLog"))
            player.bartenderQuestLog = tagCompound.GetInt("BartenderQuestLog");
        if (tagCompound.ContainsKey("Dead"))
            player.dead = tagCompound.GetBool("Dead");
        if (tagCompound.ContainsKey("RespawnTimer"))
            player.respawnTimer = tagCompound.GetInt("RespawnTimer");
        if (tagCompound.ContainsKey("GolferScoreAccumulated"))
            player.golferScoreAccumulated = tagCompound.GetInt("GolferScoreAccumulated");
        if (tagCompound.ContainsKey("CreativeTracker"))
            player.creativeTracker.Load(
                new BinaryReader((Stream)new MemoryStream(tagCompound.GetByteArray("CreativeTracker"))), 0);
        if (tagCompound.ContainsKey("MouseItem"))
            ItemIO.Load(Main.mouseItem, tagCompound.GetCompound("MouseItem"));
        if (tagCompound.ContainsKey("GuideItem"))
            ItemIO.Load(Main.guideItem, tagCompound.GetCompound("GuideItem"));
        if (tagCompound.ContainsKey("ReforgeItem"))
            ItemIO.Load(Main.reforgeItem, tagCompound.GetCompound("ReforgeItem"));
        if (tagCompound.ContainsKey("CreativeMenu"))
            Main.CreativeMenu.SetItembyIndex(ItemIO.Load(tagCompound.GetCompound("CreativeMenu")), 0);
        if (tagCompound.ContainsKey("CreativePowerManager"))
            CreativePowerManager.Instance.LoadToPlayer(player,
                new BinaryReader((Stream)new MemoryStream(tagCompound.GetByteArray("CreativePowerManager"))), 0);
        if (tagCompound.ContainsKey("HairDyeItemName"))
            LoadHairDye(player, tagCompound.GetString("HairDyeItemName"));
        if (tagCompound.ContainsKey("Research"))
            LoadResearch(player, tagCompound.GetList<TagCompound>("Research"));
        if (tagCompound.ContainsKey("ModData"))
            LoadModData(player, tagCompound.GetList<TagCompound>("ModData"));
        if (tagCompound.ContainsKey("ModBuffs"))
            LoadModBuffs(player, tagCompound.GetList<TagCompound>("ModBuffs"));
        if (tagCompound.ContainsKey("InfoDisplays"))
            LoadInfoDisplays(player, tagCompound.GetList<string>("InfoDisplays"));
        if (!tagCompound.ContainsKey("UsedMods"))
            return;
        LoadUsedMods(player, tagCompound.GetList<string>("UsedMods"));
    }

    public static TagCompound ClientTagCompound(Player player)
    {
        TagCompound tagCompound = new TagCompound();
        tagCompound.Set("TaxMoney", (object)player.taxMoney);
        tagCompound.Set("Armor", (object)SaveInventory(player.armor));
        tagCompound.Set("Dye", (object)SaveInventory(player.dye));
        tagCompound.Set("MiscEquips", (object)SaveInventory(player.miscEquips));
        tagCompound.Set("MiscDyes", (object)SaveInventory(player.miscDyes));
        tagCompound.Set("Inventory", (object)SaveInventory(player.inventory));
        tagCompound.Set("Bank", (object)SaveInventory(player.bank.item));
        tagCompound.Set("Bank2", (object)SaveInventory(player.bank2.item));
        tagCompound.Set("Bank3", (object)SaveInventory(player.bank3.item));
        tagCompound.Set("Bank4", (object)SaveInventory(player.bank4.item));
        tagCompound.Set("SpawnX", (object)player.SpawnX);
        tagCompound.Set("SpawnY", (object)player.SpawnY);
        tagCompound.Set("HbLocked", (object)player.hbLocked);
        tagCompound.Set("HideInfo", (object)((IEnumerable<bool>)player.hideInfo).ToList<bool>());
        tagCompound.Set("AnglerQuestsFinished", (object)player.anglerQuestsFinished);
        tagCompound.Set("DpadRadial", (object)player.DpadRadial.Bindings);
        tagCompound.Set("BuilderAccStatus", (object)player.builderAccStatus);
        tagCompound.Set("BartenderQuestLog", (object)player.bartenderQuestLog);
        tagCompound.Set("GolferScoreAccumulated", (object)player.golferScoreAccumulated);
        MemoryStream output1 = new MemoryStream();
        player.creativeTracker.Save(new BinaryWriter((Stream)output1));
        tagCompound.Set("CreativeTracker", (object)output1.ToArray());
        tagCompound.Set("MouseItem", (object)Main.mouseItem);
        tagCompound.Set("GuideItem", (object)Main.guideItem);
        tagCompound.Set("ReforgeItem", (object)Main.reforgeItem);
        tagCompound.Set("CreativeMenu", (object)Main.CreativeMenu.GetItemByIndex(0));
        MemoryStream output2 = new MemoryStream();
        CreativePowerManager.Instance.SaveToPlayer(player, new BinaryWriter((Stream)output2));
        tagCompound.Set("CreativePowerManager", (object)output2.ToArray());
        tagCompound.Set("HairDyeItemName", (object)SaveHairDye(player.hairDye));
        tagCompound.Set("Research", (object)SaveResearch(player));
        tagCompound.Set("ModData", (object)SaveModData(player));
        tagCompound.Set("ModBuffs", (object)SaveModBuffs(player));
        tagCompound.Set("InfoDisplays", (object)SaveInfoDisplays(player));
        tagCompound.Set("UsedMods", (object)SaveUsedMods(player));
        return tagCompound;
    }

    public static void ServerTagCompound(Player player, TagCompound tagCompound)
    {
        tagCompound.Set("Hair", (object)player.hair);
        tagCompound.Set("HairDye", (object)VanillaHairDye(player.hairDye));
        tagCompound.Set("HideVisibleAccessory",
            (object)((IEnumerable<bool>)player.hideVisibleAccessory).ToList<bool>());
        tagCompound.Set("HideMisc", (object)(byte)player.hideMisc);
        tagCompound.Set("SkinVariant", (object)player.skinVariant);
        tagCompound.Set("StatLife", (object)player.statLife);
        tagCompound.Set("StatLifeMax", (object)player.statLifeMax);
        tagCompound.Set("StatMana", (object)player.statMana);
        tagCompound.Set("StatManaMax", (object)player.statManaMax);
        tagCompound.Set("ExtraAccessory", (object)player.extraAccessory);
        tagCompound.Set("UnlockedBiomeTorches", (object)player.unlockedBiomeTorches);
        tagCompound.Set("UsingBiomeTorches", (object)player.UsingBiomeTorches);
        tagCompound.Set("DownedDD2EventAnyDifficulty", (object)player.downedDD2EventAnyDifficulty);
        tagCompound.Set("HairColor", (object)player.hairColor);
        tagCompound.Set("SkinColor", (object)player.skinColor);
        tagCompound.Set("EyeColor", (object)player.eyeColor);
        tagCompound.Set("ShirtColor", (object)player.shirtColor);
        tagCompound.Set("UnderShirtColor", (object)player.underShirtColor);
        tagCompound.Set("PantsColor", (object)player.pantsColor);
        tagCompound.Set("ShoeColor", (object)player.shoeColor);
        tagCompound.Set("Dead", (object)player.dead);
        tagCompound.Set("RespawnTimer", (object)player.respawnTimer);
    }

    private const BindingFlags BindingAttr = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

    private static Type GetPlayerIO() => Assembly.Load("tModLoader").GetType("Terraria.ModLoader.IO.PlayerIO");

    private static object Invoke(string name, params object[] args) => GetPlayerIO()
        .GetMethod(name, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
        ?.Invoke((object)null, args);

    public static byte VanillaHairDye(int hairDye)
    {
        byte capacity = 0;
        BinaryWriter binaryWriter = new BinaryWriter((Stream)new MemoryStream((int)capacity));
        Invoke("WriteByteVanillaHairDye", (object)hairDye, (object)binaryWriter);
        return capacity;
    }

    public static List<TagCompound> SaveInventory(Item[] inv)
    {
        List<TagCompound> tagCompoundList = new List<TagCompound>();
        for (int index = 0; index < inv.Length; ++index)
        {
            TagCompound tagCompound = ItemIO.Save(inv[index]);
            tagCompound.Set("slot", (object)(short)index);
            tagCompoundList.Add(tagCompound);
        }

        return tagCompoundList;
    }

    public static void LoadInventory(Item[] inv, IList<TagCompound> list) =>
        Invoke(nameof(LoadInventory), (object)inv, (object)list);

    public static string SaveHairDye(int hairDye) => (string)Invoke(nameof(SaveHairDye), (object)hairDye);

    public static void LoadHairDye(Player player, string hairDyeItemName) =>
        Invoke(nameof(LoadHairDye), (object)player, (object)hairDyeItemName);

    public static List<TagCompound> SaveResearch(Player player) =>
        (List<TagCompound>)Invoke(nameof(SaveResearch), (object)player);

    public static void LoadResearch(Player player, IList<TagCompound> list) =>
        Invoke(nameof(LoadResearch), (object)player, (object)list);

    public static List<TagCompound> SaveModData(Player player) =>
        (List<TagCompound>)Invoke(nameof(SaveModData), (object)player);

    public static void LoadModData(Player player, IList<TagCompound> list) =>
        Invoke(nameof(LoadModData), (object)player, (object)list);

    public static List<TagCompound> SaveModBuffs(Player player) =>
        (List<TagCompound>)Invoke(nameof(SaveModBuffs), (object)player);

    public static void LoadModBuffs(Player player, IList<TagCompound> list) =>
        Invoke(nameof(LoadModBuffs), (object)player, (object)list);

    public static List<string> SaveInfoDisplays(Player player) =>
        (List<string>)Invoke(nameof(SaveInfoDisplays), (object)player);

    public static void LoadInfoDisplays(Player player, IList<string> hidden) =>
        Invoke(nameof(LoadInfoDisplays), (object)player, (object)hidden);

    public static List<string> SaveUsedMods(Player player) =>
        (List<string>)Invoke(nameof(SaveUsedMods), (object)player);

    public static void LoadUsedMods(Player player, IList<string> usedMods) =>
        Invoke(nameof(LoadUsedMods), (object)player, (object)usedMods);
}