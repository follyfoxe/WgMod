using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;
using WgMod.Common.Configs;
using WgMod.Common.Players;

namespace WgMod.Common;

public class WgPlayerDrawLayer : PlayerDrawLayer
{
    public override bool IsHeadLayer => false;
    public override Transformation Transform => PlayerDrawLayers.TorsoGroup;

    public static Asset<Texture2D> BaseTexture { get; private set; }
    public static Asset<Texture2D> BellyTexture { get; private set; }
    public static Asset<Texture2D> BoobsTexture { get; private set; }

    public override void Load()
    {
        if (Main.dedServ)
            return;
        WgArmor.Load(Mod);
        BaseTexture = Mod.Assets.Request<Texture2D>("Assets/Textures/Base");
        BellyTexture = Mod.Assets.Request<Texture2D>("Assets/Textures/Belly");
        BoobsTexture = Mod.Assets.Request<Texture2D>("Assets/Textures/Boobs");
    }

    // folly: What is OffhandAcc exactly???
    public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.Head);
    public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) => true;

    public static void SetupArmorLayers(WgPlayer wg)
    {
        PlayerDrawSet drawInfo = new()
        {
            drawPlayer = wg.Player,
            skinVar = wg.Player.skinVariant,
            colorArmorBody = Color.White,
            colorUnderShirt = wg.Player.underShirtColor,
            colorShirt = wg.Player.shirtColor
        };
        SetupArmorLayers(wg, drawInfo);
    }

    public static void SetupArmorLayers(WgPlayer wg, in PlayerDrawSet drawInfo)
    {
        Array.Clear(wg._armorLayers);
        if (drawInfo.drawPlayer.body > 0)
            wg._armorLayers[0] = new(TextureAssets.ArmorBodyComposite[drawInfo.drawPlayer.body], drawInfo.colorArmorBody);
        else
        {
            wg._armorLayers[0] = new(TextureAssets.Players[drawInfo.skinVar, 4], drawInfo.colorUnderShirt);
            wg._armorLayers[1] = new(TextureAssets.Players[drawInfo.skinVar, 6], drawInfo.colorShirt);
        }
    }

    protected override void Draw(ref PlayerDrawSet drawInfo)
    {
        Player player = drawInfo.drawPlayer;
        if (player.dead)
            return;

        if (!player.TryGetModPlayer(out WgPlayer wg))
            return;

        int stage = wg.Weight.GetStage();
        if (stage == 0)
            return;

        int direction = ((drawInfo.playerEffect & SpriteEffects.FlipHorizontally) == 0).ToDirectionInt();
        Vector2 position = drawInfo.Center - Main.screenPosition;
        position.X += WeightValues.DrawOffsetX(stage) * direction;

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

        float legOffsetX = 0f;
        float legOffsetY = 0f;
        float bellyOffset = 0f;
        if (wg._finalMovementFactor > 0.01f)
        {
            if (frame == 5)
                bellyOffset = Math.Clamp(player.velocity.Y * player.gravDir / 4f, -1f, 1f) * -2f;
            else if (frame >= 6 && frame <= 19)
            {
                float frameTime = (frame - 6) / 13f;
                legOffsetX = MathF.Sin(frameTime * MathF.Tau) * 2f * direction;
                legOffsetY = MathF.Max(MathF.Cos(frameTime * MathF.Tau), 0f) * -2f;
                bellyOffset = MathF.Sin(frameTime * MathF.Tau * 2f) * -2f;
            }
        }
        wg._bellyOffset = bellyOffset;

        Color skinColor = drawInfo.colorBodySkin; //player.GetImmuneAlphaPure(player.skinColor, drawInfo.shadow);
        float t = wg.Weight.ClampedImmobility;
        float bellySquish = float.Lerp(wg._squishPos, 1f, t * t * 0.4f);
        float baseSquish = (bellySquish + 1f) * 0.5f;

        bool drawArmor = !WgClientConfig.Instance.DisableUVClothes;
        if (drawInfo.shadow != 0f && player.body <= 0)
            drawArmor = false;
        if (drawArmor)
            SetupArmorLayers(wg, drawInfo);

        Rectangle baseFrame = BaseTexture.Frame(1, Weight.StageCount, 0, stage);
        DrawData baseDrawData = new(
            BaseTexture.Value,
            PrepPos(position, MathF.Round(legOffsetX / 2f) * 2f, yOffset + MathF.Round(legOffsetY / 2f) * 2f, player.gravDir),
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

        Rectangle bellyFrame = BellyTexture.Frame(1, Weight.StageCount, 0, stage);
        DrawData bellyDrawData = new(
            BellyTexture.Value, // The texture to render.
            PrepPos(position, 0f, yOffset + MathF.Round(bellyOffset / 2f) * 2f, player.gravDir), // Position to render at.
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

        Rectangle boobsFrame = BoobsTexture.Frame(1, Weight.StageCount, 0, stage);
        DrawData boobsDrawData = new(
            BoobsTexture.Value,
            bellyDrawData.position,
            boobsFrame,
            skinColor,
            0f,
            boobsFrame.Size() * 0.5f,
            new Vector2(1f * bellySquish, 1f / bellySquish),
            drawInfo.playerEffect
        );
        drawInfo.DrawDataCache.Add(boobsDrawData);
    }

    static Vector2 PrepPos(Vector2 pos, float xOffset, float yOffset, float gravDir)
    {
        pos.X += xOffset;
        pos.Y += (1f + yOffset) * gravDir;
        return pos.Floor();
    }
}
