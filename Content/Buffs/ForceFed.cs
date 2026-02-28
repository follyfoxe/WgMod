using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Buffs;

public class ForceFed : ModBuff
{
    const int FatPerCycle = 3;
    int _cooldown;

    public override void SetStaticDefaults()
    {
        Main.debuff[Type] = true;
        Main.pvpBuff[Type] = true;
        Main.buffNoSave[Type] = true;
    }

    public override void Update(Player player, ref int buffIndex)
    {
        if (!player.TryGetModPlayer(out WgPlayer wg))
            return;

        if (_cooldown < 30)
            _cooldown++;
        else
        {
            _cooldown = 0;
            wg.SetWeight(wg.Weight + FatPerCycle);
            CombatText.NewText(player.getRect(), Color.Yellow, FatPerCycle + " kg");
            SoundEngine.PlaySound(WgSounds.Gulp, player.Center);
        }
    }
}
