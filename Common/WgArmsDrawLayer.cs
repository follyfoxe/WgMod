using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Common;

public class WgArmsDrawLayer : PlayerDrawLayer
{
    public const int ArmStageCount = 3;
    public static readonly Asset<Texture2D>[] ArmTextures = new Asset<Texture2D>[ArmStageCount];

    public override bool IsHeadLayer => false;
    public override Transformation Transform => PlayerDrawLayers.TorsoGroup;

    public override void Load()
    {
        if (Main.dedServ)
            return;
        for (int i = 0; i < ArmTextures.Length; i++)
            ArmTextures[i] = Mod.Assets.Request<Texture2D>("Assets/Textures/Arms" + i);
    }

    public override Position GetDefaultPosition() => new Between(PlayerDrawLayers.ArmOverItem, PlayerDrawLayers.HandOnAcc);
    public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) => true;

    protected override void Draw(ref PlayerDrawSet drawInfo)
    {
        if (!drawInfo.drawPlayer.TryGetModPlayer(out WgPlayer wg))
            return;
        int stage = wg.Weight.GetStage();
        int armStage = WeightValues.GetArmStage(stage);
        if (armStage < 0)
            return;

        Vector2 vector = new Vector2((int)(drawInfo.Position.X - Main.screenPosition.X - drawInfo.drawPlayer.bodyFrame.Width / 2 + drawInfo.drawPlayer.width / 2), (int)(drawInfo.Position.Y - Main.screenPosition.Y + drawInfo.drawPlayer.height - drawInfo.drawPlayer.bodyFrame.Height + 4f)) + drawInfo.drawPlayer.bodyPosition + new Vector2(drawInfo.drawPlayer.bodyFrame.Width / 2, drawInfo.drawPlayer.bodyFrame.Height / 2);
        Vector2 vector2 = Main.OffsetsPlayerHeadgear[drawInfo.drawPlayer.bodyFrame.Y / drawInfo.drawPlayer.bodyFrame.Height];
        vector2.Y -= 2f;
        vector += vector2 * -drawInfo.playerEffect.HasFlag(SpriteEffects.FlipVertically).ToDirectionInt();

        float rotation = drawInfo.drawPlayer.bodyRotation + drawInfo.compositeFrontArmRotation;
        Vector2 bodyVect = drawInfo.bodyVect;
        Vector2 compositeOffset_FrontArm = new(5 * drawInfo.playerEffect.HasFlag(SpriteEffects.FlipHorizontally).ToDirectionInt(), 0f);
        bodyVect += compositeOffset_FrontArm;
        vector += compositeOffset_FrontArm;

        if (drawInfo.compFrontArmFrame.X / drawInfo.compFrontArmFrame.Width >= 7)
            vector -= new Vector2(drawInfo.playerEffect.HasFlag(SpriteEffects.FlipHorizontally).ToDirectionInt(), drawInfo.playerEffect.HasFlag(SpriteEffects.FlipVertically).ToDirectionInt());

        Asset<Texture2D> texture = ArmTextures[armStage];
        int frameX = drawInfo.compFrontArmFrame.X / drawInfo.compFrontArmFrame.Width;
        int frameY = drawInfo.compFrontArmFrame.Y / drawInfo.compFrontArmFrame.Height;
        Rectangle frame = texture.Frame(9, 4, frameX, frameY);

        bodyVect -= drawInfo.compFrontArmFrame.Size() * 0.5f;
        bodyVect += frame.Size() * 0.5f;

        drawInfo.DrawDataCache.Add(new DrawData(
            texture.Value,
            vector,
            frame,
            drawInfo.colorBodySkin,
            rotation,
            bodyVect,
            1f,
            drawInfo.playerEffect)
        {
            shader = drawInfo.skinDyePacked
        });
    }
}
