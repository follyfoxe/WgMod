using Terraria.ModLoader.Config;

namespace WgMod.Common.Configs;

public class WgServerConfig : ModConfig
{
    public override ConfigScope Mode => ConfigScope.ServerSide;

    [Header("General")]
    public bool DisableBuffs;
}
