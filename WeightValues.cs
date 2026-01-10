using Microsoft.Xna.Framework;

namespace WgMod;

public static class WeightValues
{
    public static Vector2 GetHitboxSize(int stage) => stage switch
    {
        4 => new Vector2(20 + 16, 42 + 2),
        5 => new Vector2(20 + 32, 42 + 4),
        6 => new Vector2(20 + 48, 42 + 8),
        7 => new Vector2(20 + 72, 42 + 16),
        _ => new Vector2(20, 42)
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
        _ => 2f * 2f
    };

    public static float DrawOffsetY(int stage) => stage switch
    {
        4 => 2f,
        5 => 4f,
        6 => 8f,
        7 => 14f,
        _ => 0f
    };
    public static float BodyOffsetY(int stage) => stage switch
    {
        4 => 2f,
        5 => 3f,
        6 => 5f,
        7 => 10f,
        _ => 0f
    };
}
