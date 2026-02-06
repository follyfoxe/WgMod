using System;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace WgMod;

public static class Utility
{
    public static bool Format(this List<TooltipLine> tooltips, int line, params object[] args)
    {
        return Format(tooltips, "Tooltip" + line, args);
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
        return $"{value}[c/676767:/{max}]";
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
