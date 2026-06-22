using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Content.Projectiles.Melee;
using WgMod.Common.Players;

namespace WgMod.Content.Items.Weapons.Melee;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.igobee_)]

public class NightcrawlerSlashes : ModItem
{
    WgStat _damage = new(1f, 1.5f);
    WgStat _knockback = new(1f, 0.75f);

    public override void SetDefaults()
    {
        Item.CloneDefaults(ItemID.ShroomiteDiggingClaw);

        Item.damage = 12;
        Item.pick = 0;
        Item.axe = 0;

        Item.shoot = ModContent.ProjectileType<NightcrawlerSlashEnergy>();
        Item.noMelee = true;
        Item.shootsEveryUse = true;
        Item.autoReuse = true;
    }

    public override void UpdateInventory(Player player)
    {
        if (!player.TryGetModPlayer(out WgPlayer wg))
            return;
        float immobility = wg.Weight.ClampedImmobility;
        _damage.Lerp(immobility);
        _knockback.Lerp(immobility);
    }

    public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
    {
        damage *= _damage;
    }

    public override void ModifyWeaponKnockback(Player player, ref StatModifier knockback)
    {
        knockback *= _knockback;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        float adjustedItemScale = player.GetAdjustedItemScale(Item);
        Projectile.NewProjectile(source, player.MountedCenter, new Vector2(player.direction, 0f), type, damage, knockback, player.whoAmI, player.direction * player.gravDir, player.itemAnimationMax, adjustedItemScale);
        NetMessage.SendData(MessageID.PlayerControls, -1, -1, null, player.whoAmI);

        return base.Shoot(player, source, position, velocity, type, damage, knockback);
    }

}