using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace WgMod.Content.Dusts;
public class FollowerDustSmall : ModDust
{
    public override string Texture => "WgMod/Content/Dusts/FollowerDust";
    public override void OnSpawn(Dust dust)
    {
        dust.noGravity = true;
        dust.noLight = false;
        dust.frame = new Rectangle(0, 0, 6, 6);
        dust.alpha = 0;
    }
    public override bool PreDraw(Dust dust)
    {
        return false;
    }
    public override bool Update(Dust dust)
    {
        dust.position += dust.velocity;

        dust.velocity *= 0.97f;

        dust.scale *= 0.97f;

        float light = dust.scale * 0.001f;

        Lighting.AddLight(dust.position, new Vector3(dust.color.R * light, dust.color.G * light, dust.color.B * light));

        if (dust.scale <= 0.15f)
            dust.active = false;

        return false;
    }
}

public class FollowerDustBig : ModDust
{
    public override string Texture => "WgMod/Content/Dusts/FollowerDust";
    public override void OnSpawn(Dust dust)
    {
        dust.noGravity = true;
        dust.noLight = false;
        dust.frame = new Rectangle(0, 0, 7, 7);
        dust.alpha = 0;
    }
    public override bool PreDraw(Dust dust)
    {
        return false;
    }
    public override bool Update(Dust dust)
    {
        dust.position += dust.velocity;

        dust.velocity *= 0.9f;

        dust.scale *= 0.98f;

        float light = dust.scale * 0.001f;

        Lighting.AddLight(dust.position, new Vector3(dust.color.R * light, dust.color.G * light, dust.color.B * light));

        if (dust.scale <= 0.15f)
            dust.active = false;

        return false;
    }
}
