using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace WgMod.Content.Items.Accessories.Informational
{
	public class WeightDisplayer_Weight_Unknown : InfoDisplay
    {
        public override string Texture => "WgMod/Content/Items/Accessories/Informational/PortableScale_InfoDisplay";
        public override bool Active() => Main.player[Main.myPlayer].WG()._displayWeight;

		public override string DisplayValue(ref Color displayColor, ref Color displayShadowColor)
		{
			Player player = Main.player[Main.myPlayer];
			return player.WG().Weight.Mass.DecimalCount(1).ToString() + " kg (" + player.WG().Weight.ToPounds().DecimalCount(1).ToString() + " lbs)";
		}
	}
}
