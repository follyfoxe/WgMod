using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Buffs;

public class GainingBuff : WgBuffBase
{
    public const float TotalGain = 2f;

    public override void Update(Player player, ref int buffIndex)
    {
        if (!player.TryGetModPlayer(out WgPlayer wg))
            return;
        wg.SetWeight(wg.Weight + TotalGain / wg.buffDuration[buffIndex]);
    }

    public static bool AddBuff(Player player, float duration)
    {
        int type = ModContent.BuffType<GainingBuff>();
        if (player.HasBuff(type))
            return false;
        player.AddBuff(type, (int)MathF.Round(duration * 60f));
        SoundEngine.PlaySound(SoundID.SplashWeak);
        return true;
    }

    public static float DurationFromGainPerTick(float gainPerTick)
    {
        // GainPerTick = TotalGain / TotalTicks
        // GainPerTick * TotalTicks = TotalGain
        return TotalGain / gainPerTick / 60f;
    }
}
