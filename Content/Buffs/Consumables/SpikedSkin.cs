using Terraria;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Buffs.Consumables;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor._d_u_m_m_y_)]
public class SpikedSkin : ModBuff
{
    public override void Update(Player player, ref int buffIndex)
    {
        if (!player.TryGetModPlayer(out SpikedSkinPlayer ss))
            return;
        ss._active = true;
    }
}

public class SpikedSkinPlayer : ModPlayer
{
    public bool _active;
    public int _thorns;

    public override void ResetEffects()
    {
        _active = false;
    }

    public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
    {
        if (!_active || !Player.TryGetModPlayer(out WgPlayer wg))
            return;
        float immobility = wg.Weight.ClampedImmobility;

        float damage = hurtInfo.Damage * float.Lerp(1f, 2f, immobility);
        int direction;

        if (npc.position.X < Player.position.X)
            direction = 1;
        else
            direction = -1;

        Player.ApplyDamageToNPC(npc, (int)damage, 1, direction);
    }
}
