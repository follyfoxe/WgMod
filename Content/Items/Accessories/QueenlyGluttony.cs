using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Items.Accessories;

public class QueenlyGluttony : ModItem
{
    float _queenlyGluttonyDamage;
    float _queenlyGluttonyAttackSpeed;
    int _queenlyGluttonyCritChance;
    int _queenlyGluttonyArmorPenetration;

    public override void SetDefaults()
    {
        Item.width = 22;
        Item.height = 22;

        Item.accessory = true;
        Item.rare = ItemRarityID.Pink;
        Item.value = Item.buyPrice(gold: 1);
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        if (!player.TryGetModPlayer(out WgPlayer wg))
            return;

        float immobility = wg.Weight.ClampedImmobility;
        _queenlyGluttonyDamage = float.Lerp(1.05f, 1.2f, immobility);
        _queenlyGluttonyAttackSpeed = float.Lerp(0.95f, 0.8f, immobility);
        _queenlyGluttonyCritChance = (int)float.Lerp(5f, 10f, immobility);
        _queenlyGluttonyArmorPenetration = (int)float.Lerp(2f, 6f, immobility);

        player.GetDamage(DamageClass.Melee) *= _queenlyGluttonyDamage;
        player.GetAttackSpeed(DamageClass.Melee) *= _queenlyGluttonyAttackSpeed;
        player.GetCritChance(DamageClass.Melee) += _queenlyGluttonyCritChance;
        player.GetArmorPenetration(DamageClass.Melee) += _queenlyGluttonyArmorPenetration;

        wg._queenlyGluttony = true;
    }
}
