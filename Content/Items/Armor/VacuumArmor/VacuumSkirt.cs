using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Items.Armor.VacuumArmor
{
    [AutoloadEquip(EquipType.Legs)]
    public class VacuumSkirt : ModItem
    {
        private float _vacuumSkirtAttackSpeed;
        private int _vacuumSkirtHealth;
        private int _vacuumSkirtDefense;
        private float _vacuumSkirtResist;
        private float _vacuumSkirtMovePenalty;

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

            _vacuumSkirtAttackSpeed = float.Lerp(1.06f, 1.12f, immobility);
            _vacuumSkirtHealth = (int)MathF.Floor((int)float.Lerp(50, 100, immobility) / 5f) * 5;
            _vacuumSkirtDefense = (int)float.Lerp(8f, 16f, immobility);
            _vacuumSkirtResist = float.Lerp(0.03f, 0.06f, immobility);
            _vacuumSkirtMovePenalty = float.Lerp(1.15f, 0.9f, immobility);

            player.GetAttackSpeed(DamageClass.Generic) *= _vacuumSkirtAttackSpeed;
            player.statLifeMax2 += _vacuumSkirtHealth;
            player.statDefense += _vacuumSkirtDefense;
            player.endurance += _vacuumSkirtResist;
            wg.MovementPenalty *= _vacuumSkirtMovePenalty;

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
    }
}
