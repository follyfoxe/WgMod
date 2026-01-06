using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Items.Armor.VacuumArmor
{
    [AutoloadEquip(EquipType.Head)]
    public class VacuumHelmet : ModItem
    {
        private float _vacuumHelmetCritChance;
        private int _vacuumHelmetHealth;
        private int _vacuumHelmetDefense;
        private float _vacuumHelmetResist;
        private float _vacuumHelmetMovePenalty;

        private int _vacuumSetBonusRegen;

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.sellPrice(gold: 1);
            Item.rare = ItemRarityID.Red;
            Item.defense = 36;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<VacuumCrop>()
                && legs.type == ModContent.ItemType<VacuumSkirt>();
        }

        public override void UpdateArmorSet(Player player)
        {
            if (!player.TryGetModPlayer(out WgPlayer wg))
                return;
            wg._vacuumSetBonus = true;
        }

        public override void UpdateEquip(Player player)
        {
            if (!player.TryGetModPlayer(out WgPlayer wg))
                return;

            float immobility = wg.Weight.ClampedImmobility;

            _vacuumHelmetCritChance = float.Lerp(1.04f, 1.08f, immobility);
            _vacuumHelmetHealth = (int)MathF.Floor((int)float.Lerp(50, 100, immobility) / 5f) * 5;
            _vacuumHelmetDefense = (int)float.Lerp(6f, 12f, immobility);
            _vacuumHelmetResist = float.Lerp(0.02f, 0.04f, immobility);
            _vacuumHelmetMovePenalty = float.Lerp(1.1f, 0.95f, immobility);

            player.GetCritChance(DamageClass.Generic) *= _vacuumHelmetCritChance;
            player.statLifeMax2 += _vacuumHelmetHealth;
            player.statDefense += _vacuumHelmetDefense;
            player.endurance += _vacuumHelmetResist;
            wg.MovementPenalty *= _vacuumHelmetMovePenalty;

            player.aggro += 5;

            if (!wg._vacuumSetBonus)
            {return;}
            else
            {
                _vacuumSetBonusRegen = (int)float.Lerp(5f, 20f, immobility);

                player.lifeRegen += _vacuumSetBonusRegen;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.FragmentSolar, 2)
                .AddIngredient(ItemID.FragmentNebula, 2)
                .AddIngredient(ItemID.FragmentVortex, 2)
                .AddIngredient(ItemID.FragmentStardust, 2)
                .AddIngredient(ItemID.LunarBar, 8)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
};
