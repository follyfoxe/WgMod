using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Content.Projectiles;

namespace WgMod.Content.Items.Accessories;

[Credit(ProjectRole.Programmer, Contributor.jumpsu2)]
public class MeteorCrush : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 20;
        Item.height = 20;

        Item.accessory = true;
        Item.rare = ItemRarityID.Orange;
        Item.value = Item.buyPrice(gold: 1);
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        if (player.TryGetModPlayer(out MeteorCrushPlayer crush))
            crush._crushEffect = true;
    }
}

public class MeteorCrushPlayer : ModPlayer
{
    internal bool _crushEffect;

    /// <summary> If the player is on the ground or not.<br/>
    /// 0 = The player is in the air. <br/>
    /// 1 = The player just landed. <br/>
    /// 2 = The player is on the ground. <br/>
    /// </summary>
    internal int _landState = 2;

    float _yVelocityOfLastTick = 0;

    public override void ResetEffects()
    {
        _crushEffect = false;
    }

    public override void PostUpdate()
    {
        if (!_crushEffect)
            return;
        CheckForSolidGround();
        Projectile hitbox = null;
        foreach (var proj in Main.ActiveProjectiles)
        {
            if (proj.active && proj.type == ModContent.ProjectileType<Girth>() && proj.ai[0] == Player.whoAmI)
            {
                hitbox = proj;
                break;
            }
        }
        if (hitbox == null)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.position, Vector2.Zero, ModContent.ProjectileType<Girth>(), 0, 0, Player.whoAmI);
            else if (Player == Main.LocalPlayer)
                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.position, Vector2.Zero, ModContent.ProjectileType<Girth>(), 0, 0, Player.whoAmI);
        }
        if (_landState == 1)
        {
            float crushPower = _yVelocityOfLastTick * Player.Wg().Weight.Mass / 120f - 10;
            if (crushPower > 10)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, ModContent.ProjectileType<Girthquake>(), (int)crushPower, 0, Player.whoAmI, 0, 8f + crushPower / 12f);
                else if (Player == Main.LocalPlayer)
                    Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, ModContent.ProjectileType<Girthquake>(), (int)crushPower, 0, Player.whoAmI, 0, 8f + crushPower / 12f);
                SoundEngine.PlaySound(WgSounds.Stomp, Player.Center);
            }
        }
        _yVelocityOfLastTick = Player.velocity.Y;
    }

    bool CheckForSolidGround()
    {
        List<Point> tiles = Collision.GetTilesIn(Player.Hitbox.BottomLeft() - new Vector2(-2, -2), Player.Hitbox.BottomRight() + new Vector2(2, 6));
        bool hasSolidTile = false;
        foreach (var point in tiles)
        {
            Tile tile = Framing.GetTileSafely(point);
            if (tile.HasTile)
            {
                if (Main.tileSolid[tile.TileType])
                    hasSolidTile = true;
                if (Main.tileSolidTop[tile.TileType])
                    hasSolidTile = true;
            }
        }
        if (hasSolidTile)
        {
            if (_landState == 0)
                _landState = 1;
            else
                _landState = 2;
        }
        else
            _landState = 0;
        return hasSolidTile;
    }
}
