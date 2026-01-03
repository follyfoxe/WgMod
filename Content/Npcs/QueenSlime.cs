using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Content.Items.Accessories;

namespace WgMod.Npcs
{
    public class QueenSlime : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (npc.type == NPCID.QueenSlimeBoss)
            {
                npcLoot.Add(
                    ItemDropRule.ByCondition(
                        Condition.DownedQueenSlime.ToDropCondition(ShowItemDropInUI.Always),
                        ModContent.ItemType<QueenlyGluttony>(),
                        chanceDenominator: 2,
                        chanceNumerator: 1
                    )
                );
            }
        }
    }
}
