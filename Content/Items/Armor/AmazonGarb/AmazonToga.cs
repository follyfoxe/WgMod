using Terraria;
using Terraria.ID;
using System.Collections.Generic;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Items.Armor.AmazonGarb;

[AutoloadEquip(EquipType.Body)]
public class AmazonToga : ModItem
{
    WgStat _damage = new(0.06f, 0.12f);
    WgStat _critChance = new(0.12f, 0.24f);

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

        _damage.Lerp(immobility);
        _critChance.Lerp(immobility);

        player.GetDamage(DamageClass.Melee) += _damage;
        player.GetCritChance(DamageClass.Melee) += _critChance;
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        tooltips.FormatLines(_damage.Percent(), _critChance.Percent());
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.HellstoneBar, 8)
            .AddIngredient(ItemID.Silk, 16)
            .Register();
    }
}