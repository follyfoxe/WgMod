using System;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Configs;
using WgMod.Common.Players;

namespace WgMod.Content.Buffs;

public class FatBuff : WgBuffBase
{
    public const int MaxLifeIncrease = 100;
    public const float MaxLifeIncreasePercentage = 0.2f;
    public const float MaxDamageReduction = 0.05f;

    float _damageReduction = 0f;
    int _lifeIncrease;

    Asset<Texture2D> _stagesTexture;

    public override void SetStaticDefaults()
    {
        Main.buffNoTimeDisplay[Type] = true;
        Main.buffNoSave[Type] = true;
        BuffID.Sets.TimeLeftDoesNotDecrease[Type] = true;
        _stagesTexture = ModContent.Request<Texture2D>($"{Texture}_Stages");
    }

    public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
    {
        if (!Main.LocalPlayer.TryGetModPlayer(out WgPlayer wg))
            return;
        buffName = this.GetLocalizedValue("Stages.Name" + wg.Weight.GetStage());
        if (WgServerConfig.Instance.DisableFatBuffs)
            tip = this.GetLocalizedValue("DisabledBuffs");
        else
            tip = base.Description.Format(MathF.Round((1f - wg._finalMovementFactor) * 100f), MathF.Round(_damageReduction * 100f), _lifeIncrease);
    }

    public override void Update(Player player, ref int buffIndex)
    {
        if (WgServerConfig.Instance.DisableFatBuffs || !player.TryGetModPlayer(out WgPlayer wg))
        {
            _damageReduction = 0f;
            _lifeIncrease = 0;
            return;
        }

        // Calculate factors
        int stage = wg.Weight.GetStage();
        if (stage >= Weight.DamageReductionStage)
            _damageReduction = wg.Weight.GetClampedFactor(Weight.FromStage(Weight.DamageReductionStage), Weight.Immobile) * MaxDamageReduction;
        else
            _damageReduction = 0f;

        if (stage >= Weight.HeavyStage)
        {
            float t = wg.Weight.GetClampedFactor(Weight.FromStage(Weight.HeavyStage), Weight.Immobile) * MaxLifeIncreasePercentage;
            _lifeIncrease = (int)MathF.Floor(player.statLifeMax * t / 5f) * 5;
            _lifeIncrease = Math.Clamp(_lifeIncrease, 0, MaxLifeIncrease);
        }
        else
            _lifeIncrease = 0;

        // Apply factors
        player.endurance += _damageReduction;
        player.statLifeMax2 += _lifeIncrease;
    }

    public override bool PreDraw(SpriteBatch spriteBatch, int buffIndex, ref BuffDrawParams drawParams)
    {
        if (Main.LocalPlayer.TryGetModPlayer(out WgPlayer wg))
        {
            drawParams.Texture = _stagesTexture.Value;
            drawParams.SourceRectangle = drawParams.Texture.Frame(1, Weight.StageCount, 0, wg.Weight.GetStage());
        }
        return base.PreDraw(spriteBatch, buffIndex, ref drawParams);
    }

    public override float GetProgress(WgPlayer wg, int buffIndex)
    {
        int stage = wg.Weight.GetStage();
        if (stage < Weight.ImmobileStage)
            return wg.Weight.GetStageFactor();
        return 1f;
    }
}
