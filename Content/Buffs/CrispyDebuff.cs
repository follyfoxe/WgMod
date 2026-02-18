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
        }
    }

    public class ExampleWhipDebuffNPC : GlobalNPC
    {
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
    }
}
