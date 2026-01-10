using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Items.Accessories;

public class ChampionsBelt : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 26;
        Item.height = 32;

        Item.accessory = true;
        Item.rare = ItemRarityID.Red;
        Item.value = Item.buyPrice(gold: 1);
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        if (!player.TryGetModPlayer(out WgPlayer wg))
            return;
        float immobility = wg.Weight.ClampedImmobility;

        wg._championsBelt = true;
        wg._championsBeltMeleeScale = (int)float.Lerp(1.25f, 2, immobility);
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.GoldBar, 6)
            .AddIngredient(ItemID.Ruby, 2)
            .AddIngredient(ItemID.Emerald, 2)
            .AddIngredient(ItemID.Amethyst, 2)
            .AddTile(TileID.Anvils)
            .Register();
    }
}
