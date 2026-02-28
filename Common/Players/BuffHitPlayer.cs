using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Content.Buffs;

namespace WgMod.Common.Players;

public partial class BuffHitPlayer : ModPlayer
{
    void AddNPCs(HashSet<int> table, string mod, params string[] npcs)
    {
        if (!ModLoader.TryGetMod(mod, out Mod foundMod))
            return;
        foreach (string npc in npcs)
        {
            if (!foundMod.TryFind(npc, out ModNPC foundNpc))
            {
                Mod.Logger.Warn($"Couldn't find buff '{npc}' for mod '{mod}'");
                continue;
            }
            table.Add(foundNpc.Type);
        }
    }

    public override void Load()
    {
        AddModNPCs();
    }

    void AddBuff(int type, int timeToAdd, float weightGain)
    {
        if (!Player.TryGetModPlayer(out WgPlayer wg))
            return;
        Player.AddBuff(type, timeToAdd);
        wg.SetWeight(wg.Weight + weightGain);
        SoundEngine.PlaySound(WgSounds.Gulp, Player.Center);
        if (weightGain > 0f)
            CombatText.NewText(Player.getRect(), Color.Yellow, weightGain + " kg");
    }

    public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
    {
        if (_slimes.Contains(npc.type))
            AddBuff(BuffID.Slimed, 10 * hurtInfo.Damage, hurtInfo.Damage / 10);
        if (_bees.Contains(npc.type))
            AddBuff(BuffID.Slimed, 10 * hurtInfo.Damage, hurtInfo.Damage / 8);
        if (_feeders.Contains(npc.type))
            AddBuff(ModContent.BuffType<ForceFed>(), 10 * hurtInfo.Damage, hurtInfo.Damage / 6);
    }
}
