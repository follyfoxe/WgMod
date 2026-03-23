using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace WgMod.Content.Projectiles.Ranged;

public class Plate : ModProjectile
{
	public override void SetDefaults()
	{
		Projectile.width = 16;
		Projectile.height = 16;
		Projectile.aiStyle = ProjAIStyleID.Arrow;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.DamageType = DamageClass.Ranged;
		Projectile.ignoreWater = true;
		Projectile.tileCollide = true;
	}

	public override void OnKill(int timeLeft)
	{
		for (int i = 0; i < 10; i++)
			Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Glass, 0f, 0f, 100, default, 1);

		SoundEngine.PlaySound(SoundID.Shatter, Projectile.Center);
	}
}