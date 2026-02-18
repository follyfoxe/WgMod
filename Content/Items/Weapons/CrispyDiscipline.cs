using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using WgMod.Common.Players;
using WgMod.Content.Buffs;
using WgMod.Content.Projectiles;

namespace WgMod.Content.Items.Weapons
{
    [Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
    public class CrispyDiscipline : ModItem
    {
        WgStat _damage = new(25, 35);
        WgStat _attackSpeed = new(30, 40);
        WgStat _knockback = new(1.5f, 3f);

        public override LocalizedText Tooltip =>
            base.Tooltip.WithFormatArgs(CrispyDebuff.TagDamage);

        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.SummonMeleeSpeed;
            Item.damage = 30;
            Item.knockBack = 2;
            Item.rare = ItemRarityID.Green;

            Item.shoot = ModContent.ProjectileType<CrispyDisciplineProjectile>();
            Item.shootSpeed = 4;

            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.UseSound = SoundID.Item152;
            Item.noMelee = true;
            Item.noUseGraphic = true;
        }

        public override void UpdateInventory(Player player)
        {
            if (!player.TryGetModPlayer(out WgPlayer wg))
                return;
            float immobility = wg.Weight.ClampedImmobility;
            _damage.Lerp(immobility);
            _attackSpeed.Lerp(immobility);
            _knockback.Lerp(immobility);

            Item.damage = _damage;
            Item.useTime = _attackSpeed;
            Item.useAnimation = _attackSpeed;
            Item.knockBack = _knockback;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.CrispyHoneyBlock, 12)
                .AddIngredient(ItemID.BeeWax, 6)
                .AddIngredient(ItemID.Rope, 24)
                .AddTile(TileID.Anvils)
                .Register();
        }

        public override bool MeleePrefix()
        {
            return true;
        }
    }
}
