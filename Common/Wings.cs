using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WgMod.Common;

public class AllWings : GlobalItem
{
    public override void HorizontalWingSpeeds(Item item, Player player, ref float speed, ref float acceleration)
    {
        float mult = player.Wg()._finalMovementFactor;
        acceleration *= Math.Clamp(mult * 1.25f, 0f, 1.0f);
    }

    public override void VerticalWingSpeeds(Item item, Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
    {
        float mult = player.Wg()._finalMovementFactor;
        ascentWhenFalling *= Math.Clamp(mult * 0.8f + 0.2f, 0.001f, 1.0f);
        ascentWhenRising *= Math.Clamp(mult, 0.001f, 1.0f);
        constantAscend *= Math.Clamp(mult, 0.001f, 1.0f);
    }
}

public class NewWings : GlobalItem
{
    public static float GetWingWR(Item item)
    {
        switch (item.type)
        {
            case ItemID.LongRainbowTrailWings:
                return 0.42f;

            case ItemID.RainbowWings:
            case ItemID.WingsSolar:
            case ItemID.WingsVortex:
            case ItemID.WingsNebula:
            case ItemID.WingsStardust:
                return 0.4f;

            case ItemID.BetsyWings:
            case ItemID.FishronWings:
                return 0.35f;

            case ItemID.Hoverboard:
            case ItemID.FestiveWings:
            case ItemID.SpookyWings:
            case ItemID.TatteredFairyWings:
            case ItemID.SteampunkWings:
                return 0.32f;

            case ItemID.BoneWings:
            case ItemID.MothronWings:
            case ItemID.GhostWings:
            case ItemID.BeetleWings:
                return 0.3f;

            case ItemID.Jetpack:
            case ItemID.LeafWings:
            case ItemID.BatWings:
            case ItemID.BeeWings:
            case ItemID.ButterflyWings:
            case ItemID.FlameWings:
                return 0.27f;

            case ItemID.RedsWings:
            case ItemID.DTownsWings:
            case ItemID.WillsWings:
            case ItemID.CrownosWings:
            case ItemID.CenxsWings:
            case ItemID.BejeweledValkyrieWing:
            case ItemID.Yoraiz0rWings:
            case ItemID.JimsWings:
            case ItemID.SkiphsWings:
            case ItemID.LokisWings:
            case ItemID.ArkhalisWings:
            case ItemID.LeinforsWings:
            case ItemID.GhostarsWings:
            case ItemID.SafemanWings:
            case ItemID.FoodBarbarianWings:
            case ItemID.GroxTheGreatWings:
                return 0.25f;

            case ItemID.FairyWings:
            case ItemID.FinWings:
            case ItemID.FrozenWings:
            case ItemID.HarpyWings:
                return 0.22f;

            default:
                return 0.2f;
        }
    }

    public override bool InstancePerEntity => true;

    public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.wingSlot >= 0;

    public override void UpdateAccessory(Item item, Player player, bool hideVisual)
    {
        if (player.velocity.Y != 0 && !player.mount.Active)
            player.Wg().MovementPenalty *= 1f - GetWingWR(item);
    }

    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
    {
        int index = tooltips.FindIndex(x => x.Mod == "Terraria" && x.Name.Contains("Tooltip"));
        if (index >= 0)
        {
            string text = Mod.GetLocalization("Items.Wings.Extra").Format(GetWingWR(item).Percent());
            tooltips.Insert(index + 1, new TooltipLine(Mod, "ExtraTooltip", text));
        }
    }
}
