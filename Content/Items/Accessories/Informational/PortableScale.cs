using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WgMod.Content.Items.Accessories.Informational
{
	public class PortableScale : ModItem
    {
        public override string Texture => "WgMod/Assets/Placeholder/ExampleItem";

        public override void SetDefaults()
		{
			Item.accessory = true;

			Item.width = 30;
			Item.height = 30;
			Item.rare = ItemRarityID.Orange;
			Item.value = Item.buyPrice(
				gold: 5
			);
		}

		public override void UpdateInventory(Player player)
		{
			player.WG()._displayWeight = true;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
            player.WG()._displayWeight = true;
        }
	}
}
