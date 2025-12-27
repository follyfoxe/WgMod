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

    float _movementFactor = 1f;
    float _damageReduction = 0f;

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
        tip = base.Description.Format(MathF.Round((1f - _movementFactor) * 100f), MathF.Round(_damageReduction * 100f));
    }

    public override void Update(Player player, ref int buffIndex)
    {
        if (ModContent.GetInstance<WgServerConfig>().DisableFatBuffs || !player.TryGetModPlayer(out WgPlayer wg))
        {
            _movementFactor = 1f;
            _damageReduction = 0f;
            return;
        }

        // Calculate factors
        float immobility = wg.Weight.ClampedImmobility;
        _damageReduction = immobility * MaxDamageReduction;
        if (wg.Weight.GetStage() < Weight.ImmobileStage)
            _movementFactor = float.Lerp(1f, 0.2f, immobility * immobility);
        else
            _movementFactor = 0f;

        // Apply factors
        player.endurance += _damageReduction;
        wg._movementFactor = _movementFactor;
    }

    public override float GetProgress(WgPlayer wg, int buffIndex)
    {
        int stage = wg.Weight.GetStage();
        if (stage < Weight.ImmobileStage)
            return wg.Weight.GetStageFactor();
        return 1f;
    }
}
