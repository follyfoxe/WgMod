using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using System.Collections.Generic;
using WgMod.Common.Players;

namespace WgMod.Content.Items.Armor.AmazonGarb;

[AutoloadEquip(EquipType.Legs)]
public class AmazonSandals : ModItem
{
    WgStat _critChance = new(6f, 12f);

    public override void SetDefaults()
    {
        Item.width = 18;
        Item.height = 18;
        Item.value = Item.sellPrice(gold: 1);
        Item.rare = ItemRarityID.Orange;
        Item.defense = 1;
    }

    public override void UpdateEquip(Player player)
    {
        if (!player.TryGetModPlayer(out WgPlayer wg))
            return;
        float immobility = wg.Weight.ClampedImmobility;

        _critChance.Lerp(immobility);

        player.GetCritChance(DamageClass.Melee) += _critChance;
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        tooltips.FormatLines(_critChance);
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.HellstoneBar, 5)
            .AddIngredient(ItemID.Silk, 10)
            .Register();
    }
}