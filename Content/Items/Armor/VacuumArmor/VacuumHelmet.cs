using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Items.Armor.VacuumArmor;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[AutoloadEquip(EquipType.Head)]
public class VacuumHelmet : ModItem
{
    public const float SetBonusWeightLoss = 0.5f;

    WgStat _critChance = new(1.04f, 1.08f);
    WgStat _health = new(50, 100);
    WgStat _defense = new(6, 12);
    WgStat _resist = new(0.02f, 0.04f);
    WgStat _movePenalty = new(1.1f, 0.95f);

    WgStat _setBonusRegen = new(5, 20);

    public override void SetDefaults()
    {
        Item.width = 18;
        Item.height = 18;
        Item.value = Item.sellPrice(gold: 1);
        Item.rare = ItemRarityID.Red;
        Item.defense = 36;
    }

    public override void SetStaticDefaults()
    {
        SetBonusText = this.GetLocalization("SetBonus");
    }

    public static LocalizedText SetBonusText { get; private set; }

    public override bool IsArmorSet(Item head, Item body, Item legs)
    {
        return body.type == ModContent.ItemType<VacuumCrop>()
            && legs.type == ModContent.ItemType<VacuumSkirt>();
    }

    public override void UpdateArmorSet(Player player)
    {
        if (!player.TryGetModPlayer(out WgPlayer wg))
            return;

        float immobility = wg.Weight.ClampedImmobility;
        _setBonusRegen.Lerp(immobility);

        player.lifeRegen += _setBonusRegen;
        wg.WeightLossRate *= SetBonusWeightLoss;

        player.setBonus = SetBonusText.Format(_setBonusRegen, (1f - SetBonusWeightLoss).Percent());
    }

    public override void UpdateEquip(Player player)
    {
        if (!player.TryGetModPlayer(out WgPlayer wg))
            return;

        float immobility = wg.Weight.ClampedImmobility;
        _critChance.Lerp(immobility);
        
        _health.Lerp(immobility);
        _health.Value = MathF.Floor(_health.Value / 5f) * 5f;

        _defense.Lerp(immobility);
        _resist.Lerp(immobility);
        _movePenalty.Lerp(immobility);

        player.GetCritChance(DamageClass.Generic) *= _critChance;
        player.statLifeMax2 += _health;
        player.statDefense += _defense;
        player.endurance += _resist;
        wg.MovementPenalty *= _movePenalty;

        player.aggro += 5;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.FragmentSolar, 2)
            .AddIngredient(ItemID.FragmentNebula, 2)
            .AddIngredient(ItemID.FragmentVortex, 2)
            .AddIngredient(ItemID.FragmentStardust, 2)
            .AddIngredient(ItemID.LunarBar, 8)
            .AddTile(TileID.LunarCraftingStation)
            .Register();
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        tooltips.FormatLines((_critChance - 1f).Percent(), _health, _defense, _resist.Percent(), (_movePenalty.Value - 1f).Percent());
    }
}
