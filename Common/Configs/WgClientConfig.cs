using System.ComponentModel;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace WgMod.Common.Configs;

public class WgClientConfig : ModConfig
{
    public static WgClientConfig Instance => ModContent.GetInstance<WgClientConfig>();
    public override ConfigScope Mode => ConfigScope.ClientSide;

    [Header("Visual")]
    [DefaultValue(false)]
    public bool DisableJiggle;

    [DefaultValue(false)]
    public bool DisableUVClothes;
}
