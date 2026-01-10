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

        public static float _ambrosiaMoveSpeedMax = 1.75f;
        public static int _ambrosiaDefenseMax = 10;
        public static int _ambrosiaRegenMax = 20;

        public void UpdateStats(Player player)
        {
            _ambrosiaMoveSpeed = player.WG().Weight.GetStatModifierFromWeight(1.15f, 0.01f, 4f, 0.6f, 1.25f, _ambrosiaMoveSpeedMax);
            _ambrosiaDefense = player.WG().Weight.GetStatModifierFromWeight(0, 1, 12f, 3f, 1, _ambrosiaDefenseMax);
            _ambrosiaRegen = player.WG().Weight.GetStatModifierFromWeight(0, 1, 12f, 3f, 2, _ambrosiaRegenMax);
        }

        public static void GetStats(Player player, out float speed, out float def, out float regen)
        {
            speed = player.WG().Weight.GetStatModifierFromWeight(1.15f, 0.01f, 4f, 0.6f, 1.25f, _ambrosiaMoveSpeedMax);
            def = player.WG().Weight.GetStatModifierFromWeight(0, 1, 12f, 3f, 1, _ambrosiaDefenseMax);
            regen = player.WG().Weight.GetStatModifierFromWeight(0, 1, 6f, 2f, 4, _ambrosiaRegenMax);
        }

        public override void Update(Player player, ref int buffIndex)
        {
            UpdateStats(player);

            player.moveSpeed *= _ambrosiaMoveSpeed;
            player.maxRunSpeed *= _ambrosiaMoveSpeed;
            player.runAcceleration *= _ambrosiaMoveSpeed;
            player.accRunSpeed *= _ambrosiaMoveSpeed;
            player.statDefense += _ambrosiaDefense;
            player.lifeRegen += _ambrosiaRegen;

            player.WG().MovementPenaltyReduction *= 1f - 0.15f;

            player.WG().PassiveWeightGain += 0.8f; //gain 0.8kg every second
        }
    }
}
