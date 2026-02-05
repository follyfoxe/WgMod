using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;
using WgMod.Common.Configs;
using WgMod.Common.Players;

namespace WgMod.Common;

public class WgPlayerDrawLayer : PlayerDrawLayer
{
    // Better than having an item I suppose...
    public const int ShaderItemId = -1000;

    public override bool IsHeadLayer => false;
    public override Transformation Transform => PlayerDrawLayers.TorsoGroup;

    Asset<Texture2D> _baseTexture;
    Asset<Texture2D> _bellyTexture;
    int _baseArmorShader;
    int _bellyArmorShader;

    public override void SetStaticDefaults()
    {
        const string armorShaderPass = "MainPass";
        Asset<Effect> armorShader = Mod.Assets.Request<Effect>("Assets/Effects/FatArmor");

        Asset<Texture2D> baseTexture = Mod.Assets.Request<Texture2D>("Assets/Textures/Base_ArmorFem");
        GameShaders.Armor.BindShader(ShaderItemId, new ArmorShaderData(armorShader, armorShaderPass).UseImage(baseTexture));
        _baseArmorShader = GameShaders.Armor.GetShaderIdFromItemId(ShaderItemId);

        Asset<Texture2D> bellyTexture = Mod.Assets.Request<Texture2D>("Assets/Textures/Belly_ArmorFem");
        GameShaders.Armor.BindShader(ShaderItemId, new ArmorShaderData(armorShader, armorShaderPass).UseImage(bellyTexture));
        _bellyArmorShader = GameShaders.Armor.GetShaderIdFromItemId(ShaderItemId);
    }

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
        if (drawInfo.shadow != 0f)
            position.Y -= WeightValues.DrawOffsetY(stage);

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

        Color skinColor = drawInfo.colorBodySkin; //drawInfo.drawPlayer.GetImmuneAlphaPure(drawInfo.drawPlayer.skinColor, drawInfo.shadow);
        float t = wg.Weight.ClampedImmobility;
        float bellySquish = float.Lerp(wg._squishPos, 1f, t * t * 0.4f);
        float baseSquish = (bellySquish + 1f) * 0.5f;

        ArmorLayer layer1 = new(null, drawInfo.colorArmorBody);
        ArmorLayer layer2 = new(null, drawInfo.colorArmorBody);
        if (drawInfo.usesCompositeTorso && !WgClientConfig.Instance.DisableUVClothes)
        {
            if (wg._lastBodySlot > 0)
                layer1.Texture = TextureAssets.ArmorBodyComposite[wg._lastBodySlot];
            else
            {
                layer1.Texture = TextureAssets.Players[drawInfo.skinVar, 4];
                layer1.Color = drawInfo.colorUnderShirt;

                layer2.Texture = TextureAssets.Players[drawInfo.skinVar, 6];
                layer2.Color = drawInfo.colorShirt;
            }
        }

        Rectangle baseFrame = _baseTexture.Frame(1, Weight.StageCount, 0, stage);
        DrawData baseDrawData = new(
            _baseTexture.Value,
            PrepPos(position + new Vector2(0f, MathF.Round(MathF.Abs(offset) / 2f)) * -2f),
            baseFrame,
            skinColor,
            0f,
            baseFrame.Size() * 0.5f,
            new Vector2(1f * baseSquish, 1f / baseSquish),
            drawInfo.playerEffect
        );
        drawInfo.DrawDataCache.Add(baseDrawData);
        layer1.Draw(drawInfo, baseDrawData, _baseArmorShader);
        layer2.Draw(drawInfo, baseDrawData, _baseArmorShader);

        Rectangle bellyFrame = _bellyTexture.Frame(1, Weight.StageCount, 0, stage);
        DrawData bellyDrawData = new(
            _bellyTexture.Value, // The texture to render.
            PrepPos(position + new Vector2(0f, MathF.Round(offset / 2f) * 2f)), // Position to render at.
            bellyFrame, // Source rectangle.
            skinColor, // Color.
            0f, // Rotation.
            bellyFrame.Size() * 0.5f, // Origin. Uses the texture's center.
            new Vector2(1f / bellySquish, 1f * bellySquish), // Scale.
            drawInfo.playerEffect
        );
        drawInfo.DrawDataCache.Add(bellyDrawData);
        layer1.Draw(drawInfo, bellyDrawData, _bellyArmorShader);
        layer2.Draw(drawInfo, bellyDrawData, _bellyArmorShader);
    }

    static Vector2 PrepPos(Vector2 pos)
    {
        pos.Y += 1f;
        return pos.Floor();
    }

    record struct ArmorLayer(Asset<Texture2D> Texture, Color Color)
    {
        public void Draw(in PlayerDrawSet drawInfo, DrawData baseDrawData, int shader)
        {
            if (Texture != null)
            {
                drawInfo.DrawDataCache.Add(baseDrawData with
                {
                    texture = Texture.Value,
                    color = Color,
                    shader = shader
                });
            }
        }
    }
}
