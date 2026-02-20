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

        npc.GetGlobalNPC<CrispyDebuffNPC>().Caramelized = true;
    }

    public override void Update(Player player, ref int buffIndex)
    {
        int _dustRate = 15;
        if (Main.rand.NextBool(_dustRate))
        {
            Dust.NewDust(player.position, player.width, player.height, DustID.t_Honey, 0f, 0.5f, 150, new Color(151, 93, 15), 1.3f);
        }

        player.GetModPlayer<CrispyDebuffPlayer>().Caramelized = true;
    }
}

public class CrispyDebuffNPC : GlobalNPC
{
    public bool Caramelized;

    public override void ResetEffects(NPC npc)
    {
        Caramelized = false;
    }

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

    public override bool InstancePerEntity => true;

    public override void UpdateLifeRegen(NPC npc, ref int damage)
    {
        if (!Caramelized)
            return;

        damage = 5;

        if (npc.lifeRegen > 0)
            npc.lifeRegen = 0;

        npc.lifeRegen -= 20;
    }

    public override void DrawEffects(NPC npc, ref Color drawColor)
    {
        if (!Caramelized)
            return;
        drawColor = new Color(151, 93, 15);
    }
}

public class CrispyDebuffPlayer : ModPlayer
{
    public bool Caramelized;

    public override void ResetEffects()
    {
        Caramelized = false;
    }

    public override void UpdateBadLifeRegen()
    {
        if (!Caramelized)
            return;

        if (Player.lifeRegen > 0)
            Player.lifeRegen = 0;

        Player.lifeRegenTime = 0;
        Player.lifeRegen -= 20;
    }
}
