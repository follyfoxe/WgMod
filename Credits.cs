using System;
using System.Collections.Generic;
using System.ComponentModel;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;
using WgMod.Common.Configs;

namespace WgMod;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class CreditAttribute(ProjectRole role, Contributor contributor) : Attribute
{
    public ProjectRole Role { get; } = role;
    public Contributor Contributor { get; } = contributor;
}

public enum ProjectRole
{
    Programmer,
    Artist,
    VisualProgrammer
}

public enum Contributor
{
    [Category("Team")]
    follycake,
    maimaichubs,
    jumpsu2,
    bb_waffle_batter,
    blowyourselfup,
    _d_u_m_m_y_,

    [Category("Contributors")]
    ubulumn,
    trilophyte,
    sinnerdrip
}

public static class Credits
{
    public static readonly Dictionary<Type, TooltipLine> Tooltips = [];

    public static short GetIcon(ProjectRole role) => role switch
    {
        ProjectRole.Programmer => ItemID.ArtisanLoaf,
        ProjectRole.Artist => ItemID.PaintRoller,
        _ => ItemID.None
    };

    public static void Scan(Mod mod)
    {
        Type[] types = AssemblyManager.GetLoadableTypes(mod.Code);
        foreach (Type type in types)
        {
            object[] credits = type.GetCustomAttributes(typeof(CreditAttribute), false);
            if (credits.Length <= 0)
                continue;
            string text = "";
            for (int i = 0; i < credits.Length; i++)
            {
                CreditAttribute credit = (CreditAttribute)credits[i];
                if (i != 0)
                    text += "\n";
                text += $"[i:{GetIcon(credit.Role)}] [c/808080:{credit.Role}: @{credit.Contributor}]";
            }
            Tooltips.Add(type, new TooltipLine(mod, "Credits", text));
        }
    }
}

public class CreditsGlobalItem : GlobalItem
{
    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
    {
        if (!WgClientConfig.Instance.ShowCredits)
            return;
        if (item.ModItem is not ModItem modItem || modItem.Mod != Mod)
            return;
        if (Credits.Tooltips.TryGetValue(modItem.GetType(), out TooltipLine line))
            tooltips.Add(line);
    }
}

public class CreditsGlobalBuff : GlobalBuff
{
    public override void ModifyBuffText(int type, ref string buffName, ref string tip, ref int rare)
    {
        base.ModifyBuffText(type, ref buffName, ref tip, ref rare);
        if (!WgClientConfig.Instance.ShowCredits)
            return;
        if (ModContent.GetModBuff(type) is not ModBuff buff || buff.Mod != Mod)
            return;
        if (Credits.Tooltips.TryGetValue(buff.GetType(), out TooltipLine line))
            tip += "\n" + line.Text;
    }
}
