using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Items.Armor.VacuumSet
{
    [AutoloadEquip(EquipType.Body)]
    public class VacuumCrop : ModItem
    {
        private float _vacuumCropAttack;
        private int _vacuumCropHealth;
        private int _vacuumCropDefense;
        private float _vacuumCropResist;
        private float _vacuumCropMovePenalty;

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.sellPrice(gold: 1);
            Item.rare = ItemRarityID.Red;
            Item.defense = 46;
        }

        public override void UpdateEquip(Player player)
        {
            if (!player.TryGetModPlayer(out WgPlayer wg))
                return;
            float immobility = wg.Weight.ClampedImmobility;

            _vacuumCropAttack = float.Lerp(0.06f, 0.12f, immobility);
            _vacuumCropHealth = (int)MathF.Floor((int)float.Lerp(100f, 200f, immobility) / 5f) * 5;
            _vacuumCropDefense = (int)float.Lerp(12f, 24f, immobility);
            _vacuumCropResist = float.Lerp(0.03f, 0.06f, immobility);
            _vacuumCropMovePenalty = float.Lerp(1.15f, 0.9f, immobility);

            player.GetDamage(DamageClass.Generic) += _vacuumCropAttack;
            player.statLifeMax2 += _vacuumCropHealth;
            player.statDefense += _vacuumCropDefense;
            player.endurance += _vacuumCropResist;
            wg.MovementPenaltyReduction *= _vacuumCropMovePenalty;

            player.aggro += 5;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.FragmentSolar, 5)
                .AddIngredient(ItemID.FragmentNebula, 5)
                .AddIngredient(ItemID.FragmentVortex, 5)
                .AddIngredient(ItemID.FragmentStardust, 5)
                .AddIngredient(ItemID.LunarBar, 16)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
