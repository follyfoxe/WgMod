using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Configs;
using WgMod.Common.Players;

namespace WgMod.Content.Buffs;

public class FatBuff : ModBuff
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
        if (ModContent.GetInstance<WgServerConfig>().DisableBuffs || !Main.LocalPlayer.TryGetModPlayer(out WgPlayer wg))
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

    public override bool PreDraw(SpriteBatch spriteBatch, int buffIndex, ref BuffDrawParams drawParams)
    {
        drawParams.DrawColor = Color.DimGray;
        return true;
    }

    public override void PostDraw(SpriteBatch spriteBatch, int buffIndex, BuffDrawParams drawParams)
    {
        if (!Main.LocalPlayer.TryGetModPlayer(out WgPlayer wg))
            return;
        Rectangle rect = drawParams.SourceRectangle;
        int stage = wg.Weight.GetStage();
        if (stage < Weight.ImmobileStage)
        {
            int h = (int)MathF.Round(float.Lerp(0f, rect.Height, wg.Weight.GetStageFactor()) / 2f) * 2;
            spriteBatch.Draw(drawParams.Texture, drawParams.Position + new Vector2(0f, rect.Height - h), new Rectangle(rect.X, rect.Y + rect.Height - h, rect.Width, h), Color.White);
        }
        else
            spriteBatch.Draw(drawParams.Texture, drawParams.Position, Color.White);
        //Vector2 pos = drawParams.Position + rect.Center.ToVector2() * 0.5f;
        //spriteBatch.DrawString(FontAssets.ItemStack.Value, stage.ToString(), pos, Color.White);
    }
}
