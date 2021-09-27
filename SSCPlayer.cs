using System.Linq;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace SSC
{
    public class SSCPlayer : ModPlayer
    {
        Item mouseItem;


        public override void clientClone(ModPlayer clientClone)
        {
            if (clientClone is not SSCPlayer clone) return;

            clone.Player.taxMoney = Player.taxMoney;
            clone.Player.hbLocked = Player.hbLocked;
            clone.Player.hideInfo = (bool[]) Player.hideInfo.Clone();
            clone.Player.anglerQuestsFinished = Player.anglerQuestsFinished;
            clone.Player.DpadRadial.Bindings = (int[]) Player.DpadRadial.Bindings.Clone();
            clone.Player.builderAccStatus = (int[]) Player.builderAccStatus.Clone();
            clone.Player.bartenderQuestLog = Player.bartenderQuestLog;
            clone.Player.golferScoreAccumulated = Player.golferScoreAccumulated;

            clone.mouseItem = Main.mouseItem.Clone();

            // clone.creativeTracker = Player.creativeTracker.Save();
            // clone.mouseItem = ItemIO.Save(Main.mouseItem);
            // clone.guideItem = ItemIO.Save(Main.guideItem);
            // clone.reforgeItem = ItemIO.Save(Main.reforgeItem);
            // clone.creativeMenu = ItemIO.Save(Main.CreativeMenu.GetItemByIndex(0));
            // clone.creativePowerManager = CreativePowerManager.Instance.SaveToPlayer(Player);
            // clone.hairDyeItemName = (string) SSCUtil.PlayerIO_Invoke("SaveHairDye", Player.hairDye);
            // clone.research = (List<TagCompound>) SSCUtil.PlayerIO_Invoke("SaveResearch", Player);
            // clone.modData = (List<TagCompound>) SSCUtil.PlayerIO_Invoke("SaveModData", Player);
            // clone.modBuffs = (List<TagCompound>) SSCUtil.PlayerIO_Invoke("SaveModBuffs", Player);
            // clone.infoDisplays = (List<string>) SSCUtil.PlayerIO_Invoke("SaveInfoDisplays", Player);
            // clone.usedMods = (List<string>) SSCUtil.PlayerIO_Invoke("LoadUsedMods", Player);
        }

        public override void SendClientChanges(ModPlayer clientPlayer)
        {
            if (clientPlayer is not SSCPlayer clone) return;

            var tagCompound = new TagCompound();

            if (!clone.Player.taxMoney.Equals(Player.taxMoney))
                tagCompound.Set("taxMoney", Player.taxMoney);
            if (!clone.Player.hbLocked.Equals(Player.hbLocked))
                tagCompound.Set("hbLocked", Player.hbLocked);
            if (!clone.Player.hideInfo.SequenceEqual(Player.hideInfo))
                tagCompound.Set("hideInfo", Player.hideInfo.ToList());
            if (!clone.Player.anglerQuestsFinished.Equals(Player.anglerQuestsFinished))
                tagCompound.Set("anglerQuestsFinished", Player.anglerQuestsFinished);
            if (!clone.Player.DpadRadial.Bindings.SequenceEqual(Player.DpadRadial.Bindings))
                tagCompound.Set("DpadRadial", Player.DpadRadial.Bindings);
            if (!clone.Player.builderAccStatus.SequenceEqual(Player.builderAccStatus))
                tagCompound.Set("builderAccStatus", Player.builderAccStatus);
            if (!clone.Player.bartenderQuestLog.Equals(Player.bartenderQuestLog))
                tagCompound.Set("bartenderQuestLog", Player.bartenderQuestLog);
            if (!clone.Player.golferScoreAccumulated.Equals(Player.golferScoreAccumulated))
                tagCompound.Set("golferScoreAccumulated", Player.golferScoreAccumulated);


            // var byteTemp = Player.creativeTracker.Save();
            // if (!clone.creativeTracker.SequenceEqual(byteTemp))
            //     tagCompound.Set("creativeTracker", byteTemp);
            //
            // var tagCompoundTemp = ItemIO.Save(Main.mouseItem);
            // if (clone.mouseItem.Equals(tagCompoundTemp))
            //     tagCompound.Set("mouseItem", tagCompoundTemp);
            //
            // tagCompoundTemp = ItemIO.Save(Main.guideItem);
            // if (clone.guideItem.Equals(tagCompoundTemp))
            //     tagCompound.Set("guideItem", tagCompoundTemp);
            //
            // tagCompoundTemp = ItemIO.Save(Main.reforgeItem);
            // if (clone.reforgeItem.Equals(tagCompoundTemp))
            //     tagCompound.Set("reforgeItem", tagCompoundTemp);
            //
            // tagCompoundTemp = ItemIO.Save(Main.CreativeMenu.GetItemByIndex(0));
            // if (clone.creativeMenu.Equals(tagCompoundTemp))
            //     tagCompound.Set("creativeMenu", tagCompoundTemp);


            if (tagCompound.Count < 1) return;
            var socket = Mod.GetPacket();
            socket.Write((byte) SSCMessageID.ClientChange);
            socket.Write(Main.myPlayer);
            socket.WriteTagCompound(tagCompound);
            socket.Send();
        }

        public override void PostUpdate()
        {
            if (Main.NetmodeID != NetmodeID.Server) return;

            var tagCompound = new TagCompound();
            tagCompound.Set("TaxMoney", Player.taxMoney);
            tagCompound.Set("Armor", Player.armor.Select(SSCUtil.SaveItemWithSlot));
            tagCompound.Set("Dye", Player.dye.Select(SSCUtil.SaveItemWithSlot));
            tagCompound.Set("MiscEquips", Player.miscEquips.Select(SSCUtil.SaveItemWithSlot));
            tagCompound.Set("MiscDyes", Player.miscDyes.Select(SSCUtil.SaveItemWithSlot));
            tagCompound.Set("Inventory", Player.inventory.Select(SSCUtil.SaveItemWithSlot));
            tagCompound.Set("Bank", Player.bank.item.Select(SSCUtil.SaveItemWithSlot));
            tagCompound.Set("Bank2", Player.bank2.item.Select(SSCUtil.SaveItemWithSlot));
            tagCompound.Set("Bank3", Player.bank3.item.Select(SSCUtil.SaveItemWithSlot));
            tagCompound.Set("Bank4", Player.bank4.item.Select(SSCUtil.SaveItemWithSlot));
            // Spawn
            tagCompound.Set("HbLocked", Player.hbLocked);
            tagCompound.Set("HideInfo", Player.hideInfo.ToList());
            tagCompound.Set("AnglerQuestsFinished", Player.anglerQuestsFinished);
            tagCompound.Set("DpadRadial", Player.DpadRadial.Bindings);
            tagCompound.Set("BuilderAccStatus", Player.builderAccStatus);
            tagCompound.Set("BartenderQuestLog", Player.bartenderQuestLog);
            tagCompound.Set("GolferScoreAccumulated", Player.golferScoreAccumulated);
            tagCompound.Set("CreativeTracker", Player.creativeTracker.Save());
            // tagCompound.Set("MouseItem", Player.mouseItem);
            // tagCompound.Set("GuideItem", Player.guideItem);
            // tagCompound.Set("ReforgeItem", Player.reforgeItem);
            // tagCompound.Set("CreativeMenu", Player.creativeMenu);
            // tagCompound.Set("CreativePowerManager", CreativePowerManager.Instance.SaveToPlayer(Player));// 复制,研究,个人能力不同步.其余每个世界需要单独保存. 

            // tagCompound.Set("HairDyeItemName", PlayerIO.SaveHairDye(player.hairDye));
            // tagCompound.Set("Research", PlayerIO.SaveResearch(player));
            // tagCompound.Set("ModData", PlayerIO.SaveModData(player));
            // tagCompound.Set("ModBuffs", PlayerIO.SaveModBuffs(player));
            // tagCompound.Set("InfoDisplays", PlayerIO.SaveInfoDisplays(player));
            // tagCompound.Set("UsedMods", PlayerIO.SaveUsedMods(player));
            // tagCompound.Set("Hair", player.hair);
            // tagCompound.Set("HairDye", PlayerIO.VanillaHairDye(player.hairDye));
            // tagCompound.Set("HideVisibleAccessory", player.hideVisibleAccessory.ToList());
            // tagCompound.Set("HideMisc", (byte) player.hideMisc);
            // tagCompound.Set("SkinVariant", player.skinVariant);
            // tagCompound.Set("StatLife", player.statLife);
            // tagCompound.Set("StatLifeMax", player.statLifeMax);
            // tagCompound.Set("StatMana", player.statMana);
            // tagCompound.Set("StatManaMax", player.statManaMax);
            // tagCompound.Set("ExtraAccessory", player.extraAccessory);
            // tagCompound.Set("UnlockedBiomeTorches", player.unlockedBiomeTorches);
            // tagCompound.Set("UsingBiomeTorches", player.UsingBiomeTorches);
            // tagCompound.Set("DownedDD2EventAnyDifficulty", player.downedDD2EventAnyDifficulty);
            // tagCompound.Set("HairColor", player.hairColor);
            // tagCompound.Set("SkinColor", player.skinColor);
            // tagCompound.Set("EyeColor", player.eyeColor);
            // tagCompound.Set("ShirtColor", player.shirtColor);
            // tagCompound.Set("UnderShirtColor", player.underShirtColor);
            // tagCompound.Set("PantsColor", player.pantsColor);
            // tagCompound.Set("ShoeColor", player.shoeColor);
            // tagCompound.Set("Dead", player.dead);
            // tagCompound.Set("RespawnTimer", player.respawnTimer);
        }
    }
}