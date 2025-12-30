using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Common;

public class WgPlayerDrawLayer : PlayerDrawLayer
{
    public override bool IsHeadLayer => false;
    public override Transformation Transform => PlayerDrawLayers.TorsoGroup;

    Asset<Texture2D> _baseTexture;
    Asset<Texture2D> _bellyTexture;

    public override void Load()
    {
        _baseTexture = Mod.Assets.Request<Texture2D>("Assets/Textures/Base");
        _bellyTexture = Mod.Assets.Request<Texture2D>("Assets/Textures/Belly");
    }

    public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.Torso);

    public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
    {
        return true;
    }

    protected override void Draw(ref PlayerDrawSet drawInfo)
    {
        if (!drawInfo.drawPlayer.TryGetModPlayer(out WgPlayer wg))
            return;
        int stage = wg.Weight.GetStage();
        if (stage == 0)
            return;

        Vector2 position = drawInfo.Center - Main.screenPosition;
        if ((drawInfo.playerEffect & SpriteEffects.FlipHorizontally) != 0)
            position.X -= WeightValues.DrawOffsetX(stage);
        else
            position.X += WeightValues.DrawOffsetX(stage);
        position.Y -= drawInfo.seatYOffset;
        position.Y += drawInfo.mountOffSet * 0.5f;

        Rectangle legFrame = drawInfo.drawPlayer.legFrame;
        int frame = legFrame.Y / legFrame.Height;
        // Frame [0] - Idle
        // Frame [5] - Jump
        // Frame [6 to 19] - Walk

        float offset = 0f;
        if (stage < Weight.ImmobileStage)
        {
            if (frame == 5)
                offset = Math.Clamp(drawInfo.drawPlayer.velocity.Y / 4f, -1f, 1f) * -2f;
            else if (frame >= 6 && frame <= 19)
                offset = float.Lerp(2f, -2f, MathF.Sin((frame - 6) / 13f * MathF.Tau * 2f) * 0.5f + 0.5f);
        }
        wg._bellyOffset = offset;

        float baseSquish = (wg._squishPos + 1f) * 0.5f;
        Rectangle baseFrame = _baseTexture.Frame(1, Weight.StageCount, 0, stage);
        drawInfo.DrawDataCache.Add(new DrawData(
            _baseTexture.Value,
            PrepPos(position + new Vector2(0f, MathF.Round(MathF.Abs(offset) / 2f)) * -2f),
            baseFrame,
            drawInfo.colorBodySkin,
            0f,
            baseFrame.Size() * 0.5f,
            new Vector2(1f * baseSquish, 1f / baseSquish),
            drawInfo.playerEffect
        ));
        
        Rectangle bellyFrame = _bellyTexture.Frame(1, Weight.StageCount, 0, stage);
        drawInfo.DrawDataCache.Add(new DrawData(
            _bellyTexture.Value, // The texture to render.
            PrepPos(position + new Vector2(0f, MathF.Round(offset / 2f) * 2f)), // Position to render at.
            bellyFrame, // Source rectangle.
            drawInfo.colorBodySkin, // Color.
            0f, // Rotation.
            bellyFrame.Size() * 0.5f, // Origin. Uses the texture's center.
            new Vector2(1f / wg._squishPos, 1f * wg._squishPos), // Scale.
            drawInfo.playerEffect
        ));
    }

    static Vector2 PrepPos(Vector2 pos)
    {
        pos.Y += 1f;
        return pos.Floor();
    }
}
