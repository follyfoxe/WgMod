using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Content.Items; // replace this with your own namespace
using WgMod.Content.Items.Accessories;

namespace WgMod.Npcs // replace this with your own namespace
{
    public class Deerclops : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (npc.type == NPCID.Deerclops)
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
