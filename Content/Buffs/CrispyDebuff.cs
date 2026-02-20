using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace WgMod.Content.Buffs;

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

    public override void Update(NPC npc, ref int buffIndex)
    {
        int _dustRate = 15;
        if (Main.rand.NextBool(_dustRate))
        {
            Dust.NewDust(npc.position, npc.width, npc.height, DustID.t_Honey, 0f, 0.5f, 150, new Color(151, 93, 15), 1.3f);
        }
    }

    public override void Update(Player player, ref int buffIndex)
    {
        int _dustRate = 15;
        if (Main.rand.NextBool(_dustRate))
        {
            Dust.NewDust(player.position, player.width, player.height, DustID.t_Honey, 0f, 0.5f, 150, new Color(151, 93, 15), 1.3f);
        }
    }
}

public class CrispyDebuffNPC : GlobalNPC
{
    public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
    {
        if (projectile.npcProj || projectile.trap || !projectile.IsMinionOrSentryRelated)
            return;

        var projTagMultiplier = ProjectileID.Sets.SummonTagDamageMultiplier[projectile.type];
        if (npc.HasBuff<CrispyDebuff>())
        {
            modifiers.FlatBonusDamage += CrispyDebuff.TagDamage * projTagMultiplier;
        }
    }

    public override void DrawEffects(NPC npc, ref Color drawColor)
    {
        if (npc.HasBuff<CrispyDebuff>())
        {
            drawColor = new Color(151, 93, 15);
        }
    }
}
