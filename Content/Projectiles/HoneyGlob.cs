using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace WgMod.Content.Projectiles;

public class HoneyGlob : ModProjectile
{
    public override void SetStaticDefaults()
    {
        Main.projFrames[Type] = 3;
    }
    public override void SetDefaults()
    {
        Projectile.width = 10;
        Projectile.height = 10;

        Projectile.friendly = true;
        Projectile.DamageType = DamageClass.Melee;
        Projectile.timeLeft = 1 * 60;
    }

    public override void AI()
    {
        Projectile.ai[0] += 1f;
        if (Projectile.ai[0] >= 15f)
        {
            Projectile.ai[0] = 15f;
            Projectile.velocity.Y += 0.25f;
        }
        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

        if (Projectile.velocity.Y > 16f)
        {
            Projectile.velocity.Y = 16f;
        }

        if (++Projectile.frameCounter >= 4)
        {
            Projectile.frameCounter = 0;
            if (++Projectile.frame >= Main.projFrames[Type])
                Projectile.frame = 0;
        }
    }

    public override void OnKill(int timeLeft)
    {
        if (Main.rand.NextBool(2))
            SoundEngine.PlaySound(SoundID.NPCDeath1, Projectile.position);

        for (int i = 0; i < 5; i++)
        {
            Dust dust = Dust.NewDustDirect(
                Projectile.position,
                Projectile.width,
                Projectile.height,
                DustID.Honey
            );
            dust.noGravity = true;
            dust.velocity *= 5f;
            dust.scale *= 0.9f;
        }
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        target.AddBuff(BuffID.Honey, 2 * 60);
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        target.AddBuff(BuffID.Honey, 2 * 60);
    }

    public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
    {
        Dust dust = Dust.NewDustDirect(
            Projectile.position,
            Projectile.width,
            Projectile.height,
            DustID.Honey
        );
        int dustRate = 10;

        if (Main.rand.NextBool(dustRate))
        {
            dust.noGravity = true;
            dust.velocity *= 5f;
            dust.scale *= 0.9f;
        }
    }
}
