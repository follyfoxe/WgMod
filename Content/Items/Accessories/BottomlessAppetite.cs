using System.Configuration;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Items.Accessories;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.maimaichubs)]
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
        if (!player.TryGetModPlayer(out BottomlessAppetitePlayer ba))
            return;
        float immobility = wg.Weight.ClampedImmobility;
        ba._active = true;
        ba._range = (int)float.Lerp(2f, 9999f, immobility);
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.FragmentSolar, 6)
            .AddIngredient(ItemID.FragmentNebula, 6)
            .AddIngredient(ItemID.FragmentVortex, 6)
            .AddIngredient(ItemID.FragmentStardust, 6)
            .AddTile(TileID.LunarCraftingStation)
            .Register();
    }
}

public class BottomlessAppetitePlayer : ModPlayer
{
    internal bool _active;
    internal int _range;

    public override void ResetEffects()
    {
        _active = false;
    }
}

public class BottomlessAppetiteItem : GlobalItem
{
    public override void GrabRange(Item item, Player player, ref int grabRange)
    {
        if (!player.TryGetModPlayer(out BottomlessAppetitePlayer ba))
            return;
        if (ba._active && item.type != ItemID.FallenStar)
            grabRange *= ba._range;
    }
}
