using System;

namespace WgMod;

// Mass in Kg.
public readonly record struct Weight(float Mass)
{
    const float KgToPounds = 2.2046226218f;

    public const float Base = 70f;
    public const float Immobile = 400f;
    public const float Max = Immobile + 10f;

    public const int StageCount = 8;
    public const int ImmobileStage = StageCount - 1;
    public const int BuffStage = 3;

    public readonly float Immobility => (Mass - Base) / (Immobile - Base); // Inverese lerp
    public readonly float ClampedImmobility => Math.Clamp(Immobility, 0f, 1f);

    public override readonly string ToString() => $"{Mass} Kg";
    public readonly float ToPounds() => Mass * KgToPounds;
    public readonly int GetStage() => (int)Math.Floor(Immobility * ImmobileStage);

    public readonly float GetStageFactor()
    {
        int stage = GetStage();
        float a = FromStage(stage).Mass;
        float b = FromStage(stage + 1).Mass;
        return (Mass - a) / (b - a);
    }

    public static Weight FromStage(int stage) => FromImmobility(stage / (float)ImmobileStage);
    public static Weight FromImmobility(float factor) => new(float.Lerp(Base, Immobile, factor));

    public static Weight FromPounds(float pounds) => new(pounds / KgToPounds);
    public static Weight Clamp(Weight weight) => new(Math.Clamp(weight.Mass, Base, Max));

    public static Weight operator +(Weight w, float mass) => new(w.Mass + mass);
    public static Weight operator -(Weight w, float mass) => new(w.Mass - mass);
}
