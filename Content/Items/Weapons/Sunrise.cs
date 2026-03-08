using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WgMod.Content.Items.Weapons;

public class Sunrise : ModItem
{
    public override void SetDefaults()
    {
        Item.rare = ItemRarityID.Orange;
        Item.value = Item.sellPrice(gold: 1, silver: 50);

        Item.useStyle = ItemUseStyleID.Swing;
        Item.useAnimation = 25;
        Item.useTime = 25;
        Item.UseSound = SoundID.Item1;
        Item.autoReuse = true;

        Item.damage = 33;
        Item.knockBack = 5f;
        Item.noUseGraphic = true;
        Item.noMelee = true;
        Item.DamageType = DamageClass.Melee;

        Item.shootSpeed = 12f;
        Item.shoot = ModContent.ProjectileType<Projectiles.SunriseProjectile>();
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.BoneJavelin, 1)
            .AddIngredient(ItemID.HellstoneBar, 24)
            .AddTile(TileID.Anvils)
            .Register();
    }
}