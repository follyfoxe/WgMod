using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Buffs;

public class GainingBuff : WgBuffBase
{
    public override void Update(Player player, ref int buffIndex)
    {
        if (!player.TryGetModPlayer(out WgPlayer wg))
            return;
        wg.AddWeight(wg._buffTotalGain / wg.BuffDuration[buffIndex]);
    }

    public static bool AddBuff(WgPlayer wg, GainOptions gain)
    {
        wg._buffTotalGain = gain.TotalGain;
        wg.Player.AddBuff(ModContent.BuffType<GainingBuff>(), (int)MathF.Round(gain.Time * 60f));
        SoundEngine.PlaySound(SoundID.SplashWeak);
        return true;
    }
}
