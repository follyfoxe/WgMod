using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Content.Items.Accessories;
using static Terraria.ModLoader.ModContent;

namespace WgMod.Npcs
{
    public class GlobalNPCs : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (npc.type == NPCID.Deerclops)
            {
                npcLoot.Add(
                    ItemDropRule.ByCondition(
                        Condition.DownedDeerclops.ToDropCondition(ShowItemDropInUI.Always),
                        ItemType<WeightLossPendant>(),
                        chanceDenominator: 2,
                        chanceNumerator: 1
                    )
                );
            }
            if (npc.type == NPCID.QueenSlimeBoss)
            {
                npcLoot.Add(
                    ItemDropRule.ByCondition(
                        Condition.DownedQueenSlime.ToDropCondition(ShowItemDropInUI.Always),
                        ItemType<QueenlyGluttony>(),
                        chanceDenominator: 2,
                        chanceNumerator: 1
                    )
                );
            }
        }
    }
}
