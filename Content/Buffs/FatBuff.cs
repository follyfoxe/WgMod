using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Configs;
using WgMod.Common.Players;

namespace WgMod.Content.Buffs;

public class FatBuff : WgBuffBase
{
    public const float MaxDamageReduction = 0.1f;
    public const float MaxMeleeBoost = 0.1f;

    float _movementFactor = 1f;
    float _damageReduction = 0f;
    float _meleeBoost = 0f;

    public override void SetStaticDefaults()
    {
        Main.buffNoTimeDisplay[Type] = true;
        Main.buffNoSave[Type] = true;
        BuffID.Sets.TimeLeftDoesNotDecrease[Type] = true;
    }

    public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
    {
        if (!Main.LocalPlayer.TryGetModPlayer(out WgPlayer wg))
            return;
        buffName = this.GetLocalizedValue("Stages.Name" + wg.Weight.GetStage());
        tip = base.Description.Format(MathF.Round((1f - _movementFactor) * 100f), MathF.Round(_damageReduction * 100f), MathF.Round(_meleeBoost * 100f));
    }

    public override void Update(Player player, ref int buffIndex)
    {
        if (ModContent.GetInstance<WgServerConfig>().DisableFatBuffs || !player.TryGetModPlayer(out WgPlayer wg))
        {
            _movementFactor = 1f;
            _damageReduction = 0f;
            _meleeBoost = 0f;
            return;
        }

        // Calculate factors
        int stage = wg.Weight.GetStage();
        if (stage < Weight.ImmobileStage)
        {
            float immobility = wg.Weight.ClampedImmobility;
            _movementFactor = float.Lerp(1f, 0.3f, immobility * immobility);
        }
        else
            _movementFactor = 0f;

        if (stage >= Weight.DamageReductionStage)
            _damageReduction = wg.Weight.GetClampedFactor(Weight.FromStage(Weight.DamageReductionStage), Weight.Immobile) * MaxDamageReduction;
        else
            _damageReduction = 0f;

        if (stage >= Weight.HeavyStage)
            _meleeBoost = wg.Weight.GetClampedFactor(Weight.FromStage(Weight.HeavyStage), Weight.Immobile) * MaxMeleeBoost;
        else
            _meleeBoost = 0f;

        // Apply factors
        wg._movementFactor = _movementFactor;
        player.endurance += _damageReduction;
        player.GetDamage(DamageClass.Melee) += _meleeBoost;
    }

    public override float GetProgress(WgPlayer wg, int buffIndex)
    {
        int stage = wg.Weight.GetStage();
        if (stage < Weight.ImmobileStage)
            return wg.Weight.GetStageFactor();
        return 1f;
    }
}
