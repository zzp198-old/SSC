using ReLogic.Content;
using ReLogic.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace QOS.Unity.Visual.Systems;

public class HWYTSystem : ModSystem
{
    public override bool IsLoadingEnabled(Mod mod)
    {
        return Common.QOS.CC.HWYT;
    }

    public override void OnModLoad()
    {
        FontAssets.ItemStack = Mod.Assets.Request<DynamicSpriteFont>("Assets/Fonts/Item_Stack", AssetRequestMode.ImmediateLoad);
        FontAssets.MouseText = Mod.Assets.Request<DynamicSpriteFont>("Assets/Fonts/Mouse_Text", AssetRequestMode.ImmediateLoad);
        FontAssets.DeathText = Mod.Assets.Request<DynamicSpriteFont>("Assets/Fonts/Death_Text", AssetRequestMode.ImmediateLoad);
        FontAssets.CombatText[0] = Mod.Assets.Request<DynamicSpriteFont>("Assets/Fonts/Combat_Text", AssetRequestMode.ImmediateLoad);
        FontAssets.CombatText[1] = Mod.Assets.Request<DynamicSpriteFont>("Assets/Fonts/Combat_Crit", AssetRequestMode.ImmediateLoad);
    }

    public override void OnModUnload()
    {
        FontAssets.ItemStack = Main.Assets.Request<DynamicSpriteFont>("Fonts/Item_Stack", AssetRequestMode.ImmediateLoad);
        FontAssets.MouseText = Main.Assets.Request<DynamicSpriteFont>("Fonts/Mouse_Text", AssetRequestMode.ImmediateLoad);
        FontAssets.DeathText = Main.Assets.Request<DynamicSpriteFont>("Fonts/Death_Text", AssetRequestMode.ImmediateLoad);
        FontAssets.CombatText[0] = Main.Assets.Request<DynamicSpriteFont>("Fonts/Combat_Text", AssetRequestMode.ImmediateLoad);
        FontAssets.CombatText[1] = Main.Assets.Request<DynamicSpriteFont>("Fonts/Combat_Crit", AssetRequestMode.ImmediateLoad);
    }
}