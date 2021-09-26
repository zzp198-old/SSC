using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Terraria;
using Terraria.ModLoader.IO;

namespace SSC.Reflection
{
    public static class PlayerIO
    {
        private static Type GetPlayerIO() => Assembly.Load("tModLoader").GetType("Terraria.ModLoader.IO.PlayerIO");
        private const BindingFlags BindingAttr = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

        private static object Invoke(string name, params object[] args)
        {
            var method = GetPlayerIO().GetMethod(name, BindingAttr);
            return method?.Invoke(null, args);
        }

        public static byte VanillaHairDye(int hairDye)
        {
            byte vanillaHairDye = 0;
            var writer = new BinaryWriter(new MemoryStream(vanillaHairDye));
            Invoke("WriteByteVanillaHairDye", hairDye, writer);
            return vanillaHairDye;
        }

        public static List<TagCompound> SaveInventory(Item[] inv)
        {
            var tagCompoundList = new List<TagCompound>();
            for (var i = 0; i < inv.Length; ++i)
            {
                var tagCompound = ItemIO.Save(inv[i]);
                tagCompound.Set("slot", (short) i);
                tagCompoundList.Add(tagCompound);
            }

            return tagCompoundList;
        }

        public static void LoadInventory(Item[] inv, IList<TagCompound> list)
        {
            Invoke("LoadInventory", inv, list);
        }

        public static string SaveHairDye(int hairDye)
        {
            return (string) Invoke("SaveHairDye", hairDye);
        }

        public static void LoadHairDye(Player player, string hairDyeItemName)
        {
            Invoke("LoadHairDye", player, hairDyeItemName);
        }

        public static List<TagCompound> SaveResearch(Player player)
        {
            return (List<TagCompound>) Invoke("SaveResearch", player);
        }

        public static void LoadResearch(Player player, IList<TagCompound> list)
        {
            Invoke("LoadResearch", player, list);
        }

        public static List<TagCompound> SaveModData(Player player)
        {
            return (List<TagCompound>) Invoke("SaveModData", player);
        }

        public static void LoadModData(Player player, IList<TagCompound> list)
        {
            Invoke("LoadModData", player, list);
        }

        public static List<TagCompound> SaveModBuffs(Player player)
        {
            return (List<TagCompound>) Invoke("SaveModBuffs", player);
        }

        public static void LoadModBuffs(Player player, IList<TagCompound> list)
        {
            Invoke("LoadModBuffs", player, list);
        }

        public static List<string> SaveInfoDisplays(Player player)
        {
            return (List<string>) Invoke("SaveInfoDisplays", player);
        }

        public static void LoadInfoDisplays(Player player, IList<string> hidden)
        {
            Invoke("LoadInfoDisplays", player, hidden);
        }

        public static List<string> SaveUsedMods(Player player)
        {
            return (List<string>) Invoke("SaveUsedMods", player);
        }

        public static void LoadUsedMods(Player player, IList<string> usedMods)
        {
            Invoke("LoadUsedMods", player, usedMods);
        }
    }
}