using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SSC.Items;

public class zzp198_Sword : ModItem
{
    public override void SetStaticDefaults()
    {
        // DisplayName.SetDefault("zzp198_Sword"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
        Tooltip.SetDefault("This is a basic modded sword.");
    }

    public override void SetDefaults()
    {
        Item.damage = 1900000;
        Item.DamageType = DamageClass.Melee;
        Item.width = 400;
        Item.height = 400;
        Item.useTime = 20;
        Item.useAnimation = 20;
        Item.useStyle = 1;
        Item.knockBack = 60;
        Item.value = 10000;
        Item.rare = 2;
        Item.UseSound = SoundID.Item1;
        Item.autoReuse = true;
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemID.DirtBlock, 10);
        recipe.AddTile(TileID.WorkBenches);
        recipe.Register();
    }
}