using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Items.Accessories;

public class WeightLossPendant : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 26;
        Item.height = 32;

        Item.accessory = true;
        Item.rare = ItemRarityID.Orange;
        Item.value = Item.buyPrice(gold: 1);
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        if (!player.TryGetModPlayer(out WgPlayer wg))
            return;
        wg.WeightLossFactor += 5f;
    }
}
