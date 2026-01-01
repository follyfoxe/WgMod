using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using WgMod.Common.Players;
using WgMod.Content.Buffs;

namespace WgMod;

// TODO: Use calories instead
public record struct GainOptions(float TotalGain, float Time = 0f)
{
    public readonly bool IsInstant => Time < 0.01f;
    public static implicit operator GainOptions(float mass) => new(mass);
}

// Please read https://github.com/tModLoader/tModLoader/wiki/Basic-tModLoader-Modding-Guide#mod-skeleton-contents for more information about the various files in a mod.
public partial class WgMod : Mod
{
    static readonly Dictionary<int, GainOptions> _buffTable = [];

    // Vanilla
    void AddBuffs((int id, GainOptions gain)[] table)
    {
        foreach (var (id, gain) in table)
            _buffTable[id] = gain;
    }

    // Mods
    void AddBuffs(string mod, (string name, GainOptions gain)[] table)
    {
        if (!ModLoader.TryGetMod(mod, out Mod foundMod))
            return;
        foreach (var (name, gain) in table)
        {
            if (!foundMod.TryFind(name, out ModBuff foundBuff))
            {
                Logger.Warn($"Couldn't find buff '{name}' for mod '{mod}'");
                return;
            }
            _buffTable[foundBuff.Type] = gain;
        }
    }

    public override void Load()
    {
        RegisterBuffs();
        On_Player.AddBuff += OnPlayerAddBuff;
        On_Player.DelBuff += OnPlayerDelBuff;
        On_Mount.Draw += OnMountDraw;
    }

    public override void Unload()
    {
        On_Player.AddBuff -= OnPlayerAddBuff;
        On_Player.DelBuff -= OnPlayerDelBuff;
        On_Mount.Draw -= OnMountDraw;
    }

    public static void OnPlayerAddBuff(On_Player.orig_AddBuff orig, Player self, int type, int timeToAdd, bool quiet, bool foodHack)
    {
        if (!self.TryGetModPlayer(out WgPlayer wg))
        {
            orig(self, type, timeToAdd, quiet, foodHack);
            return;
        }

        int previousTime = int.MinValue;
        if (self.HasBuff(type))
            previousTime = self.buffTime[self.FindBuffIndex(type)];
        orig(self, type, timeToAdd, quiet, foodHack);
        if (!self.HasBuff(type))
            return;

        int index = self.FindBuffIndex(type);
        wg.BuffDuration[index] = timeToAdd;
        
        if (_buffTable.TryGetValue(type, out var gain))
        {
            if (gain.IsInstant)
            {
                if (previousTime < timeToAdd - 2) // Apply once (2 ticks of leeway)
                    wg.SetWeight(wg.Weight + gain.TotalGain);
            }
            else if (!self.HasBuff<GainingBuff>())
                GainingBuff.AddBuff(wg, gain);
        }
    }

    public static void OnPlayerDelBuff(On_Player.orig_DelBuff orig, Player self, int index)
    {
        if (self.TryGetModPlayer(out WgPlayer wg))
        {
            wg.BuffDuration[index] = 0;
            int num = 0;
            for (int i = 0; i < wg.BuffDuration.Length - 1; i++)
            {
                if (wg.BuffDuration[i] != 0)
                {
                    if (num < i)
                    {
                        wg.BuffDuration[num] = wg.BuffDuration[i];
                        wg.BuffDuration[i] = 0;
                    }
                    num++;
                }
            }
        }
        orig(self, index);
    }

    public static void OnMountDraw(On_Mount.orig_Draw orig, Mount self, List<DrawData> playerDrawData, int drawType, Player drawPlayer, Vector2 Position, Color drawColor, SpriteEffects playerEffect, float shadow)
    {
        if (drawPlayer.TryGetModPlayer(out WgPlayer wg) && self.Active)
            Position.Y += WeightValues.DrawOffsetY(wg.Weight.GetStage());
        orig(self, playerDrawData, drawType, drawPlayer, Position, drawColor, playerEffect, shadow);
    }
}
