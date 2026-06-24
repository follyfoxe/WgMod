using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Content.Items.Accessories.Fat;
using WgMod.Content.Items.Accessories.Ranged;
using WgMod.Content.Items.Ammo;
using WgMod.Content.Items.Consumables;
using WgMod.Content.Items.Consumables.Baked;
using WgMod.Content.Items.Consumables.Potions.WeightGainPotions;
using WgMod.Content.Items.Consumables.Potions.WeightLossPotions;

namespace WgMod.Common.Systems;

public class ChestLootSystem : ModSystem
{
    record struct LootEntry(int Type, int Chance, int MinAmount = 1, int MaxAmount = 1);

    public override void PostWorldGen()
    {
        int[] lesserWeightPotions = [ModContent.ItemType<LesserWeightGainPotion>(), ModContent.ItemType<LesserWeightLossPotion>()];
        int[] weightPotions = [ModContent.ItemType<WeightGainPotion>(), ModContent.ItemType<WeightLossPotion>()];

        for (int chestIndex = 0; chestIndex < Main.maxChests; chestIndex++)
        {
            Chest chest = Main.chest[chestIndex];
            if (chest == null)
                continue;
            Tile tile = Main.tile[chest.x, chest.y];

            int weightPotionAmount = Main.rand.Next(3, 6);
            int arrowAmount = Main.rand.Next(25, 51);
            int buffPotionAmount = Main.rand.Next(1, 3);

            // Ice chest
            if (tile.TileFrameX == 11 * 36)
            {
                for (int inventoryIndex = 0; inventoryIndex < Chest.maxItems; inventoryIndex++)
                {
                    if (chest.item[inventoryIndex].type == ItemID.None)
                    {
                        if (Main.rand.NextBool(3))
                        {
                            chest.item[inventoryIndex].SetDefaults(ModContent.ItemType<AmuletOfStarving>());
                            chest.item[inventoryIndex].stack = 1;
                        }
                        break;
                    }
                }
            }

            // Skyware chest
            if (tile.TileFrameX == 13 * 36)
            {
                for (int inventoryIndex = 0; inventoryIndex < 40; inventoryIndex++)
                {
                    if (chest.item[inventoryIndex].type == ItemID.None)
                    {
                        if (Main.rand.NextBool(3))
                        {
                            chest.item[inventoryIndex].SetDefaults(ModContent.ItemType<MeteorCrush>());
                            chest.item[inventoryIndex].stack = 1;
                        }
                        else if (Main.rand.NextBool(3))
                        {
                            chest.item[inventoryIndex].SetDefaults(ModContent.ItemType<HeliumTank>());
                            chest.item[inventoryIndex].stack = 1;
                        }
                        break;
                    }
                }
            }

            // Mushroom chest
            if (tile.TileFrameX == 32 * 36)
            {
                for (int inventoryIndex = 0; inventoryIndex < 40; inventoryIndex++)
                {
                    if (chest.item[inventoryIndex].type == ItemID.None)
                    {
                        if (Main.rand.NextBool(3))
                        {
                            chest.item[inventoryIndex].SetDefaults(ModContent.ItemType<StuffedTruffle>());
                            chest.item[inventoryIndex].stack = 1;
                        }
                        break;
                    }
                }
            }

            // Living wood chest
            if (tile.TileFrameX == 12 * 36)
            {
                for (int inventoryIndex = 0; inventoryIndex < 40; inventoryIndex++)
                {
                    if (chest.item[inventoryIndex].type == ItemID.None)
                    {
                        if (Main.rand.NextBool(2))
                        {
                            chest.item[inventoryIndex].SetDefaults(ModContent.ItemType<AcornCake>());
                            chest.item[inventoryIndex].stack = buffPotionAmount;
                        }
                        break;
                    }
                }
            }

            // Not lihzahrd chest
            if (tile.TileFrameX != 16 * 36)
            {
                for (int inventoryIndex = 0; inventoryIndex < 40; inventoryIndex++)
                {
                    if (chest.item[inventoryIndex].type == ItemID.None)
                    {
                        if (Main.rand.NextBool(3))
                        {
                            chest.item[inventoryIndex].SetDefaults(ModContent.ItemType<WeightlessPotion>());
                            chest.item[inventoryIndex].stack = buffPotionAmount;
                        }
                        else if (Main.rand.NextBool(2))
                        {
                            chest.item[inventoryIndex].SetDefaults(lesserWeightPotions[Main.rand.Next(0, 2)]);
                            chest.item[inventoryIndex].stack = weightPotionAmount;
                        }
                        break;
                    }
                }
            }

            // Lihzahrd chest
            if (tile.TileFrameX == 16 * 36)
            {
                for (int inventoryIndex = 0; inventoryIndex < 40; inventoryIndex++)
                {
                    if (chest.item[inventoryIndex].type == ItemID.None)
                    {
                        if (Main.rand.NextBool(3))
                        {
                            chest.item[inventoryIndex].SetDefaults(ModContent.ItemType<WeightlessPotion>());
                            chest.item[inventoryIndex].stack = buffPotionAmount;
                        }
                        else if (Main.rand.NextBool(2))
                        {
                            chest.item[inventoryIndex].SetDefaults(weightPotions[Main.rand.Next(0, 2)]);
                            chest.item[inventoryIndex].stack = weightPotionAmount;
                        }
                        break;
                    }
                }
            }

            // Gold chest
            if (tile.TileFrameX == 1 * 36)
            {
                for (int inventoryIndex = 0; inventoryIndex < 40; inventoryIndex++)
                {
                    if (chest.item[inventoryIndex].type == ItemID.None)
                    {
                        if (Main.rand.NextBool(3))
                        {
                            chest.item[inventoryIndex].SetDefaults(ModContent.ItemType<CaramelArrow>());
                            chest.item[inventoryIndex].stack = arrowAmount;
                        }
                        break;
                    }
                }
            }
        }
    }

    static void FillChest(Chest chest, LootEntry[] loot)
    {
        int lootIndex = 0;
        for (int i = 0; i < Chest.maxItems && lootIndex < loot.Length; i++)
        {
            Item item = chest.item[i];
            if (item.type != ItemID.None)
                continue;
            for (; lootIndex < loot.Length; lootIndex++)
            {
                LootEntry entry = loot[lootIndex];
                if (!Main.rand.NextBool(entry.Chance))
                    continue;
                int amount = Main.rand.Next(entry.MinAmount, entry.MaxAmount + 1);
                if (amount <= 0)
                    continue;
                item.SetDefaults(entry.Type);
                item.stack = amount;
                break;
            }
        }
    }
}
