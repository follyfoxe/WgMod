using Terraria.ID;

namespace WgMod;

public static class WeightValues
{
    public static float GetDeathPenalty(int difficulty) => difficulty switch
    {
        PlayerDifficultyID.SoftCore => 0.8f,
        PlayerDifficultyID.MediumCore => 0.85f,
        PlayerDifficultyID.Hardcore => 0.9f,
        _ => 1f
    };

    public static int GetHitboxWidthInTiles(int stage) => stage switch
    {
        4 => 3,
        5 => 4,
        6 => 5,
        7 => 6,
        _ => 2,
    };

    public static int GetArmStage(int stage) => stage switch
    {
        3 => 0,
        4 => 0,
        5 => 1,
        6 => 1,
        7 => 2,
        _ => -1
    };

    // TODO
    public static float DrawOffsetX(int stage) => stage switch
    {
        7 => 3f * 2f,
        _ => 2f * 2f
    };

    public static float DrawOffsetY(int stage) => stage switch
    {
        6 => 2f * 2f,
        7 => 8f * 2f,
        _ => 0f
    };
}
