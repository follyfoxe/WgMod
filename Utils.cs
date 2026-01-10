using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using WgMod.Common.Items;
using WgMod.Common.Players;

namespace WgMod
{
    public static class Utils
    {
        public static WgPlayer WG(this Player player) => player.GetModPlayer<WgPlayer>();
        public static GeneralPlayer General(this Player player) => player.GetModPlayer<GeneralPlayer>();
        public static WgItem WG(this Item item)
        {
            if (item.IsAir)
                return null;

            bool success = item.TryGetGlobalItem(out WgItem wgItem);
            if (success)
                return wgItem;
            else
                return null;
        }
        public static NewWings NewWings(this Item item)
        {
            if (item.IsAir)
                return null;

            bool success = item.TryGetGlobalItem(out NewWings wings);
            if (success)
                return wings;
            else
                return null;
        }
        public static void AddBetterItemTooltip(this List<TooltipLine> tooltips, string tooltipKey, object tooltipParameter)
        {
            
            TooltipLine betterTooltip = new TooltipLine(
                WgMod.Instance,
                "BetterTooltip",
                Language.GetText(tooltipKey).Format(tooltipParameter)
            );

            if (tooltips.FirstOrDefault(x => x.Mod == "Terraria" && x.Name.Contains("Tooltip")) is TooltipLine tooltipLine)
            {
                foreach (TooltipLine oldTooltipLine in tooltips)
                {
                    if (oldTooltipLine.Mod == "Terraria" && oldTooltipLine.Name.Contains("Tooltip"))
                        oldTooltipLine.Hide();
                }
                tooltips.Insert(
                    tooltips.IndexOf(tooltipLine) + 1,
                    betterTooltip
                );
            }
        }
        public static void AddBetterItemTooltip(this List<TooltipLine> tooltips, string tooltipKey, List<object> tooltipParameters)
        {

            TooltipLine betterTooltip = new TooltipLine(
                WgMod.Instance,
                "BetterTooltip",
                Language.GetText(tooltipKey).Format(tooltipParameters.ToArray())
            );

            if (tooltips.FirstOrDefault(x => x.Mod == "Terraria" && x.Name.Contains("Tooltip")) is TooltipLine tooltipLine)
            {
                foreach (TooltipLine oldTooltipLine in tooltips)
                {
                    if (oldTooltipLine.Mod == "Terraria" && oldTooltipLine.Name.Contains("Tooltip"))
                        oldTooltipLine.Hide();
                }
                tooltips.Insert(
                    tooltips.IndexOf(tooltipLine) + 1,
                    betterTooltip
                );
            }
        }
        public static void AddExtraItemTooltip(this List<TooltipLine> tooltips, string tooltipKey, object tooltipParameters)
        {
            TooltipLine betterTooltip = new TooltipLine(
                WgMod.Instance,
                "BetterTooltip",
                Language.GetText(tooltipKey).Format(tooltipParameters)
            );

            if (tooltips.FirstOrDefault(x => x.Mod == "Terraria" && x.Name.Contains("Tooltip")) is TooltipLine tooltipLine)
            {
                tooltips.Insert(
                    tooltips.IndexOf(tooltipLine) + 1,
                    betterTooltip
                );
            }
        }

        public static double DecimalCount(this double target, int count) => Math.Round(target * Math.Pow(10.0, (double)count)) / Math.Pow(10.0, (double)count);
        public static float DecimalCount(this float target, int count) => (float)(Math.Round(target * Math.Pow(10.0, (double)count)) / Math.Pow(10.0, (double)count));
        public static float JumpSpeedBoost(float percentage) => 5.01f * (percentage / 100);
        public static bool IsAirborne(this Player player)
        {
            if (player.mount.Active)
                return !MountID.Sets.Cart[player.mount.Type];

            if (player.velocity.Y == 0f)
                return false;

            return true;
        }

        public static void NewSolarDashStart(this Player player, int dashDirection)
        {
            player.solarDashing = true;
            player.solarDashConsumedFlare = false;
        }
        public static void HealMana(this Player self, int amount, bool fromPickup)
        {
            self.statMana += amount;
            if (Main.myPlayer == self.whoAmI)
            {
                self.ManaEffect(amount);
            }
            if (self.statMana > self.statManaMax2)
            {
                self.statMana = self.statManaMax2;
                float newAmount = amount - (self.statMana - self.statManaMax2);
                if (fromPickup)
                    newAmount /= 3;
                self.WG().AddWeight(newAmount / 15f);
            }
        }
    }
}
