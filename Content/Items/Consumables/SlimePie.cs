using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Items.Consumables
{
    [Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
    public class SlimePie : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(int.MaxValue, 3));
            ItemID.Sets.FoodParticleColors[Type] =
            [
                new Color(186, 222, 239),
                new Color(162, 117, 242),
                new Color(72, 33, 148),
            ];

            ItemID.Sets.IsFood[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.DefaultToFood(
                22,
                22,
                ModContent.BuffType<Buffs.Consumables.WobWobWobWob>(),
                10 * 60 * 60
            );
            Item.value = Item.buyPrice(0, 3);
            Item.rare = ItemRarityID.Blue;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SlimeBlock, 6)
                .AddIngredient(ItemID.Gel, 12)
                .AddIngredient(ItemID.PinkGel, 3)
                .AddTile<Tiles.Furniture.Oven>()
                .Register();
        }
    }
}
