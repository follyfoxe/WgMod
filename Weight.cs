using System;
using Terraria;
using Terraria.ModLoader;
using WgMod.Common.Configs;

namespace WgMod;

// Mass in Kg.
public class Weight
{

    

    //FYI these values are for the player

    const float KgToPounds = 2.2046226218f;

    public float Mass = 0f;

    public float Base = 70f;
    public float Immobile = 350f;
    public float Max = 600f;

    public int StageCount = 7;

    public float WeightPerStage = 30f;
    public float ExtraWeightPerStage = 8f;

    public override string ToString() => $"{Mass} Kg";
    public float ToPounds() => Mass * KgToPounds;
    public int GetStage()
    {
        int currentStage = 0;
        float currentMass = Mass;
        float requiredMass = Base + WeightPerStage;
        while (currentMass >= requiredMass && currentStage < StageCount)
        {
            requiredMass += WeightPerStage;
            requiredMass += ExtraWeightPerStage * currentStage;
            currentStage++;
        }
        return currentStage;
    }
    public float GetStageFactor()
    {
        int currentStage = 0;
        float currentMass = Mass;
        float requiredMass = Base + WeightPerStage;
        while (currentMass >= requiredMass && currentStage < StageCount)
        {
            requiredMass += WeightPerStage;
            requiredMass += ExtraWeightPerStage * currentStage;
            currentStage++;
        }
        float lastStageMass = requiredMass - WeightPerStage - ExtraWeightPerStage * (currentStage - 1);
        if (currentStage == 0)
            lastStageMass = Base;
        float factor = Math.Clamp((Mass - lastStageMass) / (requiredMass - lastStageMass), 0f, 1f);
        return factor;
    }

    /// <summary>
    /// Everytime the entity's weight exceeds <paramref name="everyKG"/>, <paramref name="addedValue"/> is added to <paramref name="defaultValue"/>, then <paramref name="extraKG"/> is added to <paramref name="everyKG"/>. <br/>
    /// <paramref name="min"/> and <paramref name="max"/> will clamp the returned value if they aren't left as null.
	/// </summary>
	/// <param name="defaultValue">The value to be modified.</param>
	/// <param name="addedValue">The value that is added to <paramref name="defaultValue"/>.</param>
	/// <param name="addedValue">The value that is added.</param>
    /// 
    public float GetStatModifierFromWeight(float defaultValue, float addedValue, float everyKG, float extraKG = 0, float? min = null, float? max = null)
    {
        float plrWeight = Mass - Base;
        if (ModContent.GetInstance<WgServerConfig>().DisableFatBuffs)
            plrWeight = 0f;
        float newValue = defaultValue;
        while (plrWeight >= everyKG)
        {
            plrWeight -= everyKG;
            everyKG += extraKG;
            newValue += addedValue;
        }
        if (min == null && max == null)
            return newValue;
        else if (min == null && max != null)
            return Math.Min(newValue, (float)max);
        else if (min != null && max == null)
            return Math.Max(newValue, (float)min);
        return Math.Clamp(newValue, (float)min, (float)max);
    }
    /// <summary>
    /// Everytime the entity's weight exceeds <paramref name="everyKG"/>, <paramref name="addedValue"/> is added to <paramref name="defaultValue"/>, then <paramref name="extraKG"/> is added to <paramref name="everyKG"/>. <br/>
    /// <paramref name="min"/> and <paramref name="max"/> will clamp the returned value if they aren't left as null.
	/// </summary>
	/// <param name="defaultValue">The value to be modified.</param>
	/// <param name="addedValue">The value that is added to <paramref name="defaultValue"/>.</param>
	/// <param name="addedValue">The value that is added.</param>
    /// 
    public int GetStatModifierFromWeight(int defaultValue, int addedValue, float everyKG, float extraKG = 0, int? min = null, int? max = null)
    {
        float plrWeight = Mass - Base;
        int newValue = defaultValue;
        while (plrWeight >= everyKG)
        {
            plrWeight -= everyKG;
            everyKG += extraKG;
            newValue += addedValue;
        }
        if (min == null && max == null)
            return newValue;
        else if (min == null && max != null)
            return Math.Min(newValue, (int)max);
        else if (min != null && max == null)
            return Math.Max(newValue, (int)min);
        return Math.Clamp(newValue, (int)min, (int)max);
    }

    public float Clamp(float weight) => Math.Clamp(weight, Base, Max);

    public void SetWeight(float weight)
    {
        Mass = Clamp(weight);
    }

    //old, although things still use this
    public float ClampedImmobility => GetClampedFactor(Base, Immobile);
    public float GetClampedFactor(float start, float end) => Math.Clamp(GetFactor(start, end), 0f, 1f);
    public float GetFactor(float start, float end) => (Mass - start) / (end - start);
}