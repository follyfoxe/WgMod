using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Configs;
using WgMod.Common.Players;

namespace WgMod.Common;

public class WgPlayerDrawLayer : PlayerDrawLayer
{
    public override bool IsHeadLayer => false;
    public override Transformation Transform => PlayerDrawLayers.TorsoGroup;

    Asset<Texture2D> _baseTexture;
    Asset<Texture2D> _bellyTexture;

    public override void SetStaticDefaults()
    {
        if (Main.netMode == NetmodeID.Server)
            return;
        WgArms.SetupDrawing(Mod);
    }

    public override void Load()
    {
        if (Main.netMode == NetmodeID.Server)
            return;
        WgArms.Load(Mod);
        WgArmor.Load(Mod);
        _baseTexture = Mod.Assets.Request<Texture2D>("Assets/Textures/Base");
        _bellyTexture = Mod.Assets.Request<Texture2D>("Assets/Textures/Belly");
    }

    // folly: What is OffhandAcc exactly???
    public override Position GetDefaultPosition() => new Between(PlayerDrawLayers.Torso, PlayerDrawLayers.OffhandAcc);
    public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) => true;

    protected override void Draw(ref PlayerDrawSet drawInfo)
    {
        Player player = drawInfo.drawPlayer;
        if (!player.TryGetModPlayer(out WgPlayer wg))
            return;

        int stage = wg.Weight.GetStage();
        if (stage == 0)
            return;

        Vector2 position = drawInfo.Center - Main.screenPosition;
        if ((drawInfo.playerEffect & SpriteEffects.FlipHorizontally) != 0)
            position.X -= WeightValues.DrawOffsetX(stage);
        else
            position.X += WeightValues.DrawOffsetX(stage);

        float yOffset = 0f;
        yOffset -= drawInfo.seatYOffset;
        yOffset += drawInfo.mountOffSet * 0.5f;
        if (drawInfo.shadow != 0f)
            yOffset -= WeightValues.DrawOffsetY(stage);

        Rectangle legFrame = player.legFrame;
        int frame = legFrame.Y / legFrame.Height;
        // Frame [0] - Idle
        // Frame [5] - Jump
        // Frame [6 to 19] - Walk

        float animOffset = 0f;
        if (stage < Weight.ImmobileStage)
        {
            if (frame == 5)
                animOffset = Math.Clamp(player.velocity.Y * player.gravDir / 4f, -1f, 1f) * -2f;
            else if (frame >= 6 && frame <= 19)
                animOffset = float.Lerp(2f, -2f, MathF.Sin((frame - 6) / 13f * MathF.Tau * 2f) * 0.5f + 0.5f);
        }
        wg._bellyOffset = animOffset;

        Color skinColor = drawInfo.colorBodySkin; //player.GetImmuneAlphaPure(player.skinColor, drawInfo.shadow);
        float t = wg.Weight.ClampedImmobility;
        float bellySquish = float.Lerp(wg._squishPos, 1f, t * t * 0.4f);
        float baseSquish = (bellySquish + 1f) * 0.5f;

        bool drawArmor = drawInfo.usesCompositeTorso && !WgClientConfig.Instance.DisableUVClothes;
        if (drawArmor)
        {
            Array.Clear(wg._armorLayers);
            if (wg._lastBodySlot > 0)
                wg._armorLayers[0] = new(TextureAssets.ArmorBodyComposite[wg._lastBodySlot], drawInfo.colorArmorBody);
            else
            {
                wg._armorLayers[0] = new(TextureAssets.Players[drawInfo.skinVar, 4], drawInfo.colorUnderShirt);
                wg._armorLayers[1] = new(TextureAssets.Players[drawInfo.skinVar, 6], drawInfo.colorShirt);
            }
        }

        Rectangle baseFrame = _baseTexture.Frame(1, Weight.StageCount, 0, stage);
        DrawData baseDrawData = new(
            _baseTexture.Value,
            PrepPos(position, yOffset - MathF.Round(MathF.Abs(animOffset) / 2f) * 2f, player.gravDir),
            baseFrame,
            skinColor,
            0f,
            baseFrame.Size() * 0.5f,
            new Vector2(1f * baseSquish, 1f / baseSquish),
            drawInfo.playerEffect
        );
        drawInfo.DrawDataCache.Add(baseDrawData);
        if (drawArmor)
            WgArmor.Draw(wg, ref drawInfo, baseDrawData, 0);

        Rectangle bellyFrame = _bellyTexture.Frame(1, Weight.StageCount, 0, stage);
        DrawData bellyDrawData = new(
            _bellyTexture.Value, // The texture to render.
            PrepPos(position, yOffset + MathF.Round(animOffset / 2f) * 2f, player.gravDir), // Position to render at.
            bellyFrame, // Source rectangle.
            skinColor, // Color.
            0f, // Rotation.
            bellyFrame.Size() * 0.5f, // Origin. Uses the texture's center.
            new Vector2(1f / bellySquish, 1f * bellySquish), // Scale.
            drawInfo.playerEffect
        );
        drawInfo.DrawDataCache.Add(bellyDrawData);
        if (drawArmor)
            WgArmor.Draw(wg, ref drawInfo, bellyDrawData, 1);
    }

    static Vector2 PrepPos(Vector2 pos, float yOffset, float gravDir)
    {
        pos.Y += (1f + yOffset) * gravDir;
        return pos.Floor();
    }
}
