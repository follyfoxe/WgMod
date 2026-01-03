using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Items.Accessories
{
    public class FlaskOfAmbrosia : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 26;

            Item.accessory = true;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.buyPrice(gold: 1);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (!player.TryGetModPlayer(out WgPlayer wg))
                return;
            wg._ambrosiaOnHit = true;
            wg.WeightLossFactor = +1f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SweetheartNecklace)
                .AddIngredient<Content.Items.Accessories.WeightLossPendant>()
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }
}
