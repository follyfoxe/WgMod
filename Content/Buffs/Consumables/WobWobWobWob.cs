using System;
using System.IO;
using System.Numerics;
using SteelSeries.GameSense;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Buffs.Consumables;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
public class WobWobWobWob : ModBuff
{
    WgStat _jumpSpeedBoost = new(5f, 15f);

    public override void SetStaticDefaults()
    {
        Main.debuff[Type] = false;
        Main.pvpBuff[Type] = true;
        Main.buffNoSave[Type] = true;
    }

    public override void Update(Player player, ref int buffIndex) // Todo: Add more bouncy! Make enemies bounce away from the player! Make the player bounce off of walls!
    {
        if (!player.TryGetModPlayer(out WgPlayer wg))
            return;
        if (!player.TryGetModPlayer(out WobPlayer wb))
            return;
        float immobility = wg.Weight.ClampedImmobility;
        _jumpSpeedBoost.Lerp(immobility);
        player.jumpSpeedBoost += _jumpSpeedBoost;
        player.maxFallSpeed += _jumpSpeedBoost;

        wb._wobBounce = true;
    }

    public class WobPlayer : ModPlayer
    {
        internal bool _wobBounce;

        WgStat _wobDamage = new(5, 20);
        WgStat _wobKnockback = new(100, 500);

        public override void ResetEffects()
        {
            _wobBounce = false;
        }

        public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
        {
            base.OnHitByNPC(npc, hurtInfo);

            if (!Player.TryGetModPlayer(out WgPlayer wg))
                return;

            float immobility = wg.Weight.ClampedImmobility;

            _wobDamage.Lerp(immobility);
            _wobKnockback.Lerp(immobility);
            if (_wobBounce)
            {
                wg._squishVel *= 0.5f;
                wg._squishPos *= 0.5f;

                Player.ApplyDamageToNPC(
                    npc,
                    _wobDamage,
                    _wobKnockback,
                    -1 * npc.direction,
                    false,
                    null,
                    false
                );
            }
        }
    }
}
