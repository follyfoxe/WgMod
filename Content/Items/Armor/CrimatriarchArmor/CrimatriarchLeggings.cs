using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Items.Armor.CrimatriarchArmor
{
    [AutoloadEquip(EquipType.Legs)]
    public class CrimatriarchLeggings : ModItem
    {
        float _crimatriarchLeggingsDamage;
        float _crimatriarchLeggingsAttackSpeed;

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.sellPrice(gold: 1);
            Item.rare = ItemRarityID.Orange;
            Item.defense = 5;
        }

        public override void UpdateEquip(Player player)
        {
            if (!player.TryGetModPlayer(out WgPlayer wg))
                return;
            float immobility = wg.Weight.ClampedImmobility;

            _crimatriarchLeggingsDamage = float.Lerp(0.03f, 0.09f, immobility);
            _crimatriarchLeggingsAttackSpeed = float.Lerp(0.98f, 0.94f, immobility);

            player.GetDamage(DamageClass.Summon) += _crimatriarchLeggingsDamage;
            player.GetAttackSpeed(DamageClass.SummonMeleeSpeed) *= _crimatriarchLeggingsAttackSpeed;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.CrimtaneBar, 25)
                .AddIngredient(ItemID.Bone, 20)
                .AddIngredient(ItemID.TissueSample, 10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
