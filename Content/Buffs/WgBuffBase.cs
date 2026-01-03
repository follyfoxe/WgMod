using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Buffs;

public abstract class WgBuffBase : ModBuff
{
    public override bool PreDraw(SpriteBatch spriteBatch, int buffIndex, ref BuffDrawParams drawParams)
    {
        drawParams.DrawColor = Color.DimGray;
        return true;
    }

    public override void PostDraw(SpriteBatch spriteBatch, int buffIndex, BuffDrawParams drawParams)
    {
        if (!Main.LocalPlayer.TryGetModPlayer(out WgPlayer wg))
            return;
        Rectangle rect = drawParams.SourceRectangle;
        float t = GetProgress(wg, buffIndex);
        if (t < 1f)
        {
            int h = (int)MathF.Round(float.Lerp(0f, rect.Height, t) / 2f) * 2;
            spriteBatch.Draw(drawParams.Texture, drawParams.Position + new Vector2(0f, rect.Height - h), new Rectangle(rect.X, rect.Y + rect.Height - h, rect.Width, h), Color.White);
        }
        else
            spriteBatch.Draw(drawParams.Texture, drawParams.Position, drawParams.SourceRectangle, Color.White);
    }

    public virtual float GetProgress(WgPlayer wg, int buffIndex)
    {
        return 1f - wg.Player.buffTime[buffIndex] / (float)wg.BuffDuration[buffIndex];
    }
}
