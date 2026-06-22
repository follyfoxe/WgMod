using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace WgMod.Content.Projectiles.Melee;

public class NightcrawlerSlashEnergy : ModProjectile
{
	public override void SetStaticDefaults()
	{
		ProjectileID.Sets.AllowsContactDamageFromJellyfish[Type] = true;
		Main.projFrames[Type] = 4;
	}

	public override void SetDefaults()
	{
		Projectile.width = 16;
		Projectile.height = 16;
		Projectile.friendly = true;
		Projectile.DamageType = DamageClass.Melee;
		Projectile.penetrate = 3;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = -1;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = true;
		Projectile.ownerHitCheck = true;
		Projectile.ownerHitCheckDistance = 300f;
		Projectile.usesOwnerMeleeHitCD = true;
		Projectile.stopsDealingDamageAfterPenetrateHits = true;

		Projectile.aiStyle = -1;

		Projectile.noEnchantmentVisuals = true;
	}

	public override void AI()
	{
		// In our item, we spawn the projectile with the direction, max time, and scale
		// Projectile.ai[0] == direction
		// Projectile.ai[1] == max time
		// Projectile.ai[2] == scale
		// Projectile.localAI[0] == current time

		// Terra Blade makes an extra sound when spawning.
		// if (Projectile.localAI[0] == 0f) {
		// 	SoundEngine.PlaySound(SoundID.Item60 with { Volume = 0.65f }, Projectile.position);
		// }

		Projectile.localAI[0]++; // Current time that the projectile has been alive.
		Player player = Main.player[Projectile.owner];
		float percentageOfLife = Projectile.localAI[0] / Projectile.ai[1]; // The current time over the max time.
		float direction = Projectile.ai[0];
		float velocityRotation = Projectile.velocity.ToRotation();
		float adjustedRotation = MathHelper.Pi * direction * percentageOfLife + velocityRotation + direction * MathHelper.Pi + player.fullRotation;
		Projectile.rotation = adjustedRotation; // Set the rotation to our to the new rotation we calculated.

		float scaleMulti = 0.2f; // Excalibur, Terra Blade, and The Horseman's Blade is 0.6f; True Excalibur is 1f; default is 0.2f 
		float scaleAdder = 0.6f; // Excalibur, Terra Blade, and The Horseman's Blade is 1f; True Excalibur is 1.2f; default is 1f 

		Projectile.Center = player.RotatedRelativePoint(player.MountedCenter) - Projectile.velocity;
		Projectile.scale = scaleAdder + percentageOfLife * scaleMulti;

		float dustRotation = Projectile.rotation + Main.rand.NextFloatDirection() * MathHelper.PiOver2 * 0.7f;
		Vector2 dustPosition = Projectile.Center + dustRotation.ToRotationVector2() * 84f * Projectile.scale;
		Vector2 dustVelocity = (dustRotation + Projectile.ai[0] * MathHelper.PiOver2).ToRotationVector2();
		if (Main.rand.NextFloat() * 2f < Projectile.Opacity)
		{
			Color dustColor = Color.Lerp(Color.Red, Color.DarkRed, Main.rand.NextFloat() * 0.3f);
			Dust coloredDust = Dust.NewDustPerfect(Projectile.Center + dustRotation.ToRotationVector2() * (Main.rand.NextFloat() * 80f * Projectile.scale + 20f * Projectile.scale), DustID.FireworksRGB, dustVelocity * 1f, 100, dustColor, 0.4f);
			coloredDust.fadeIn = 0.4f + Main.rand.NextFloat() * 0.15f;
			coloredDust.noGravity = true;
		}

		if (Main.rand.NextFloat() * 1.5f < Projectile.Opacity)
		{
			Dust.NewDustPerfect(dustPosition, DustID.TintableDustLighted, dustVelocity, 100, Color.Red * Projectile.Opacity, 1.2f * Projectile.Opacity);
		}

		Projectile.scale *= Projectile.ai[2];

		if (Projectile.localAI[0] >= Projectile.ai[1])
		{
			Projectile.Kill();
		}

		for (float i = -MathHelper.PiOver4; i <= MathHelper.PiOver4; i += MathHelper.PiOver2)
		{
			Rectangle rectangle = Utils.CenteredRectangle(Projectile.Center + (Projectile.rotation + i).ToRotationVector2() * 70f * Projectile.scale, new Vector2(60f * Projectile.scale, 60f * Projectile.scale));
			Projectile.EmitEnchantmentVisualsAt(rectangle.TopLeft(), rectangle.Width, rectangle.Height);
		}
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		float coneLength = 94f * Projectile.scale;

		float collisionRotation = MathHelper.Pi * 2f / 25f * Projectile.ai[0];
		float maximumAngle = MathHelper.PiOver4;
		float coneRotation = Projectile.rotation + collisionRotation;

		if (targetHitbox.IntersectsConeSlowMoreAccurate(Projectile.Center, coneLength, coneRotation, maximumAngle))
		{
			return true;
		}

		float backOfTheSwing = Utils.Remap(Projectile.localAI[0], Projectile.ai[1] * 0.3f, Projectile.ai[1] * 0.5f, 1f, 0f);
		if (backOfTheSwing > 0f)
		{
			float coneRotation2 = coneRotation - MathHelper.PiOver4 * Projectile.ai[0] * backOfTheSwing;

			if (targetHitbox.IntersectsConeSlowMoreAccurate(Projectile.Center, coneLength, coneRotation2, maximumAngle))
			{
				return true;
			}
		}

		return false;
	}

	public override void CutTiles()
	{
		Vector2 starting = (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * 60f * Projectile.scale;
		Vector2 ending = (Projectile.rotation + MathHelper.PiOver4).ToRotationVector2() * 60f * Projectile.scale;
		float width = 60f * Projectile.scale;
		Utils.PlotTileLine(Projectile.Center + starting, Projectile.Center + ending, width, DelegateMethods.CutTiles);
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		ParticleOrchestrator.RequestParticleSpawn(clientOnly: false, ParticleOrchestraType.ChlorophyteLeafCrystalShot,
			new ParticleOrchestraSettings { PositionInWorld = Main.rand.NextVector2FromRectangle(target.Hitbox) },
			Projectile.owner);

		hit.HitDirection = (Main.player[Projectile.owner].Center.X < target.Center.X) ? 1 : (-1);
	}

	public override void OnHitPlayer(Player target, Player.HurtInfo info)
	{
		ParticleOrchestrator.RequestParticleSpawn(clientOnly: false, ParticleOrchestraType.ChlorophyteLeafCrystalShot,
			new ParticleOrchestraSettings { PositionInWorld = Main.rand.NextVector2FromRectangle(target.Hitbox) },
			Projectile.owner);

		info.HitDirection = (Main.player[Projectile.owner].Center.X < target.Center.X) ? 1 : (-1);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Vector2 position = Projectile.Center - Main.screenPosition;
		Texture2D texture = TextureAssets.Projectile[Type].Value;
		Rectangle sourceRectangle = texture.Frame(1, 4);
		Vector2 origin = sourceRectangle.Size() / 2f;
		float scale = Projectile.scale * 1.1f;
		SpriteEffects spriteEffects = (!(Projectile.ai[0] >= 0f)) ? SpriteEffects.FlipVertically : SpriteEffects.None;
		float percentageOfLife = Projectile.localAI[0] / Projectile.ai[1];
		float lerpTime = Utils.Remap(percentageOfLife, 0f, 0.6f, 0f, 1f) * Utils.Remap(percentageOfLife, 0.6f, 1f, 1f, 0f);
		float lightingColor = Lighting.GetColor(Projectile.Center.ToTileCoordinates()).ToVector3().Length() / (float)Math.Sqrt(3.0);
		lightingColor = Utils.Remap(lightingColor, 0.2f, 1f, 0f, 1f);

		Color backDarkColor = new(255, 80, 100);
		Color middleMediumColor = new(255, 135, 120);
		Color frontLightColor = new(255, 240, 140);

		Color redTimesLerpTime = Color.Red * lerpTime * 0.5f;
		redTimesLerpTime.A = (byte)(redTimesLerpTime.A * (1f - lightingColor));
		Color faintLightingColor = redTimesLerpTime * lightingColor * 0.5f;
		faintLightingColor.G = (byte)(faintLightingColor.G * lightingColor);
		faintLightingColor.B = (byte)(faintLightingColor.R * (0.25f + lightingColor * 0.75f));

		Main.EntitySpriteDraw(texture, position, sourceRectangle, backDarkColor * lightingColor * lerpTime, Projectile.rotation + Projectile.ai[0] * MathHelper.PiOver4 * -1f * (1f - percentageOfLife), origin, scale, spriteEffects, 0f);
		Main.EntitySpriteDraw(texture, position, sourceRectangle, faintLightingColor * 0.15f, Projectile.rotation + Projectile.ai[0] * 0.01f, origin, scale, spriteEffects, 0f);
		Main.EntitySpriteDraw(texture, position, sourceRectangle, middleMediumColor * lightingColor * lerpTime * 0.3f, Projectile.rotation, origin, scale, spriteEffects, 0f);
		Main.EntitySpriteDraw(texture, position, sourceRectangle, frontLightColor * lightingColor * lerpTime * 0.5f, Projectile.rotation, origin, scale * 0.975f, spriteEffects, 0f);
		Main.EntitySpriteDraw(texture, position, texture.Frame(1, 4, 0, 3), Color.Red * 0.6f * lerpTime, Projectile.rotation + Projectile.ai[0] * 0.01f, origin, scale, spriteEffects, 0f);
		Main.EntitySpriteDraw(texture, position, texture.Frame(1, 4, 0, 3), Color.Red * 0.5f * lerpTime, Projectile.rotation + Projectile.ai[0] * -0.05f, origin, scale * 0.8f, spriteEffects, 0f);
		Main.EntitySpriteDraw(texture, position, texture.Frame(1, 4, 0, 3), Color.Red * 0.4f * lerpTime, Projectile.rotation + Projectile.ai[0] * -0.1f, origin, scale * 0.6f, spriteEffects, 0f);

		for (float i = 0f; i < 8f; i += 1f)
		{
			float edgeRotation = Projectile.rotation + Projectile.ai[0] * i * (MathHelper.Pi * -2f) * 0.025f + Utils.Remap(percentageOfLife, 0f, 1f, 0f, MathHelper.PiOver4) * Projectile.ai[0];
			Vector2 drawPos = position + edgeRotation.ToRotationVector2() * (texture.Width * 0.5f - 6f) * scale;
			DrawPrettyStarSparkle(Projectile.Opacity, SpriteEffects.None, drawPos, new Color(255, 135, 120, 0) * lerpTime * (i / 9f), middleMediumColor, percentageOfLife, 0f, 0.5f, 0.5f, 1f, edgeRotation, new Vector2(0f, Utils.Remap(percentageOfLife, 0f, 1f, 3f, 0f)) * scale, Vector2.One * scale);
		}

		Vector2 drawPos2 = position + (Projectile.rotation + Utils.Remap(percentageOfLife, 0f, 1f, 0f, MathHelper.PiOver4) * Projectile.ai[0]).ToRotationVector2() * ((float)texture.Width * 0.5f - 4f) * scale;
		DrawPrettyStarSparkle(Projectile.Opacity, SpriteEffects.None, drawPos2, new Color(255, 135, 120, 0) * lerpTime * 0.5f, middleMediumColor, percentageOfLife, 0f, 0.5f, 0.5f, 1f, 0f, new Vector2(2f, Utils.Remap(percentageOfLife, 0f, 1f, 4f, 1f)) * scale, Vector2.One * scale);

		return false;
	}

	static void DrawPrettyStarSparkle(float opacity, SpriteEffects dir, Vector2 drawPos, Color drawColor, Color shineColor, float flareCounter, float fadeInStart, float fadeInEnd, float fadeOutStart, float fadeOutEnd, float rotation, Vector2 scale, Vector2 fatness)
	{
		Texture2D sparkleTexture = TextureAssets.Extra[ExtrasID.SharpTears].Value;
		Color bigColor = shineColor * opacity * 0.5f;
		bigColor.A = 0;
		Vector2 origin = sparkleTexture.Size() / 2f;
		Color smallColor = drawColor * 0.5f;
		float lerpValue = Utils.GetLerpValue(fadeInStart, fadeInEnd, flareCounter, clamped: true) * Utils.GetLerpValue(fadeOutEnd, fadeOutStart, flareCounter, clamped: true);
		Vector2 scaleLeftRight = new Vector2(fatness.X * 0.5f, scale.X) * lerpValue;
		Vector2 scaleUpDown = new Vector2(fatness.Y * 0.5f, scale.Y) * lerpValue;
		bigColor *= lerpValue;
		smallColor *= lerpValue;
		Main.EntitySpriteDraw(sparkleTexture, drawPos, null, bigColor, MathHelper.PiOver2 + rotation, origin, scaleLeftRight, dir);
		Main.EntitySpriteDraw(sparkleTexture, drawPos, null, bigColor, 0f + rotation, origin, scaleUpDown, dir);
		Main.EntitySpriteDraw(sparkleTexture, drawPos, null, smallColor, MathHelper.PiOver2 + rotation, origin, scaleLeftRight * 0.6f, dir);
		Main.EntitySpriteDraw(sparkleTexture, drawPos, null, smallColor, 0f + rotation, origin, scaleUpDown * 0.6f, dir);
	}
}