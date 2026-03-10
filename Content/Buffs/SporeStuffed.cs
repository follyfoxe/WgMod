using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace WgMod.Content.Buffs;

public class SporeStuffed : ModBuff
{
    public override void SetStaticDefaults()
    {
        Main.debuff[Type] = true;
        Main.pvpBuff[Type] = true;
        Main.buffNoSave[Type] = true;
        BuffID.Sets.LongerExpertDebuff[Type] = true;
    }

    public override void Update(NPC npc, ref int buffIndex)
    {
        npc.GetGlobalNPC<SporeStuffedNPC>()._active = true;
    }
}

public class SporeStuffedNPC : GlobalNPC
{
    public bool _active;

    public override void ResetEffects(NPC npc)
    {
        _active = false;
    }

    public override bool InstancePerEntity => true;

    public override void UpdateLifeRegen(NPC npc, ref int damage)
    {
        if (!_active)
            return;

        damage = 5;

        if (npc.lifeRegen > 0)
            npc.lifeRegen = 0;

        npc.lifeRegen -= 20;
    }
}