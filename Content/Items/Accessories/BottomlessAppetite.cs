using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Items.Accessories;

public class BottomlessAppetite : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 22;
        Item.height = 22;

        Item.accessory = true;
        Item.rare = ItemRarityID.Red;
        Item.value = Item.buyPrice(gold: 1);
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        if (!player.TryGetModPlayer(out WgPlayer wg))
            return;

        float immobility = wg.Weight.ClampedImmobility;
        wg._bottomlessAppetite = true;
        wg._bottomlessAppetiteGrabRange = (int)float.Lerp(2f, 9999f, immobility);
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.FragmentSolar, 2)
            .AddIngredient(ItemID.FragmentNebula, 2)
            .AddIngredient(ItemID.FragmentVortex, 2)
            .AddIngredient(ItemID.FragmentStardust, 2)
            .AddTile(TileID.LunarCraftingStation)
            .Register();
    }
}
