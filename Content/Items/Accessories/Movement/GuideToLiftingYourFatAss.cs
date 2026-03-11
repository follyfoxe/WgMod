using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Items.Accessories.Movement;

[Credit(ProjectRole.Programmer, Contributor.jumpsu2)]
public class GuideToLiftingYourFatAss : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 20;
        Item.height = 20;

        Item.accessory = true;
        Item.rare = ItemRarityID.Orange;
        Item.value = Item.buyPrice(gold: 1);
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        if (player.TryGetModPlayer(out GuideToLiftingPlayer lp))
            lp._enabledGuideToLifting = true;
    }
}

public class GuideToLiftingPlayer : ModPlayer
{
    internal int _guideToLiftingTimer;
    internal bool _enabledGuideToLifting;

    public override void ResetEffects()
    {
        _enabledGuideToLifting = false;
    }

    public override void PostUpdateEquips()
    {
        if (_enabledGuideToLifting)
            LiftYourFatAss();
    }

    void LiftYourFatAss()
    {
        if (!AttemptingToMove(Player))
            return;

        WgPlayer wg = Player.Wg();
        _guideToLiftingTimer++;

        bool usesMana = _guideToLiftingTimer >= 10;
        float magicDamage = (Player.GetDamage(DamageClass.Magic).ApplyTo(500f) - 400f) / 100f;
        int manaToUse = (int)Math.Round((wg.Weight.Mass - Weight.Base.Mass) / 30f / magicDamage) - 1;

        if (manaToUse <= 0)
            return;

        if (Player.CheckMana(manaToUse, usesMana, false))
        {
            if (usesMana)
            {
                _guideToLiftingTimer = 0;
                Player.manaRegenDelay = Math.Max(Player.manaRegenDelay, 11);
            }
            wg.MovementPenalty *= 0.5f;
        }
    }

    static bool AttemptingToMove(Player player)
    {
        return player.controlDown || player.controlUp || player.controlRight || player.controlLeft || player.controlJump;
    }
}
