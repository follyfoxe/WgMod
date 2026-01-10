using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
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
        _baseTexture = Mod.Assets.Request<Texture2D>("Assets/Textures/Legs");
        _bellyTexture = Mod.Assets.Request<Texture2D>("Assets/Textures/Belly");
    }

    public override Position GetDefaultPosition() => new Between(PlayerDrawLayers.Torso, PlayerDrawLayers.OffhandAcc);

    public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
    {
        return true;
    }

    protected override void Draw(ref PlayerDrawSet drawInfo)
    {
        Player drawPlr = drawInfo.drawPlayer;
        float flipVal = drawPlr.gravDir;
        if (!drawPlr.TryGetModPlayer(out WgPlayer wg))
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

        if (flipVal == -1f)
            position.Y -= 6f;
        Rectangle legFrame = drawPlr.legFrame;
        int frame = legFrame.Y / legFrame.Height;
        // Frame [0] - Idle
        // Frame [5] - Jump
        // Frame [6 to 19] - Walk

        float offset = 0f;

        if (frame == 5)
            offset = Math.Clamp(drawPlr.velocity.Y / 4f, -1f, 1f) * -2f * flipVal;
        else if (frame >= 6 && frame <= 19)
            offset = float.Lerp(2f, -2f, MathF.Sin((frame - 6) / 13f * MathF.Tau * 2f) * 0.5f + 0.5f) * flipVal;

        wg._bellyOffset = offset;

        Color skinColor = drawInfo.colorBodySkin; //drawPlr.GetImmuneAlphaPure(drawPlr.skinColor, drawInfo.shadow);
        //float t = wg.Weight.ClampedImmobility;
        float bellySquish = float.Lerp(wg._squishPos, 1f, 0.4f); //t * t * 0.4f
        float baseSquish = (bellySquish + 1f) * 0.5f;

        if (wg._squishedCauseNoSpace)
        {
            bellySquish = 1.15f;
            baseSquish = 0.85f;
        }

        //Rectangle baseFrame = _baseTexture.Frame(1, Weight.StageCount, 0, stage);
        Rectangle baseFrame = GetLegFrame(stage);
        baseFrame = new Rectangle(baseFrame.X, baseFrame.Y + 2, baseFrame.Width, baseFrame.Height - 2);
        drawInfo.DrawDataCache.Add(new DrawData(
            _baseTexture.Value,
            PrepPos(position + new Vector2(0f, MathF.Round(MathF.Abs(offset) / 2f)) * -2f * flipVal, stage, flipVal),
            baseFrame,
            skinColor,
            0f,
            baseFrame.Size() * 0.5f,
            new Vector2(1f * baseSquish, 1f / baseSquish),
            drawInfo.playerEffect
        ));
        
        Rectangle bellyFrame = GetLegFrame(stage);
        bellyFrame = new Rectangle(bellyFrame.X, bellyFrame.Y + 2, bellyFrame.Width, bellyFrame.Height - 2);
        drawInfo.DrawDataCache.Add(new DrawData(
            _bellyTexture.Value, // The texture to render.
            PrepPos(position + new Vector2(0f, MathF.Round(offset / 2f) * 2f) * flipVal, stage, flipVal), // Position to render at.
            bellyFrame, // Source rectangle.
            skinColor, // Color.
            0f, // Rotation.
            baseFrame.Size() * 0.5f, // Origin. Uses the texture's center.
            new Vector2(1f / bellySquish, 1f * bellySquish), // Scale.
            drawInfo.playerEffect
        ));
    }

    static Vector2 PrepPos(Vector2 pos, int stage, float grav)
    {
        pos.Y += 3f;
        pos.Y += WeightValues.BodyOffsetY(stage) * grav;
        if (stage == 7 && grav == -1f)
            pos.Y += 6f;
        return pos.Floor();
    }
    public Rectangle GetLegFrame(int stage)
    {
        switch (stage)
        {
            case 1: return new Rectangle(54, 0, 32, 42);
            case 2: return new Rectangle(54, 44, 32, 42);
            case 3: return new Rectangle(48, 88, 44, 42);
            case 4: return new Rectangle(42, 132, 56, 42);
            case 5: return new Rectangle(36, 176, 68, 44);
            case 6: return new Rectangle(30, 222, 80, 48);
            case 7: return new Rectangle(16, 270, 108, 56);
            default: return new Rectangle();
        }
    }
}
public class WgPlayerArmDrawLayer : PlayerDrawLayer
{
    public override bool IsHeadLayer => false;
    public override Transformation Transform => PlayerDrawLayers.TorsoGroup;
    public override Position GetDefaultPosition() => new Between(PlayerDrawLayers.ArmOverItem, PlayerDrawLayers.HandOnAcc);

    public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
    {
        return true;
    }

    protected override void Draw(ref PlayerDrawSet drawInfo)
    {
        if (!drawInfo.drawPlayer.TryGetModPlayer(out WgPlayer wg))
            return;
        int stage = wg.Weight.GetStage();
        int armstage = WeightValues.GetArmStage(stage);
        if (armstage <= 0)
            return;

        Vector2 vector = new Vector2((float)((int)(drawInfo.Position.X - Main.screenPosition.X - (float)(drawInfo.drawPlayer.bodyFrame.Width / 2) + (float)(drawInfo.drawPlayer.width / 2))), (float)((int)(drawInfo.Position.Y - Main.screenPosition.Y + (float)drawInfo.drawPlayer.height - (float)drawInfo.drawPlayer.bodyFrame.Height + 4f))) + drawInfo.drawPlayer.bodyPosition + new Vector2((float)(drawInfo.drawPlayer.bodyFrame.Width / 2), (float)(drawInfo.drawPlayer.bodyFrame.Height / 2));
        Vector2 vector2 = Main.OffsetsPlayerHeadgear[drawInfo.drawPlayer.bodyFrame.Y / drawInfo.drawPlayer.bodyFrame.Height];
        vector2.Y -= 2f;
        vector += vector2 * (float)(-(float)drawInfo.playerEffect.HasFlag((SpriteEffects)2).ToDirectionInt());
        float bodyRotation = drawInfo.drawPlayer.bodyRotation;
        float rotation = drawInfo.drawPlayer.bodyRotation + drawInfo.compositeFrontArmRotation;
        Vector2 bodyVect = drawInfo.bodyVect;
        Vector2 compositeOffset_FrontArm = new Vector2((float)(-5 * ((!drawInfo.playerEffect.HasFlag((SpriteEffects)1)) ? 1 : -1)), 0f);
        bodyVect += compositeOffset_FrontArm;
        vector += compositeOffset_FrontArm;
        Vector2 position = vector + drawInfo.frontShoulderOffset;
        if (drawInfo.compFrontArmFrame.X / drawInfo.compFrontArmFrame.Width >= 7)
        {
            vector += new Vector2((float)((!drawInfo.playerEffect.HasFlag((SpriteEffects)1)) ? 1 : -1), (float)((!drawInfo.playerEffect.HasFlag((SpriteEffects)2)) ? 1 : -1));
        }

        DrawData drawData = new DrawData(WgArms.ArmTextures[armstage].Value,
            vector,
            new Rectangle?(drawInfo.compFrontArmFrame),
            drawInfo.colorBodySkin,
            rotation,
            bodyVect,
            1f,
            drawInfo.playerEffect, 0f)
        {
            shader = drawInfo.skinDyePacked
        };

        drawInfo.DrawDataCache.Add(drawData);
    }
}
