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
    public class WgItem : GlobalItem
    {
        public override bool AppliesToEntity(Item item, bool lateInstantiation) => ItemLists.ConsumableItems.ContainsKey(item.type);
        public override bool InstancePerEntity => true;

        public float weightGainOnUse;
        public override void SetDefaults(Item item)
        {
            item.WG().weightGainOnUse = ItemLists.ConsumableItems[item.type];
        }
        public override bool? UseItem(Item item, Player player)
        {
            return true;
        }
        public override void OnConsumeItem(Item item, Player player)
        {
            if (item.type == ItemID.StrangeBrew)
            {
                player.WG().AddWeight(Main.rand.Next(5, 500) / 10f);
                return;
            }
            player.WG().AddWeight(item.WG().weightGainOnUse);
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
                tooltips.AddBetterItemTooltip("Mods.WgMod.Items.Vanilla.StrangeBrew.Tooltip", new {});
        }
    }
}
