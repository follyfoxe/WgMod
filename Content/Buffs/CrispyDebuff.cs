using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Buffs
{
    public class CrispyDebuff : ModBuff
    {
        public static readonly int TagDamage = 10;

        public override void SetStaticDefaults()
        {
            BuffID.Sets.IsATagBuff[Type] = true;

            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.LongerExpertDebuff[Type] = true;
        }

        /*public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<CrispyDebuffNPC>()._caramelizedDebuff = true;
        }*/
    }

    public class CrispyDebuffNPC : GlobalNPC
    {
        /*public bool _caramelizedDebuff;

        public override void ResetEffects(NPC npc)
        {
            _caramelizedDebuff = false;
        }*/

        public override void ModifyHitByProjectile(
            NPC npc,
            Projectile projectile,
            ref NPC.HitModifiers modifiers
        )
        {
            if (projectile.npcProj || projectile.trap || !projectile.IsMinionOrSentryRelated)
                return;

            var projTagMultiplier = ProjectileID.Sets.SummonTagDamageMultiplier[projectile.type];
            if (npc.HasBuff<CrispyDebuff>())
            {
                modifiers.FlatBonusDamage += CrispyDebuff.TagDamage * projTagMultiplier;
            }
        }

        /*public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (_caramelizedDebuff == true)
            {
                if (npc.lifeRegen > 0)
                {
                    npc.lifeRegen = 0;
                }

                npc.lifeRegen -= 16;
            }
        }*/

        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
            if (npc.HasBuff<CrispyDebuff>())
            {
                drawColor = new Color(80, 40, 20);
            }
        }
    }
}
