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
            .AddIngredient(ItemID.FragmentSolar, 2)
            .AddIngredient(ItemID.FragmentNebula, 2)
            .AddIngredient(ItemID.FragmentVortex, 2)
            .AddIngredient(ItemID.FragmentStardust, 2)
            .AddTile(TileID.LunarCraftingStation)
            .Register();
    }
}

public class BottomlessAppetitePlayer : ModPlayer
{
    internal bool _active; // BottomlessAppetite effect
    internal int _range; // How much BottomlessAppetite increases grab range

    public override void Load()
    {
        On_Player.GetItemGrabRange += OnGetItemGrabRange;
    }

    public override void Unload()
    {
        On_Player.GetItemGrabRange -= OnGetItemGrabRange;
    }

    public override void ResetEffects()
    {
        _active = false;
    }

    // folly: I think you can do this in a simpler way with a GlobalItem
    // This is needed for BottomlessAppetite item pickup range changes!
    static int OnGetItemGrabRange(On_Player.orig_GetItemGrabRange orig, Player self, Item item)
    {
        int num = Player.defaultItemGrabRange;
        if (self.goldRing && item.IsACoin)
            num += Item.coinGrabRange;
        if (self.manaMagnet && (item.type == ItemID.Star || item.type == ItemID.SoulCake || item.type == ItemID.SugarPlum))
            num += Item.manaGrabRange;
        if (item.type == ItemID.ManaCloakStar)
            num += Item.manaGrabRange;
        if (self.lifeMagnet && (item.type == ItemID.Heart || item.type == ItemID.CandyApple || item.type == ItemID.CandyCane))
            num += Item.lifeGrabRange;
        if (self.treasureMagnet)
            num += Item.treasureGrabRange;
        if (item.type == ItemID.DD2EnergyCrystal)
            num += 50;
        if (ItemID.Sets.NebulaPickup[item.type])
            num += 100;
        if (self.difficulty == PlayerDifficultyID.Creative && CreativePowerManager.Instance.GetPower<CreativePowers.FarPlacementRangePower>().IsEnabledForPlayer(self.whoAmI))
            num += 240;
        if (self.TryGetModPlayer(out BottomlessAppetitePlayer ba))
        {
            if (ba._active && item.type != ItemID.FallenStar)
                num *= ba._range;
        }
        ItemLoader.GrabRange(item, self, ref num);
        return num;
    }
}
