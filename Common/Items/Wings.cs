using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WgMod.Common.Items
{
    public class AllWings : GlobalItem
    {
        public override void HorizontalWingSpeeds(Item item, Player player, ref float speed, ref float acceleration)
        {
            float mult = player.WG().SetMobilityMultiplier();
            acceleration *= Math.Clamp(mult * 1.25f, 0f, 1.0f);
        }

        public override void VerticalWingSpeeds(Item item, Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            float mult = player.WG().SetMobilityMultiplier();

            ascentWhenFalling *= Math.Clamp(mult * 0.5f + 0.5f, 0.001f, 1.0f);
            ascentWhenRising *= Math.Clamp(mult * 1.05f, 0.001f, 1.0f);
        }
    }

    public class NewWings : GlobalItem
    {
        public float GetWingWR(Item item)
        {
            switch(item.type)
            {
                case ItemID.LongRainbowTrailWings: return 0.42f;

                case ItemID.RainbowWings or ItemID.WingsSolar or ItemID.WingsVortex or ItemID.WingsNebula or ItemID.WingsStardust: return 0.4f;

                case ItemID.BetsyWings or ItemID.FishronWings: return 0.3f;

                case ItemID.Hoverboard or ItemID.FestiveWings or ItemID.SpookyWings or ItemID.TatteredFairyWings or ItemID.SteampunkWings: return 0.27f;

                case ItemID.BoneWings or ItemID.MothronWings or ItemID.GhostWings or ItemID.BeetleWings: return 0.24f;

                case ItemID.Jetpack or ItemID.LeafWings or ItemID.BatWings or ItemID.BeeWings or ItemID.ButterflyWings or ItemID.FlameWings: return 0.22f;

                case ItemID.RedsWings or ItemID.DTownsWings or ItemID.WillsWings or ItemID.CrownosWings or ItemID.CenxsWings or ItemID.BejeweledValkyrieWing or
                ItemID.Yoraiz0rWings or ItemID.JimsWings or ItemID.SkiphsWings or ItemID.LokisWings or ItemID.ArkhalisWings or ItemID.LeinforsWings or
                ItemID.GhostarsWings or ItemID.SafemanWings or ItemID.FoodBarbarianWings or ItemID.GroxTheGreatWings: return 0.2f;

                case ItemID.FairyWings or ItemID.FinWings or ItemID.FrozenWings or ItemID.HarpyWings: return 0.17f;

                default: return 0.15f;
            }
        }
        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.wingSlot >= 0;
        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            if (player.IsAirborne() && !player.mount.Active)
                player.WG().MovementPenaltyReduction *= 1f - GetWingWR(item);
        }
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            int percentage = (int)(GetWingWR(item) * 100);
            tooltips.AddExtraItemTooltip("Mods.WgMod.Items.Wings.Extra", percentage);
        }
    }
}
