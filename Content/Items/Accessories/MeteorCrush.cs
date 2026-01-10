using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Items.Accessories;

public class MeteorCrush : ModItem
{
    public override string Texture => "WgMod/Assets/Placeholder/ExampleItem";
    public override void SetDefaults()
    {
        Item.width = 20;
        Item.height = 20;

        Item.accessory = true;
        Item.rare = ItemRarityID.Orange;
        Item.value = Item.buyPrice(gold: 1);
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.WG()._crushEffect = true;
    }
}
