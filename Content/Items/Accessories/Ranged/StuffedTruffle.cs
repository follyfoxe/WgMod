using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using WgMod.Content.Projectiles.Ranged;
using System;
using WgMod.Common.Players;

namespace WgMod.Content.Items.Accessories.Ranged;

class StuffedTruffle : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 40;
        Item.height = 40;

        Item.accessory = true;
        Item.rare = ItemRarityID.Blue;
        Item.value = Item.buyPrice(silver: 20);
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        if (!player.TryGetModPlayer(out StuffedTrufflePlayer st))
            return;

        st._active = true;

        if (st._cooldown < StuffedTrufflePlayer.CooldownMax)
            st._cooldown++;
    }
}

public class StuffedTrufflePlayer : ModPlayer
{
    public const int CooldownMax = 200;
    internal bool _active;
    internal int _cooldown = CooldownMax;
    internal WgStat _damage = new(5f, 15f);

    public override void ResetEffects()
    {
        _active = false;
    }
}

public class StuffedTruffleItem : GlobalItem
{
    public override void UseAnimation(Item item, Player player)
    {
        if (!player.TryGetModPlayer(out StuffedTrufflePlayer st) || !player.TryGetModPlayer(out WgPlayer wg) || !st._active || st._cooldown < StuffedTrufflePlayer.CooldownMax || item.DamageType != DamageClass.Ranged)
            return;
        float immobility = wg.Weight.ClampedImmobility;

        st._cooldown = (int)float.Lerp(0, 20, immobility);

        st._damage.Lerp(immobility);

        float angle = (float)(Main.rand.NextDouble() * Math.Tau);
        Vector2 dir = new(MathF.Cos(angle), MathF.Sin(angle));

        Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, new Vector2(dir.X * 0.1f, dir.Y * 0.1f), ModContent.ProjectileType<SporeCloud>(), st._damage, 0);
    }
}