using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace WgMod.Common.Configs;

public class WgClientConfig : ModConfig
{
    public override ConfigScope Mode => ConfigScope.ClientSide;

    [Header("$Mods.WgMod.Configs.Client.Headers.Visual")]

    [LabelKey("$Mods.WgMod.Configs.Client.DisableJiggle.Label")]
    [TooltipKey("$Mods.WgMod.Configs.Client.DisableJiggle.Tooltip")]
    [DefaultValue(false)]
    public bool DisableJiggle;

    [LabelKey("$Mods.WgMod.Configs.Client.DetailedTooltips.Label")]
    [TooltipKey("$Mods.WgMod.Configs.Client.DetailedTooltips.Tooltip")]
    [DefaultValue(false)]
    public bool DetailedTooltips;
}
