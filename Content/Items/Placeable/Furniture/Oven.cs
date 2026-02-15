using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WgMod.Content.Items.Placeable.Furniture
{
    public class Oven : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.Oven>());
            Item.width = 26;
            Item.height = 22;
            Item.value = 500;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.CookingPot)
                .AddIngredient(ItemID.Wire, 12)
                .AddIngredient(ItemID.Fireblossom, 6)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
