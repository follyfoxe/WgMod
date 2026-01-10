using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WgMod.Common.Projectiles
{
    public class Lifesteal : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(Projectile projectile, bool lateInstantiation) => projectile.type is ProjectileID.VampireHeal or ProjectileID.SpiritHeal; 
        public override void AI(Projectile projectile)
        {
            int num410 = (int)projectile.ai[0];
            float num411 = 4f;
            Vector2 vector29 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
            float num412 = Main.player[num410].Center.X - vector29.X;
            float num413 = Main.player[num410].Center.Y - vector29.Y;
            float num414 = (float)Math.Sqrt((double)(num412 * num412 + num413 * num413));
            if (num414 < 50f && projectile.position.X < Main.player[num410].position.X + (float)Main.player[num410].width && projectile.position.X + (float)projectile.width > Main.player[num410].position.X && projectile.position.Y < Main.player[num410].position.Y + (float)Main.player[num410].height && projectile.position.Y + (float)projectile.height > Main.player[num410].position.Y)
            {
                if (projectile.owner == Main.myPlayer && !Main.player[Main.myPlayer].moonLeech)
                {
                    int num415 = (int)projectile.ai[1];
                    Player player3 = Main.player[num410];
                    /*
                    Main.player[num410].HealEffect(num415, false);
                    player3.statLife += num415;
                    if (Main.player[num410].statLife > Main.player[num410].statLifeMax2)
                    {
                        Main.player[num410].statLife = Main.player[num410].statLifeMax2;
                    }
                    */
                    Main.player[num410].Heal(num415);
                    NetMessage.SendData(66, -1, -1, null, num410, (float)num415, 0f, 0f, 0, 0, 0);
                }
                projectile.Kill();
            }
            num414 = num411 / num414;
            num412 *= num414;
            num413 *= num414;
            projectile.velocity.X = (projectile.velocity.X * 15f + num412) / 16f;
            projectile.velocity.Y = (projectile.velocity.Y * 15f + num413) / 16f;
            int num3;
            if (projectile.type == 305)
            {
                for (int num416 = 0; num416 < 3; num416 = num3 + 1)
                {
                    float num417 = projectile.velocity.X * 0.334f * (float)num416;
                    float num418 = -(projectile.velocity.Y * 0.334f) * (float)num416;
                    int num419 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 183, 0f, 0f, 100, default(Color), 1.1f);
                    Main.dust[num419].noGravity = true;
                    Dust dust2 = Main.dust[num419];
                    dust2.velocity *= 0f;
                    ref float ptr = ref Main.dust[num419].position.X;
                    ptr -= num417;
                    ptr = ref Main.dust[num419].position.Y;
                    ptr -= num418;
                    num3 = num416;
                }
                return;
            }
            for (int num420 = 0; num420 < 5; num420 = num3 + 1)
            {
                float num421 = projectile.velocity.X * 0.2f * (float)num420;
                float num422 = -(projectile.velocity.Y * 0.2f) * (float)num420;
                int num423 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 175, 0f, 0f, 100, default(Color), 1.3f);
                Main.dust[num423].noGravity = true;
                Dust dust2 = Main.dust[num423];
                dust2.velocity *= 0f;
                ref float ptr = ref Main.dust[num423].position.X;
                ptr -= num421;
                ptr = ref Main.dust[num423].position.Y;
                ptr -= num422;
                num3 = num420;
            }
            return;
        }
    }
}
