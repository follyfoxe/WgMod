using System;

namespace WgMod;

public struct WgStat(float min, float max)
{
    public readonly float Min = min;
    public readonly float Max = max;
    public float Value = min;

    public void Reset()
    {
        Value = Min;
    }

    public void Clamp()
    {
        Value = Math.Clamp(Value, Min, Max);
    }

    public void Lerp(float t)
    {
        Value = float.Lerp(Min, Max, t);
    }

    public readonly WgStat Percent()
    {
        return this * 100f;
    }

    public override readonly string ToString()
    {
        int min = (int)MathF.Round(Min);
        int max = (int)MathF.Round(Max);
        int value = (int)MathF.Round(Value);
        return value.Range(min, max);
    }

    public static WgStat operator *(WgStat a, float b) => new(a.Min * b, a.Max * b) { Value = a.Value * b };
    public static WgStat operator /(WgStat a, float b) => new(a.Min / b, a.Max / b) { Value = a.Value / b };
    public static WgStat operator +(WgStat a, float b) => new(a.Min + b, a.Max + b) { Value = a.Value + b };
    public static WgStat operator -(WgStat a, float b) => new(a.Min - b, a.Max - b) { Value = a.Value - b };
    public static WgStat operator -(float a, WgStat b) => new(a - b.Min, a - b.Max) { Value = a - b.Value };

    public static implicit operator float(WgStat stat) => stat.Value;
    public static implicit operator int(WgStat stat) => (int)stat.Value;
}
