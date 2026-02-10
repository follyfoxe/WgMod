using Terraria;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Buffs;

public class GluttedGut : ModBuff
{
    WgStat _moveSpeed = new(0.9f, 0.75f);
    WgStat _damage = new(1.25f, 1.5f);
    WgStat _attackSpeed = new(0.9f, 0.75f);

    public override void SetStaticDefaults()
    {
        Main.debuff[Type] = false;
        Main.pvpBuff[Type] = true;
        Main.buffNoSave[Type] = true;
    }

    public override void Update(Player player, ref int buffIndex)
    {
        if (!player.TryGetModPlayer(out WgPlayer wg))
            return;

        float immobility = wg.Weight.ClampedImmobility;
        _moveSpeed.Lerp(immobility);
        _damage.Lerp(immobility);
        _attackSpeed.Lerp(immobility);

        player.moveSpeed *= _moveSpeed;
        player.maxRunSpeed *= _moveSpeed;
        player.runAcceleration *= _moveSpeed;
        player.accRunSpeed *= _moveSpeed;
        player.GetDamage(DamageClass.Generic) *= _damage;
        player.GetAttackSpeed(DamageClass.Generic) *= _attackSpeed;
    }

    public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
    {
        tip = base.Description.Format((_moveSpeed - 1f).Percent(), (_damage - 1f).Percent(), (_attackSpeed - 1f).Percent());
    }
}
