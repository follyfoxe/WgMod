using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Items.Armor.HexborneArmor;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[AutoloadEquip(EquipType.Head)]
public class HexborneHood : ModItem
{
    WgStat _damage = new(0.01f, 0.06f);
    WgStat _manaCost = new(1.02f, 1.08f);

    WgStat _setBonusDamage = new(0.05f, 0.15f);
    WgStat _setBonusDefense = new(3f, 5f);

    public override void SetDefaults()
    {
        Item.width = 22;
        Item.height = 20;
        Item.value = Item.sellPrice(silver: 70);
        Item.rare = ItemRarityID.Orange;
        Item.defense = 5;
    }

    public override void SetStaticDefaults()
    {
        SetBonusText = this.GetLocalization("SetBonus");
    }

    public static LocalizedText SetBonusText { get; private set; }

    public override void UpdateEquip(Player player)
    {
        if (!player.TryGetModPlayer(out WgPlayer wg))
            return;

        float immobility = wg.Weight.ClampedImmobility;
        _damage.Lerp(immobility);
        _manaCost.Lerp(immobility);

        player.GetDamage(DamageClass.Magic) += _damage;
        player.manaCost *= _manaCost;
    }

    public override bool IsArmorSet(Item head, Item body, Item legs)
    {
        return body.type == ModContent.ItemType<HexborneCrop>()
            && legs.type == ModContent.ItemType<HexborneSkirt>();
    }

    public override void UpdateArmorSet(Player player)
    {
        if (!player.TryGetModPlayer(out WgPlayer wg))
            return;

        float immobility = wg.Weight.ClampedImmobility;
        _setBonusDamage.Lerp(immobility);
        _setBonusDefense.Lerp(immobility);

        player.GetDamage(DamageClass.Magic) += _setBonusDamage;
        player.statDefense += _setBonusDefense;
        player.setBonus = SetBonusText.Format(_setBonusDamage.Percent(), _setBonusDefense);
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.DemoniteBar, 20)
            .AddIngredient(ItemID.PurificationPowder, 10)
            .AddIngredient(ItemID.ShadowScale, 5)
            .AddTile(TileID.Anvils)
            .Register();
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        tooltips.FormatLines(_damage.Percent(), (1 - _manaCost).Percent());
    }
}
