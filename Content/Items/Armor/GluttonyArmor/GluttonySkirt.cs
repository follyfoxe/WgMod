using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Items.Armor.GluttonyArmor;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[AutoloadEquip(EquipType.Legs)]
public class GluttonySkirt : ModItem
{
    WgStat _damage = new(0f, 0.05f);
    WgStat _critChance = new(1, 1.06f);
    WgStat _defense = new(0f, 6f);
    WgStat _resist = new(0f, 0.01f);

    public override void SetDefaults()
    {
        Item.width = 30;
        Item.height = 20;
        Item.value = Item.sellPrice(gold: 1, silver: 20);
        Item.rare = ItemRarityID.Orange;
        Item.defense = 10;
    }

    public override void UpdateEquip(Player player)
    {
        if (!player.TryGetModPlayer(out WgPlayer wg))
            return;
        float immobility = wg.Weight.ClampedImmobility;
        _damage.Lerp(immobility);
        _critChance.Lerp(immobility);
        _defense.Lerp(immobility);
        _resist.Lerp(immobility);

        player.GetDamage(DamageClass.Generic) += _damage;
        player.GetCritChance(DamageClass.Generic) *= _critChance;
        player.statDefense += _defense;
        player.endurance += _resist;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.HellstoneBar, 5)
            .AddIngredient(ItemID.MeteoriteBar, 5)
            .AddIngredient(ItemID.BeeWax, 5)
            .AddIngredient(ItemID.Bone, 5)
            .AddTile(TileID.Anvils)
            .Register();
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        tooltips.FormatLines(_damage.Percent(), (_critChance - 1f).Percent(), _defense, _resist);
    }
}
