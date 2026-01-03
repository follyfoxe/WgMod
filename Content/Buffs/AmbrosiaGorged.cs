using System;
using Terraria;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Buffs
{
    public class AmbrosiaGorged : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        float _ambrosiaMoveSpeed;
        int _ambrosiaDefense;
        int _ambrosiaRegen;

        public override void Update(Player player, ref int buffIndex)
        {
            if (!player.TryGetModPlayer(out WgPlayer wg))
                return;

            float immobility = wg.Weight.ClampedImmobility;

            _ambrosiaMoveSpeed = float.Lerp(1.25f, 1.5f, immobility);
            _ambrosiaDefense = (int)float.Lerp(1f, 10f, immobility);
            _ambrosiaRegen = (int)float.Lerp(1f, 5f, immobility);

            player.moveSpeed *= _ambrosiaMoveSpeed;
            player.maxRunSpeed *= _ambrosiaMoveSpeed;
            player.runAcceleration *= _ambrosiaMoveSpeed;
            player.accRunSpeed *= _ambrosiaMoveSpeed;
            player.statDefense += _ambrosiaDefense;
            player.lifeRegen += _ambrosiaRegen;
        }

        public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
        {
            if (!Main.LocalPlayer.TryGetModPlayer(out WgPlayer wg))
                return;
            else
                tip = base.Description.Format(
                    MathF.Round(_ambrosiaMoveSpeed * 100f - 100f),
                    _ambrosiaDefense,
                    _ambrosiaRegen
                );
        }
    }
}
