using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SSC;

public static class SSCTools
{
    public static void Boot(int whoAmI, string tip)
    {
        switch (Main.netMode)
        {
            case NetmodeID.MultiplayerClient:
                Netplay.Disconnect = true;
                Main.statusText = tip;
                Main.menuMode = MenuID.MultiplayerJoining;
                break;
            case NetmodeID.Server:
                NetMessage.BootPlayer(whoAmI, NetworkText.FromLiteral(tip));
                break;
            default:
                throw new Exception("Boot can only be used in C/S.");
        }
    }

    public static void SetupPlayerStatsAndInventoryBasedOnDifficulty(Player player)
    {
        int index1 = 0;
        int num1;
        if (player.difficulty == (byte)3)
        {
            player.statLife = player.statLifeMax = 100;
            player.statMana = player.statManaMax = 20;
            player.inventory[index1].SetDefaults(6);
            Item[] inventory1 = player.inventory;
            int index2 = index1;
            int index3 = index2 + 1;
            inventory1[index2].Prefix(-1);
            player.inventory[index3].SetDefaults(1);
            Item[] inventory2 = player.inventory;
            int index4 = index3;
            int index5 = index4 + 1;
            inventory2[index4].Prefix(-1);
            player.inventory[index5].SetDefaults(10);
            Item[] inventory3 = player.inventory;
            int index6 = index5;
            int index7 = index6 + 1;
            inventory3[index6].Prefix(-1);
            player.inventory[index7].SetDefaults(7);
            Item[] inventory4 = player.inventory;
            int index8 = index7;
            int index9 = index8 + 1;
            inventory4[index8].Prefix(-1);
            player.inventory[index9].SetDefaults(4281);
            Item[] inventory5 = player.inventory;
            int index10 = index9;
            int index11 = index10 + 1;
            inventory5[index10].Prefix(-1);
            player.inventory[index11].SetDefaults(8);
            Item[] inventory6 = player.inventory;
            int index12 = index11;
            int index13 = index12 + 1;
            inventory6[index12].stack = 100;
            player.inventory[index13].SetDefaults(965);
            Item[] inventory7 = player.inventory;
            int index14 = index13;
            int num2 = index14 + 1;
            inventory7[index14].stack = 100;
            Item[] inventory8 = player.inventory;
            int index15 = num2;
            int num3 = index15 + 1;
            inventory8[index15].SetDefaults(50);
            Item[] inventory9 = player.inventory;
            int index16 = num3;
            num1 = index16 + 1;
            inventory9[index16].SetDefaults(84);
            player.armor[3].SetDefaults(4978);
            player.armor[3].Prefix(-1);
            player.AddBuff(216, 3600);
        }
        else
        {
            player.inventory[index1].SetDefaults(3507);
            Item[] inventory10 = player.inventory;
            int index17 = index1;
            int index18 = index17 + 1;
            inventory10[index17].Prefix(-1);
            player.inventory[index18].SetDefaults(3509);
            Item[] inventory11 = player.inventory;
            int index19 = index18;
            int index20 = index19 + 1;
            inventory11[index19].Prefix(-1);
            player.inventory[index20].SetDefaults(3506);
            Item[] inventory12 = player.inventory;
            int index21 = index20;
            num1 = index21 + 1;
            inventory12[index21].Prefix(-1);
        }

        if (Main.runningCollectorsEdition)
        {
            Item[] inventory = player.inventory;
            int index22 = num1;
            int num4 = index22 + 1;
            inventory[index22].SetDefaults(603);
        }

        player.savedPerPlayerFieldsThatArentInThePlayerClass = new Terraria.Player.SavedPlayerDataWithAnnoyingRules();
        CreativePowerManager.Instance.ResetDataForNewPlayer(player);
        PlayerLoader.SetStartInventory(player,
            (IList<Item>)PlayerLoader.GetStartingItems(player,
                ((IEnumerable<Item>)player.inventory).Where<Item>((Func<Item, bool>)(item => !item.IsAir))
                .Select<Item, Item>((Func<Item, Item>)(x => x.Clone()))));
    }
}