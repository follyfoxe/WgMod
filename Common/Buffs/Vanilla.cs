using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WgMod.Common.Buffs
{
    public class Honey : GlobalBuff
    {
        public override void Update(int type, Player player, ref int buffIndex)
        {
            if (type != BuffID.Honey)
                return;
            player.WG().PassiveWeightGain += 0.03f; //gain 0.03kg every second
        }
    }
}
