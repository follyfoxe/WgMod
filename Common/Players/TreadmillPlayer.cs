using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using WgMod.Content.Tiles;

namespace WgMod.Common.Players;

public class TreadmillPlayer : ModPlayer
{
    internal bool _onTreadmill;
    internal float _treadmillX;

    public override void PostUpdateBuffs()
    {
        if (_onTreadmill && Player.TryGetModPlayer(out WgPlayer wg))
            wg.WeightLossFactor += Treadmill.WeightLoss;
    }

    public override void PreUpdate()
    {
        if (!Player.sitting.isSitting)
            _onTreadmill = false;
    }

    public override void PostUpdate()
    {
        if (_onTreadmill)
            Player.Center = new Vector2(_treadmillX, Player.Center.Y);
    }

    public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
    {
        drawInfo.isSitting = false;
        drawInfo.torsoOffset = 0f;
        drawInfo.seatYOffset = 0f;
    }
}
