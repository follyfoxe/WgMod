using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Items.Armor.CrimatriarchArmor
{
    [AutoloadEquip(EquipType.Body)]
    public class CrimatriarchGown : ModItem
    {
        float _crimatriarchGownDamage;
        float _crimatriarchGownCritChance;
        int _crimatriarchGownMinions = 1;

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

            player.buffImmune[BuffID.Bleeding] = true;

            _crimatriarchGownDamage = float.Lerp(0.05f, 0.1f, immobility);
            _crimatriarchGownCritChance = float.Lerp(1.03f, 1.09f, immobility);

            player.GetDamage(DamageClass.Summon) += _crimatriarchGownDamage;
            player.GetCritChance(DamageClass.Summon) *= _crimatriarchGownCritChance;
            player.maxMinions += _crimatriarchGownMinions;
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
    }
}
