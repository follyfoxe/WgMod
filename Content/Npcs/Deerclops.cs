using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Content.Items.Accessories;

namespace WgMod.Npcs
{
    public class Deerclops : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (npc.type == NPCID.Deerclops && Main.expertMode == false)
            {
                npcLoot.Add(
                    ItemDropRule.ByCondition(
                        Condition.DownedDeerclops.ToDropCondition(ShowItemDropInUI.Always),
                        ModContent.ItemType<WeightLossPendant>(),
                        chanceDenominator: 2,
                        chanceNumerator: 1
                    )
                );
            }
        }
    }
}

namespace WgMod.Npcs
{
    public class BossBagLoot : GlobalItem
    {
        public override void ModifyItemLoot(Item item, ItemLoot itemloot)
        {
            if (item.type == ItemID.DeerclopsBossBag && Main.expertMode == true)
            {
                itemloot.Add(
                    ItemDropRule.ByCondition(
                        Condition.DownedDeerclops.ToDropCondition(ShowItemDropInUI.Always),
                        ModContent.ItemType<WeightLossPendant>(),
                        chanceDenominator: 2,
                        chanceNumerator: 1
                    )
                );
            }
        }
    }
}
