using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using WgMod.Content.NPCs.GroundedHarpy;
using WgMod.Content.NPCs.Milkmaid;

namespace WgMod.Common.Systems;

public class TownNPCRespawnSystem : ModSystem
{
    public static bool unlockGroundedHarpy = false;
    public static bool unlockMilkmaid = false;

    public override void ClearWorld()
    {
        unlockGroundedHarpy = false;
        unlockMilkmaid = false;
    }

    public override void SaveWorldData(TagCompound tag)
    {
        tag[nameof(unlockGroundedHarpy)] = unlockGroundedHarpy;
        tag[nameof(unlockMilkmaid)] = unlockMilkmaid;
    }

    public override void LoadWorldData(TagCompound tag)
    {
        unlockGroundedHarpy = tag.GetBool(nameof(unlockGroundedHarpy));
        unlockGroundedHarpy |= NPC.AnyNPCs(ModContent.NPCType<GroundedHarpyNPC>());
        unlockMilkmaid = tag.GetBool(nameof(unlockMilkmaid));
        unlockMilkmaid |= NPC.AnyNPCs(ModContent.NPCType<MilkmaidNPC>());
    }

    public override void NetSend(BinaryWriter writer)
    {
        writer.WriteFlags(unlockGroundedHarpy);
        writer.WriteFlags(unlockMilkmaid);
    }

    public override void NetReceive(BinaryReader reader)
    {
        reader.ReadFlags(out unlockGroundedHarpy);
        reader.ReadFlags(out unlockMilkmaid);
    }
}
