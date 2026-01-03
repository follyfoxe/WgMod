using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Buffs
{
    public class AmbrosiaGorged : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (!player.TryGetModPlayer(out WgPlayer wg))
                return;

            float immobility = wg.Weight.ClampedImmobility;

            player.moveSpeed *= float.Lerp(1.5f, 2f, immobility);
            player.maxRunSpeed *= float.Lerp(1.5f, 2f, immobility);
            player.runAcceleration *= float.Lerp(1.5f, 2f, immobility);
            player.accRunSpeed *= float.Lerp(1.5f, 2f, immobility);
            player.statDefense += (int)float.Lerp(1f, 10f, immobility);
            player.lifeRegen += (int)float.Lerp(1f, 5f, immobility);
        }
    }
}
