using Terraria;
using Terraria.ModLoader;

namespace SSC
{
    public class SSCPlayer : ModPlayer
    {
        public int SaveCountdown;

        public override void OnEnterWorld(Player player)
        {
            // PlayerIO.Reset(player);
        }

        public static void Reset(Player player)
        {
            // player.hair = 0;
            // player.hairDye = 0;
            // player.hideVisibleAccessory.Initialize();
            // player.hideMisc = 0;
            // player.skinVariant = 0;
            // player.statLife = player.statLifeMax = 100;
            // player.statMana = player.statManaMax = 20;
            // player.extraAccessory = false;
            // player.unlockedBiomeTorches = false;
            // player.UsingBiomeTorches = false;
            // player.downedDD2EventAnyDifficulty = false;
            // player.taxMoney = 0;
            // player.hairColor = new Color(215, 90, 55);
            // player.skinColor = new Color(255, 125, 90);
            // player.eyeColor = new Color(105, 90, 75);
            // player.shirtColor = new Color(175, 165, 140);
            // player.underShirtColor = new Color(160, 180, 215);
            // player.pantsColor = new Color(255, 230, 175);
            // player.shoeColor = new Color(160, 105, 60);
            // Array.ForEach(player.armor, item => item.SetDefaults());
            // Array.ForEach(player.dye, item => item.SetDefaults());
            // Array.ForEach(player.miscEquips, item => item.SetDefaults());
            // Array.ForEach(player.miscDyes, item => item.SetDefaults());
            // Array.ForEach(player.inventory, item => item.SetDefaults());
            // Array.ForEach(player.bank.item, item => item.SetDefaults());
            // Array.ForEach(player.bank2.item, item => item.SetDefaults());
            // Array.ForEach(player.bank3.item, item => item.SetDefaults());
            // Array.ForEach(player.bank4.item, item => item.SetDefaults());
            // player.voidVaultInfo = 0;
            // player.SpawnX = -1;
            // player.SpawnY = -1;
            // player.hbLocked = false;
            // player.hideInfo.Initialize();
            // player.anglerQuestsFinished = 0;
            // player.DpadRadial.Bindings.Initialize();
            // player.builderAccStatus.Initialize();
            // player.bartenderQuestLog = 0;
            // player.dead = false;
            // player.respawnTimer = 0;
            // player.lastTimePlayerWasSaved = 0;
            // player.golferScoreAccumulated = 0;
            // player.creativeTracker.Reset();
            // // CreativePowerManager.Instance.ResetDataForNewPlayer(player);
            // Invoke("LoadModBuffs", new object[] {player, Invoke("SaveModBuffs", new object[] {Main.player[byte.MaxValue]})});
            // Invoke("LoadModData", new object[] {player, Invoke("SaveModData", new object[] {Main.player[byte.MaxValue]})});
            // Player.ClearPlayerTempInfo();
        }
    }
}