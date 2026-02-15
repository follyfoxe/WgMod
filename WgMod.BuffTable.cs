using Terraria.ID;

namespace WgMod;

partial class WgMod
{
    static GainOptions GainOverTime(float totalGain, float totalTime) => new(totalGain, totalTime);

    void RegisterBuffs()
    {
        // Vanilla
        AddBuffs([
            (BuffID.WellFed, 4f),
            (BuffID.WellFed2, 6f),
            (BuffID.WellFed3, 8f),
            (BuffID.ObsidianSkin, 3f),
            (BuffID.Regeneration, 6f),
            (BuffID.Swiftness, 1f),
            (BuffID.Gills, 2f),
            (BuffID.Ironskin, 4f),
            (BuffID.ManaRegeneration, 4f),
            (BuffID.MagicPower, 3f),
            (BuffID.Featherfall, 1f),
            (BuffID.Spelunker, 6f),
            (BuffID.Invisibility, 2f),
            (BuffID.Shine, 1f),
            (BuffID.NightOwl, 1f),
            (BuffID.Battle, 2f),
            (BuffID.Thorns, 1f),
            (BuffID.Archery, 2f),
            (BuffID.Hunter, 3f),
            (BuffID.Gravitation, 2f),
            (BuffID.PotionSickness, 18f),
            (BuffID.Tipsy, 12f),
            (BuffID.Ichor, 2f),
            (BuffID.ManaSickness, 12f),
            (BuffID.Mining, 1f),
            (BuffID.Heartreach, 3f),
            (BuffID.Calm, 6f),
            (BuffID.Builder, 1f),
            (BuffID.Titan, 8f),
            (BuffID.Flipper, 2f),
            (BuffID.Summoning, 3f),
            (BuffID.Dangersense, 1f),
            (BuffID.AmmoReservation, 2f),
            (BuffID.Lifeforce, 6f),
            (BuffID.Endurance, 4f),
            (BuffID.Rage, 2f),
            (BuffID.Inferno, 3f),
            (BuffID.Wrath, 4f),
            (BuffID.Fishing, 1f),
            (BuffID.Sonar, 1f),
            (BuffID.Crate, 1f),
            (BuffID.Slimed, 2f),
            (BuffID.SoulDrain, 1f),
            (BuffID.SugarRush, 12f),
            (BuffID.Lucky, 2f),
            (BuffID.HeartyMeal, 2f),
            (BuffID.Honey, GainOverTime(3f, 1f)), // Gain 3 kg every 1 second
        ]);

        // WgMod
        AddBuffs("WgMod", [("AmbrosiaGorged", 6f), ("WobWobWobWob", 8f)]);

        // Calamity Mod
        AddBuffs(
            "CalamityMod",
            [
                ("BloodyMaryBuff", 6f),
                ("CaribbeanRumBuff", 6f),
                ("CinnamonRollBuff", 8f),
                ("EverclearBuff", 6f),
                ("EvergreenGinBuff", 6f),
                ("FireballBuff", 6f),
                ("GrapeBeerBuff", 6f),
                ("MargaritaBuff", 6f),
                ("MoonshineBuff", 6f),
                ("MoscowMuleBuff", 6f),
                ("OldFashionedBuff", 6f),
                ("PurpleHazeBuff", 6f),
                ("RedWineBuff", 6f),
                ("RumBuff", 6f),
                ("ScrewdriverBuff", 6f),
                ("StarBeamRyeBuff", 6f),
                ("TequilaBuff", 6f),
                ("VodkaBuff", 6f),
                ("WhiskeyBuff", 6f),
                ("WhiteWineBuff", 2f),
                ("AnechoicCoatingBuff", 2f),
                ("BaguetteBuff", 6f),
                ("BloodfinBoost", 3f),
                ("BoundingBuff", 2f),
                ("CalciumBuff", 1f),
                ("CeaselessHunger", 12f),
                ("GravityNormalizerBuff", 6f),
                ("Omniscience", 6f),
                ("PhotosynthesisBuff", 4f),
                ("ShadowBuff", 1f),
                ("Soaring", 2f),
                ("SulphurskinBuff", 3f),
                ("TeslaBuff", 6f),
                ("Zen", 6f),
                ("Zerg", 6f),
                ("AbsorberRegen", 2f),
                ("GreenJellyRegen", GainOverTime(3f, 1f)),
                ("PinkJellyRegen", GainOverTime(3f, 1f)),
            ]
        );

        // Better Potions
        AddBuffs(
            "BetterPotions",
            [
                ("BerserkerBuff", 4f),
                ("DeterringBuff", 4f),
                ("DiscoInfernoBuff", 8f),
                ("FlightBuff", 4f),
                ("HeightenedSensesBuff", 6f),
                ("ImmovableBuff", 4f),
                ("InstigatingBuff", 4f),
                ("LeapingBuff", 2f),
                ("OrichalcumskinBuff", 6f),
                ("PiercingBuff", 4f),
                ("PredatorBuff", 2f),
                ("SteelfallBuff", 24f),
                ("WarBuff", 4f)
            ]
        );
    }
}
