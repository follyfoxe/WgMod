using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Items.Armor.CrimatriarchArmor;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[AutoloadEquip(EquipType.Body)]
public class CrimatriarchGown : ModItem
{
    public const int MinionCount = 1;

    WgStat _damage = new(0.05f, 0.1f);
    WgStat _critChance = new(1.03f, 1.09f);

    public override void SetDefaults()
    {
        Item.width = 18;
        Item.height = 18;
        Item.value = Item.sellPrice(gold: 1);
        Item.rare = ItemRarityID.Orange;
        Item.defense = 6;
    }

    public override void UpdateEquip(Player player)
    {
        if (!player.TryGetModPlayer(out WgPlayer wg))
            return;
        float immobility = wg.Weight.ClampedImmobility;
        _damage.Lerp(immobility);
        _critChance.Lerp(immobility);

        player.buffImmune[BuffID.Bleeding] = true;
        player.GetDamage(DamageClass.Summon) += _damage;
        player.GetCritChance(DamageClass.Summon) *= _critChance;
        player.maxMinions += MinionCount;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.CrimtaneBar, 30)
            .AddIngredient(ItemID.Bone, 25)
            .AddIngredient(ItemID.TissueSample, 15)
            .AddTile(TileID.Anvils)
            .Register();
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        tooltips.FormatLines(_damage.Percent(), (_critChance - 1f).Percent());
    }
}
