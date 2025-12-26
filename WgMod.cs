using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Players;
using WgMod.Content.Buffs;

namespace WgMod;

// Please read https://github.com/tModLoader/tModLoader/wiki/Basic-tModLoader-Modding-Guide#mod-skeleton-contents for more information about the various files in a mod.
public partial class WgMod : Mod
{
    // TODO: Use calories instead
    public static readonly Dictionary<int, float> BuffTable = new()
    {
        [BuffID.WellFed] = 4f,
        [BuffID.WellFed2] = 5f,
        [BuffID.WellFed3] = 6f,
        [BuffID.ObsidianSkin] = 3f,
        [BuffID.Regeneration] = 6f,
        [BuffID.Swiftness] = 1f,
        [BuffID.Gills] = 2f,
        [BuffID.Ironskin] = 4f,
        [BuffID.ManaRegeneration] = 4f,
        [BuffID.MagicPower] = 3f,
        [BuffID.Featherfall] = 1f,
        [BuffID.Spelunker] = 6f,
        [BuffID.Invisibility] = 2f,
        [BuffID.Shine] = 1f,
        [BuffID.NightOwl] = 1f,
        [BuffID.Battle] = 2f,
        [BuffID.Thorns] = 1f,
        [BuffID.Archery] = 2f,
        [BuffID.Hunter] = 3f,
        [BuffID.Gravitation] = 2f,
        [BuffID.PotionSickness] = 24f,
        [BuffID.Tipsy] = 12f,
        [BuffID.Ichor] = 2f,
        [BuffID.ManaSickness] = 12f,
        [BuffID.Mining] = 1f,
        [BuffID.Heartreach] = 3f,
        [BuffID.Calm] = 6f,
        [BuffID.Builder] = 1f,
        [BuffID.Titan] = 8f,
        [BuffID.Flipper] = 2f,
        [BuffID.Summoning] = 3f,
        [BuffID.Dangersense] = 1f,
        [BuffID.AmmoReservation] = 2f,
        [BuffID.Lifeforce] = 6f,
        [BuffID.Endurance] = 4f,
        [BuffID.Rage] = 2f,
        [BuffID.Inferno] = 3f,
        [BuffID.Wrath] = 4f,
        [BuffID.Fishing] = 1f,
        [BuffID.Sonar] = 1f,
        [BuffID.Crate] = 1f,
        [BuffID.Slimed] = 2f,
        [BuffID.SoulDrain] = 1f,
        [BuffID.SugarRush] = 12f,
        [BuffID.Lucky] = 2f,
        [BuffID.HeartyMeal] = 2f
    };

    // Duration in seconds
    public static readonly Dictionary<int, float> GainTable = new()
    {
        [BuffID.Honey] = GainingBuff.DurationFromGainPerTick(0.05f)
    };

    public override void Load()
    {
        On_Player.AddBuff += OnPlayerAddBuff;
    }

    public override void Unload()
    {
        On_Player.AddBuff -= OnPlayerAddBuff;
    }

    public static void OnPlayerAddBuff(On_Player.orig_AddBuff orig, Player self, int type, int timeToAdd, bool quiet, bool foodHack)
    {
        if (self.TryGetModPlayer(out WgPlayer wg))
        {
            if (BuffTable.TryGetValue(type, out float mass))
                wg.SetWeight(wg.Weight + mass);
            else if (GainTable.TryGetValue(type, out float time))
                GainingBuff.AddBuff(self, time);
        }
        orig(self, type, timeToAdd, quiet, foodHack);
    }
}
