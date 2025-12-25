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
    int _duration;

    public override void Update(Player player, ref int buffIndex)
    {
        if (!Main.LocalPlayer.TryGetModPlayer(out WgPlayer wg))
            return;
        GetProgress(buffIndex);
        wg.SetWeight(wg.Weight + TotalGain / _duration);
    }

    public override float GetProgress(int buffIndex)
    {
        int time = Main.LocalPlayer.buffTime[buffIndex];
        _duration = Math.Max(_duration, time);
        return 1f - time / (float)_duration;
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
}
