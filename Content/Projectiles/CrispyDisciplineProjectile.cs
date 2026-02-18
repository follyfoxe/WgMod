using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Players;
using WgMod.Content.Buffs;
using WgMod.Content.Projectiles.Minions;

namespace WgMod.Content.Projectiles
{
    public class CrispyDisciplineProjectile : ModProjectile
    {
        WgStat _damage = new(25, 30);
        WgStat _knockback = new(4, 8);

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.IsAWhip[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ownerHitCheck = true;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.DamageType = DamageClass.SummonMeleeSpeed;
            Projectile.WhipSettings.Segments = 10;
            Projectile.WhipSettings.RangeMultiplier = 1.5f;
        }

        private float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            Projectile.Center = Main.GetPlayerArmPosition(Projectile) + Projectile.velocity * Timer;
            Projectile.spriteDirection = Projectile.velocity.X >= 0f ? 1 : -1;

            Timer++;

            float swingTime = owner.itemAnimationMax * Projectile.MaxUpdates;
            if (Timer >= swingTime || owner.itemAnimation <= 0)
            {
                Projectile.Kill();
                return;
            }

            owner.heldProj = Projectile.whoAmI;
            if (Timer == swingTime / 2)
            {
                List<Vector2> points = Projectile.WhipPointsForCollision;
                Projectile.FillWhipControlPoints(Projectile, points);
                SoundEngine.PlaySound(SoundID.Item153, points[points.Count - 1]);
            }

            float swingProgress = Timer / swingTime;
            if (
                Utils.GetLerpValue(0.1f, 0.7f, swingProgress, clamped: true)
                    * Utils.GetLerpValue(0.9f, 0.7f, swingProgress, clamped: true)
                    > 0.5f
                && !Main.rand.NextBool(3)
            )
            {
                List<Vector2> points = Projectile.WhipPointsForCollision;
                points.Clear();
                Projectile.FillWhipControlPoints(Projectile, points);
                int pointIndex = Main.rand.Next(points.Count - 10, points.Count);
                Rectangle spawnArea = Utils.CenteredRectangle(
                    points[pointIndex],
                    new Vector2(30f, 30f)
                );
                int dustType = DustID.Lava;
                if (Main.rand.NextBool(3))
                    dustType = DustID.YellowTorch;

                Dust dust = Dust.NewDustDirect(
                    spawnArea.TopLeft(),
                    spawnArea.Width,
                    spawnArea.Height,
                    dustType,
                    0f,
                    0f,
                    100,
                    Color.White
                );
                dust.position = points[pointIndex];
                dust.fadeIn = 0.3f;
                Vector2 spinningPoint = points[pointIndex] - points[pointIndex - 1];
                dust.noGravity = true;
                dust.velocity *= 0.5f;
                dust.velocity += spinningPoint.RotatedBy(owner.direction * ((float)Math.PI / 2f));
                dust.velocity *= 0.5f;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];

            if (!player.TryGetModPlayer(out WgPlayer wg))
                return;
            float immobility = wg.Weight.ClampedImmobility;

            _damage.Lerp(immobility);
            _knockback.Lerp(immobility);

            if (!player.HasBuff(ModContent.BuffType<HellsBeesBuff>()))
            {
                Projectile.NewProjectile(
                    player.GetSource_FromThis(),
                    player.position,
                    new Vector2(0, 0),
                    ModContent.ProjectileType<HellishBee>(),
                    _damage,
                    _knockback
                );
            }

            target.AddBuff(ModContent.BuffType<CrispyDebuff>(), 240);
            target.AddBuff(BuffID.OnFire, 4 * 60);
            player.AddBuff(ModContent.BuffType<HellsBeesBuff>(), 4 * 60);
            player.MinionAttackTargetNPC = target.whoAmI;
            Projectile.damage = (int)(Projectile.damage * 0.7f);
        }

        private void DrawLine(List<Vector2> list)
        {
            Texture2D texture = TextureAssets.FishingLine.Value;
            Rectangle frame = texture.Frame();
            Vector2 origin = new Vector2(frame.Width / 2, 2);

            Vector2 pos = list[0];
            for (int i = 0; i < list.Count - 1; i++)
            {
                Vector2 element = list[i];
                Vector2 diff = list[i + 1] - element;

                float rotation = diff.ToRotation() - MathHelper.PiOver2;
                Color color = Lighting.GetColor(element.ToTileCoordinates(), Color.White);
                Vector2 scale = new Vector2(1, (diff.Length() + 2) / frame.Height);

                Main.EntitySpriteDraw(
                    texture,
                    pos - Main.screenPosition,
                    frame,
                    color,
                    rotation,
                    origin,
                    scale,
                    SpriteEffects.None,
                    0
                );

                pos += diff;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            List<Vector2> list = new List<Vector2>();
            Projectile.FillWhipControlPoints(Projectile, list);

            DrawLine(list);

            SpriteEffects flip =
                Projectile.spriteDirection < 0
                    ? SpriteEffects.None
                    : SpriteEffects.FlipHorizontally;

            Texture2D texture = TextureAssets.Projectile[Type].Value;

            Vector2 pos = list[0];

            for (int i = 0; i < list.Count - 1; i++)
            {
                Rectangle frame = new Rectangle(0, 0, 10, 26);
                Vector2 origin = new Vector2(5, 8);
                float scale = 1;

                if (i == list.Count - 2)
                {
                    frame.Y = 74;
                    frame.Height = 18;

                    Projectile.GetWhipSettings(
                        Projectile,
                        out float timeToFlyOut,
                        out int _,
                        out float _
                    );
                    float t = Timer / timeToFlyOut;
                    scale = MathHelper.Lerp(
                        0.5f,
                        1.5f,
                        Utils.GetLerpValue(0.1f, 0.7f, t, true)
                            * Utils.GetLerpValue(0.9f, 0.7f, t, true)
                    );
                }
                else if (i > 10)
                {
                    frame.Y = 58;
                    frame.Height = 16;
                }
                else if (i > 5)
                {
                    frame.Y = 42;
                    frame.Height = 16;
                }
                else if (i > 0)
                {
                    frame.Y = 26;
                    frame.Height = 16;
                }

                Vector2 element = list[i];
                Vector2 diff = list[i + 1] - element;

                float rotation = diff.ToRotation() - MathHelper.PiOver2;
                Color color = Lighting.GetColor(element.ToTileCoordinates());

                Main.EntitySpriteDraw(
                    texture,
                    pos - Main.screenPosition,
                    frame,
                    color,
                    rotation,
                    origin,
                    scale,
                    flip,
                    0
                );

                pos += diff;
            }
            return false;
        }
    }

    /*public class CrispyDisciplinePlayer : ModPlayer
    {
        public bool _hellsBeesBuff;

        public override void PostUpdate()
        {
            if (!_hellsBeesBuff)
                return;

            Vector2 _velocity = new Vector2(0, 0);
            if (_hellsBeesTimer == 0)
            {
                Projectile.NewProjectile(
                    Player.GetSource_FromThis(),
                    Player.position,
                    _velocity,
                    ModContent.ProjectileType<HellishBee>(),
                    20,
                    20
                );
            }
            _hellsBeesTimer++;

            if (_hellsBeesTimer >= 4 * 60)
            {
                _hellsBeesTimer = 0;
                _hellsBeesBuff = false;
            }
        }
    }*/
}
