using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;

namespace WgMod.Common.Items
{
    public class ItemLists
    {
        //first is ItemID, second is the amount of weight to gain
        //unfinished as im waiting on the stomach system
        public static Dictionary<int, float> ConsumableItems = new Dictionary<int, float>
        {
            { ItemID.LesserHealingPotion, 3f },
            { ItemID.HealingPotion, 7f },
            { ItemID.GreaterHealingPotion, 12f },
            { ItemID.SuperHealingPotion, 18f },
            { ItemID.RestorationPotion, 10f },
            { ItemID.Mushroom, 0.8f },
            { ItemID.BottledHoney, 12f },
            { ItemID.Honeyfin, 10f },
            { ItemID.StrangeBrew, 0f },
            { ItemID.Eggnog, 10f },
            { ItemID.RecallPotion, 3f },
        };
    }
}
