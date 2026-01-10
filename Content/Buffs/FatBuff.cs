using System;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using WgMod.Common.Configs;
using WgMod.Common.Players;

namespace WgMod.Content.Buffs;

public class FatBuff : WgBuffBase
{
    Asset<Texture2D> _stagesTexture;

    public override void SetStaticDefaults()
    {
        Main.buffNoTimeDisplay[Type] = true;
        Main.buffNoSave[Type] = true;
        BuffID.Sets.TimeLeftDoesNotDecrease[Type] = true;
        _stagesTexture = ModContent.Request<Texture2D>($"{Texture}_Stages");
    }
    public override bool RightClick(int buffIndex)
    {
        return false;
    }
    public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
    {
        Player plr = Main.LocalPlayer;

        buffName = this.GetLocalizedValue("Stages." + plr.WG().Weight.GetStage());
        if (ModContent.GetInstance<WgServerConfig>().DisableFatBuffs)
            tip = this.GetLocalizedValue("DisabledBuffs");
        else
        {
            string tipMobility = plr.WG().FatBuffMovementPenalty >= 0.01f ?
                Language.GetText(
                "Mods.WgMod.Buffs.FatBuff.Mobility").Format((int)Math.Floor(plr.WG().FatBuffMovementPenalty * 100)) + "\n"
                : "";

            string tipAttackSpeed = plr.WG().FatBuffAttackSpeedPenalty >= 0.01f ?
                Language.GetText(
                "Mods.WgMod.Buffs.FatBuff.AttackSpeed").Format((int)Math.Floor(plr.WG().FatBuffAttackSpeedPenalty * 100)) + "\n"
                : "";

            string tipMaxLife = plr.WG().FatBuffMaxLife > 0 ?
                Language.GetText(
                "Mods.WgMod.Buffs.FatBuff.MaxLife").Format(plr.WG().FatBuffMaxLife) + "\n"
                : "";

            string tipDR = plr.WG().FatBuffDamageReduction >= 0.01f ?
                Language.GetText(
                "Mods.WgMod.Buffs.FatBuff.DR").Format((int)Math.Floor(plr.WG().FatBuffDamageReduction * 100)) : "";

            tip = tipMobility + tipAttackSpeed + tipMaxLife + tipDR;
        }
    }

    public override bool PreDraw(SpriteBatch spriteBatch, int buffIndex, ref BuffDrawParams drawParams)
    {
        if (Main.LocalPlayer.TryGetModPlayer(out WgPlayer wg))
        {
            drawParams.Texture = _stagesTexture.Value;
            drawParams.SourceRectangle = drawParams.Texture.Frame(1, wg.Weight.StageCount + 1, 0, wg.Weight.GetStage());
        }
        return base.PreDraw(spriteBatch, buffIndex, ref drawParams);
    }

    public override float GetProgress(WgPlayer wg, int buffIndex)
    {
        return wg.Weight.GetStageFactor();
    }
}
