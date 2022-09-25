using System;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;
using CreativePowers = On.Terraria.GameContent.Creative.CreativePowers;

namespace QOS.Class.Creative;

public class PermissionSystem : ModSystem
{
    public override void Load()
    {
    
        On.Terraria.GameContent.Creative.CreativePowersHelper.IsAvailableForPlayer +=
            On_GameContent_Creative_CreativePowersHelper_IsAvailableForPlayer;
    }

    private bool On_GameContent_Creative_CreativePowers_DifficultySliderPower_GetIsUnlocked(
        CreativePowers.DifficultySliderPower.orig_GetIsUnlocked orig,
        Terraria.GameContent.Creative.CreativePowers.DifficultySliderPower self)
    {
        return false;
    }

    private static bool On_GameContent_Creative_CreativePowersHelper_IsAvailableForPlayer(
        On.Terraria.GameContent.Creative.CreativePowersHelper.orig_IsAvailableForPlayer orig, ICreativePower power, int plr)
    {
        return true;
    }

    public override void Unload()
    {
        On.Terraria.GameContent.Creative.CreativePowersHelper.IsAvailableForPlayer -=
            On_GameContent_Creative_CreativePowersHelper_IsAvailableForPlayer;
    }
}