using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Items.Accessories;

public class GuideToLiftingYourFatAss : ModItem
{
    public override string Texture => "WgMod/Assets/Placeholder/ExampleItem";
    public override void SetDefaults()
    {
        Item.width = 20;
        Item.height = 20;

        Item.accessory = true;
        Item.rare = ItemRarityID.Orange;
        Item.value = Item.buyPrice(gold: 1);
    }

    public static bool AttemptingToMove(Player player)
    {
        return player.controlDown || player.controlUp || player.controlRight || player.controlLeft || player.controlJump;
    }

    public static void LiftYourFatAss(Player player)
    {
        if (!AttemptingToMove(player)) return;

        player.WG()._guideToLiftingTimer++;

        bool usesMana = player.WG()._guideToLiftingTimer >= 10;
        float MagicDamage = (player.GetDamage(DamageClass.Magic).ApplyTo(500f) - 400f) / 100f;
        int manaToUse = (int)Math.Round((player.WG().Weight.Mass - player.WG().Weight.Base) / 30 / MagicDamage) - 1;

        if (manaToUse <= 0) return;

        if (player.CheckMana(manaToUse, usesMana, false))
        {
            if (usesMana)
            {
                player.WG()._guideToLiftingTimer = 0;
                player.manaRegenDelay = Math.Max(player.manaRegenDelay, 11);
            }
            player.WG().MovementPenaltyReduction *= 0.5f;
        }
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.WG()._enabledGuideToLifting = true;
    }
}
