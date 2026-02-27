using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Items.Armor.GluttonyArmor;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[AutoloadEquip(EquipType.Body)]
public class GluttonyCrop : ModItem
{
    WgStat _damage = new(0f, 0.05f);
    WgStat _attackSpeed = new(1f, 0.96f);
    WgStat _defense = new(0f, 6f);
    WgStat _resist = new(0f, 1f);

    public override void SetDefaults()
    {
        Item.width = 22;
        Item.height = 18;
        Item.value = Item.sellPrice(silver: 60);
        Item.rare = ItemRarityID.Orange;
        Item.defense = 11;
    }

    public override void UpdateEquip(Player player)
    {
        if (!player.TryGetModPlayer(out WgPlayer wg))
            return;

        float immobility = wg.Weight.ClampedImmobility;
        _damage.Lerp(immobility);
        _attackSpeed.Lerp(immobility);
        _defense.Lerp(immobility);
        _resist.Lerp(immobility);

        player.GetDamage(DamageClass.Generic) += _damage;
        player.GetAttackSpeed(DamageClass.Generic) *= _attackSpeed;
        player.statDefense += _defense;
        player.endurance += _resist;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.HellstoneBar, 7)
            .AddIngredient(ItemID.MeteoriteBar, 7)
            .AddIngredient(ItemID.BeeWax, 7)
            .AddIngredient(ItemID.Bone, 7)
            .AddTile(TileID.Anvils)
            .Register();
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        tooltips.FormatLines(_damage.Percent(), (1f - _attackSpeed).Percent(), _defense, _resist);
    }
}
