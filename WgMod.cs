using System;
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
        [BuffID.WellFed3] = 6f
    };

    // Duration in seconds
    public static readonly Dictionary<int, float> GainTable = new()
    {
        [BuffID.Honey] = 1.5f
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
