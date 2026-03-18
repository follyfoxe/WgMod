using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using WgMod.Common.Players;
using System;
using WgMod.Content.NPCs;

namespace WgMod.Content.Items.Consumables;

public class HornOfMore : ModItem
{
    WgStat _healBonus = new(100, 150);

    public override void SetDefaults()
    {
        Item.width = 32;
        Item.height = 28;
        Item.useStyle = ItemUseStyleID.DrinkLiquid;
        Item.useAnimation = 15;
        Item.useTime = 15;
        Item.useTurn = true;
        Item.UseSound = SoundID.Item3;
        Item.maxStack = 1;
        Item.consumable = true;
        Item.rare = ItemRarityID.Orange;
        Item.value = Item.buyPrice(gold: 2);

        Item.healLife = 100;
        Item.potion = true;
    }

    public override void UpdateInventory(Player player)
    {
        if (!player.TryGetModPlayer(out WgPlayer wg))
            return;
        float immobility = wg.Weight.ClampedImmobility;

        _healBonus.Lerp(immobility);
        _healBonus.Value = MathF.Floor(_healBonus.Value / 5f) * 5f;

        Item.healLife = _healBonus;
    }

    public override bool ConsumeItem(Player player)
    {
        return false;
    }
}

public class HornOfMoreNPC : GlobalNPC
{
    public override void ModifyShop(NPCShop shop)
    {
        if (shop.NpcType == ModContent.NPCType<GroundedHarpy>() && NPC.downedBoss2)
            shop.Add<HornOfMore>();
    }
}