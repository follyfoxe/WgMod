using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using WgMod.Content.NPCs;

namespace WgMod.Common.Systems;

public class TownNPCRespawnSystem : ModSystem
{
    public static bool unlockGroundedHarpy = false;

    public override void ClearWorld()
    {
        unlockGroundedHarpy = false;
    }

    public override void SaveWorldData(TagCompound tag)
    {
        tag[nameof(unlockGroundedHarpy)] = unlockGroundedHarpy;
    }

    public override void LoadWorldData(TagCompound tag)
    {
        unlockGroundedHarpy = tag.GetBool(nameof(unlockGroundedHarpy));

        unlockGroundedHarpy |= NPC.AnyNPCs(ModContent.NPCType<GroundedHarpy>());
    }

    public override void NetSend(BinaryWriter writer)
    {
        writer.WriteFlags(unlockGroundedHarpy);
    }

    public override void NetReceive(BinaryReader reader)
    {
        reader.ReadFlags(out unlockGroundedHarpy);
    }
}
