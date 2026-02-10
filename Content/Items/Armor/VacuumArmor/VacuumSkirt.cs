using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Items.Armor.VacuumArmor;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[AutoloadEquip(EquipType.Legs)]
public class VacuumSkirt : ModItem
{
    WgStat _attackSpeed = new(1.06f, 1.12f);
    WgStat _health = new(50, 100);
    WgStat _defense = new(8, 16);
    WgStat _resist = new(0.03f, 0.06f);
    WgStat _movePenalty = new(1.10f, 0.95f);

    public override void SetDefaults()
    {
        Item.width = 18;
        Item.height = 18;
        Item.value = Item.sellPrice(gold: 1);
        Item.rare = ItemRarityID.Red;
        Item.defense = 32;
    }

    public override void UpdateEquip(Player player)
    {
        if (!player.TryGetModPlayer(out WgPlayer wg))
            return;
            
        float immobility = wg.Weight.ClampedImmobility;
        _attackSpeed.Lerp(immobility);
        
        _health.Lerp(immobility);
        _health.Value = MathF.Floor(_health.Value / 5f) * 5f;

        _defense.Lerp(immobility);
        _resist.Lerp(immobility);
        _movePenalty.Lerp(immobility);

        player.GetAttackSpeed(DamageClass.Generic) *= _attackSpeed;
        player.statLifeMax2 += _health;
        player.statDefense += _defense;
        player.endurance += _resist;
        wg.MovementPenalty *= _movePenalty;

        player.aggro += 5;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.FragmentSolar, 3)
            .AddIngredient(ItemID.FragmentNebula, 3)
            .AddIngredient(ItemID.FragmentVortex, 3)
            .AddIngredient(ItemID.FragmentStardust, 3)
            .AddIngredient(ItemID.LunarBar, 12)
            .AddTile(TileID.LunarCraftingStation)
            .Register();
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        tooltips.FormatLines((_attackSpeed - 1f).Percent(), _health, _defense, _resist.Percent(), (_movePenalty.Value - 1f).Percent());
    }
}
