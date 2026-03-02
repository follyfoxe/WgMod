using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Content.Items.Consumables;

namespace WgMod.Content.Items.Ammo;

public class CaramelArrow : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 99;
    }

    public override void SetDefaults()
    {
        Item.width = 14;
        Item.height = 36;

        Item.damage = 14;
        Item.DamageType = DamageClass.Ranged;

        Item.maxStack = Item.CommonMaxStack;
        Item.consumable = true;
        Item.knockBack = 2f;
        Item.value = Item.sellPrice(copper: 16);
        Item.shoot = ModContent.ProjectileType<Projectiles.CaramelArrowProjectile>();
        Item.shootSpeed = 3.5f;
        Item.ammo = AmmoID.Arrow;
    }

    public override void AddRecipes()
    {
        CreateRecipe(25)
            .AddIngredient(ItemID.FlamingArrow, 25)
            .AddIngredient<BottledCaramel>()
            .Register();
    }
}
