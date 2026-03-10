using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace WgMod.Content.Projectiles;

public class SporeCloud : ModProjectile
{
    public const int _cooldownMax = 30;
    public int _cooldown = _cooldownMax;
    public int _spinnyDirection;
    public float _spinnySpeed;
    public float _time;

    public override void SetStaticDefaults()
    {
        Main.projFrames[Type] = 5;
    }

    public override void SetDefaults()
    {
        Projectile.width = 160;
        Projectile.height = 160;
        Projectile.friendly = true;
        Projectile.DamageType = DamageClass.Ranged;

        Projectile.penetrate = -1;
        Projectile.timeLeft = 8 * 60;
        Projectile.tileCollide = false;
    }

    public override void OnSpawn(IEntitySource source)
    {
        Projectile.rotation = float.Lerp(0, MathF.Tau, Main.rand.NextFloat());
        _spinnySpeed = float.Lerp(0.005f, 0.025f, Main.rand.NextFloat());
        _spinnyDirection = Main.rand.NextFromList(-1, 1);
    }

    public override void AI()
    {
        if (_cooldown < _cooldownMax)
            _cooldown++;
        else
            _cooldown = 0;

        Projectile.rotation += _spinnySpeed * _spinnyDirection;

        _time += 0.0075f;

        float t = MathF.Sin(_time);

        Projectile.scale = t;
        Projectile.Opacity = t * 0.5f;
        //Projectile.width = (int)float.Lerp(0, 160, t); // Scaling the height and width causes the projectile to spawn above the player
        //Projectile.height = (int)float.Lerp(0, 160, t); // Figure out a fix later

        if (_time > MathF.PI)
            Projectile.Kill();

        Projectile.frameCounter++;
        if (Projectile.frameCounter > 30)
        {
            Projectile.frameCounter = 0;
            Projectile.frame++;
            Projectile.frame %= 5;
        }
    }

    public override bool? CanDamage()
    {
        return _cooldown == 0;
    }
}