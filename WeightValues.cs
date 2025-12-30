namespace WgMod;

public static class WeightValues
{
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
        7 => 1,
        _ => -1
    };

    // TODO
    public static float DrawOffsetX(int stage) => stage switch
    {
        6 => 1f * 2f,
        7 => 1f * 2f,
        _ => 0f
    };

    public static float DrawOffsetY(int stage) => stage switch
    {
        6 => 3f * 2f,
        7 => 8f * 2f,
        _ => 0f
    };
}
