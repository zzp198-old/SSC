using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Steamworks;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace SSC;

public partial class SSC : Mod
{
    internal static string SavePath => Path.Combine(Main.SavePath, "SSC");
    internal static SSC Mod => ModContent.GetInstance<SSC>();
    internal static ulong Sid => SteamUser.GetSteamID().m_SteamID;

    public static void Boot(int plr, string msg)
    {
        switch (Main.netMode)
        {
            case NetmodeID.MultiplayerClient:
                Netplay.Disconnect = true;
                Main.statusText = msg;
                Main.menuMode = MenuID.MultiplayerJoining;
                break;
            case NetmodeID.Server:
                NetMessage.BootPlayer(plr, NetworkText.FromLiteral(msg));
                break;
            default:
                throw new Exception("Boot can only be used in C/S.");
        }
    }

    private static void InternalSavePlayer(PlayerFileData data)
    {
        var invoke = typeof(Player).GetMethod("InternalSavePlayerFile", (BindingFlags)40);
        FileUtilities.ProtectedInvoke(() => invoke?.Invoke(null, new object[] { data }));
    }

    private static void SetupPlayerStatsAndInventoryBasedOnDifficulty(Player player)
    {
        player.statLife = player.statLifeMax = 100;
        player.statMana = player.statManaMax = 20;
        switch (player.difficulty)
        {
            case PlayerDifficultyID.SoftCore:
            case PlayerDifficultyID.MediumCore:
            case PlayerDifficultyID.Hardcore:
                player.inventory[0].SetDefaults(ItemID.CopperShortsword);
                player.inventory[1].SetDefaults(ItemID.CopperPickaxe);
                player.inventory[2].SetDefaults(ItemID.CopperAxe);
                player.inventory[3].SetDefaults(ItemID.Carrot);
                break;
            case PlayerDifficultyID.Creative:
                player.inventory[0].SetDefaults(ItemID.IronShortsword);
                player.inventory[1].SetDefaults(ItemID.IronPickaxe);
                player.inventory[2].SetDefaults(ItemID.IronAxe);
                player.inventory[3].SetDefaults(ItemID.IronHammer);
                player.inventory[4].SetDefaults(ItemID.BabyBirdStaff);
                player.inventory[5].SetDefaults(ItemID.Torch);
                player.inventory[5].stack = 100;
                player.inventory[6].SetDefaults(ItemID.Rope);
                player.inventory[6].stack = 100;
                player.inventory[7].SetDefaults(ItemID.MagicMirror);
                player.inventory[8].SetDefaults(ItemID.GrapplingHook);
                player.inventory[9].SetDefaults(ItemID.Carrot);
                player.armor[3].SetDefaults(ItemID.CreativeWings);
                player.AddBuff(BuffID.BabyBird, 3600);
                break;
        }

        player.savedPerPlayerFieldsThatArentInThePlayerClass = new Player.SavedPlayerDataWithAnnoyingRules();
        CreativePowerManager.Instance.ResetDataForNewPlayer(player);
        var items = PlayerLoader.GetStartingItems(player, player.inventory.Where(x => !x.IsAir).Select(x => x.Clone()));
        PlayerLoader.SetStartInventory(player, items);
    }
}