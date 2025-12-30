using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ID;
using Terraria.ModLoader;

namespace WgMod.Common;

public static class WgArms
{
    public const int ArmStageCount = 2;
    public static readonly Asset<Texture2D>[] ArmTextures = new Asset<Texture2D>[ArmStageCount];

    public static string GetArmName(int armStage) => "Arms" + armStage;

    public static void Load(Mod mod)
    {
        for (int i = 0; i < ArmTextures.Length; i++)
        {
            string name = GetArmName(i);
            EquipLoader.AddEquipTexture(mod, "WgMod/Assets/Textures/" + name, EquipType.Body, null, name);
            ArmTextures[i] = mod.Assets.Request<Texture2D>("Assets/Textures/" + name);
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
