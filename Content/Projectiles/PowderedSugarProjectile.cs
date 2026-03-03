using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WgMod.Content.Projectiles;

public class PowderedSugarProjectile : ModProjectile
{
    public override void SetDefaults()
    {
        Projectile.CloneDefaults(ProjectileID.PurificationPowder);
    }

    public override string Texture
    {
        get { return "WgMod/Content/Dusts/CutieHeart"; }
    }
}
