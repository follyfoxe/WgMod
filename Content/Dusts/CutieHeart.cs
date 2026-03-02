using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace WgMod.Content.Dusts;

public class CutieHeart : ModDust
{
    public override void OnSpawn(Dust dust)
    {
        dust.noGravity = true;
        dust.frame = new Rectangle(0, Main.rand.Next(3) * 12, 14, 12);
    }

    public override bool Update(Dust dust)
    {
        float light = 0.35f * dust.scale;

        Lighting.AddLight(dust.position, light * 2, light, light * 2);

        return true;
    }
}
