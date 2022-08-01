using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SSC;

public static class Utils
{
    public static void Boot(int id, string tip)
    {
        switch (Main.netMode)
        {
            case NetmodeID.MultiplayerClient:
                Netplay.Disconnect = true;
                Main.statusText = tip;
                Main.menuMode = MenuID.MultiplayerJoining;
                break;
            case NetmodeID.Server:
                NetMessage.BootPlayer(id, NetworkText.FromLiteral(tip));
                break;
            default:
                throw new Exception("Boot can only be used in C/S");
        }
    }

    public static Player ErasePLR(int whoAmI, string name, byte GameMode)
    {
        Main.player[whoAmI] = new Player
        {
            name = name, difficulty = GameMode,
            savedPerPlayerFieldsThatArentInThePlayerClass = new Player.SavedPlayerDataWithAnnoyingRules()
        };

        if (whoAmI == Main.myPlayer)
        {
            var data = new PlayerFileData { Player = Main.player[whoAmI] };
            data.MarkAsServerSide();
            data.SetAsActive();
        }

        return Main.player[whoAmI];
    }

    public static void SetupPlayerStatsAndInventoryBasedOnDifficulty(Player player)
    {
        const int index1 = 0;
        int num1;
        if (player.difficulty == 3)
        {
            player.statLife = player.statLifeMax = 100;
            player.statMana = player.statManaMax = 20;
            player.inventory[index1].SetDefaults(6);
            var inventory1 = player.inventory;
            const int index3 = index1 + 1;
            inventory1[index1].Prefix(-1);
            player.inventory[index3].SetDefaults(1);
            var inventory2 = player.inventory;
            const int index5 = index3 + 1;
            inventory2[index3].Prefix(-1);
            player.inventory[index5].SetDefaults(10);
            var inventory3 = player.inventory;
            const int index7 = index5 + 1;
            inventory3[index5].Prefix(-1);
            player.inventory[index7].SetDefaults(7);
            var inventory4 = player.inventory;
            const int index9 = index7 + 1;
            inventory4[index7].Prefix(-1);
            player.inventory[index9].SetDefaults(4281);
            var inventory5 = player.inventory;
            const int index11 = index9 + 1;
            inventory5[index9].Prefix(-1);
            player.inventory[index11].SetDefaults(8);
            var inventory6 = player.inventory;
            const int index13 = index11 + 1;
            inventory6[index11].stack = 100;
            player.inventory[index13].SetDefaults(965);
            var inventory7 = player.inventory;
            const int num2 = index13 + 1;
            inventory7[index13].stack = 100;
            var inventory8 = player.inventory;
            const int num3 = num2 + 1;
            inventory8[num2].SetDefaults(50);
            var inventory9 = player.inventory;
            num1 = num3 + 1;
            inventory9[num3].SetDefaults(84);
            player.armor[3].SetDefaults(4978);
            player.armor[3].Prefix(-1);
            player.AddBuff(216, 3600);
        }
        else
        {
            player.inventory[index1].SetDefaults(3507);
            var inventory10 = player.inventory;
            const int index18 = index1 + 1;
            inventory10[index1].Prefix(-1);
            player.inventory[index18].SetDefaults(3509);
            var inventory11 = player.inventory;
            const int index20 = index18 + 1;
            inventory11[index18].Prefix(-1);
            player.inventory[index20].SetDefaults(3506);
            var inventory12 = player.inventory;
            num1 = index20 + 1;
            inventory12[index20].Prefix(-1);
        }

        var inventory = player.inventory;
        inventory[num1].SetDefaults(603);

        player.savedPerPlayerFieldsThatArentInThePlayerClass = new Player.SavedPlayerDataWithAnnoyingRules();
        CreativePowerManager.Instance.ResetDataForNewPlayer(player);
        PlayerLoader.SetStartInventory(player,
            PlayerLoader.GetStartingItems(player,
                player.inventory.Where((Func<Item, bool>)(item => !item.IsAir))
                    .Select((Func<Item, Item>)(x => x.Clone()))));
    }

    public static void CleanCache()
    {
        if (Directory.Exists(Path.Combine(SSC.SavePath, "Cache")))
        {
            Directory.Delete(Path.Combine(SSC.SavePath, "Cache"), true);
        }

        Directory.CreateDirectory(Path.Combine(SSC.SavePath, "Cache"));
    }
}