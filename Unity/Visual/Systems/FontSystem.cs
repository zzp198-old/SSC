using ReLogic.Content;
using ReLogic.Graphics;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace QOS.Unity.Visual.Systems;

public class FontSystem : ModSystem
{
    public override void Load()
    {
        FontAssets.ItemStack = Mod.Assets.Request<DynamicSpriteFont>("Assets/Fonts/Item_Stack", AssetRequestMode.ImmediateLoad);
        FontAssets.MouseText = Mod.Assets.Request<DynamicSpriteFont>("Assets/Fonts/Mouse_Text", AssetRequestMode.ImmediateLoad);
        FontAssets.DeathText = Mod.Assets.Request<DynamicSpriteFont>("Assets/Fonts/Death_Text", AssetRequestMode.ImmediateLoad);
        FontAssets.CombatText[0] = Mod.Assets.Request<DynamicSpriteFont>("Assets/Fonts/Combat_Text", AssetRequestMode.ImmediateLoad);
        FontAssets.CombatText[1] = Mod.Assets.Request<DynamicSpriteFont>("Assets/Fonts/Combat_Crit", AssetRequestMode.ImmediateLoad);
    }
}