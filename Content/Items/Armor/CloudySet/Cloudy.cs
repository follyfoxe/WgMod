using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using WgMod.Common.Configs;

namespace WgMod.Content.Items.Armor.CloudySet
{
    [AutoloadEquip(EquipType.Head)]
    public class CloudyHeadgear : ModItem
    {
        public override string Texture => "WgMod/Assets/Placeholder/ExampleHelmet";

        public override LocalizedText DisplayName => Language.GetText("Mods.WgMod.Items.Armor.CloudyHeadgear.DisplayName");
        public override LocalizedText Tooltip => Language.GetText("Mods.WgMod.Items.Armor.CloudyHeadgear.Tooltip");

        public int _cloudyMaxDef = 20;

        public override void SetStaticDefaults()
        {
            ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true;
            ArmorIDs.Head.Sets.DrawsBackHairWithoutHeadgear[Item.headSlot] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 18; // Width of the item
            Item.height = 18; // Height of the item
            Item.value = Item.sellPrice(silver: 35); // How many coins the item is worth
            Item.rare = ItemRarityID.Blue; // The rarity of the item
            Item.defense = 2; // The amount of defense the item will give when equipped
        }

        // IsArmorSet determines what armor pieces are needed for the setbonus to take effect
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<CloudyCrop>() && legs.type == ModContent.ItemType<CloudySkirt>();
        }

        // UpdateArmorSet allows you to give set bonuses to the armor.
        public override void UpdateArmorSet(Player player)
        {
            int def = player.WG().Weight.GetStatModifierFromWeight(-2, 1, 10f, 0.1f, 0, _cloudyMaxDef);
            if (ModContent.GetInstance<WgClientConfig>().DetailedTooltips)
                player.setBonus = Language.GetText("Mods.WgMod.Items.Armor.CloudyHeadgear.SetBonusAlt").Format(def, _cloudyMaxDef);
            else
                player.setBonus = Language.GetTextValue("Mods.WgMod.Items.Armor.CloudyHeadgear.SetBonus");
            player.statDefense += def;
        }

        public override void UpdateEquip(Player player)
        {
            //player.jumpSpeedBoost += 10f / 100f;

            player.General().JumpSpeedBoost += 1f;
            player.WG().MovementPenaltyReduction.Flat -= 20f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Cloud, 15)
                .AddIngredient(ItemID.FallenStar)
                .AddIngredient(ItemID.Feather, 4)
                .AddIngredient(ItemID.SunplateBlock, 5)
                .AddTile(TileID.SkyMill)
                .Register();
        }
    }

    [AutoloadEquip(EquipType.Body)]
    public class CloudyCrop : ModItem
    {
        public override string Texture => "WgMod/Assets/Placeholder/ExampleBreastplate";

        public override LocalizedText DisplayName => Language.GetText("Mods.WgMod.Items.Armor.CloudyCrop.DisplayName");
        public override LocalizedText Tooltip => Language.GetText("Mods.WgMod.Items.Armor.CloudyCrop.Tooltip");

        public override void SetDefaults()
        {
            Item.width = 18; // Width of the item
            Item.height = 18; // Height of the item
            Item.value = Item.sellPrice(silver: 25); // How many coins the item is worth
            Item.rare = ItemRarityID.Blue; // The rarity of the item
            Item.defense = 3; // The amount of defense the item will give when equipped
        }
        public override void UpdateEquip(Player player)
        {
            player.WG().MovementPenaltyReduction.Flat -= 20f;
            player.WG().AttackSpeedPenaltyReduction.Flat -= 20f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Cloud, 25)
                .AddIngredient(ItemID.FallenStar)
                .AddIngredient(ItemID.SunplateBlock, 10)
                .AddTile(TileID.SkyMill)
                .Register();
        }
    }

    [AutoloadEquip(EquipType.Legs)]
    public class CloudySkirt : ModItem
    {
        public override string Texture => "WgMod/Assets/Placeholder/ExampleLeggings";

        public override LocalizedText DisplayName => Language.GetText("Mods.WgMod.Items.Armor.CloudySkirt.DisplayName");
        public override LocalizedText Tooltip => Language.GetText("Mods.WgMod.Items.Armor.CloudySkirt.Tooltip");

        public override void SetDefaults()
        {
            Item.width = 18; // Width of the item
            Item.height = 18; // Height of the item
            Item.value = Item.sellPrice(silver: 15); // How many coins the item is worth
            Item.rare = ItemRarityID.Blue; // The rarity of the item
            Item.defense = 2; // The amount of defense the item will give when equipped
        }

        public override void UpdateEquip(Player player)
        {
            player.WG().MovementPenaltyReduction.Flat -= 10f;
            player.WG().MovementPenaltyReduction *= 1f - 0.05f;
            player.moveSpeed += 0.1f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Cloud, 25)
                .AddIngredient(ItemID.SunplateBlock, 25)
                .AddTile(TileID.SkyMill)
                .Register();
        }
    }
}
