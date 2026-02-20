using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Buffs
{
    public class Caramel : ModBuff
    {
        WgStat _defense = new(5, 15);

        public override LocalizedText Description => base.Description.WithFormatArgs(_defense);

        public override void Update(Player player, ref int buffIndex)
        {
            if (!player.TryGetModPlayer(out WgPlayer wg))
                return;
            float immobility = wg.Weight.ClampedImmobility;
            _defense.Lerp(immobility);

            player.statDefense += _defense;

            int _dustRate = 5;
            if (Main.rand.NextBool(_dustRate))
            {
                Dust.NewDust(player.position, player.width, player.height, DustID.t_Honey, 0f, 0.5f, 150, new Color(151, 93, 15), 0.7f);
            }
        }

        public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
        {
            tip = base.Description.Format(_defense);
        }
    }
}
