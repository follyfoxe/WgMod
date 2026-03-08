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
    public override bool IsHeadLayer => false;
    public override Transformation Transform => PlayerDrawLayers.TorsoGroup;

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

        drawInfo.DrawDataCache.Add(new DrawData(
            SpriteSet.Current.ArmTextures[armStage].Value,
            vector,
            drawInfo.compFrontArmFrame,
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
