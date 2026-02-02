using System;
using Terraria;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Buffs
{
    public class Weightless : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        float _weightlessMovementPenalty;

        public override void Update(Player player, ref int buffIndex)
        {
            if (!player.TryGetModPlayer(out WgPlayer wg))
                return;

            float immobility = wg.Weight.ClampedImmobility;

            _weightlessMovementPenalty = float.Lerp(1f, 0.8f, immobility);

            wg.MovementPenalty *= _weightlessMovementPenalty;
        }

        public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
        {
            if (!Main.LocalPlayer.TryGetModPlayer(out WgPlayer wg))
                return;
            else
                tip = base.Description.Format(
                    MathF.Round(_weightlessMovementPenalty * -100f + 100f)
                );
        }
    }
}
