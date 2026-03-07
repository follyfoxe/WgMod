using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using WgMod.Common.Players;
using System.Collections.Generic;
using Terraria.Localization;

namespace WgMod.Content.Items.Armor;

public class TurtleArmor : GlobalItem
{
    WgStat _helmetDamage = new(0.03f, 0.09f);
    WgStat _helmetDefense = new(3, 7);
    WgStat _helmetHealth = new(10, 20);

    WgStat _scaleMailDamageCrit = new(0.04f, 0.1f);
    WgStat _scaleMailDefense = new(5, 9);
    WgStat _scaleMailHealth = new(10, 20);

    WgStat _leggingsCrit = new(0.02f, 0.06f);
    WgStat _leggingsDefense = new(2, 6);
    WgStat _leggingsHealth = new(5, 10);

    public override bool InstancePerEntity => true;

    public override void SetDefaults(Item item)
    {
        if (item.type == ItemID.TurtleHelmet)
            item.defense -= 5;

        if (item.type == ItemID.TurtleScaleMail)
            item.defense -= 7;

        if (item.type == ItemID.TurtleLeggings)
            item.defense -= 4;
    }

    public override void UpdateEquip(Item item, Player player)
    {
        if (!player.TryGetModPlayer(out WgPlayer wg))
            return;
        float immobility = wg.Weight.ClampedImmobility;

        if (item.type == ItemID.TurtleHelmet)
        {
            player.GetDamage(DamageClass.Melee) -= 0.06f;

            _helmetDamage.Lerp(immobility);
            _helmetDefense.Lerp(immobility);
            _helmetHealth.Lerp(immobility);

            player.GetDamage(DamageClass.Generic) += _helmetDamage;
            player.statDefense += _helmetDefense;
            player.statLifeMax2 += _helmetHealth;
        }

        if (item.type == ItemID.TurtleScaleMail)
        {
            player.GetDamage(DamageClass.Melee) -= 0.08f;
            player.GetCritChance(DamageClass.Melee) -= 0.08f;

            _scaleMailDamageCrit.Lerp(immobility);
            _scaleMailDefense.Lerp(immobility);
            _scaleMailHealth.Lerp(immobility);

            player.GetDamage(DamageClass.Generic) += _scaleMailDamageCrit;
            player.GetCritChance(DamageClass.Generic) += _scaleMailDamageCrit;
            player.statDefense += _scaleMailDefense;
            player.statLifeMax2 += _scaleMailHealth;
        }

        if (item.type == ItemID.TurtleLeggings)
        {
            player.GetCritChance(DamageClass.Melee) -= 0.04f;

            _leggingsCrit.Lerp(immobility);
            _leggingsDefense.Lerp(immobility);
            _leggingsHealth.Lerp(immobility);

            player.GetCritChance(DamageClass.Generic) += _leggingsCrit;
            player.statDefense += _leggingsDefense;
            player.statLifeMax2 += _leggingsHealth;
        }
    }

    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
    {
        if (item.type == ItemID.TurtleLeggings)
        {
            tooltips.Find(t => t.Name == "Tooltip0")
            .Text = Mod.GetLocalization("Items.TurtleLeggings.Tooltip")
            .Format(_leggingsCrit.Percent(), _leggingsHealth, _leggingsDefense);
        }

        if (item.type == ItemID.TurtleScaleMail)
        {
            tooltips.Find(t => t.Name == "Tooltip0")
            .Text = Mod.GetLocalization("Items.TurtleScaleMail.Tooltip")
            .Format(_scaleMailDamageCrit.Percent(), _scaleMailHealth, _scaleMailDefense);
        }

        if (item.type == ItemID.TurtleHelmet)
        {
            tooltips.Find(t => t.Name == "Tooltip0")
            .Text = Mod.GetLocalization("Items.TurtleHelmet.Tooltip")
            .Format(_helmetDamage.Percent(), _helmetHealth, _helmetDefense);
        }
    }
}