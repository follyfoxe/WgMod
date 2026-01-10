using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace WgMod.Common.Configs;

public class WgServerConfig : ModConfig
{
    public override ConfigScope Mode => ConfigScope.ServerSide;

    [Header("$Mods.WgMod.Configs.Server.Headers.General")]

    [LabelKey("$Mods.WgMod.Configs.Server.DisableFatBuffs.Label")]
    [TooltipKey("$Mods.WgMod.Configs.Server.DisableFatBuffs.Tooltip")]
    [DefaultValue(false)]
    public bool DisableFatBuffs;

    [LabelKey("$Mods.WgMod.Configs.Server.DisableFatHitbox.Label")]
    [TooltipKey("$Mods.WgMod.Configs.Server.DisableFatHitbox.Tooltip")]
    [DefaultValue(false)]
    public bool DisableFatHitbox;

    /*
    [Header("$Mods.WgMod.Configs.Server.Headers.Extra")]

    [LabelKey("$Mods.WgMod.Configs.Server.WeightChangeMult.Label")]
    [TooltipKey("$Mods.WgMod.Configs.Server.WeightChangeMult.Tooltip")]
    [Range(0.5f, 1.5f)]
    [Increment(.1f)]
    [DrawTicks]
    [DefaultValue(1f)]
    public float WeightChangeMult;
    */
}
