using System;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace WgMod;

public static class Utility
{
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

    public static string OutOf(this int value, int max)
    {
        return OutOf(value.ToString(), max.ToString());
    }

    public static string OutOf(this float value, float max)
    {
        return OutOf(value.ToString(), max.ToString());
    }

    public static string OutOf(this string value, string max)
    {
        return $"{value}[c/900090:/{max}]";
    }

    public static string Percent(this float value, bool addSign = false)
    {
        string str = MathF.Round(value * 100f).ToString();
        if (addSign)
            str += "%";
        return str;
    }

    public static string Percent(this float value, float max, bool addSign = false)
    {
        return value.Percent(false).OutOf(max.Percent(addSign));
    }
}
