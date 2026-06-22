using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ModLoader;

namespace WgMod.Content.Dusts;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
public class Bat : ModDust
{
    public override void OnSpawn(Dust dust)
    {
        dust.noGravity = false;
        dust.frame = new Rectangle(0, 0, 24, 18);
    }

    public override bool Update(Dust dust)
    {
        Lighting.AddLight(dust.position, 1, 1, 1);

        dust.velocity.Y += 0.1f;
        dust.rotation = 0f;

        return true;
    }
}
