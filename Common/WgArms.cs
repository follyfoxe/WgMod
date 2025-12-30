using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WgMod.Common;

public static class WgArms
{
    public const int ArmStageCount = 2;
    public static string GetArmName(int armStage) => "Arms" + armStage;
    public static int GetArmStage(int fatStage) => fatStage switch
    {
        3 => 0,
        4 => 0,
        5 => 1,
        6 => 1,
        7 => 1,
        _ => -1
    };

    public static void Load(Mod mod)
    {
        SkinEquipTexture skin = new();
        for (int i = 0; i < ArmStageCount; i++)
        {
            string name = GetArmName(i);
            EquipLoader.AddEquipTexture(mod, "WgMod/Assets/Textures/" + name, EquipType.Body, null, name, skin);
        }
    }

    public static void SetupDrawing(Mod mod)
    {
        for (int i = 0; i < ArmStageCount; i++)
            ArmorIDs.Body.Sets.HidesArms[GetArmEquipSlot(mod, i)] = true;
    }

    public static int GetArmEquipSlot(Mod mod, int armStage)
    {
        return EquipLoader.GetEquipSlot(mod, GetArmName(armStage), EquipType.Body);
    }
}

public class SkinEquipTexture : EquipTexture
{
    // TODO: Somehow fix everything else being tinted too (accessories and such), also fix the arm glowing in the dark for some reason
    public override void DrawArmorColor(Player drawPlayer, float shadow, ref Color color, ref int glowMask, ref Color glowMaskColor)
    {
        color = drawPlayer.GetImmuneAlphaPure(drawPlayer.skinColor, shadow);
    }
}
