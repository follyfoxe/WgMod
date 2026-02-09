using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod;

public static class Utility
{
    // TODO: Decide if we're gonna use this or not...
    public static WgPlayer Wg(this Player player)
    {
        return player.GetModPlayer<WgPlayer>();
    }

    public static bool FormatLines(this List<TooltipLine> tooltips, params object[] args)
    {
        int start = tooltips.FindIndex(t => t.Name == "Tooltip0");
        if (start < 0)
            return false;
        for (int i = start; i < tooltips.Count; i++)
        {
            TooltipLine line = tooltips[i];
            if (!line.Name.StartsWith("Tooltip"))
                break;
            if (!line.Text.Contains('{'))
                continue;
            line.Text = string.Format(line.Text, args);
        }
        return true;
    }

    public static bool Format(this List<TooltipLine> tooltips, string name, params object[] args)
    {
        TooltipLine tooltip = tooltips.Find(t => t.Name == name);
        if (tooltip == null)
            return false;
        tooltip.Text = string.Format(tooltip.Text, args);
        return true;
    }

    public static string Red(this string value)
    {
        return $"[c/C42254:{value}]";
    }

    public static string Range(this int value, int min, int max)
    {
        return Range(value.ToString(), min.ToString(), max.ToString());
    }

    public static string Range(this string value, string min, string max)
    {
        if (value.Equals(min, StringComparison.InvariantCultureIgnoreCase))
            return OutOf(value.Red(), max);
        return OutOf(value, max);
    }

    public static string OutOf(this string value, string max)
    {
        return $"{value}[c/B000B0:/{max}]";
    }

    public static string Percent(this float value)
    {
        int rounded = (int)MathF.Round(value * 100f);
        if (rounded == 0)
            return rounded.ToString().Red();
        return rounded.ToString();
    }

    public static string Percent(this float value, float max)
    {
        return value.Percent().OutOf(max.Percent());
    }
}
