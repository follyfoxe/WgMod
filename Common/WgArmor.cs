using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Common;

public static class WgArmor
{
    public const int TextureWidth = 100;
    public const int TextureHeight = 672;

    public record struct Layer(Asset<Texture2D> ArmorTexture, Color Color);

    public static Asset<Effect> UVShader { get; private set; }
    public static Asset<Texture2D> BaseTexture { get; private set; }
    public static Asset<Texture2D> BellyTexture { get; private set; }

    public static void Load(Mod mod)
    {
        UVShader = mod.Assets.Request<Effect>("Assets/Effects/FatArmor");
        BaseTexture = mod.Assets.Request<Texture2D>("Assets/Textures/Base_ArmorFem");
        BellyTexture = mod.Assets.Request<Texture2D>("Assets/Textures/Belly_ArmorFem");
    }

    public static void Render(ref RenderTarget2D target, ReadOnlySpan<Layer> layers)
    {
        UVShader.Wait();
        BaseTexture.Wait();
        BellyTexture.Wait();

        GraphicsDevice device = Main.graphics.GraphicsDevice;
        SpriteBatch spriteBatch = Main.spriteBatch;
        target ??= new RenderTarget2D(device, TextureWidth * 2, TextureHeight, false, device.PresentationParameters.BackBufferFormat, DepthFormat.None);

        device.SetRenderTarget(target);
        device.Clear(Color.Transparent);
        spriteBatch.Begin(
            SpriteSortMode.Immediate,
            BlendState.AlphaBlend,
            SamplerState.PointClamp,
            DepthStencilState.None,
            RasterizerState.CullCounterClockwise,
            UVShader.Value
        );

        foreach (Layer layer in layers)
        {
            if (layer.ArmorTexture == null)
                continue;
            device.Textures[1] = layer.ArmorTexture.Value;
            spriteBatch.Draw(BaseTexture.Value, Vector2.Zero, layer.Color);
            spriteBatch.Draw(BellyTexture.Value, new Vector2(TextureWidth, 0f), layer.Color);
        }

        spriteBatch.End();
        device.SetRenderTarget(null);
    }

    public static void Draw(WgPlayer wg, ref PlayerDrawSet drawInfo, in DrawData baseDrawData, int index)
    {
        Rectangle rect = baseDrawData.sourceRect.Value;
        rect.X += index * TextureWidth;
        drawInfo.DrawDataCache.Add(baseDrawData with
        {
            texture = wg._armorTarget,
            sourceRect = rect,
            shader = drawInfo.cBody,
            color = Color.White
        });
    }
}
