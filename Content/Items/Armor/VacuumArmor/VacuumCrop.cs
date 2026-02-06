using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Items.Armor.VacuumArmor;

[AutoloadEquip(EquipType.Body)]
public class VacuumCrop : ModItem
{
    WgStat _attack = new(0.06f, 0.12f);
    WgStat _health = new(100, 200);
    WgStat _defense = new(12, 24);
    WgStat _resist = new(0.03f, 0.06f);
    WgStat _movePenalty = new(1.15f, 0.9f);

    public override void SetDefaults()
    {
        Item.width = 18;
        Item.height = 18;
        Item.value = Item.sellPrice(gold: 1);
        Item.rare = ItemRarityID.Red;
        Item.defense = 46;
    }

    public override void UpdateEquip(Player player)
    {
        if (!player.TryGetModPlayer(out WgPlayer wg))
            return;

        float immobility = wg.Weight.ClampedImmobility;
        _attack.Lerp(immobility);

        _health.Lerp(immobility);
        _health.Value = MathF.Floor(_health.Value / 5f) * 5f;

        _defense.Lerp(immobility);
        _resist.Lerp(immobility);
        _movePenalty.Lerp(immobility);

        player.GetDamage(DamageClass.Generic) += _attack;
        player.statLifeMax2 += _health;
        player.statDefense += _defense;
        player.endurance += _resist;
        wg.MovementPenalty *= _movePenalty;

        player.aggro += 5;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.FragmentSolar, 5)
            .AddIngredient(ItemID.FragmentNebula, 5)
            .AddIngredient(ItemID.FragmentVortex, 5)
            .AddIngredient(ItemID.FragmentStardust, 5)
            .AddIngredient(ItemID.LunarBar, 16)
            .AddTile(TileID.LunarCraftingStation)
            .Register();
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        tooltips.FormatLines(_attack.Percent(), _health, _defense, _resist.Percent(), (_movePenalty.Value - 1f).Percent());
    }
}
