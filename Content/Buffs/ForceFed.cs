using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Buffs;

public class ForceFed : ModBuff
{
    int cooldown;

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

        if (cooldown < 30)
            cooldown++;
        else
        {
            cooldown = 0;
            wg.SetWeight(wg.Weight + 3);
            SoundEngine.PlaySound(
                new SoundStyle("WgMod/Assets/Sounds/Gulp_", 4, SoundType.Sound),
                player.Center
            );
        }
    }
}
