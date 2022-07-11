using System;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;

namespace QSMod;

public static class SSCIO
{
    static readonly Type SSC_IO = Assembly.Load("tModLoader").GetType("Terraria.ModLoader.IO.PlayerIO");
    static BindingFlags SSC_BF = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

    public static void PuppetLP()
    {
        var LP = Main.LocalPlayer;
        LP.difficulty = Main.GameMode == GameModeID.Creative
            ? PlayerDifficultyID.Creative
            : PlayerDifficultyID.Hardcore;
        Main.ActivePlayerFileData.SetPlayTime(TimeSpan.Zero);
        LP.hair = 0;
        LP.hairDye = 0;
        LP.hideVisibleAccessory.Initialize();
        LP.hideMisc = 0;
        LP.skinVariant = 0;
        LP.statLife = 100;
        LP.statLifeMax = 100;
        LP.statMana = 0;
        LP.statManaMax = 0;
        LP.extraAccessory = false;
        LP.unlockedBiomeTorches = false;
        LP.UsingBiomeTorches = false;
        LP.downedDD2EventAnyDifficulty = false;
        LP.taxMoney = 0;
        LP.hairColor = new Color(215, 90, 55);
        LP.skinColor = new Color(byte.MaxValue, 125, 90);
        LP.eyeColor = new Color(105, 90, 75);
        LP.shirtColor = new Color(175, 165, 140);
        LP.underShirtColor = new Color(160, 180, 215);
        LP.pantsColor = new Color(byte.MaxValue, 230, 175);
        LP.shoeColor = new Color(160, 105, 60);
        LP.armor.AsParallel().ForAll(i => i.TurnToAir());
        LP.dye.AsParallel().ForAll(i => i.TurnToAir());
        LP.inventory.AsParallel().ForAll(i => i.TurnToAir());
        LP.miscEquips.AsParallel().ForAll(i => i.TurnToAir());
        LP.bank.name = string.Empty;
        LP.bank.item.AsParallel().ForAll(i => i.TurnToAir());
        LP.bank2.name = string.Empty;
        LP.bank2.item.AsParallel().ForAll(i => i.TurnToAir());
        LP.bank3.name = string.Empty;
        LP.bank3.item.AsParallel().ForAll(i => i.TurnToAir());
        LP.bank4.name = string.Empty;
        LP.bank4.item.AsParallel().ForAll(i => i.TurnToAir());
        LP.voidVaultInfo = 0;
        LP.buffType.Initialize();
        LP.buffTime.Initialize();
        LP.spX.Initialize();
        LP.spY.Initialize();
        LP.spI.Initialize();
        LP.spN.Initialize();
        LP.hbLocked = false;
        LP.hideInfo.Initialize();
        LP.anglerQuestsFinished = 0;
        LP.DpadRadial.Bindings.Initialize();
        LP.builderAccStatus.Initialize();
        LP.builderAccStatus[0] = 1;
        LP.bartenderQuestLog = 0;
        LP.dead = false;
        LP.respawnTimer = 0;
        LP.lastTimePlayerWasSaved = DateTime.UtcNow.ToBinary();
        LP.golferScoreAccumulated = 0;
        LP.creativeTracker.Reset();
        LP.trashItem.TurnToAir();
        Main.mouseItem.TurnToAir();
        Main.CreativeMenu.GetItemByIndex(0).TurnToAir();
        Main.guideItem.TurnToAir();
        Main.reforgeItem.TurnToAir();
        CreativePowerManager.Instance.ResetDataForNewPlayer(LP);
        LP.skinVariant = 0;
        SSC_IO.GetMethod("LoadModData", SSC_BF)?.Invoke(null, new[]
        {
            LP,
            SSC_IO.GetMethod("SaveModData", SSC_BF)?.Invoke(null, new object[] { new Player() })
        });
        SSC_IO.GetMethod("LoadUsedMods", SSC_BF)?.Invoke(null, new[]
        {
            LP,
            SSC_IO.GetMethod("SaveUsedMods", SSC_BF)?.Invoke(null, new object[] { new Player() })
        });
        LP.lavaTime = 0;
        LP.lavaMax = 0;
        LP.wingsLogic = 0;
        LP.equippedWings?.TurnToAir(); // Special, the default value is null.
        LP.noFallDmg = false;
        LP.ResetEffects();
        LP.PlayerFrame();
    }
}