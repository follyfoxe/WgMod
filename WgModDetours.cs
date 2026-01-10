using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Creative;
using Terraria.Graphics;
using Terraria.Graphics.Renderers;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common;
using WgMod.Common.Players;
using WgMod.Content.Buffs;
using WgMod.Content.Projectiles;

namespace WgMod
{
    public partial class Detours
    {
        public static void OnUpdateSitting(On_PlayerSittingHelper.orig_UpdateSitting orig, ref PlayerSittingHelper self, Player player)
        {
            if (!player.TryGetModPlayer(out WgPlayer wg) || !wg._onTreadmill)
            {
                orig(ref self, player);
                return;
            }
            bool left = player.controlLeft;
            bool right = player.controlRight;
            player.controlLeft = false;
            player.controlRight = false;
            orig(ref self, player);
            player.controlLeft = left;
            player.controlRight = right;
        }

        public static void OnMountDraw(On_Mount.orig_Draw orig, Mount self, List<DrawData> playerDrawData, int drawType, Player drawPlayer, Vector2 Position, Color drawColor, SpriteEffects playerEffect, float shadow)
        {
            if (drawPlayer.TryGetModPlayer(out WgPlayer wg) && self.Active)
                Position.Y += WeightValues.DrawOffsetY(wg.Weight.GetStage());
            orig(self, playerDrawData, drawType, drawPlayer, Position, drawColor, playerEffect, shadow);
        }
       
        public static void OnPlayer_StatusToPlayerPvP(On_Player.orig_StatusToPlayerPvP orig, Player self, int type, int i)
        {
            if (self.meleeEnchant > 0)
            {
                if (self.meleeEnchant == 1)
                {
                    Main.player[i].AddBuff(70, 60 * Main.rand.Next(5, 10), false, false);
                }
                if (self.meleeEnchant == 2)
                {
                    Main.player[i].AddBuff(39, 60 * Main.rand.Next(3, 7), false, false);
                }
                if (self.meleeEnchant == 3)
                {
                    Main.player[i].AddBuff(24, 60 * Main.rand.Next(3, 7), false, false);
                }
                if (self.meleeEnchant == 5)
                {
                    Main.player[i].AddBuff(69, 60 * Main.rand.Next(10, 20), false, false);
                }
                if (self.meleeEnchant == 6)
                {
                    Main.player[i].AddBuff(31, 60 * Main.rand.Next(1, 4), false, false);
                }
                if (self.meleeEnchant == 8)
                {
                    Main.player[i].AddBuff(20, 60 * Main.rand.Next(5, 10), false, false);
                }
            }
            if (self.frostBurn)
            {
                Main.player[i].AddBuff(324, 60 * Main.rand.Next(1, 8), false, false);
            }
            if (self.magmaStone)
            {
                if (Main.rand.Next(7) == 0)
                {
                    Main.player[i].AddBuff(323, 360, false, false);
                }
                else if (Main.rand.Next(3) == 0)
                {
                    Main.player[i].AddBuff(323, 120, false, false);
                }
                else
                {
                    Main.player[i].AddBuff(323, 60, false, false);
                }
            }
            if (type == 5129)
            {
                Main.player[i].AddBuff(120, 300, false, false);
            }
            if (type <= 190)
            {
                if (type != 121)
                {
                    if (type != 122)
                    {
                        if (type != 190)
                        {
                            return;
                        }
                        if (Main.rand.Next(4) == 0)
                        {
                            Main.player[i].AddBuff(20, 420, false, false);
                            return;
                        }
                    }
                    else if (Main.rand.Next(10) == 0)
                    {
                        Main.player[i].AddBuff(24, 180, false, false);
                        return;
                    }
                }
                else if (Main.rand.Next(2) == 0)
                {
                    Main.player[i].AddBuff(24, 180, false, false);
                    return;
                }
            }
            else if (type <= 1123)
            {
                if (type != 217)
                {
                    if (type != 1123)
                    {
                        return;
                    }
                    if (Main.rand.Next(9) != 0)
                    {
                        Main.player[i].AddBuff(31, 120, false, false);
                    }
                }
                else if (Main.rand.Next(5) == 0)
                {
                    Main.player[i].AddBuff(24, 180, false, false);
                    return;
                }
            }
            else if (type != 3823)
            {
                if (type != 5382)
                {
                    return;
                }
                if (Main.rand.Next(3) == 0)
                {
                    Main.player[i].AddBuff(323, 300, false, false);
                    return;
                }
            }
            else if (Main.rand.Next(4) == 0)
            {
                Main.player[i].AddBuff(323, 300, false, false);
                return;
            }
        }

        public static void OnLegacyPlayerRenderer_DrawPlayerFull(On_LegacyPlayerRenderer.orig_DrawPlayerFull orig, LegacyPlayerRenderer self, Terraria.Graphics.Camera camera, Player drawPlayer)
        {
            drawPlayer.position.Y -= drawPlayer.height + drawPlayer.WG().AddedVisualHeight - Player.defaultHeight;
            SpriteBatch spriteBatch = camera.SpriteBatch;
            SamplerState samplerState = camera.Sampler;
            if (drawPlayer.mount.Active && drawPlayer.fullRotation != 0f)
            {
                samplerState = LegacyPlayerRenderer.MountedSamplerState;
            }
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, samplerState, DepthStencilState.None, camera.Rasterizer, null, camera.GameViewMatrix.TransformationMatrix);
            if (Main.gamePaused)
            {
                drawPlayer.PlayerFrame();
            }
            if (drawPlayer.ghost)
            {
                for (int i = 0; i < 3; i++)
                {
                    Vector2 vector = drawPlayer.shadowPos[i];
                    vector = drawPlayer.position - drawPlayer.velocity * (float)(2 + i * 2);
                    DrawGhost(camera, drawPlayer, vector, 0.5f + 0.2f * (float)i);
                }
                DrawGhost(camera, drawPlayer, drawPlayer.position, 0f);
            }
            else
            {
                if (drawPlayer.inventory[drawPlayer.selectedItem].flame || drawPlayer.head == 137 || drawPlayer.wings == 22)
                {
                    drawPlayer.itemFlameCount--;
                    if (drawPlayer.itemFlameCount <= 0)
                    {
                        drawPlayer.itemFlameCount = 5;
                        for (int j = 0; j < 7; j++)
                        {
                            drawPlayer.itemFlamePos[j].X = (float)Main.rand.Next(-10, 11) * 0.15f;
                            drawPlayer.itemFlamePos[j].Y = (float)Main.rand.Next(-10, 1) * 0.35f;
                        }
                    }
                }
                PlayerLoader.DrawPlayer(drawPlayer, camera);
                if (drawPlayer.armorEffectDrawShadowEOCShield)
                {
                    int num = drawPlayer.eocDash / 4;
                    if (num > 3)
                    {
                        num = 3;
                    }
                    for (int k = 0; k < num; k++)
                    {
                        self.DrawPlayer(camera, drawPlayer, drawPlayer.shadowPos[k], drawPlayer.shadowRotation[k], drawPlayer.shadowOrigin[k], 0.5f + 0.2f * (float)k, 1f);
                    }
                }
                Vector2 position = default(Vector2);
                if (drawPlayer.invis)
                {
                    drawPlayer.armorEffectDrawOutlines = false;
                    drawPlayer.armorEffectDrawShadow = false;
                    drawPlayer.armorEffectDrawShadowSubtle = false;
                    position = drawPlayer.position;
                    if (drawPlayer.aggro <= -750)
                    {
                        self.DrawPlayer(camera, drawPlayer, position, drawPlayer.fullRotation, drawPlayer.fullRotationOrigin, 1f, 1f);
                    }
                    else
                    {
                        drawPlayer.invis = false;
                        self.DrawPlayer(camera, drawPlayer, position, drawPlayer.fullRotation, drawPlayer.fullRotationOrigin, 0f, 1f);
                        drawPlayer.invis = true;
                    }
                }
                if (drawPlayer.armorEffectDrawOutlines)
                {
                    Vector2 position2 = drawPlayer.position;
                    if (!Main.gamePaused)
                    {
                        drawPlayer.ghostFade += drawPlayer.ghostDir * 0.075f;
                    }
                    if ((double)drawPlayer.ghostFade < 0.1)
                    {
                        drawPlayer.ghostDir = 1f;
                        drawPlayer.ghostFade = 0.1f;
                    }
                    else if ((double)drawPlayer.ghostFade > 0.9)
                    {
                        drawPlayer.ghostDir = -1f;
                        drawPlayer.ghostFade = 0.9f;
                    }
                    float num2 = drawPlayer.ghostFade * 5f;
                    for (int l = 0; l < 4; l++)
                    {
                        float num3;
                        float num4;
                        switch (l)
                        {
                            case 1:
                                num3 = 0f - num2;
                                num4 = 0f;
                                break;
                            case 2:
                                num3 = 0f;
                                num4 = num2;
                                break;
                            case 3:
                                num3 = 0f;
                                num4 = 0f - num2;
                                break;
                            default:
                                num3 = num2;
                                num4 = 0f;
                                break;
                        }
                        position = new Vector2(drawPlayer.position.X + num3, drawPlayer.position.Y + drawPlayer.gfxOffY + num4);
                        self.DrawPlayer(camera, drawPlayer, position, drawPlayer.fullRotation, drawPlayer.fullRotationOrigin, drawPlayer.ghostFade, 1f);
                    }
                }
                if (drawPlayer.armorEffectDrawOutlinesForbidden)
                {
                    Vector2 position3 = drawPlayer.position;
                    if (!Main.gamePaused)
                    {
                        drawPlayer.ghostFade += drawPlayer.ghostDir * 0.025f;
                    }
                    if ((double)drawPlayer.ghostFade < 0.1)
                    {
                        drawPlayer.ghostDir = 1f;
                        drawPlayer.ghostFade = 0.1f;
                    }
                    else if ((double)drawPlayer.ghostFade > 0.9)
                    {
                        drawPlayer.ghostDir = -1f;
                        drawPlayer.ghostFade = 0.9f;
                    }
                    float num5 = drawPlayer.ghostFade * 5f;
                    for (int m = 0; m < 4; m++)
                    {
                        float num6;
                        float num7;
                        switch (m)
                        {
                            case 1:
                                num6 = 0f - num5;
                                num7 = 0f;
                                break;
                            case 2:
                                num6 = 0f;
                                num7 = num5;
                                break;
                            case 3:
                                num6 = 0f;
                                num7 = 0f - num5;
                                break;
                            default:
                                num6 = num5;
                                num7 = 0f;
                                break;
                        }
                        position = new Vector2(drawPlayer.position.X + num6, drawPlayer.position.Y + drawPlayer.gfxOffY + num7);
                        self.DrawPlayer(camera, drawPlayer, position, drawPlayer.fullRotation, drawPlayer.fullRotationOrigin, drawPlayer.ghostFade, 1f);
                    }
                }
                if (drawPlayer.armorEffectDrawShadowBasilisk)
                {
                    int num8 = (int)(drawPlayer.basiliskCharge * 3f);
                    for (int n = 0; n < num8; n++)
                    {
                        self.DrawPlayer(camera, drawPlayer, drawPlayer.shadowPos[n], drawPlayer.shadowRotation[n], drawPlayer.shadowOrigin[n], 0.5f + 0.2f * (float)n, 1f);
                    }
                }
                else if (drawPlayer.armorEffectDrawShadow)
                {
                    for (int num9 = 0; num9 < 3; num9++)
                    {
                        self.DrawPlayer(camera, drawPlayer, drawPlayer.shadowPos[num9], drawPlayer.shadowRotation[num9], drawPlayer.shadowOrigin[num9], 0.5f + 0.2f * (float)num9, 1f);
                    }
                }
                if (drawPlayer.armorEffectDrawShadowLokis)
                {
                    for (int num10 = 0; num10 < 3; num10++)
                    {
                        self.DrawPlayer(camera, drawPlayer, Vector2.Lerp(drawPlayer.shadowPos[num10], drawPlayer.position + new Vector2(0f, drawPlayer.gfxOffY), 0.5f), drawPlayer.shadowRotation[num10], drawPlayer.shadowOrigin[num10], MathHelper.Lerp(1f, 0.5f + 0.2f * (float)num10, 0.5f), 1f);
                    }
                }
                if (drawPlayer.armorEffectDrawShadowSubtle)
                {
                    for (int num11 = 0; num11 < 4; num11++)
                    {
                        position.X = drawPlayer.position.X + (float)Main.rand.Next(-20, 21) * 0.1f;
                        position.Y = drawPlayer.position.Y + (float)Main.rand.Next(-20, 21) * 0.1f + drawPlayer.gfxOffY;
                        self.DrawPlayer(camera, drawPlayer, position, drawPlayer.fullRotation, drawPlayer.fullRotationOrigin, 0.9f, 1f);
                    }
                }
                if (drawPlayer.shadowDodge)
                {
                    drawPlayer.shadowDodgeCount += 1f;
                    if (drawPlayer.shadowDodgeCount > 30f)
                    {
                        drawPlayer.shadowDodgeCount = 30f;
                    }
                }
                else
                {
                    drawPlayer.shadowDodgeCount -= 1f;
                    if (drawPlayer.shadowDodgeCount < 0f)
                    {
                        drawPlayer.shadowDodgeCount = 0f;
                    }
                }
                if (drawPlayer.shadowDodgeCount > 0f)
                {
                    Vector2 position4 = drawPlayer.position;
                    position.X = drawPlayer.position.X + drawPlayer.shadowDodgeCount;
                    position.Y = drawPlayer.position.Y + drawPlayer.gfxOffY;
                    self.DrawPlayer(camera, drawPlayer, position, drawPlayer.fullRotation, drawPlayer.fullRotationOrigin, 0.5f + (float)Main.rand.Next(-10, 11) * 0.005f, 1f);
                    position.X = drawPlayer.position.X - drawPlayer.shadowDodgeCount;
                    self.DrawPlayer(camera, drawPlayer, position, drawPlayer.fullRotation, drawPlayer.fullRotationOrigin, 0.5f + (float)Main.rand.Next(-10, 11) * 0.005f, 1f);
                }
                if (drawPlayer.brainOfConfusionDodgeAnimationCounter > 0)
                {
                    Vector2 vector2 = drawPlayer.position + new Vector2(0f, drawPlayer.gfxOffY);
                    float lerpValue = Terraria.Utils.GetLerpValue(300f, 270f, (float)drawPlayer.brainOfConfusionDodgeAnimationCounter, false);
                    float y = MathHelper.Lerp(2f, 120f, lerpValue);
                    if (lerpValue >= 0f && lerpValue <= 1f)
                    {
                        for (float num12 = 0f; num12 < 6.2831855f; num12 += 1.0471976f)
                        {
                            position = vector2 + new Vector2(0f, y).RotatedBy((double)(6.2831855f * lerpValue * 0.5f + num12), default(Vector2));
                            self.DrawPlayer(camera, drawPlayer, position, drawPlayer.fullRotation, drawPlayer.fullRotationOrigin, lerpValue, 1f);
                        }
                    }
                }
                position = drawPlayer.position;
                position.Y += drawPlayer.gfxOffY;
                if (drawPlayer.stoned)
                {
                    DrawPlayerStoned(camera, drawPlayer, position);
                }
                else if (!drawPlayer.invis)
                {
                    self.DrawPlayer(camera, drawPlayer, position, drawPlayer.fullRotation, drawPlayer.fullRotationOrigin, 0f, 1f);
                }
            }
            spriteBatch.End();

            drawPlayer.position.Y += drawPlayer.height + drawPlayer.WG().AddedVisualHeight - Player.defaultHeight;
        }
        public static void DrawGhost(Camera camera, Player drawPlayer, Vector2 position, float shadow = 0f)
        {
            byte mouseTextColor = Main.mouseTextColor;
            SpriteEffects effects = (SpriteEffects)((drawPlayer.direction != 1) ? 1 : 0);
            Color immuneAlpha = drawPlayer.GetImmuneAlpha(Lighting.GetColor((int)((double)drawPlayer.position.X + (double)drawPlayer.width * 0.5) / 16, (int)((double)drawPlayer.position.Y + (double)drawPlayer.height * 0.5) / 16, new Color((int)(mouseTextColor / 2 + 100), (int)(mouseTextColor / 2 + 100), (int)(mouseTextColor / 2 + 100), (int)(mouseTextColor / 2 + 100))), shadow);
            immuneAlpha.A = (byte)((float)immuneAlpha.A * (1f - Math.Max(0.5f, shadow - 0.5f)));
            Rectangle value;
            value = new Rectangle(0, TextureAssets.Ghost.Height() / 4 * drawPlayer.ghostFrame, TextureAssets.Ghost.Width(), TextureAssets.Ghost.Height() / 4);
            Vector2 origin;
            origin = new Vector2((float)value.Width * 0.5f, (float)value.Height * 0.5f);
            camera.SpriteBatch.Draw(TextureAssets.Ghost.Value, new Vector2((float)((int)(position.X - camera.UnscaledPosition.X + (float)(value.Width / 2))), (float)((int)(position.Y - camera.UnscaledPosition.Y + (float)(value.Height / 2)))), new Rectangle?(value), immuneAlpha, 0f, origin, 1f, effects, 0f);
        }
        public static void DrawPlayerStoned(Camera camera, Player drawPlayer, Vector2 position)
        {
            if (!drawPlayer.dead)
            {
                SpriteEffects spriteEffects = (SpriteEffects)((drawPlayer.direction != 1) ? 1 : 0);
                camera.SpriteBatch.Draw(TextureAssets.Extra[37].Value, new Vector2((float)((int)(position.X - camera.UnscaledPosition.X - (float)(drawPlayer.bodyFrame.Width / 2) + (float)(drawPlayer.width / 2))), (float)((int)(position.Y - camera.UnscaledPosition.Y + (float)drawPlayer.height - (float)drawPlayer.bodyFrame.Height + 8f))) + drawPlayer.bodyPosition + new Vector2((float)(drawPlayer.bodyFrame.Width / 2), (float)(drawPlayer.bodyFrame.Height / 2)), null, Lighting.GetColor((int)((double)position.X + (double)drawPlayer.width * 0.5) / 16, (int)((double)position.Y + (double)drawPlayer.height * 0.5) / 16, Color.White), 0f, new Vector2((float)(TextureAssets.Extra[37].Width() / 2), (float)(TextureAssets.Extra[37].Height() / 2)), 1f, spriteEffects, 0f);
            }
        }

        public static void OnPlayer_ResizeHitbox(On_Player.orig_ResizeHitbox orig, Player self)
        {
            self.position.Y = self.position.Y + (float)self.height;
            self.height = 42 + self.HeightOffsetBoost + self.WG().AddedHeight;
            self.position.Y = self.position.Y - (float)self.height;
        }

        public static void OnPlayerDrawSet_HeadOnlySetup(On_PlayerDrawSet.orig_HeadOnlySetup orig, ref PlayerDrawSet self, Player drawPlayer2, List<DrawData> drawData, List<int> dust, List<int> gore, float X, float Y, float Alpha, float Scale)
        {
            self.projectileDrawPosition = -1;
            self.headOnlyRender = true;
            self.DrawDataCache = drawData;
            self.DustCache = dust;
            self.GoreCache = gore;
            self.drawPlayer = drawPlayer2;
            self.Position = self.drawPlayer.position;
            CopyBasicPlayerFields(ref self);
            self.drawUnicornHorn = false;
            self.drawAngelHalo = false;
            self.skinVar = self.drawPlayer.skinVariant;
            self.hairDyePacked = PlayerDrawHelper.PackShader(self.drawPlayer.hairDye, PlayerDrawHelper.ShaderConfiguration.HairShader);
            if (self.drawPlayer.head == 0 && self.drawPlayer.hairDye == 0)
            {
                self.hairDyePacked = PlayerDrawHelper.PackShader(1, PlayerDrawHelper.ShaderConfiguration.HairShader);
            }
            self.skinDyePacked = self.drawPlayer.skinDyePacked;
            if (self.drawPlayer.face > 0 && self.drawPlayer.face < (int)ArmorIDs.Face.Count)
            {
                Main.instance.LoadAccFace(self.drawPlayer.face);
            }
            self.drawUnicornHorn = self.drawPlayer.hasUnicornHorn;
            self.drawAngelHalo = self.drawPlayer.hasAngelHalo;
            Main.instance.LoadHair(self.drawPlayer.hair);
            self.colorEyeWhites = Main.quickAlpha(Color.White, Alpha);
            self.colorEyes = Main.quickAlpha(self.drawPlayer.eyeColor, Alpha);
            self.colorHair = Main.quickAlpha(self.drawPlayer.GetHairColor(false), Alpha);
            self.colorHead = Main.quickAlpha(self.drawPlayer.skinColor, Alpha);
            self.colorArmorHead = Main.quickAlpha(Color.White, Alpha);
            if (self.drawPlayer.isDisplayDollOrInanimate)
            {
                self.colorDisplayDollSkin = Main.quickAlpha(PlayerDrawHelper.DISPLAY_DOLL_DEFAULT_SKIN_COLOR, Alpha);
            }
            else
            {
                self.colorDisplayDollSkin = self.colorHead;
            }
            self.playerEffect = 0;
            if (self.drawPlayer.direction < 0)
            {
                self.playerEffect = (SpriteEffects)1;
            }
            self.headVect = new Vector2((float)self.drawPlayer.legFrame.Width * 0.5f, (float)self.drawPlayer.legFrame.Height * 0.4f);
            self.Position = new Vector2(X, Y);
            self.Position.X = self.Position.X - 6f - Math.Max((self.drawPlayer.width / 2) - 10, 0);
            self.Position.Y = self.Position.Y - 4f - self.drawPlayer.WG().AddedVisualHeight;
            self.Position.Y = self.Position.Y - (float)self.drawPlayer.HeightMapOffset;
            SetupHairFrames(ref self);
            self.Position -= Main.OffsetsPlayerHeadgear[self.drawPlayer.bodyFrame.Y / self.drawPlayer.bodyFrame.Height];
            if (self.drawPlayer.head > 0 && self.drawPlayer.head < ArmorIDs.Head.Count)
            {
                Main.instance.LoadArmorHead(self.drawPlayer.head);
                int num = ArmorIDs.Head.Sets.FrontToBackID[self.drawPlayer.head];
                if (num >= 0)
                {
                    Main.instance.LoadArmorHead(num);
                }
            }
            if (self.drawPlayer.face > 0 && self.drawPlayer.face < (int)ArmorIDs.Face.Count)
            {
                Main.instance.LoadAccFace(self.drawPlayer.face);
            }
            if (self.drawPlayer.faceHead > 0 && self.drawPlayer.faceHead < (int)ArmorIDs.Face.Count)
            {
                Main.instance.LoadAccFace(self.drawPlayer.faceHead);
            }
            if (self.drawPlayer.faceFlower > 0 && self.drawPlayer.faceFlower < (int)ArmorIDs.Face.Count)
            {
                Main.instance.LoadAccFace(self.drawPlayer.faceFlower);
            }
            if (self.drawPlayer.beard > 0 && self.drawPlayer.beard < (int)ArmorIDs.Beard.Count)
            {
                Main.instance.LoadAccBeard(self.drawPlayer.beard);
            }
            self.hairOffset = self.drawPlayer.GetHairDrawOffset(self.drawPlayer.hair, self.hatHair);
            self.hairOffset.Y = self.hairOffset.Y * self.drawPlayer.Directions.Y;
            self.helmetOffset = self.drawPlayer.GetHelmetDrawOffset();
            self.helmetOffset.Y = self.helmetOffset.Y * self.drawPlayer.Directions.Y;
            self.drawPlayer.GetHairSettings(out self.fullHair, out self.hatHair, out self.hideHair, out self.backHairDraw, out self.drawsBackHairWithoutHeadgear);
        }
        public static void CopyBasicPlayerFields(ref PlayerDrawSet self)
        {
            self.heldItem = self.drawPlayer.lastVisualizedSelectedItem;
            self.cHead = self.drawPlayer.cHead;
            self.cBody = self.drawPlayer.cBody;
            self.cLegs = self.drawPlayer.cLegs;
            if (self.drawPlayer.wearsRobe)
            {
                self.cLegs = self.cBody;
            }
            self.cHandOn = self.drawPlayer.cHandOn;
            self.cHandOff = self.drawPlayer.cHandOff;
            self.cBack = self.drawPlayer.cBack;
            self.cFront = self.drawPlayer.cFront;
            self.cShoe = self.drawPlayer.cShoe;
            self.cFlameWaker = self.drawPlayer.cFlameWaker;
            self.cWaist = self.drawPlayer.cWaist;
            self.cShield = self.drawPlayer.cShield;
            self.cNeck = self.drawPlayer.cNeck;
            self.cFace = self.drawPlayer.cFace;
            self.cBalloon = self.drawPlayer.cBalloon;
            self.cWings = self.drawPlayer.cWings;
            self.cCarpet = self.drawPlayer.cCarpet;
            self.cPortableStool = self.drawPlayer.cPortableStool;
            self.cFloatingTube = self.drawPlayer.cFloatingTube;
            self.cUnicornHorn = self.drawPlayer.cUnicornHorn;
            self.cAngelHalo = self.drawPlayer.cAngelHalo;
            self.cLeinShampoo = self.drawPlayer.cLeinShampoo;
            self.cBackpack = self.drawPlayer.cBackpack;
            self.cTail = self.drawPlayer.cTail;
            self.cFaceHead = self.drawPlayer.cFaceHead;
            self.cFaceFlower = self.drawPlayer.cFaceFlower;
            self.cBalloonFront = self.drawPlayer.cBalloonFront;
            self.cBeard = self.drawPlayer.cBeard;
            self.isSitting = self.drawPlayer.sitting.isSitting;
        }
        public static void SetupHairFrames(ref PlayerDrawSet self)
        {
            Rectangle bodyFrame = self.drawPlayer.bodyFrame;
            bodyFrame = self.drawPlayer.bodyFrame;
            bodyFrame.Y -= 336;
            if (bodyFrame.Y < 0)
            {
                bodyFrame.Y = 0;
            }
            self.hairFrontFrame = bodyFrame;
            self.hairBackFrame = bodyFrame;
            if (self.hideHair)
            {
                self.hairFrontFrame.Height = 0;
                self.hairBackFrame.Height = 0;
                return;
            }
            if (self.backHairDraw)
            {
                int height = 26;
                self.hairFrontFrame.Height = height;
            }
        }

        public static void OnPlayer_Heal(On_Player.orig_Heal orig, Player self, int amount)
        {
            self.statLife += amount;
            if (Main.myPlayer == self.whoAmI)
            {
                self.HealEffect(amount, true);
            }
            if (self.statLife > self.statLifeMax2)
            {
                self.statLife = self.statLifeMax2;
                int newAmount = amount - (self.statLife - self.statLifeMax2);
                self.WG().AddWeight(newAmount / 10f);
            }
        }

        public static void OnPlayer_ApplyLifeAndOrMana(On_Player.orig_ApplyLifeAndOrMana orig, Player self, Item item)
        {
            int num = (item.healLife > 0) ? self.GetHealLife(item, true) : 0;
            int healMana = (item.healMana > 0) ? self.GetHealMana(item, true) : 0;
            if (item.type == ItemID.StrangeBrew)
            {
                int healLife = item.healLife;
                int num2 = 120;
                num = Main.rand.Next(healLife, num2 + 1);
                if (Main.myPlayer == self.whoAmI)
                {
                    float num3 = Main.rand.NextFloat();
                    int num4 = 0;
                    if (num3 <= 0.1f)
                    {
                        num4 = 240;
                    }
                    else if (num3 <= 0.3f)
                    {
                        num4 = 120;
                    }
                    else if (num3 <= 0.6f)
                    {
                        num4 = 60;
                    }
                    if (num4 > 0)
                    {
                        self.SetImmuneTimeForAllTypes(num4);
                    }
                }
            }
            self.statLife += num;
            self.statMana += healMana;
            if (self.statLife > self.statLifeMax2)
            {
                self.statLife = self.statLifeMax2;
                int newAmount = num - (self.statLife - self.statLifeMax2);
                self.WG().AddWeight(newAmount / 10f);
            }
            if (self.statMana > self.statManaMax2)
            {
                self.statMana = self.statManaMax2;
                int newAmount = healMana - (self.statMana - self.statManaMax2);
                self.WG().AddWeight(newAmount / 15f);
            }
            if (num > 0 && Main.myPlayer == self.whoAmI)
            {
                self.HealEffect(num, true);
            }
            if (healMana > 0)
            {
                self.AddBuff(94, Player.manaSickTime, true, false);
                if (Main.myPlayer == self.whoAmI)
                {
                    self.ManaEffect(healMana);
                }
            }
        }

        public static int OnPlayer_GetItemGrabRange(On_Player.orig_GetItemGrabRange orig, Player self, Item item)
        {
            int num = Player.defaultItemGrabRange;
            if (self.goldRing && item.IsACoin)
            {
                num += Item.coinGrabRange;
            }
            if (self.manaMagnet && (item.type == 184 || item.type == 1735 || item.type == 1868))
            {
                num += Item.manaGrabRange;
            }
            if (item.type == 4143)
            {
                num += Item.manaGrabRange;
            }
            if (self.lifeMagnet && (item.type == 58 || item.type == 1734 || item.type == 1867))
            {
                num += Item.lifeGrabRange;
            }
            if (self.treasureMagnet)
            {
                num += Item.treasureGrabRange;
            }
            if (item.type == 3822)
            {
                num += 50;
            }
            if (ItemID.Sets.NebulaPickup[item.type])
            {
                num += 100;
            }
            if (self.difficulty == 3 && CreativePowerManager.Instance.GetPower<CreativePowers.FarPlacementRangePower>().IsEnabledForPlayer(self.whoAmI))
            {
                num += 240;
            }
            if (self.WG()._bottomlessAppetite)
                num *= self.WG()._bottomlessAppetiteGrabRange;

            ItemLoader.GrabRange(item, self, ref num);
            return num;
        }

        public static void OnPlayer_UpdateJumpHeight(On_Player.orig_UpdateJumpHeight orig, Player self)
        {
            if (self.mount.Active)
            {
                Player.jumpHeight = self.mount.JumpHeight(self, self.velocity.X);
                Player.jumpSpeed = self.mount.JumpSpeed(self, self.velocity.X);
            }
            else
            {
                if (self.jumpBoost)
                {
                    Player.jumpHeight = 20;
                    Player.jumpSpeed = 6.51f;
                }
                if (self.empressBrooch)
                {
                    self.jumpSpeedBoost += 1.8f;
                }
                if (self.frogLegJumpBoost)
                {
                    self.jumpSpeedBoost += 2.4f;
                    self.extraFall += 15;
                }
                if (self.moonLordLegs)
                {
                    self.jumpSpeedBoost += 1.8f;
                    self.extraFall += 10;
                    Player.jumpHeight++;
                }
                if (self.wereWolf)
                {
                    Player.jumpHeight += 2;
                    Player.jumpSpeed += 0.2f;
                }
                if (self.portableStoolInfo.IsInUse)
                {
                    Player.jumpHeight += 5;
                }
                if (self.General().JumpSpeedBoost > 0)
                    self.jumpSpeedBoost += self.General().JumpSpeedBoost;
                Player.jumpSpeed += self.jumpSpeedBoost;
            }
            if (self.sticky)
            {
                Player.jumpHeight /= 10;
                Player.jumpSpeed /= 5f;
            }
            if (self.dazed)
            {
                Player.jumpHeight /= 5;
                Player.jumpSpeed /= 2f;
            }
        }

        public static void OnMain_DrawProj_DrawExtras(On_Main.orig_DrawProj_DrawExtras orig, Main self, Projectile proj, Vector2 mountedCenter, ref float polePosX, ref float polePosY)
        {
            Player plr = Main.player[proj.owner];
            if (plr.whoAmI >= 0 && plr.whoAmI < 255)
                if (proj.aiStyle == ProjAIStyleID.Yoyo || proj.aiStyle == ProjAIStyleID.Drill)
                    proj.gfxOffY = 0;
            if (proj.bobber && Main.player[proj.owner].inventory[Main.player[proj.owner].selectedItem].holdStyle != 0)
            {
                DrawProj_FishingLine(proj, ref polePosX, ref polePosY, mountedCenter);
                return;
            }
            if (proj.type == 32)
            {
                Vector2 vector3 = new Vector2(proj.position.X + (float)proj.width * 0.5f, proj.position.Y + (float)proj.height * 0.5f);
                float num24 = mountedCenter.X - vector3.X;
                float num25 = mountedCenter.Y - vector3.Y;
                float rotation = (float)Math.Atan2((double)num25, (double)num24) - 1.57f;
                bool flag5 = true;
                if (num24 == 0f && num25 == 0f)
                {
                    flag5 = false;
                }
                else
                {
                    float num26 = (float)Math.Sqrt((double)(num24 * num24 + num25 * num25));
                    num26 = 8f / num26;
                    num24 *= num26;
                    num25 *= num26;
                    vector3.X -= num24;
                    vector3.Y -= num25;
                    num24 = mountedCenter.X - vector3.X;
                    num25 = mountedCenter.Y - vector3.Y;
                }
                while (flag5)
                {
                    float num27 = (float)Math.Sqrt((double)(num24 * num24 + num25 * num25));
                    if (num27 < 28f)
                    {
                        flag5 = false;
                    }
                    else if (float.IsNaN(num27))
                    {
                        flag5 = false;
                    }
                    else
                    {
                        num27 = 28f / num27;
                        num24 *= num27;
                        num25 *= num27;
                        vector3.X += num24;
                        vector3.Y += num25;
                        num24 = mountedCenter.X - vector3.X;
                        num25 = mountedCenter.Y - vector3.Y;
                        Color color7 = Lighting.GetColor((int)vector3.X / 16, (int)(vector3.Y / 16f));
                        Main.EntitySpriteDraw(TextureAssets.Chain5.Value, new Vector2(vector3.X - Main.screenPosition.X, vector3.Y - Main.screenPosition.Y), new Rectangle?(new Rectangle(0, 0, TextureAssets.Chain5.Width(), TextureAssets.Chain5.Height())), color7, rotation, new Vector2((float)TextureAssets.Chain5.Width() * 0.5f, (float)TextureAssets.Chain5.Height() * 0.5f), 1f, 0, 0f);
                    }
                }
                return;
            }
            if (proj.type == 73)
            {
                Vector2 vector4 = new Vector2(proj.position.X + (float)proj.width * 0.5f, proj.position.Y + (float)proj.height * 0.5f);
                float num28 = mountedCenter.X - vector4.X;
                float num29 = mountedCenter.Y - vector4.Y;
                float rotation2 = (float)Math.Atan2((double)num29, (double)num28) - 1.57f;
                bool flag6 = true;
                while (flag6)
                {
                    float num30 = (float)Math.Sqrt((double)(num28 * num28 + num29 * num29));
                    if (num30 < 25f)
                    {
                        flag6 = false;
                    }
                    else if (float.IsNaN(num30))
                    {
                        flag6 = false;
                    }
                    else
                    {
                        num30 = 12f / num30;
                        num28 *= num30;
                        num29 *= num30;
                        vector4.X += num28;
                        vector4.Y += num29;
                        num28 = mountedCenter.X - vector4.X;
                        num29 = mountedCenter.Y - vector4.Y;
                        Color color8 = Lighting.GetColor((int)vector4.X / 16, (int)(vector4.Y / 16f));
                        Main.EntitySpriteDraw(TextureAssets.Chain8.Value, new Vector2(vector4.X - Main.screenPosition.X, vector4.Y - Main.screenPosition.Y), new Rectangle?(new Rectangle(0, 0, TextureAssets.Chain8.Width(), TextureAssets.Chain8.Height())), color8, rotation2, new Vector2((float)TextureAssets.Chain8.Width() * 0.5f, (float)TextureAssets.Chain8.Height() * 0.5f), 1f, 0, 0f);
                    }
                }
                return;
            }
            if (proj.type == 186)
            {
                Vector2 vector5 = new Vector2(proj.localAI[0], proj.localAI[1]);
                float num31 = Vector2.Distance(proj.Center, vector5) - proj.velocity.Length();
                float num32 = (float)TextureAssets.Chain17.Height() - num31;
                if (num31 > 0f && proj.ai[1] > 0f)
                {
                    Color color9 = Lighting.GetColor((int)proj.position.X / 16, (int)proj.position.Y / 16);
                    Main.EntitySpriteDraw(TextureAssets.Chain17.Value, vector5 - Main.screenPosition, new Rectangle?(new Rectangle(0, (int)num32, TextureAssets.Chain17.Width(), (int)num31)), color9, proj.rotation, new Vector2((float)(TextureAssets.Chain17.Width() / 2), 0f), 1f, 0, 0f);
                    return;
                }
            }
            else
            {
                if (proj.type == 74)
                {
                    Vector2 vector6 = new Vector2(proj.position.X + (float)proj.width * 0.5f, proj.position.Y + (float)proj.height * 0.5f);
                    float num33 = mountedCenter.X - vector6.X;
                    float num34 = mountedCenter.Y - vector6.Y;
                    float rotation3 = (float)Math.Atan2((double)num34, (double)num33) - 1.57f;
                    bool flag7 = true;
                    while (flag7)
                    {
                        float num35 = (float)Math.Sqrt((double)(num33 * num33 + num34 * num34));
                        if (num35 < 25f)
                        {
                            flag7 = false;
                        }
                        else if (float.IsNaN(num35))
                        {
                            flag7 = false;
                        }
                        else
                        {
                            num35 = 12f / num35;
                            num33 *= num35;
                            num34 *= num35;
                            vector6.X += num33;
                            vector6.Y += num34;
                            num33 = mountedCenter.X - vector6.X;
                            num34 = mountedCenter.Y - vector6.Y;
                            Color color10 = Lighting.GetColor((int)vector6.X / 16, (int)(vector6.Y / 16f));
                            Main.EntitySpriteDraw(TextureAssets.Chain9.Value, new Vector2(vector6.X - Main.screenPosition.X, vector6.Y - Main.screenPosition.Y), new Rectangle?(new Rectangle(0, 0, TextureAssets.Chain8.Width(), TextureAssets.Chain8.Height())), color10, rotation3, new Vector2((float)TextureAssets.Chain8.Width() * 0.5f, (float)TextureAssets.Chain8.Height() * 0.5f), 1f, 0, 0f);
                        }
                    }
                    return;
                }
                if (proj.type == 171)
                {
                    Vector2 vector7 = new Vector2(proj.position.X + (float)proj.width * 0.5f, proj.position.Y + (float)proj.height * 0.5f);
                    float num36 = 0f - proj.velocity.X;
                    float num37 = 0f - proj.velocity.Y;
                    float num38 = 1f;
                    if (proj.ai[0] <= 17f)
                    {
                        num38 = proj.ai[0] / 17f;
                    }
                    int num39 = (int)(30f * num38);
                    float num40 = 1f;
                    if (proj.ai[0] <= 30f)
                    {
                        num40 = proj.ai[0] / 30f;
                    }
                    float num41 = 0.4f * num40;
                    float num42 = num41;
                    num37 += num42;
                    Vector2[] array = new Vector2[num39];
                    float[] array2 = new float[num39];
                    for (int i = 0; i < num39; i++)
                    {
                        float num43 = (float)Math.Sqrt((double)(num36 * num36 + num37 * num37));
                        float num44 = 5.6f;
                        if (Math.Abs(num36) + Math.Abs(num37) < 1f)
                        {
                            num44 *= Math.Abs(num36) + Math.Abs(num37) / 1f;
                        }
                        num43 = num44 / num43;
                        num36 *= num43;
                        num37 *= num43;
                        float num45 = (float)Math.Atan2((double)num37, (double)num36) - 1.57f;
                        array[i].X = vector7.X;
                        array[i].Y = vector7.Y;
                        array2[i] = num45;
                        vector7.X += num36;
                        vector7.Y += num37;
                        num36 = 0f - proj.velocity.X;
                        num37 = 0f - proj.velocity.Y;
                        num42 += num41;
                        num37 += num42;
                    }
                    for (int num46 = num39 - 1; num46 >= 0; num46--)
                    {
                        vector7.X = array[num46].X;
                        vector7.Y = array[num46].Y;
                        float rotation4 = array2[num46];
                        Color color11 = Lighting.GetColor((int)vector7.X / 16, (int)(vector7.Y / 16f));
                        Main.EntitySpriteDraw(TextureAssets.Chain16.Value, new Vector2(vector7.X - Main.screenPosition.X, vector7.Y - Main.screenPosition.Y), new Rectangle?(new Rectangle(0, 0, TextureAssets.Chain16.Width(), TextureAssets.Chain16.Height())), color11, rotation4, new Vector2((float)TextureAssets.Chain16.Width() * 0.5f, (float)TextureAssets.Chain16.Height() * 0.5f), 0.8f, 0, 0f);
                    }
                    return;
                }
                if (proj.type == 475)
                {
                    Vector2 vector8 = new Vector2(proj.position.X + (float)proj.width * 0.5f, proj.position.Y + (float)proj.height * 0.5f);
                    float num47 = 0f - proj.velocity.X;
                    float num48 = 0f - proj.velocity.Y;
                    float num49 = 1f;
                    if (proj.ai[0] <= 17f)
                    {
                        num49 = proj.ai[0] / 17f;
                    }
                    int num50 = (int)(30f * num49);
                    float num51 = 1f;
                    if (proj.ai[0] <= 30f)
                    {
                        num51 = proj.ai[0] / 30f;
                    }
                    float num52 = 0.4f * num51;
                    float num53 = num52;
                    num48 += num53;
                    Vector2[] array3 = new Vector2[num50];
                    float[] array4 = new float[num50];
                    for (int j = 0; j < num50; j++)
                    {
                        float num54 = (float)Math.Sqrt((double)(num47 * num47 + num48 * num48));
                        float num55 = 5.6f;
                        if (Math.Abs(num47) + Math.Abs(num48) < 1f)
                        {
                            num55 *= Math.Abs(num47) + Math.Abs(num48) / 1f;
                        }
                        num54 = num55 / num54;
                        num47 *= num54;
                        num48 *= num54;
                        float num56 = (float)Math.Atan2((double)num48, (double)num47) - 1.57f;
                        array3[j].X = vector8.X;
                        array3[j].Y = vector8.Y;
                        array4[j] = num56;
                        vector8.X += num47;
                        vector8.Y += num48;
                        num47 = 0f - proj.velocity.X;
                        num48 = 0f - proj.velocity.Y;
                        num53 += num52;
                        num48 += num53;
                    }
                    int num57 = 0;
                    for (int num58 = num50 - 1; num58 >= 0; num58--)
                    {
                        vector8.X = array3[num58].X;
                        vector8.Y = array3[num58].Y;
                        float rotation5 = array4[num58];
                        Color color12 = Lighting.GetColor((int)vector8.X / 16, (int)(vector8.Y / 16f));
                        if (num57 % 2 == 0)
                        {
                            Main.EntitySpriteDraw(TextureAssets.Chain38.Value, new Vector2(vector8.X - Main.screenPosition.X, vector8.Y - Main.screenPosition.Y), new Rectangle?(new Rectangle(0, 0, TextureAssets.Chain38.Width(), TextureAssets.Chain38.Height())), color12, rotation5, new Vector2((float)TextureAssets.Chain38.Width() * 0.5f, (float)TextureAssets.Chain38.Height() * 0.5f), 0.8f, 0, 0f);
                        }
                        else
                        {
                            Main.EntitySpriteDraw(TextureAssets.Chain39.Value, new Vector2(vector8.X - Main.screenPosition.X, vector8.Y - Main.screenPosition.Y), new Rectangle?(new Rectangle(0, 0, TextureAssets.Chain39.Width(), TextureAssets.Chain39.Height())), color12, rotation5, new Vector2((float)TextureAssets.Chain39.Width() * 0.5f, (float)TextureAssets.Chain39.Height() * 0.5f), 0.8f, 0, 0f);
                        }
                        num57++;
                    }
                    return;
                }
                if (proj.type == 505 || proj.type == 506)
                {
                    Vector2 vector9 = new Vector2(proj.position.X + (float)proj.width * 0.5f, proj.position.Y + (float)proj.height * 0.5f);
                    float num59 = 0f - proj.velocity.X;
                    float num60 = 0f - proj.velocity.Y;
                    float num61 = 1f;
                    if (proj.ai[0] <= 17f)
                    {
                        num61 = proj.ai[0] / 17f;
                    }
                    int num62 = (int)(30f * num61);
                    float num63 = 1f;
                    if (proj.ai[0] <= 30f)
                    {
                        num63 = proj.ai[0] / 30f;
                    }
                    float num64 = 0.4f * num63;
                    float num65 = num64;
                    num60 += num65;
                    Vector2[] array5 = new Vector2[num62];
                    float[] array6 = new float[num62];
                    for (int k = 0; k < num62; k++)
                    {
                        float num66 = (float)Math.Sqrt((double)(num59 * num59 + num60 * num60));
                        float num67 = 5.6f;
                        if (Math.Abs(num59) + Math.Abs(num60) < 1f)
                        {
                            num67 *= Math.Abs(num59) + Math.Abs(num60) / 1f;
                        }
                        num66 = num67 / num66;
                        num59 *= num66;
                        num60 *= num66;
                        float num68 = (float)Math.Atan2((double)num60, (double)num59) - 1.57f;
                        array5[k].X = vector9.X;
                        array5[k].Y = vector9.Y;
                        array6[k] = num68;
                        vector9.X += num59;
                        vector9.Y += num60;
                        num59 = 0f - proj.velocity.X;
                        num60 = 0f - proj.velocity.Y;
                        num65 += num64;
                        num60 += num65;
                    }
                    int num69 = 0;
                    for (int num70 = num62 - 1; num70 >= 0; num70--)
                    {
                        vector9.X = array5[num70].X;
                        vector9.Y = array5[num70].Y;
                        float rotation6 = array6[num70];
                        Color color13 = Lighting.GetColor((int)vector9.X / 16, (int)(vector9.Y / 16f));
                        int num71 = 4;
                        if (proj.type == 506)
                        {
                            num71 = 6;
                        }
                        num71 += num69 % 2;
                        Main.EntitySpriteDraw(TextureAssets.Chains[num71].Value, new Vector2(vector9.X - Main.screenPosition.X, vector9.Y - Main.screenPosition.Y), new Rectangle?(new Rectangle(0, 0, TextureAssets.Chains[num71].Width(), TextureAssets.Chains[num71].Height())), color13, rotation6, new Vector2((float)TextureAssets.Chains[num71].Width() * 0.5f, (float)TextureAssets.Chains[num71].Height() * 0.5f), 0.8f, 0, 0f);
                        num69++;
                    }
                    return;
                }
                if (proj.type == 165)
                {
                    Vector2 vector10 = new Vector2(proj.position.X + (float)proj.width * 0.5f, proj.position.Y + (float)proj.height * 0.5f);
                    float num72 = mountedCenter.X - vector10.X;
                    float num73 = mountedCenter.Y - vector10.Y;
                    float rotation7 = (float)Math.Atan2((double)num73, (double)num72) - 1.57f;
                    bool flag8 = true;
                    while (flag8)
                    {
                        float num74 = (float)Math.Sqrt((double)(num72 * num72 + num73 * num73));
                        if (num74 < 25f)
                        {
                            flag8 = false;
                        }
                        else if (float.IsNaN(num74))
                        {
                            flag8 = false;
                        }
                        else
                        {
                            num74 = 24f / num74;
                            num72 *= num74;
                            num73 *= num74;
                            vector10.X += num72;
                            vector10.Y += num73;
                            num72 = mountedCenter.X - vector10.X;
                            num73 = mountedCenter.Y - vector10.Y;
                            Color color14 = Lighting.GetColor((int)vector10.X / 16, (int)(vector10.Y / 16f));
                            Main.EntitySpriteDraw(TextureAssets.Chain15.Value, new Vector2(vector10.X - Main.screenPosition.X, vector10.Y - Main.screenPosition.Y), new Rectangle?(new Rectangle(0, 0, TextureAssets.Chain15.Width(), TextureAssets.Chain15.Height())), color14, rotation7, new Vector2((float)TextureAssets.Chain15.Width() * 0.5f, (float)TextureAssets.Chain15.Height() * 0.5f), 1f, 0, 0f);
                        }
                    }
                    return;
                }
                if (proj.type >= 230 && proj.type <= 235)
                {
                    int num75 = proj.type - 229;
                    Vector2 vector11 = new Vector2(proj.position.X + (float)proj.width * 0.5f, proj.position.Y + (float)proj.height * 0.5f);
                    float num76 = mountedCenter.X - vector11.X;
                    float num77 = mountedCenter.Y - vector11.Y;
                    float rotation8 = (float)Math.Atan2((double)num77, (double)num76) - 1.57f;
                    bool flag9 = true;
                    while (flag9)
                    {
                        float num78 = (float)Math.Sqrt((double)(num76 * num76 + num77 * num77));
                        if (num78 < 25f)
                        {
                            flag9 = false;
                        }
                        else if (float.IsNaN(num78))
                        {
                            flag9 = false;
                        }
                        else
                        {
                            num78 = (float)TextureAssets.GemChain[num75].Height() / num78;
                            num76 *= num78;
                            num77 *= num78;
                            vector11.X += num76;
                            vector11.Y += num77;
                            num76 = mountedCenter.X - vector11.X;
                            num77 = mountedCenter.Y - vector11.Y;
                            Color color15 = Lighting.GetColor((int)vector11.X / 16, (int)(vector11.Y / 16f));
                            Main.EntitySpriteDraw(TextureAssets.GemChain[num75].Value, new Vector2(vector11.X - Main.screenPosition.X, vector11.Y - Main.screenPosition.Y), new Rectangle?(new Rectangle(0, 0, TextureAssets.GemChain[num75].Width(), TextureAssets.GemChain[num75].Height())), color15, rotation8, new Vector2((float)TextureAssets.GemChain[num75].Width() * 0.5f, (float)TextureAssets.GemChain[num75].Height() * 0.5f), 1f, 0, 0f);
                        }
                    }
                    return;
                }
                if (proj.type == 753)
                {
                    Vector2 vector12 = new Vector2(proj.position.X + (float)proj.width * 0.5f, proj.position.Y + (float)proj.height * 0.5f);
                    float num79 = mountedCenter.X - vector12.X;
                    float num80 = mountedCenter.Y - vector12.Y;
                    float rotation9 = (float)Math.Atan2((double)num80, (double)num79) - 1.57f;
                    bool flag10 = true;
                    Texture2D value7 = TextureAssets.Extra[95].Value;
                    while (flag10)
                    {
                        float num81 = (float)Math.Sqrt((double)(num79 * num79 + num80 * num80));
                        if (num81 < 25f)
                        {
                            flag10 = false;
                        }
                        else if (float.IsNaN(num81))
                        {
                            flag10 = false;
                        }
                        else
                        {
                            num81 = (float)value7.Height / num81;
                            num79 *= num81;
                            num80 *= num81;
                            vector12.X += num79;
                            vector12.Y += num80;
                            num79 = mountedCenter.X - vector12.X;
                            num80 = mountedCenter.Y - vector12.Y;
                            Color color16 = Lighting.GetColor((int)vector12.X / 16, (int)(vector12.Y / 16f));
                            Main.EntitySpriteDraw(value7, new Vector2(vector12.X - Main.screenPosition.X, vector12.Y - Main.screenPosition.Y), null, color16, rotation9, value7.Size() / 2f, 1f, 0, 0f);
                        }
                    }
                    return;
                }
                if (proj.type == 865)
                {
                    Vector2 vector13 = new Vector2(proj.position.X + (float)proj.width * 0.5f, proj.position.Y + (float)proj.height * 0.5f);
                    float num82 = mountedCenter.X - vector13.X;
                    float num83 = mountedCenter.Y - vector13.Y;
                    float rotation10 = (float)Math.Atan2((double)num83, (double)num82) - 1.57f;
                    bool flag11 = true;
                    bool flag12 = true;
                    Texture2D value8 = TextureAssets.Extra[154].Value;
                    while (flag11)
                    {
                        float num84 = (float)Math.Sqrt((double)(num82 * num82 + num83 * num83));
                        if (num84 < 25f)
                        {
                            flag11 = false;
                        }
                        else if (float.IsNaN(num84))
                        {
                            flag11 = false;
                        }
                        else
                        {
                            num84 = (float)value8.Height / num84;
                            num82 *= num84;
                            num83 *= num84;
                            vector13.X += num82;
                            vector13.Y += num83;
                            num82 = mountedCenter.X - vector13.X;
                            num83 = mountedCenter.Y - vector13.Y;
                            if (!flag12)
                            {
                                Color color17 = Lighting.GetColor((int)vector13.X / 16, (int)(vector13.Y / 16f));
                                Main.EntitySpriteDraw(value8, new Vector2(vector13.X - Main.screenPosition.X, vector13.Y - Main.screenPosition.Y), null, color17, rotation10, value8.Size() / 2f, 1f, 0, 0f);
                            }
                            flag12 = false;
                        }
                    }
                    return;
                }
                if (proj.type == 935)
                {
                    Vector2 vector14 = new Vector2(proj.position.X + (float)proj.width * 0.5f, proj.position.Y + (float)proj.height * 0.5f);
                    float num85 = mountedCenter.X - vector14.X;
                    float num86 = mountedCenter.Y - vector14.Y;
                    float rotation11 = (float)Math.Atan2((double)num86, (double)num85) - 1.57f;
                    bool flag13 = true;
                    bool flag14 = true;
                    Texture2D value9 = TextureAssets.Extra[208].Value;
                    while (flag13)
                    {
                        float num87 = (float)Math.Sqrt((double)(num85 * num85 + num86 * num86));
                        if (num87 < 8f)
                        {
                            flag13 = false;
                        }
                        else if (float.IsNaN(num87))
                        {
                            flag13 = false;
                        }
                        else
                        {
                            num87 = (float)value9.Height / num87;
                            num85 *= num87;
                            num86 *= num87;
                            vector14.X += num85;
                            vector14.Y += num86;
                            num85 = mountedCenter.X - vector14.X;
                            num86 = mountedCenter.Y - vector14.Y;
                            if (!flag14)
                            {
                                Color color18 = Lighting.GetColor((int)vector14.X / 16, (int)(vector14.Y / 16f));
                                Main.EntitySpriteDraw(value9, new Vector2(vector14.X - Main.screenPosition.X, vector14.Y - Main.screenPosition.Y), null, color18, rotation11, value9.Size() / 2f, 1f, 0, 0f);
                            }
                            flag14 = false;
                        }
                    }
                    return;
                }
                if (proj.type == 256)
                {
                    Vector2 vector15 = new Vector2(proj.position.X + (float)proj.width * 0.5f, proj.position.Y + (float)proj.height * 0.5f);
                    float num88 = mountedCenter.X - vector15.X;
                    float num89 = mountedCenter.Y - vector15.Y;
                    float num90 = (float)Math.Atan2((double)num89, (double)num88) - 1.57f;
                    bool flag15 = true;
                    while (flag15)
                    {
                        float num91 = (float)Math.Sqrt((double)(num88 * num88 + num89 * num89));
                        if (num91 < 26f)
                        {
                            flag15 = false;
                        }
                        else if (float.IsNaN(num91))
                        {
                            flag15 = false;
                        }
                        else
                        {
                            num91 = 26f / num91;
                            num88 *= num91;
                            num89 *= num91;
                            vector15.X += num88;
                            vector15.Y += num89;
                            num88 = Main.player[proj.owner].position.X + (float)(Main.player[proj.owner].width / 2) - vector15.X;
                            num89 = Main.player[proj.owner].position.Y + (float)(Main.player[proj.owner].height / 2) - vector15.Y;
                            Color color19 = Lighting.GetColor((int)vector15.X / 16, (int)(vector15.Y / 16f));
                            Main.EntitySpriteDraw(TextureAssets.Chain20.Value, new Vector2(vector15.X - Main.screenPosition.X, vector15.Y - Main.screenPosition.Y), new Rectangle?(new Rectangle(0, 0, TextureAssets.Chain20.Width(), TextureAssets.Chain20.Height())), color19, num90 - 0.785f, new Vector2((float)TextureAssets.Chain20.Width() * 0.5f, (float)TextureAssets.Chain20.Height() * 0.5f), 1f, 0, 0f);
                        }
                    }
                    return;
                }
                if (proj.type == 322)
                {
                    Vector2 vector16 = new Vector2(proj.position.X + (float)proj.width * 0.5f, proj.position.Y + (float)proj.height * 0.5f);
                    float num92 = mountedCenter.X - vector16.X;
                    float num93 = mountedCenter.Y - vector16.Y;
                    float rotation12 = (float)Math.Atan2((double)num93, (double)num92) - 1.57f;
                    bool flag16 = true;
                    while (flag16)
                    {
                        float num94 = (float)Math.Sqrt((double)(num92 * num92 + num93 * num93));
                        if (num94 < 22f)
                        {
                            flag16 = false;
                        }
                        else if (float.IsNaN(num94))
                        {
                            flag16 = false;
                        }
                        else
                        {
                            num94 = 22f / num94;
                            num92 *= num94;
                            num93 *= num94;
                            vector16.X += num92;
                            vector16.Y += num93;
                            num92 = mountedCenter.X - vector16.X;
                            num93 = mountedCenter.Y - vector16.Y;
                            Color color20 = Lighting.GetColor((int)vector16.X / 16, (int)(vector16.Y / 16f));
                            Main.EntitySpriteDraw(TextureAssets.Chain29.Value, new Vector2(vector16.X - Main.screenPosition.X, vector16.Y - Main.screenPosition.Y), new Rectangle?(new Rectangle(0, 0, TextureAssets.Chain29.Width(), TextureAssets.Chain29.Height())), color20, rotation12, new Vector2((float)TextureAssets.Chain29.Width() * 0.5f, (float)TextureAssets.Chain29.Height() * 0.5f), 1f, 0, 0f);
                        }
                    }
                    return;
                }
                if (proj.type == 315)
                {
                    Vector2 vector17 = new Vector2(proj.position.X + (float)proj.width * 0.5f, proj.position.Y + (float)proj.height * 0.5f);
                    float num95 = mountedCenter.X - vector17.X;
                    float num96 = mountedCenter.Y - vector17.Y;
                    float rotation13 = (float)Math.Atan2((double)num96, (double)num95) - 1.57f;
                    bool flag17 = true;
                    while (flag17)
                    {
                        float num97 = (float)Math.Sqrt((double)(num95 * num95 + num96 * num96));
                        if (num97 < 50f)
                        {
                            flag17 = false;
                        }
                        else if (float.IsNaN(num97))
                        {
                            flag17 = false;
                        }
                        else
                        {
                            num97 = 40f / num97;
                            num95 *= num97;
                            num96 *= num97;
                            vector17.X += num95;
                            vector17.Y += num96;
                            num95 = mountedCenter.X - vector17.X;
                            num96 = mountedCenter.Y - vector17.Y;
                            Color color21 = Lighting.GetColor((int)vector17.X / 16, (int)(vector17.Y / 16f));
                            Main.EntitySpriteDraw(TextureAssets.Chain28.Value, new Vector2(vector17.X - Main.screenPosition.X, vector17.Y - Main.screenPosition.Y), new Rectangle?(new Rectangle(0, 0, TextureAssets.Chain28.Width(), TextureAssets.Chain28.Height())), color21, rotation13, new Vector2((float)TextureAssets.Chain28.Width() * 0.5f, (float)TextureAssets.Chain28.Height() * 0.5f), 1f, 0, 0f);
                        }
                    }
                    return;
                }
                if (proj.type == 331)
                {
                    Vector2 vector18 = new Vector2(proj.position.X + (float)proj.width * 0.5f, proj.position.Y + (float)proj.height * 0.5f);
                    float num98 = mountedCenter.X - vector18.X;
                    float num99 = mountedCenter.Y - vector18.Y;
                    float rotation14 = (float)Math.Atan2((double)num99, (double)num98) - 1.57f;
                    bool flag18 = true;
                    while (flag18)
                    {
                        float num100 = (float)Math.Sqrt((double)(num98 * num98 + num99 * num99));
                        if (num100 < 30f)
                        {
                            flag18 = false;
                        }
                        else if (float.IsNaN(num100))
                        {
                            flag18 = false;
                        }
                        else
                        {
                            num100 = 24f / num100;
                            num98 *= num100;
                            num99 *= num100;
                            vector18.X += num98;
                            vector18.Y += num99;
                            num98 = mountedCenter.X - vector18.X;
                            num99 = mountedCenter.Y - vector18.Y;
                            Color color22 = Lighting.GetColor((int)vector18.X / 16, (int)(vector18.Y / 16f));
                            Main.EntitySpriteDraw(TextureAssets.Chain30.Value, new Vector2(vector18.X - Main.screenPosition.X, vector18.Y - Main.screenPosition.Y), new Rectangle?(new Rectangle(0, 0, TextureAssets.Chain30.Width(), TextureAssets.Chain30.Height())), color22, rotation14, new Vector2((float)TextureAssets.Chain30.Width() * 0.5f, (float)TextureAssets.Chain30.Height() * 0.5f), 1f, 0, 0f);
                        }
                    }
                    return;
                }
                if (proj.type == 332)
                {
                    int num101 = 0;
                    Vector2 vector19 = new Vector2(proj.position.X + (float)proj.width * 0.5f, proj.position.Y + (float)proj.height * 0.5f);
                    float num102 = mountedCenter.X - vector19.X;
                    float num103 = mountedCenter.Y - vector19.Y;
                    float rotation15 = (float)Math.Atan2((double)num103, (double)num102) - 1.57f;
                    bool flag19 = true;
                    while (flag19)
                    {
                        float num104 = (float)Math.Sqrt((double)(num102 * num102 + num103 * num103));
                        if (num104 < 30f)
                        {
                            flag19 = false;
                        }
                        else if (float.IsNaN(num104))
                        {
                            flag19 = false;
                        }
                        else
                        {
                            int i2 = (int)vector19.X / 16;
                            int j2 = (int)vector19.Y / 16;
                            if (num101 == 0)
                            {
                                Lighting.AddLight(i2, j2, 0f, 0.2f, 0.2f);
                            }
                            if (num101 == 1)
                            {
                                Lighting.AddLight(i2, j2, 0.1f, 0.2f, 0f);
                            }
                            if (num101 == 2)
                            {
                                Lighting.AddLight(i2, j2, 0.2f, 0.1f, 0f);
                            }
                            if (num101 == 3)
                            {
                                Lighting.AddLight(i2, j2, 0.2f, 0f, 0.2f);
                            }
                            num104 = 16f / num104;
                            num102 *= num104;
                            num103 *= num104;
                            vector19.X += num102;
                            vector19.Y += num103;
                            num102 = mountedCenter.X - vector19.X;
                            num103 = mountedCenter.Y - vector19.Y;
                            Color color23 = Lighting.GetColor((int)vector19.X / 16, (int)(vector19.Y / 16f));
                            Main.EntitySpriteDraw(TextureAssets.Chain31.Value, new Vector2(vector19.X - Main.screenPosition.X, vector19.Y - Main.screenPosition.Y), new Rectangle?(new Rectangle(0, TextureAssets.Chain31.Height() / 4 * num101, TextureAssets.Chain31.Width(), TextureAssets.Chain31.Height() / 4)), color23, rotation15, new Vector2((float)TextureAssets.Chain30.Width() * 0.5f, (float)(TextureAssets.Chain30.Height() / 8)), 1f, 0, 0f);
                            Main.EntitySpriteDraw(TextureAssets.Chain32.Value, new Vector2(vector19.X - Main.screenPosition.X, vector19.Y - Main.screenPosition.Y), new Rectangle?(new Rectangle(0, TextureAssets.Chain31.Height() / 4 * num101, TextureAssets.Chain31.Width(), TextureAssets.Chain31.Height() / 4)), new Color(200, 200, 200, 0), rotation15, new Vector2((float)TextureAssets.Chain30.Width() * 0.5f, (float)(TextureAssets.Chain30.Height() / 8)), 1f, 0, 0f);
                            num101++;
                            if (num101 > 3)
                            {
                                num101 = 0;
                            }
                        }
                    }
                    return;
                }
                if (proj.type == 372 || proj.type == 383 || proj.type == 396 || proj.type == 403 || proj.type == 404 || proj.type == 446 || (proj.type >= 486 && proj.type <= 489) || (proj.type >= 646 && proj.type <= 649) || proj.type == 652)
                {
                    Texture2D texture2D = null;
                    Color color24 = Color.Transparent;
                    Texture2D value10 = TextureAssets.Chain33.Value;
                    if (proj.type == 383)
                    {
                        value10 = TextureAssets.Chain34.Value;
                    }
                    if (proj.type == 396)
                    {
                        value10 = TextureAssets.Chain35.Value;
                    }
                    if (proj.type == 403)
                    {
                        value10 = TextureAssets.Chain36.Value;
                    }
                    if (proj.type == 404)
                    {
                        value10 = TextureAssets.Chain37.Value;
                    }
                    if (proj.type == 446)
                    {
                        value10 = TextureAssets.Extra[3].Value;
                    }
                    if (proj.type >= 486 && proj.type <= 489)
                    {
                        value10 = TextureAssets.Chains[proj.type - 486].Value;
                    }
                    if (proj.type >= 646 && proj.type <= 649)
                    {
                        value10 = TextureAssets.Chains[proj.type - 646 + 8].Value;
                        texture2D = TextureAssets.Chains[proj.type - 646 + 12].Value;
                        color24 = new Color(255, 255, 255, 127);
                    }
                    if (proj.type == 652)
                    {
                        value10 = TextureAssets.Chains[16].Value;
                    }
                    Vector2 center = proj.Center;
                    Rectangle? sourceRectangle = null;
                    Vector2 origin4 = new Vector2((float)value10.Width * 0.5f, (float)value10.Height * 0.5f);
                    float num105 = (float)value10.Height;
                    float num106 = 0f;
                    if (proj.type == 446)
                    {
                        int num107 = 7;
                        int num108 = (int)proj.localAI[0] / num107;
                        sourceRectangle = new Rectangle?(new Rectangle(0, value10.Height / 4 * num108, value10.Width, value10.Height / 4));
                        origin4.Y /= 4f;
                        num105 /= 4f;
                    }
                    int type = proj.type;
                    if (type <= 446)
                    {
                        if (type != 383)
                        {
                            if (type == 446)
                            {
                                num106 = 20f;
                            }
                        }
                        else
                        {
                            num106 = 14f;
                        }
                    }
                    else if (type != 487)
                    {
                        if (type == 489)
                        {
                            num106 = 10f;
                        }
                    }
                    else
                    {
                        num106 = 8f;
                    }
                    if (num106 != 0f)
                    {
                        float num109 = -1.57f;
                        Vector2 vector20 = new Vector2((float)Math.Cos((double)(proj.rotation + num109)), (float)Math.Sin((double)(proj.rotation + num109)));
                        center -= vector20 * num106;
                        vector20 = mountedCenter - center;
                        vector20.Normalize();
                        center -= vector20 * num105 / 2f;
                    }
                    Vector2 vector21 = mountedCenter - center;
                    float rotation16 = (float)Math.Atan2((double)vector21.Y, (double)vector21.X) - 1.57f;
                    bool flag20 = true;
                    if (float.IsNaN(center.X) && float.IsNaN(center.Y))
                    {
                        flag20 = false;
                    }
                    if (float.IsNaN(vector21.X) && float.IsNaN(vector21.Y))
                    {
                        flag20 = false;
                    }
                    while (flag20)
                    {
                        if (vector21.Length() < num105 + 1f)
                        {
                            flag20 = false;
                        }
                        else
                        {
                            Vector2 vector22 = vector21;
                            vector22.Normalize();
                            center += vector22 * num105;
                            vector21 = mountedCenter - center;
                            Color color25 = Lighting.GetColor((int)center.X / 16, (int)(center.Y / 16f));
                            if (proj.type == 396)
                            {
                                color25 *= (float)(255 - proj.alpha) / 255f;
                            }
                            if (proj.type == 446)
                            {
                                color25 = proj.GetAlpha(color25);
                            }
                            if (proj.type == 488)
                            {
                                Lighting.AddLight(center, 0.2f, 0f, 0.175f);
                                color25 = new Color(255, 255, 255, 255);
                            }
                            if (proj.type >= 646 && proj.type <= 649)
                            {
                                color25 = proj.GetAlpha(color25);
                            }
                            Main.EntitySpriteDraw(value10, center - Main.screenPosition, sourceRectangle, color25, rotation16, origin4, 1f, 0, 0f);
                            if (texture2D != null)
                            {
                                Main.EntitySpriteDraw(texture2D, center - Main.screenPosition, sourceRectangle, color24, rotation16, origin4, 1f, 0, 0f);
                            }
                        }
                    }
                    return;
                }
                if (proj.aiStyle == 7)
                {
                    Vector2 vector23 = new Vector2(proj.position.X + (float)proj.width * 0.5f, proj.position.Y + (float)proj.height * 0.5f);
                    float num110 = mountedCenter.X - vector23.X;
                    float num111 = mountedCenter.Y - vector23.Y;
                    float rotation17 = (float)Math.Atan2((double)num111, (double)num110) - 1.57f;
                    bool flag21 = true;
                    while (flag21)
                    {
                        float num112 = (float)Math.Sqrt((double)(num110 * num110 + num111 * num111));
                        if (num112 < 25f)
                        {
                            flag21 = false;
                        }
                        else if (float.IsNaN(num112))
                        {
                            flag21 = false;
                        }
                        else
                        {
                            num112 = 12f / num112;
                            num110 *= num112;
                            num111 *= num112;
                            vector23.X += num110;
                            vector23.Y += num111;
                            num110 = mountedCenter.X - vector23.X;
                            num111 = mountedCenter.Y - vector23.Y;
                            Color color26 = Lighting.GetColor((int)vector23.X / 16, (int)(vector23.Y / 16f));
                            Main.EntitySpriteDraw(TextureAssets.Chain.Value, new Vector2(vector23.X - Main.screenPosition.X, vector23.Y - Main.screenPosition.Y), new Rectangle?(new Rectangle(0, 0, TextureAssets.Chain.Width(), TextureAssets.Chain.Height())), color26, rotation17, new Vector2((float)TextureAssets.Chain.Width() * 0.5f, (float)TextureAssets.Chain.Height() * 0.5f), 1f, 0, 0f);
                        }
                    }
                    return;
                }
                if (proj.type == 262)
                {
                    float x = proj.Center.X;
                    float y = proj.Center.Y;
                    float x2 = proj.velocity.X;
                    float y2 = proj.velocity.Y;
                    float num113 = (float)Math.Sqrt((double)(x2 * x2 + y2 * y2));
                    num113 = 4f / num113;
                    if (proj.ai[0] == 0f)
                    {
                        x -= proj.velocity.X * num113;
                        y -= proj.velocity.Y * num113;
                    }
                    else
                    {
                        x += proj.velocity.X * num113;
                        y += proj.velocity.Y * num113;
                    }
                    Vector2 vector24 = new Vector2(x, y);
                    x2 = mountedCenter.X - vector24.X;
                    y2 = mountedCenter.Y - vector24.Y;
                    float rotation18 = (float)Math.Atan2((double)y2, (double)x2) - 1.57f;
                    if (proj.alpha == 0)
                    {
                        int num114 = -1;
                        if (proj.position.X + (float)(proj.width / 2) < mountedCenter.X)
                        {
                            num114 = 1;
                        }
                        if (Main.player[proj.owner].direction == 1)
                        {
                            Main.player[proj.owner].itemRotation = (float)Math.Atan2((double)(y2 * (float)num114), (double)(x2 * (float)num114));
                        }
                        else
                        {
                            Main.player[proj.owner].itemRotation = (float)Math.Atan2((double)(y2 * (float)num114), (double)(x2 * (float)num114));
                        }
                    }
                    bool flag22 = true;
                    while (flag22)
                    {
                        float num115 = (float)Math.Sqrt((double)(x2 * x2 + y2 * y2));
                        if (num115 < 25f)
                        {
                            flag22 = false;
                        }
                        else if (float.IsNaN(num115))
                        {
                            flag22 = false;
                        }
                        else
                        {
                            num115 = 12f / num115;
                            x2 *= num115;
                            y2 *= num115;
                            vector24.X += x2;
                            vector24.Y += y2;
                            x2 = mountedCenter.X - vector24.X;
                            y2 = mountedCenter.Y - vector24.Y;
                            Color color27 = Lighting.GetColor((int)vector24.X / 16, (int)(vector24.Y / 16f));
                            Main.EntitySpriteDraw(TextureAssets.Chain22.Value, new Vector2(vector24.X - Main.screenPosition.X, vector24.Y - Main.screenPosition.Y), new Rectangle?(new Rectangle(0, 0, TextureAssets.Chain22.Width(), TextureAssets.Chain22.Height())), color27, rotation18, new Vector2((float)TextureAssets.Chain22.Width() * 0.5f, (float)TextureAssets.Chain22.Height() * 0.5f), 1f, 0, 0f);
                        }
                    }
                    return;
                }
                if (proj.type == 273)
                {
                    float x3 = proj.Center.X;
                    float y3 = proj.Center.Y;
                    float x4 = proj.velocity.X;
                    float y4 = proj.velocity.Y;
                    float num116 = (float)Math.Sqrt((double)(x4 * x4 + y4 * y4));
                    num116 = 4f / num116;
                    if (proj.ai[0] == 0f)
                    {
                        x3 -= proj.velocity.X * num116;
                        y3 -= proj.velocity.Y * num116;
                    }
                    else
                    {
                        x3 += proj.velocity.X * num116;
                        y3 += proj.velocity.Y * num116;
                    }
                    Vector2 vector25 = new Vector2(x3, y3);
                    x4 = mountedCenter.X - vector25.X;
                    y4 = mountedCenter.Y - vector25.Y;
                    float rotation19 = (float)Math.Atan2((double)y4, (double)x4) - 1.57f;
                    if (proj.alpha == 0)
                    {
                        int num117 = -1;
                        if (proj.position.X + (float)(proj.width / 2) < mountedCenter.X)
                        {
                            num117 = 1;
                        }
                        if (Main.player[proj.owner].direction == 1)
                        {
                            Main.player[proj.owner].itemRotation = (float)Math.Atan2((double)(y4 * (float)num117), (double)(x4 * (float)num117));
                        }
                        else
                        {
                            Main.player[proj.owner].itemRotation = (float)Math.Atan2((double)(y4 * (float)num117), (double)(x4 * (float)num117));
                        }
                    }
                    bool flag23 = true;
                    while (flag23)
                    {
                        float num118 = (float)Math.Sqrt((double)(x4 * x4 + y4 * y4));
                        if (num118 < 25f)
                        {
                            flag23 = false;
                        }
                        else if (float.IsNaN(num118))
                        {
                            flag23 = false;
                        }
                        else
                        {
                            num118 = 12f / num118;
                            x4 *= num118;
                            y4 *= num118;
                            vector25.X += x4;
                            vector25.Y += y4;
                            x4 = mountedCenter.X - vector25.X;
                            y4 = mountedCenter.Y - vector25.Y;
                            Color color28 = Lighting.GetColor((int)vector25.X / 16, (int)(vector25.Y / 16f));
                            Main.EntitySpriteDraw(TextureAssets.Chain23.Value, new Vector2(vector25.X - Main.screenPosition.X, vector25.Y - Main.screenPosition.Y), new Rectangle?(new Rectangle(0, 0, TextureAssets.Chain23.Width(), TextureAssets.Chain23.Height())), color28, rotation19, new Vector2((float)TextureAssets.Chain23.Width() * 0.5f, (float)TextureAssets.Chain23.Height() * 0.5f), 1f, 0, 0f);
                        }
                    }
                    return;
                }
                if (proj.type == 481)
                {
                    float x5 = proj.Center.X;
                    float y5 = proj.Center.Y;
                    float x6 = proj.velocity.X;
                    float y6 = proj.velocity.Y;
                    float num119 = (float)Math.Sqrt((double)(x6 * x6 + y6 * y6));
                    num119 = 4f / num119;
                    if (proj.ai[0] == 0f)
                    {
                        x5 -= proj.velocity.X * num119;
                        y5 -= proj.velocity.Y * num119;
                    }
                    else
                    {
                        x5 += proj.velocity.X * num119;
                        y5 += proj.velocity.Y * num119;
                    }
                    Vector2 origin5 = new Vector2(x5, y5);
                    x6 = mountedCenter.X - origin5.X;
                    y6 = mountedCenter.Y - origin5.Y;
                    float rotation20 = (float)Math.Atan2((double)y6, (double)x6) - 1.57f;
                    if (proj.alpha == 0)
                    {
                        int num120 = -1;
                        if (proj.position.X + (float)(proj.width / 2) < mountedCenter.X)
                        {
                            num120 = 1;
                        }
                        if (Main.player[proj.owner].direction == 1)
                        {
                            Main.player[proj.owner].itemRotation = (float)Math.Atan2((double)(y6 * (float)num120), (double)(x6 * (float)num120));
                        }
                        else
                        {
                            Main.player[proj.owner].itemRotation = (float)Math.Atan2((double)(y6 * (float)num120), (double)(x6 * (float)num120));
                        }
                    }
                    bool flag24 = true;
                    while (flag24)
                    {
                        float num121 = 0.85f;
                        float num122 = origin5.Distance(mountedCenter);
                        float num123 = num122;
                        if ((double)num122 < (double)TextureAssets.Chain40.Height() * 1.5)
                        {
                            flag24 = false;
                        }
                        else if (float.IsNaN(num122))
                        {
                            flag24 = false;
                        }
                        else
                        {
                            num122 = (float)TextureAssets.Chain40.Height() * num121 / num122;
                            x6 *= num122;
                            y6 *= num122;
                            origin5.X += x6;
                            origin5.Y += y6;
                            x6 = mountedCenter.X - origin5.X;
                            y6 = mountedCenter.Y - origin5.Y;
                            if (num123 > (float)(TextureAssets.Chain40.Height() * 2))
                            {
                                for (int l = 0; l < 2; l++)
                                {
                                    float num124 = 0.75f;
                                    float num125 = (l != 0) ? Math.Abs(Main.player[proj.owner].velocity.Y) : Math.Abs(Main.player[proj.owner].velocity.X);
                                    if (num125 > 10f)
                                    {
                                        num125 = 10f;
                                    }
                                    num125 /= 10f;
                                    num124 *= num125;
                                    num125 = num123 / 80f;
                                    if (num125 > 1f)
                                    {
                                        num125 = 1f;
                                    }
                                    num124 *= num125;
                                    if (num124 < 0f)
                                    {
                                        num124 = 0f;
                                    }
                                    if (!float.IsNaN(num124))
                                    {
                                        if (l == 0)
                                        {
                                            if (Main.player[proj.owner].velocity.X < 0f && proj.Center.X < mountedCenter.X)
                                            {
                                                y6 *= 1f - num124;
                                            }
                                            if (Main.player[proj.owner].velocity.X > 0f && proj.Center.X > mountedCenter.X)
                                            {
                                                y6 *= 1f - num124;
                                            }
                                        }
                                        else
                                        {
                                            if (Main.player[proj.owner].velocity.Y < 0f && proj.Center.Y < mountedCenter.Y)
                                            {
                                                x6 *= 1f - num124;
                                            }
                                            if (Main.player[proj.owner].velocity.Y > 0f && proj.Center.Y > mountedCenter.Y)
                                            {
                                                x6 *= 1f - num124;
                                            }
                                        }
                                    }
                                }
                            }
                            Color color29 = Lighting.GetColor((int)origin5.X / 16, (int)(origin5.Y / 16f));
                            Main.EntitySpriteDraw(TextureAssets.Chain40.Value, new Vector2(origin5.X - Main.screenPosition.X, origin5.Y - Main.screenPosition.Y), new Rectangle?(new Rectangle(0, 0, TextureAssets.Chain40.Width(), TextureAssets.Chain40.Height())), color29, rotation20, new Vector2((float)TextureAssets.Chain40.Width() * 0.5f, (float)TextureAssets.Chain40.Height() * 0.5f), num121, 0, 0f);
                        }
                    }
                    return;
                }
                if (proj.type == 271)
                {
                    float x7 = proj.Center.X;
                    float y7 = proj.Center.Y;
                    float x8 = proj.velocity.X;
                    float y8 = proj.velocity.Y;
                    float num126 = (float)Math.Sqrt((double)(x8 * x8 + y8 * y8));
                    num126 = 4f / num126;
                    if (proj.ai[0] == 0f)
                    {
                        x7 -= proj.velocity.X * num126;
                        y7 -= proj.velocity.Y * num126;
                    }
                    else
                    {
                        x7 += proj.velocity.X * num126;
                        y7 += proj.velocity.Y * num126;
                    }
                    Vector2 vector26 = new Vector2(x7, y7);
                    x8 = mountedCenter.X - vector26.X;
                    y8 = mountedCenter.Y - vector26.Y;
                    float rotation21 = (float)Math.Atan2((double)y8, (double)x8) - 1.57f;
                    if (proj.alpha == 0)
                    {
                        int num127 = -1;
                        if (proj.position.X + (float)(proj.width / 2) < mountedCenter.X)
                        {
                            num127 = 1;
                        }
                        if (Main.player[proj.owner].direction == 1)
                        {
                            Main.player[proj.owner].itemRotation = (float)Math.Atan2((double)(y8 * (float)num127), (double)(x8 * (float)num127));
                        }
                        else
                        {
                            Main.player[proj.owner].itemRotation = (float)Math.Atan2((double)(y8 * (float)num127), (double)(x8 * (float)num127));
                        }
                    }
                    bool flag25 = true;
                    while (flag25)
                    {
                        float num128 = (float)Math.Sqrt((double)(x8 * x8 + y8 * y8));
                        if (num128 < 25f)
                        {
                            flag25 = false;
                        }
                        else if (float.IsNaN(num128))
                        {
                            flag25 = false;
                        }
                        else
                        {
                            num128 = 12f / num128;
                            x8 *= num128;
                            y8 *= num128;
                            vector26.X += x8;
                            vector26.Y += y8;
                            x8 = mountedCenter.X - vector26.X;
                            y8 = mountedCenter.Y - vector26.Y;
                            Color color30 = Lighting.GetColor((int)vector26.X / 16, (int)(vector26.Y / 16f));
                            Main.EntitySpriteDraw(TextureAssets.Chain18.Value, new Vector2(vector26.X - Main.screenPosition.X, vector26.Y - Main.screenPosition.Y), new Rectangle?(new Rectangle(0, 0, TextureAssets.Chain18.Width(), TextureAssets.Chain18.Height())), color30, rotation21, new Vector2((float)TextureAssets.Chain18.Width() * 0.5f, (float)TextureAssets.Chain18.Height() * 0.5f), 1f, 0, 0f);
                        }
                    }
                    return;
                }
                if (proj.aiStyle == 13)
                {
                    float num129 = proj.position.X + 8f;
                    float num130 = proj.position.Y + 2f;
                    float x9 = proj.velocity.X;
                    float num131 = proj.velocity.Y;
                    if (x9 == 0f && num131 == 0f)
                    {
                        num131 = 0.0001f;
                    }
                    float num132 = (float)Math.Sqrt((double)(x9 * x9 + num131 * num131));
                    num132 = 20f / num132;
                    if (proj.ai[0] == 0f)
                    {
                        num129 -= proj.velocity.X * num132;
                        num130 -= proj.velocity.Y * num132;
                    }
                    else
                    {
                        num129 += proj.velocity.X * num132;
                        num130 += proj.velocity.Y * num132;
                    }
                    Vector2 vector27 = new Vector2(num129, num130);
                    x9 = mountedCenter.X - vector27.X;
                    num131 = mountedCenter.Y - vector27.Y;
                    float rotation22 = (float)Math.Atan2((double)num131, (double)x9) - 1.57f;
                    if (proj.alpha == 0)
                    {
                        int num133 = -1;
                        if (proj.position.X + (float)(proj.width / 2) < mountedCenter.X)
                        {
                            num133 = 1;
                        }
                        if (Main.player[proj.owner].direction == 1)
                        {
                            Main.player[proj.owner].itemRotation = (float)Math.Atan2((double)(num131 * (float)num133), (double)(x9 * (float)num133));
                        }
                        else
                        {
                            Main.player[proj.owner].itemRotation = (float)Math.Atan2((double)(num131 * (float)num133), (double)(x9 * (float)num133));
                        }
                    }
                    bool flag26 = true;
                    while (flag26)
                    {
                        float num134 = (float)Math.Sqrt((double)(x9 * x9 + num131 * num131));
                        if (num134 < 25f)
                        {
                            flag26 = false;
                        }
                        else if (float.IsNaN(num134))
                        {
                            flag26 = false;
                        }
                        else
                        {
                            num134 = 12f / num134;
                            x9 *= num134;
                            num131 *= num134;
                            vector27.X += x9;
                            vector27.Y += num131;
                            x9 = mountedCenter.X - vector27.X;
                            num131 = mountedCenter.Y - vector27.Y;
                            Color color31 = Lighting.GetColor((int)vector27.X / 16, (int)(vector27.Y / 16f));
                            Main.EntitySpriteDraw(TextureAssets.Chain.Value, new Vector2(vector27.X - Main.screenPosition.X, vector27.Y - Main.screenPosition.Y), new Rectangle?(new Rectangle(0, 0, TextureAssets.Chain.Width(), TextureAssets.Chain.Height())), color31, rotation22, new Vector2((float)TextureAssets.Chain.Width() * 0.5f, (float)TextureAssets.Chain.Height() * 0.5f), 1f, 0, 0f);
                        }
                    }
                    return;
                }
                if (proj.type == 190)
                {
                    float x10 = proj.position.X + (float)(proj.width / 2);
                    float y9 = proj.position.Y + (float)(proj.height / 2);
                    float x11 = proj.velocity.X;
                    float y10 = proj.velocity.Y;
                    Math.Sqrt((double)(x11 * x11 + y10 * y10));
                    Vector2 vector28 = new Vector2(x10, y9);
                    x11 = mountedCenter.X - vector28.X;
                    y10 = mountedCenter.Y + Main.player[proj.owner].gfxOffY - vector28.Y;
                    Math.Atan2((double)y10, (double)x11);
                    if (proj.alpha == 0)
                    {
                        int num135 = -1;
                        if (proj.position.X + (float)(proj.width / 2) < mountedCenter.X)
                        {
                            num135 = 1;
                        }
                        if (Main.player[proj.owner].direction == 1)
                        {
                            Main.player[proj.owner].itemRotation = (float)Math.Atan2((double)(y10 * (float)num135), (double)(x11 * (float)num135));
                            return;
                        }
                        Main.player[proj.owner].itemRotation = (float)Math.Atan2((double)(y10 * (float)num135), (double)(x11 * (float)num135));
                        return;
                    }
                }
                else if (proj.aiStyle == 15)
                {
                    DrawProj_FlailChains(proj, mountedCenter);
                }
            }
        }
        private static void DrawProj_FishingLine(Projectile proj, ref float polePosX, ref float polePosY, Vector2 mountedCenter)
        {
            Player player = Main.player[proj.owner];
            polePosX = mountedCenter.X;
            polePosY = mountedCenter.Y;
            //polePosY += player.gfxOffY;
            polePosY += player.WG().AddedHeight;
            if (player.mount.Active && player.mount.Type == 52)
            {
                polePosX -= (float)(player.direction * 14);
                polePosY -= -10f;
            }
            int type = player.inventory[player.selectedItem].type;
            Color stringColor;
            stringColor = new Color(200, 200, 200, 100);
            if (type == 2294)
            {
                stringColor = new Color(100, 180, 230, 100);
            }
            if (type == 2295)
            {
                stringColor = new Color(250, 90, 70, 100);
            }
            if (type == 2293)
            {
                stringColor = new Color(203, 190, 210, 100);
            }
            if (type == 2421)
            {
                stringColor = new Color(183, 77, 112, 100);
            }
            if (type == 2422)
            {
                stringColor = new Color(255, 226, 116, 100);
            }
            if (type == 4325)
            {
                stringColor = new Color(200, 100, 100, 100);
            }
            if (type == 4442)
            {
                stringColor = new Color(100, 100, 200, 100);
            }
            ProjectileLoader.ModifyFishingLine(proj, ref polePosX, ref polePosY, ref stringColor);
            ItemLoader.ModifyFishingLine(proj, ref polePosX, ref polePosY, ref stringColor);
            stringColor = TryApplyingPlayerStringColor(Main.player[proj.owner].stringColor, stringColor);
            float gravDir = Main.player[proj.owner].gravDir;
            if (type <= 2421)
            {
                switch (type)
                {
                    case 2289:
                        polePosX += (float)(43 * Main.player[proj.owner].direction);
                        if (Main.player[proj.owner].direction < 0)
                        {
                            polePosX -= 13f;
                        }
                        polePosY -= 36f * gravDir;
                        break;
                    case 2290:
                        break;
                    case 2291:
                        polePosX += (float)(43 * Main.player[proj.owner].direction);
                        if (Main.player[proj.owner].direction < 0)
                        {
                            polePosX -= 13f;
                        }
                        polePosY -= 34f * gravDir;
                        break;
                    case 2292:
                        polePosX += (float)(46 * Main.player[proj.owner].direction);
                        if (Main.player[proj.owner].direction < 0)
                        {
                            polePosX -= 13f;
                        }
                        polePosY -= 34f * gravDir;
                        break;
                    case 2293:
                        polePosX += (float)(43 * Main.player[proj.owner].direction);
                        if (Main.player[proj.owner].direction < 0)
                        {
                            polePosX -= 13f;
                        }
                        polePosY -= 34f * gravDir;
                        break;
                    case 2294:
                        polePosX += (float)(43 * Main.player[proj.owner].direction);
                        if (Main.player[proj.owner].direction < 0)
                        {
                            polePosX -= 13f;
                        }
                        polePosY -= 30f * gravDir;
                        break;
                    case 2295:
                        polePosX += (float)(43 * Main.player[proj.owner].direction);
                        if (Main.player[proj.owner].direction < 0)
                        {
                            polePosX -= 13f;
                        }
                        polePosY -= 30f * gravDir;
                        break;
                    case 2296:
                        polePosX += (float)(43 * Main.player[proj.owner].direction);
                        if (Main.player[proj.owner].direction < 0)
                        {
                            polePosX -= 13f;
                        }
                        polePosY -= 30f * gravDir;
                        break;
                    default:
                        if (type == 2421)
                        {
                            polePosX += (float)(47 * Main.player[proj.owner].direction);
                            if (Main.player[proj.owner].direction < 0)
                            {
                                polePosX -= 13f;
                            }
                            polePosY -= 36f * gravDir;
                        }
                        break;
                }
            }
            else if (type != 2422)
            {
                if (type != 4325)
                {
                    if (type == 4442)
                    {
                        polePosX += (float)(44 * Main.player[proj.owner].direction);
                        if (Main.player[proj.owner].direction < 0)
                        {
                            polePosX -= 13f;
                        }
                        polePosY -= 32f * gravDir;
                    }
                }
                else
                {
                    polePosX += (float)(44 * Main.player[proj.owner].direction);
                    if (Main.player[proj.owner].direction < 0)
                    {
                        polePosX -= 13f;
                    }
                    polePosY -= 32f * gravDir;
                }
            }
            else
            {
                polePosX += (float)(47 * Main.player[proj.owner].direction);
                if (Main.player[proj.owner].direction < 0)
                {
                    polePosX -= 13f;
                }
                polePosY -= 32f * gravDir;
            }
            if (gravDir == -1f)
            {
                polePosY -= 12f;
            }
            Vector2 vector = new Vector2(polePosX, polePosY);
            vector = Main.player[proj.owner].RotatedRelativePoint(vector + new Vector2(8f), false, true) - new Vector2(8f);
            float num = proj.position.X + (float)proj.width * 0.5f - vector.X;
            float num2 = proj.position.Y + (float)proj.height * 0.5f - vector.Y;
            Math.Sqrt((double)(num * num + num2 * num2));
            float num3 = (float)Math.Atan2((double)num2, (double)num) - 1.57f;
            bool flag = true;
            if (num == 0f && num2 == 0f)
            {
                flag = false;
            }
            else
            {
                float num4 = (float)Math.Sqrt((double)(num * num + num2 * num2));
                num4 = 12f / num4;
                num *= num4;
                num2 *= num4;
                vector.X -= num;
                vector.Y -= num2;
                num = proj.position.X + (float)proj.width * 0.5f - vector.X;
                num2 = proj.position.Y + (float)proj.height * 0.5f - vector.Y;
            }
            while (flag)
            {
                float num5 = 12f;
                float num6 = (float)Math.Sqrt((double)(num * num + num2 * num2));
                float num7 = num6;
                if (float.IsNaN(num6) || float.IsNaN(num7))
                {
                    flag = false;
                }
                else
                {
                    if (num6 < 20f)
                    {
                        num5 = num6 - 8f;
                        flag = false;
                    }
                    num6 = 12f / num6;
                    num *= num6;
                    num2 *= num6;
                    vector.X += num;
                    vector.Y += num2;
                    num = proj.position.X + (float)proj.width * 0.5f - vector.X;
                    num2 = proj.position.Y + (float)proj.height * 0.1f - vector.Y;
                    if (num7 > 12f)
                    {
                        float num8 = 0.3f;
                        float num9 = Math.Abs(proj.velocity.X) + Math.Abs(proj.velocity.Y);
                        if (num9 > 16f)
                        {
                            num9 = 16f;
                        }
                        num9 = 1f - num9 / 16f;
                        num8 *= num9;
                        num9 = num7 / 80f;
                        if (num9 > 1f)
                        {
                            num9 = 1f;
                        }
                        num8 *= num9;
                        if (num8 < 0f)
                        {
                            num8 = 0f;
                        }
                        num9 = 1f - proj.localAI[0] / 100f;
                        num8 *= num9;
                        if (num2 > 0f)
                        {
                            num2 *= 1f + num8;
                            num *= 1f - num8;
                        }
                        else
                        {
                            num9 = Math.Abs(proj.velocity.X) / 3f;
                            if (num9 > 1f)
                            {
                                num9 = 1f;
                            }
                            num9 -= 0.5f;
                            num8 *= num9;
                            if (num8 > 0f)
                            {
                                num8 *= 2f;
                            }
                            num2 *= 1f + num8;
                            num *= 1f - num8;
                        }
                    }
                    num3 = (float)Math.Atan2((double)num2, (double)num) - 1.57f;
                    Color color = Lighting.GetColor((int)vector.X / 16, (int)(vector.Y / 16f), stringColor);
                    Main.EntitySpriteDraw(TextureAssets.FishingLine.Value, new Vector2(vector.X - Main.screenPosition.X + (float)TextureAssets.FishingLine.Width() * 0.5f, vector.Y - Main.screenPosition.Y + (float)TextureAssets.FishingLine.Height() * 0.5f), new Rectangle?(new Rectangle(0, 0, TextureAssets.FishingLine.Width(), (int)num5)), color, num3, new Vector2((float)TextureAssets.FishingLine.Width() * 0.5f, 0f), 1f, 0, 0f);
                }
            }
        }
        private static Color TryApplyingPlayerStringColor(int playerStringColor, Color stringColor)
        {
            if (playerStringColor > 0)
            {
                stringColor = WorldGen.paintColor(playerStringColor);
                if (stringColor.R < 75)
                {
                    stringColor.R = 75;
                }
                if (stringColor.G < 75)
                {
                    stringColor.G = 75;
                }
                if (stringColor.B < 75)
                {
                    stringColor.B = 75;
                }
                if (playerStringColor <= 13)
                {
                    if (playerStringColor != 0)
                    {
                        if (playerStringColor != 13)
                        {
                            goto IL_BA;
                        }
                        stringColor = new Color(20, 20, 20);
                        goto IL_BA;
                    }
                }
                else if (playerStringColor != 14)
                {
                    if (playerStringColor == 27)
                    {
                        stringColor = new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB);
                        goto IL_BA;
                    }
                    if (playerStringColor != 28)
                    {
                        goto IL_BA;
                    }
                    stringColor = new Color(163, 116, 91);
                    goto IL_BA;
                }
                stringColor = new Color(200, 200, 200);
            IL_BA:
                stringColor.A = (byte)((float)stringColor.A * 0.4f);
            }
            return stringColor;
        }
        private static void DrawProj_FlailChains(Projectile proj, Vector2 mountedCenter)
        {
            Player player = Main.player[proj.owner];
            Vector2 playerArmPosition = Main.GetPlayerArmPosition(proj);
            playerArmPosition -= Vector2.UnitY * player.gfxOffY;
            Rectangle? sourceRectangle = null;
            float num = 0f;
            int type = proj.type;
            Asset<Texture2D> asset;
            if (type <= 154)
            {
                if (type <= 35)
                {
                    if (type == 25)
                    {
                        asset = TextureAssets.Chain2;
                        goto IL_104;
                    }
                    if (type == 35)
                    {
                        asset = TextureAssets.Chain6;
                        goto IL_104;
                    }
                }
                else
                {
                    if (type == 63)
                    {
                        asset = TextureAssets.Chain7;
                        goto IL_104;
                    }
                    if (type == 154)
                    {
                        asset = TextureAssets.Chain13;
                        goto IL_104;
                    }
                }
            }
            else if (type <= 757)
            {
                if (type == 247)
                {
                    asset = TextureAssets.Chain19;
                    goto IL_104;
                }
                if (type == 757)
                {
                    asset = TextureAssets.Extra[99];
                    sourceRectangle = new Rectangle?(asset.Frame(1, 6, 0, 0, 0, 0));
                    num = -2f;
                    goto IL_104;
                }
            }
            else
            {
                if (type == 947)
                {
                    asset = TextureAssets.Chain41;
                    goto IL_104;
                }
                if (type == 948)
                {
                    asset = TextureAssets.Chain43;
                    goto IL_104;
                }
            }
            asset = TextureAssets.Chain3;
        IL_104:
            Vector2 origin = (sourceRectangle != null) ? (sourceRectangle.Value.Size() / 2f) : (asset.Size() / 2f);
            Vector2 center = proj.Center;
            Vector2 v = playerArmPosition.MoveTowards(center, 4f) - center;
            Vector2 vector = v.SafeNormalize(Vector2.Zero);
            float num2 = (float)((sourceRectangle != null) ? sourceRectangle.Value.Height : asset.Height()) + num;
            float rotation = vector.ToRotation() + 1.5707964f;
            int num3 = 0;
            float num4 = v.Length() + num2 / 2f;
            int num5 = 0;
            while (num4 > 0f)
            {
                Color color = Lighting.GetColor((int)center.X / 16, (int)(center.Y / 16f));
                type = proj.type;
                if (type != 757)
                {
                    if (type == 948)
                    {
                        if (num5 >= 6)
                        {
                            asset = TextureAssets.Chain41;
                        }
                        else if (num5 >= 4)
                        {
                            asset = TextureAssets.Chain42;
                            byte b = 140;
                            if (color.R < b)
                            {
                                color.R = b;
                            }
                            if (color.G < b)
                            {
                                color.G = b;
                            }
                            if (color.B < b)
                            {
                                color.B = b;
                            }
                        }
                        else
                        {
                            color = Color.White;
                        }
                        num5++;
                    }
                }
                else
                {
                    sourceRectangle = new Rectangle?(asset.Frame(1, 6, 0, num3 % 6, 0, 0));
                }
                Main.spriteBatch.Draw(asset.Value, center - Main.screenPosition, sourceRectangle, color, rotation, origin, 1f, 0, 0f);
                center += vector * num2;
                num3++;
                num4 -= num2;
            }
        }

        public static Vector2 OnMain_GetPlayerArmPosition(On_Main.orig_GetPlayerArmPosition orig, Projectile proj)
        {
            Player player = Main.player[proj.owner];
            Vector2 vector = Main.OffsetsPlayerOnhand[player.bodyFrame.Y / 56] * 2f;
            if (player.direction != 1)
            {
                vector.X = (float)player.bodyFrame.Width - vector.X;
            }
            if (player.gravDir != 1f)
            {
                vector.Y = (float)player.bodyFrame.Height - vector.Y;
            }
            vector -= new Vector2((float)(player.bodyFrame.Width - player.width), (float)(player.bodyFrame.Height - player.height)) / 2f;
            Vector2 pos = player.MountedCenter - new Vector2(player.width, player.height) / 2f + vector + Vector2.UnitY * player.gfxOffY; //+ new Vector2(player.WG().AddedHeight);
            if (player.mount.Active && player.mount.Type == 52)
            {
                pos.Y -= (float)player.mount.PlayerOffsetHitbox;
                pos += new Vector2((float)(12 * player.direction), -12f);
            }
            return player.RotatedRelativePoint(pos, false, true);
        }

        public static void OnPlayerDrawLayers_DrawStarboardRainbowTrail(On_PlayerDrawLayers.orig_DrawStarboardRainbowTrail orig, ref PlayerDrawSet drawinfo, Vector2 commonWingPosPreFloor, Vector2 dirsVec)
        {
            if (drawinfo.shadow != 0f)
            {
                return;
            }
            int num = Math.Min(drawinfo.drawPlayer.availableAdvancedShadowsCount - 1, 30);
            float num2 = 0f;
            for (int num3 = num; num3 > 0; num3--)
            {
                EntityShadowInfo advancedShadow = drawinfo.drawPlayer.GetAdvancedShadow(num3);
                float num10 = num2;
                Vector2 position = drawinfo.drawPlayer.GetAdvancedShadow(num3 - 1).Position;
                num2 = num10 + Vector2.Distance(advancedShadow.Position, position);
            }
            float num4 = MathHelper.Clamp(num2 / 160f, 0f, 1f);
            Main.instance.LoadProjectile(250);
            Texture2D value = TextureAssets.Projectile[250].Value;
            float x = 1.7f;
            Vector2 origin = new Vector2((float)(value.Width / 2), (float)(value.Height / 2));
            Color white = Color.White;
            white.A = 64;
            Vector2 vector2 = new Vector2(drawinfo.drawPlayer.width, drawinfo.drawPlayer.height) * new Vector2(0.5f, 1f) + new Vector2(0f, -4f);
            if (dirsVec.Y < 0f)
            {
                vector2 = new Vector2(drawinfo.drawPlayer.width, drawinfo.drawPlayer.height) * new Vector2(0.5f, 0f) + new Vector2(0f, 4f);
            }
            for (int num5 = num; num5 > 0; num5--)
            {
                EntityShadowInfo advancedShadow2 = drawinfo.drawPlayer.GetAdvancedShadow(num5);
                EntityShadowInfo advancedShadow3 = drawinfo.drawPlayer.GetAdvancedShadow(num5 - 1);
                Vector2 pos = advancedShadow2.Position + vector2 + advancedShadow2.HeadgearOffset;
                Vector2 pos2 = advancedShadow3.Position + vector2 + advancedShadow3.HeadgearOffset;
                pos = drawinfo.drawPlayer.RotatedRelativePoint(pos, true, false);
                pos2 = drawinfo.drawPlayer.RotatedRelativePoint(pos2, true, false);
                float num6 = (pos2 - pos).ToRotation() - 1.5707964f;
                num6 = 1.5707964f * (float)drawinfo.drawPlayer.direction;
                float num7 = Math.Abs(pos2.X - pos.X);
                Vector2 scale = new Vector2(x, num7 / (float)value.Height);
                float num8 = 1f - (float)num5 / (float)num;
                num8 *= num8;
                num8 *= Terraria.Utils.GetLerpValue(0f, 4f, num7, true);
                num8 *= 0.5f;
                num8 *= num8;
                Color color = white * num8 * num4;
                if (!(color == Color.Transparent))
                {
                    DrawData item = new DrawData(value, pos - Main.screenPosition, null, color, num6, origin, scale, drawinfo.playerEffect, 0f);
                    item.shader = drawinfo.cWings;
                    drawinfo.DrawDataCache.Add(item);
                    for (float num9 = 0.25f; num9 < 1f; num9 += 0.25f)
                    {
                        item = new DrawData(value, Vector2.Lerp(pos, pos2, num9) - Main.screenPosition, null, color, num6, origin, scale, drawinfo.playerEffect, 0f);
                        item.shader = drawinfo.cWings;
                        drawinfo.DrawDataCache.Add(item);
                    }
                }
            }
        }

        public static void OnEntityShadowInfo_CopyPlayer(On_EntityShadowInfo.orig_CopyPlayer orig, ref EntityShadowInfo self, Player player)
        {
            self.Position = player.position + new Vector2(0, player.WG().AddedHeight);
            self.Rotation = player.fullRotation;
            self.Origin = player.fullRotationOrigin;
            self.Direction = player.direction;
            self.GravityDirection = (int)player.gravDir;
            self.BodyFrameIndex = player.bodyFrame.Y / player.bodyFrame.Height;
        }

        public static void OnPlayer_UpdateSocialShadow(On_Player.orig_UpdateSocialShadow orig, Player self)
        {
            for (int num = 2; num > 0; num--)
            {
                self.shadowDirection[num] = self.shadowDirection[num - 1];
            }
            self.shadowDirection[0] = self.direction;
            self.shadowCount++;
            if (self.shadowCount == 1)
            {
                self.shadowPos[2] = self.shadowPos[1];
                self.shadowRotation[2] = self.shadowRotation[1];
                self.shadowOrigin[2] = self.shadowOrigin[1];
                return;
            }
            if (self.shadowCount == 2)
            {
                self.shadowPos[1] = self.shadowPos[0];
                self.shadowRotation[1] = self.shadowRotation[0];
                self.shadowOrigin[1] = self.shadowOrigin[0];
                return;
            }
            if (self.shadowCount >= 3)
            {
                Vector2 selfPos = self.position - new Vector2(0, WeightValues.DrawOffsetY(self.WG().Weight.GetStage())) * self.gravDir;
                if (self.WG().Weight.GetStage() == 7 && self.gravDir == -1f)
                    selfPos -= new Vector2(0, 5f);
                self.shadowCount = 0;
                self.shadowPos[0] = selfPos;
                Vector2[] array = self.shadowPos;
                int num2 = 0;
                array[num2].Y = array[num2].Y + self.gfxOffY;
                self.shadowRotation[0] = self.fullRotation;
                self.shadowOrigin[0] = self.fullRotationOrigin;
            }
        }

        public static void OnPlayerDrawLayers_DrawPlayer_03_PortableStool(On_PlayerDrawLayers.orig_DrawPlayer_03_PortableStool orig, ref PlayerDrawSet drawinfo)
        {
            if (drawinfo.drawPlayer.portableStoolInfo.IsInUse)
            {
                Texture2D value = TextureAssets.Extra[102].Value;
                Vector2 position = new Vector2((float)((int)(drawinfo.Position.X - Main.screenPosition.X + (float)(drawinfo.drawPlayer.width / 2))), (float)((int)(drawinfo.Position.Y - Main.screenPosition.Y + (float)drawinfo.drawPlayer.height + 28f + drawinfo.drawPlayer.WG().AddedVisualHeight)));
                if (drawinfo.drawPlayer.WG().Weight.GetStage() == 7)
                    position.Y += 3f;
                Rectangle rectangle = value.Frame(1, 1, 0, 0, 0, 0);
                Vector2 origin = rectangle.Size() * new Vector2(0.5f, 1f);
                DrawData item = new DrawData(value, position, new Rectangle?(rectangle), drawinfo.colorArmorLegs, drawinfo.drawPlayer.bodyRotation, origin, 1f, drawinfo.playerEffect, 0f);
                item.shader = drawinfo.cPortableStool;
                drawinfo.DrawDataCache.Add(item);
            }
        }

        public static void OnPlayer_DashMovement(On_Player.orig_DashMovement orig, Player self)
        {
            float dashMult = self.WG().SetMobilityMultiplier();
            dashMult = Math.Clamp(dashMult * 1.33f, 0f, 1.0f);
            if (self.dashDelay == 0)
            {
                self.dash = self.dashType;
            }
            if (self.dash == 0)
            {
                self.dashTime = 0;
                self.dashDelay = 0;
            }
            if (self.dash == 2 && self.eocDash > 0)
            {
                if (self.eocHit < 0)
                {
                    Rectangle rectangle = new Rectangle((int)((double)self.position.X + (double)self.velocity.X * 0.5 - 4.0), (int)((double)self.position.Y + (double)self.velocity.Y * 0.5 - 4.0), self.width + 8, self.height + 8);
                    for (int i = 0; i < 200; i++)
                    {
                        NPC nPC = Main.npc[i];
                        if (nPC.active && !nPC.dontTakeDamage && !nPC.friendly && (nPC.aiStyle != 112 || nPC.ai[2] <= 1f) && self.CanNPCBeHitByPlayerOrPlayerProjectile(nPC, null))
                        {
                            Rectangle rect = nPC.getRect();
                            if (rectangle.Intersects(rect) && (nPC.noTileCollide || self.CanHit(nPC)))
                            {
                                float num = self.GetTotalDamage(DamageClass.Melee).ApplyTo(30f);
                                float num2 = self.GetTotalKnockback(DamageClass.Melee).ApplyTo(9f);
                                bool crit = false;
                                if ((float)Main.rand.Next(100) < self.GetTotalCritChance(DamageClass.Melee))
                                {
                                    crit = true;
                                }
                                int num3 = self.direction;
                                if (self.velocity.X < 0f)
                                {
                                    num3 = -1;
                                }
                                if (self.velocity.X > 0f)
                                {
                                    num3 = 1;
                                }
                                if (self.whoAmI == Main.myPlayer)
                                {
                                    self.ApplyDamageToNPC(nPC, (int)num, num2, num3, crit, DamageClass.Melee, false);
                                }
                                self.eocDash = 10;
                                self.dashDelay = 30;
                                self.velocity.X = (float)(-(float)num3 * 9);
                                self.velocity.Y = -4f;
                                self.GiveImmuneTimeForCollisionAttack(4);
                                self.eocHit = i;
                            }
                        }
                    }
                }
                else if ((!self.controlLeft || self.velocity.X >= 0f) && (!self.controlRight || self.velocity.X <= 0f))
                {
                    self.velocity.X = self.velocity.X * 0.95f;
                }
            }
            if (self.dash == 3 && self.dashDelay < 0 && self.whoAmI == Main.myPlayer)
            {
                Rectangle rectangle2 = new Rectangle((int)((double)self.position.X + (double)self.velocity.X * 0.5 - 4.0), (int)((double)self.position.Y + (double)self.velocity.Y * 0.5 - 4.0), self.width + 8, self.height + 8);
                for (int j = 0; j < 200; j++)
                {
                    NPC nPC2 = Main.npc[j];
                    if (nPC2.active && !nPC2.dontTakeDamage && !nPC2.friendly && nPC2.immune[self.whoAmI] <= 0 && (nPC2.aiStyle != 112 || nPC2.ai[2] <= 1f) && self.CanNPCBeHitByPlayerOrPlayerProjectile(nPC2, null))
                    {
                        Rectangle rect2 = nPC2.getRect();
                        if (rectangle2.Intersects(rect2) && (nPC2.noTileCollide || self.CanHit(nPC2)))
                        {
                            if (!self.solarDashConsumedFlare)
                            {
                                self.solarDashConsumedFlare = true;
                                self.ConsumeSolarFlare();
                            }
                            float num4 = self.GetTotalDamage(DamageClass.Melee).ApplyTo(150f);
                            float num5 = self.GetTotalKnockback(DamageClass.Melee).ApplyTo(9f);
                            bool crit2 = false;
                            if ((float)Main.rand.Next(100) < self.GetTotalCritChance(DamageClass.Melee))
                            {
                                crit2 = true;
                            }
                            int num6 = self.direction;
                            if (self.velocity.X < 0f)
                            {
                                num6 = -1;
                            }
                            if (self.velocity.X > 0f)
                            {
                                num6 = 1;
                            }
                            if (self.whoAmI == Main.myPlayer)
                            {
                                self.ApplyDamageToNPC(nPC2, (int)num4, num5, num6, crit2, DamageClass.Melee, false);
                                int num7 = Projectile.NewProjectile(self.GetSource_OnHit(nPC2, "SetBonus_SolarExplosion_WhenDashing"), self.Center.X, self.Center.Y, 0f, 0f, ProjectileID.SolarCounter, (int)num4, 15f, Main.myPlayer, 0f, 0f, 0f);
                                Main.projectile[num7].Kill();
                            }
                            nPC2.immune[self.whoAmI] = 6;
                            self.GiveImmuneTimeForCollisionAttack(4);
                        }
                    }
                }
            }
            if (self.dashDelay > 0)
            {
                if (self.eocDash > 0)
                {
                    self.eocDash--;
                }
                if (self.eocDash == 0)
                {
                    self.eocHit = -1;
                }
                self.dashDelay--;
                return;
            }
            if (self.dashDelay < 0)
            {
                self.StopVanityActions(true);
                float num8 = 12f;
                float num9 = 0.992f;
                float num10 = Math.Max(self.accRunSpeed, self.maxRunSpeed);
                float num11 = 0.96f;
                int num12 = 20;
                if (self.dash == 1)
                {
                    for (int k = 0; k < 2; k++)
                    {
                        int num13 = (self.velocity.Y != 0f) ? Dust.NewDust(new Vector2(self.position.X, self.position.Y + (float)(self.height / 2) - 8f), self.width, 16, 31, 0f, 0f, 100, default(Color), 1.4f) : Dust.NewDust(new Vector2(self.position.X, self.position.Y + (float)self.height - 4f), self.width, 8, 31, 0f, 0f, 100, default(Color), 1.4f);
                        Main.dust[num13].velocity *= 0.1f;
                        Main.dust[num13].scale *= 1f + (float)Main.rand.Next(20) * 0.01f;
                    }
                }
                else if (self.dash == 2)
                {
                    for (int l = 0; l < 0; l++)
                    {
                        int num14 = (self.velocity.Y != 0f) ? Dust.NewDust(new Vector2(self.position.X, self.position.Y + (float)(self.height / 2) - 8f), self.width, 16, 31, 0f, 0f, 100, default(Color), 1.4f) : Dust.NewDust(new Vector2(self.position.X, self.position.Y + (float)self.height - 4f), self.width, 8, 31, 0f, 0f, 100, default(Color), 1.4f);
                        Main.dust[num14].velocity *= 0.1f;
                        Main.dust[num14].scale *= 1f + (float)Main.rand.Next(20) * 0.01f;
                    }
                    num9 = 0.985f;
                    num11 = 0.94f;
                    num12 = 30;
                }
                else if (self.dash == 3)
                {
                    for (int m = 0; m < 4; m++)
                    {
                        int num15 = Dust.NewDust(new Vector2(self.position.X, self.position.Y + 4f), self.width, self.height - 8, 6, 0f, 0f, 100, default(Color), 1.7f);
                        Main.dust[num15].velocity *= 0.1f;
                        Main.dust[num15].scale *= 1f + (float)Main.rand.Next(20) * 0.01f;
                        Main.dust[num15].shader = GameShaders.Armor.GetSecondaryShader(self.ArmorSetDye(), self);
                        Main.dust[num15].noGravity = true;
                        if (Main.rand.Next(2) == 0)
                        {
                            Main.dust[num15].fadeIn = 0.5f;
                        }
                    }
                    num8 = 14f;
                    num9 = 0.985f;
                    num11 = 0.94f;
                    num12 = 20;
                }
                else if (self.dash == 4)
                {
                    for (int n = 0; n < 2; n++)
                    {
                        int num16 = Dust.NewDust(new Vector2(self.position.X, self.position.Y + 4f), self.width, self.height - 8, 229, 0f, 0f, 100, default(Color), 1.2f);
                        Main.dust[num16].velocity *= 0.1f;
                        Main.dust[num16].scale *= 1f + (float)Main.rand.Next(20) * 0.01f;
                        Main.dust[num16].noGravity = true;
                        if (Main.rand.Next(2) == 0)
                        {
                            Main.dust[num16].fadeIn = 0.3f;
                        }
                    }
                    num9 = 0.985f;
                    num11 = 0.94f;
                    num12 = 20;
                }
                if (self.dash == 5)
                {
                    for (int num17 = 0; num17 < 2; num17++)
                    {
                        int type = (int)Main.rand.NextFromList(new short[]
                        {
                    68,
                    69,
                    70
                        });
                        int num18 = (self.velocity.Y != 0f) ? Dust.NewDust(new Vector2(self.position.X, self.position.Y + (float)(self.height / 2) - 8f), self.width, 16, type, 0f, 0f, 100, default(Color), 1f) : Dust.NewDust(new Vector2(self.position.X, self.position.Y + (float)self.height - 4f), self.width, 8, type, 0f, 0f, 100, default(Color), 1f);
                        Main.dust[num18].velocity *= 0.2f;
                        Main.dust[num18].scale *= 1f + (float)Main.rand.Next(20) * 0.01f;
                        Main.dust[num18].fadeIn = 0.5f + (float)Main.rand.Next(20) * 0.01f;
                        Main.dust[num18].noGravity = true;
                        Main.dust[num18].shader = GameShaders.Armor.GetSecondaryShader(self.ArmorSetDye(), self);
                    }
                }
                if (self.dash <= 0)
                {
                    return;
                }
                self.doorHelper.AllowOpeningDoorsByVelocityAloneForATime(num12 * 3);
                self.vortexStealthActive = false;
                if (self.velocity.X > num8 || self.velocity.X < 0f - num8)
                {
                    self.velocity.X = self.velocity.X * num9;
                    return;
                }
                if (self.velocity.X > num10 || self.velocity.X < 0f - num10)
                {
                    self.velocity.X = self.velocity.X * num11;
                    return;
                }
                self.dashDelay = num12;
                if (self.velocity.X < 0f)
                {
                    self.velocity.X = 0f - num10;
                    return;
                }
                if (self.velocity.X > 0f)
                {
                    self.velocity.X = num10;
                    return;
                }
            }
            else
            {
                if (self.dash <= 0 || self.mount.Active)
                {
                    return;
                }
                if (self.dash == 1)
                {
                    int dir;
                    bool dashing;
                    DoCommonDashHandle(self, out dir, out dashing, null);
                    if (dashing)
                    {
                        self.velocity.X = 16.9f * (float)dir * dashMult;
                        Point point = (self.Center + new Vector2((float)(dir * self.width / 2 + 2), self.gravDir * (float)(-(float)self.height) / 2f + self.gravDir * 2f)).ToTileCoordinates();
                        Point point2 = (self.Center + new Vector2((float)(dir * self.width / 2 + 2), 0f)).ToTileCoordinates();
                        if (WorldGen.SolidOrSlopedTile(point.X, point.Y) || WorldGen.SolidOrSlopedTile(point2.X, point2.Y))
                        {
                            self.velocity.X = self.velocity.X / 2f;
                        }
                        self.dashDelay = -1;
                        for (int num19 = 0; num19 < 20; num19++)
                        {
                            int num20 = Dust.NewDust(new Vector2(self.position.X, self.position.Y), self.width, self.height, 31, 0f, 0f, 100, default(Color), 2f);
                            Dust dust = Main.dust[num20];
                            dust.position.X = dust.position.X + (float)Main.rand.Next(-5, 6);
                            Dust dust2 = Main.dust[num20];
                            dust2.position.Y = dust2.position.Y + (float)Main.rand.Next(-5, 6);
                            Main.dust[num20].velocity *= 0.2f;
                            Main.dust[num20].scale *= 1f + (float)Main.rand.Next(20) * 0.01f;
                        }
                        int num21 = Gore.NewGore(self.GetSource_FromThis(), new Vector2(self.position.X + (float)(self.width / 2) - 24f, self.position.Y + (float)(self.height / 2) - 34f), default(Vector2), Main.rand.Next(61, 64), 1f);
                        Main.gore[num21].velocity.X = (float)Main.rand.Next(-50, 51) * 0.01f;
                        Main.gore[num21].velocity.Y = (float)Main.rand.Next(-50, 51) * 0.01f;
                        Main.gore[num21].velocity *= 0.4f;
                        num21 = Gore.NewGore(self.GetSource_FromThis(), new Vector2(self.position.X + (float)(self.width / 2) - 24f, self.position.Y + (float)(self.height / 2) - 14f), default(Vector2), Main.rand.Next(61, 64), 1f);
                        Main.gore[num21].velocity.X = (float)Main.rand.Next(-50, 51) * 0.01f;
                        Main.gore[num21].velocity.Y = (float)Main.rand.Next(-50, 51) * 0.01f;
                        Main.gore[num21].velocity *= 0.4f;
                    }
                }
                else if (self.dash == 2)
                {
                    int dir2;
                    bool dashing2;
                    DoCommonDashHandle(self, out dir2, out dashing2, null);
                    if (dashing2)
                    {
                        self.velocity.X = 14.5f * (float)dir2 * dashMult;
                        Point point3 = (self.Center + new Vector2((float)(dir2 * self.width / 2 + 2), self.gravDir * (float)(-(float)self.height) / 2f + self.gravDir * 2f)).ToTileCoordinates();
                        Point point4 = (self.Center + new Vector2((float)(dir2 * self.width / 2 + 2), 0f)).ToTileCoordinates();
                        if (WorldGen.SolidOrSlopedTile(point3.X, point3.Y) || WorldGen.SolidOrSlopedTile(point4.X, point4.Y))
                        {
                            self.velocity.X = self.velocity.X / 2f;
                        }
                        self.dashDelay = -1;
                        self.eocDash = 15;
                        for (int num22 = 0; num22 < 0; num22++)
                        {
                            int num23 = Dust.NewDust(new Vector2(self.position.X, self.position.Y), self.width, self.height, 31, 0f, 0f, 100, default(Color), 2f);
                            Dust dust3 = Main.dust[num23];
                            dust3.position.X = dust3.position.X + (float)Main.rand.Next(-5, 6);
                            Dust dust4 = Main.dust[num23];
                            dust4.position.Y = dust4.position.Y + (float)Main.rand.Next(-5, 6);
                            Main.dust[num23].velocity *= 0.2f;
                            Main.dust[num23].scale *= 1f + (float)Main.rand.Next(20) * 0.01f;
                        }
                    }
                }
                else if (self.dash == 3)
                {
                    int dir3;
                    bool dashing3;
                    DoCommonDashHandle(self, out dir3, out dashing3, new Player.DashStartAction(self.NewSolarDashStart));
                    if (dashing3)
                    {
                        self.velocity.X = 21.9f * (float)dir3 * dashMult;
                        Point point5 = (self.Center + new Vector2((float)(dir3 * self.width / 2 + 2), self.gravDir * (float)(-(float)self.height) / 2f + self.gravDir * 2f)).ToTileCoordinates();
                        Point point6 = (self.Center + new Vector2((float)(dir3 * self.width / 2 + 2), 0f)).ToTileCoordinates();
                        if (WorldGen.SolidOrSlopedTile(point5.X, point5.Y) || WorldGen.SolidOrSlopedTile(point6.X, point6.Y))
                        {
                            self.velocity.X = self.velocity.X / 2f;
                        }
                        self.dashDelay = -1;
                        for (int num24 = 0; num24 < 20; num24++)
                        {
                            int num25 = Dust.NewDust(new Vector2(self.position.X, self.position.Y), self.width, self.height, 6, 0f, 0f, 100, default(Color), 2f);
                            Dust dust5 = Main.dust[num25];
                            dust5.position.X = dust5.position.X + (float)Main.rand.Next(-5, 6);
                            Dust dust6 = Main.dust[num25];
                            dust6.position.Y = dust6.position.Y + (float)Main.rand.Next(-5, 6);
                            Main.dust[num25].velocity *= 0.2f;
                            Main.dust[num25].scale *= 1f + (float)Main.rand.Next(20) * 0.01f;
                            Main.dust[num25].shader = GameShaders.Armor.GetSecondaryShader(self.ArmorSetDye(), self);
                            Main.dust[num25].noGravity = true;
                            Main.dust[num25].fadeIn = 0.5f;
                        }
                    }
                }
                if (self.dash != 5)
                {
                    return;
                }
                int dir4;
                bool dashing4;
                DoCommonDashHandle(self, out dir4, out dashing4, null);
                if (dashing4)
                {
                    self.velocity.X = 16.9f * (float)dir4 * dashMult;
                    Point point7 = (self.Center + new Vector2((float)(dir4 * self.width / 2 + 2), self.gravDir * (float)(-(float)self.height) / 2f + self.gravDir * 2f)).ToTileCoordinates();
                    Point point8 = (self.Center + new Vector2((float)(dir4 * self.width / 2 + 2), 0f)).ToTileCoordinates();
                    if (WorldGen.SolidOrSlopedTile(point7.X, point7.Y) || WorldGen.SolidOrSlopedTile(point8.X, point8.Y))
                    {
                        self.velocity.X = self.velocity.X / 2f;
                    }
                    self.dashDelay = -1;
                    for (int num26 = 0; num26 < 20; num26++)
                    {
                        int type2 = (int)Main.rand.NextFromList(new short[]
                        {
                    68,
                    69,
                    70
                        });
                        int num27 = Dust.NewDust(new Vector2(self.position.X, self.position.Y), self.width, self.height, type2, 0f, 0f, 100, default(Color), 1.5f);
                        Dust dust7 = Main.dust[num27];
                        dust7.position.X = dust7.position.X + (float)Main.rand.Next(-5, 6);
                        Dust dust8 = Main.dust[num27];
                        dust8.position.Y = dust8.position.Y + (float)Main.rand.Next(-5, 6);
                        Main.dust[num27].velocity = self.DirectionTo(Main.dust[num27].position) * 2f;
                        Main.dust[num27].scale *= 1f + (float)Main.rand.Next(20) * 0.01f;
                        Main.dust[num27].fadeIn = 0.5f + (float)Main.rand.Next(20) * 0.01f;
                        Main.dust[num27].noGravity = true;
                        Main.dust[num27].shader = GameShaders.Armor.GetSecondaryShader(self.ArmorSetDye(), self);
                    }
                }
            }
        }
        private static void DoCommonDashHandle(Player self, out int dir, out bool dashing, Player.DashStartAction dashStartAction = null)
        {
            dir = 0;
            dashing = false;
            if (self.dashTime > 0)
            {
                self.dashTime--;
            }
            if (self.dashTime < 0)
            {
                self.dashTime++;
            }
            if (self.controlRight && self.releaseRight)
            {
                if (self.dashTime <= 0)
                {
                    self.dashTime = 15;
                    return;
                }
                dir = 1;
                dashing = true;
                self.dashTime = 0;
                self.timeSinceLastDashStarted = 0;
                if (dashStartAction != null)
                {
                    dashStartAction(dir);
                    return;
                }
            }
            else if (self.controlLeft && self.releaseLeft)
            {
                if (self.dashTime < 0)
                {
                    dir = -1;
                    dashing = true;
                    self.dashTime = 0;
                    self.timeSinceLastDashStarted = 0;
                    if (dashStartAction != null)
                    {
                        dashStartAction(dir);
                        return;
                    }
                }
                else
                {
                    self.dashTime = -15;
                }
            }
        }

        public static bool OnMount_Hover(On_Mount.orig_Hover orig, Mount self, Player mountedPlayer)
        {
            if (self._type == 8)
                orig(self, mountedPlayer); //fuck that


            float hoverMult = mountedPlayer.WG().SetMobilityMultiplier();
            hoverMult = Math.Clamp(hoverMult * 1.5f, 0.3f, 1f);

            bool flag = self._type == 7 || self._type == 8 || self._type == 12 || self._type == 23 || self._type == 44 || self._type == 49;
            bool flag2 = self._frameState == 2 || self._frameState == 4;
            if (self._type == 49)
            {
                flag2 = (self._frameState == 4);
            }
            if (flag2)
            {
                bool flag3 = true;
                float num = 1f;
                float num2 = mountedPlayer.gravity / Player.defaultGravity;
                if (mountedPlayer.slowFall)
                {
                    num2 /= 3f;
                }
                if (num2 < 0.25f)
                {
                    num2 = 0.25f;
                }
                if (!flag)
                {
                    if (self._flyTime > 0)
                    {
                        self._flyTime--;
                    }
                    else if (self._fatigue < self._fatigueMax)
                    {
                        self._fatigue += num2;
                    }
                    else
                    {
                        flag3 = false;
                    }
                }
                if (self._type == 12 && !mountedPlayer.MountFishronSpecial)
                {
                    num = 0.5f;
                }
                float num3 = self._fatigue / self._fatigueMax;
                if (flag)
                {
                    num3 = 0f;
                }
                bool flag4 = true;
                if (self._type == 48)
                {
                    flag4 = false;
                }
                float num4 = 4f * num3;
                float num5 = 4f * num3;
                bool flag5 = false;
                if (self._type == 48)
                {
                    num4 = 0f;
                    num5 = 0f;
                    if (!flag3)
                    {
                        flag5 = true;
                    }
                    if (mountedPlayer.controlDown)
                    {
                        num5 = 8f;
                    }
                }
                if (num4 == 0f)
                {
                    num4 = -0.001f;
                }
                if (num5 == 0f)
                {
                    num5 = -0.001f;
                }
                float num6 = mountedPlayer.velocity.Y;
                if (flag4 && (mountedPlayer.controlUp || mountedPlayer.controlJump) && flag3)
                {
                    num4 = -2f - 6f * (1f - num3);
                    if (self._type == 48)
                    {
                        num4 /= 3f;
                    }
                    num6 -= self._data.acceleration * num * hoverMult;
                }
                else if (mountedPlayer.controlDown)
                {
                    num6 += self._data.acceleration * num * (1f + 1f - hoverMult);
                    num5 = 8f;
                }
                else if (flag5)
                {
                    float num7 = mountedPlayer.gravity * mountedPlayer.gravDir;
                    num6 += num7;
                    num5 = 4f;
                }
                else
                {
                    int jump = mountedPlayer.jump;
                }
                if (num6 < num4 * hoverMult)
                {
                    num6 = ((num4 - num6 >= self._data.acceleration) ? (num6 + self._data.acceleration * num * hoverMult) : num4 * hoverMult);
                }
                else if (num6 > num5 * (1f + 1f - hoverMult))
                {
                    num6 = ((num6 - num5 >= self._data.acceleration) ? (num6 - self._data.acceleration * num) : num5 * (1f + 1f - hoverMult));
                }
                mountedPlayer.velocity.Y = num6;
                if (num4 == -0.001f && num5 == -0.001f && num6 == -0.001f)
                {
                    mountedPlayer.position.Y = mountedPlayer.position.Y - -0.001f;
                }
                mountedPlayer.fallStart = (int)(mountedPlayer.position.Y / 16f);
            }
            else if (!flag)
            {
                mountedPlayer.velocity.Y = mountedPlayer.velocity.Y + mountedPlayer.gravity * mountedPlayer.gravDir;
            }
            else if (mountedPlayer.velocity.Y == 0f)
            {
                Vector2 velocity = Vector2.UnitY * mountedPlayer.gravDir * 1f;
                if (Collision.TileCollision(mountedPlayer.position, velocity, mountedPlayer.width, mountedPlayer.height, false, false, (int)mountedPlayer.gravDir).Y != 0f || mountedPlayer.controlDown)
                {
                    mountedPlayer.velocity.Y = 0.001f;
                }
            }
            else if (mountedPlayer.velocity.Y == -0.001f)
            {
                mountedPlayer.velocity.Y = mountedPlayer.velocity.Y - -0.001f;
            }
            if (self._type == 7)
            {
                float num8 = mountedPlayer.velocity.X / self._data.dashSpeed;
                if ((double)num8 > 0.95)
                {
                    num8 = 0.95f;
                }
                if ((double)num8 < -0.95)
                {
                    num8 = -0.95f;
                }
                float fullRotation = 0.7853982f * num8 / 2f;
                float num9 = Math.Abs(2f - (float)self._frame / 2f) / 2f;
                Lighting.AddLight((int)(mountedPlayer.position.X + (float)(mountedPlayer.width / 2)) / 16, (int)(mountedPlayer.position.Y + (float)(mountedPlayer.height / 2)) / 16, 0.4f, 0.2f * num9, 0f);
                mountedPlayer.fullRotation = fullRotation;
            }
            else if (self._type == 23)
            {
                float value = (0f - mountedPlayer.velocity.Y) / self._data.dashSpeed;
                value = MathHelper.Clamp(value, -1f, 1f);
                float value2 = mountedPlayer.velocity.X / self._data.dashSpeed;
                value2 = MathHelper.Clamp(value2, -1f, 1f);
                float num12 = -0.19634955f * value * (float)mountedPlayer.direction;
                float num11 = 0.19634955f * value2;
                float fullRotation3 = num12 + num11;
                mountedPlayer.fullRotation = fullRotation3;
                mountedPlayer.fullRotationOrigin = new Vector2((float)(mountedPlayer.width / 2), (float)mountedPlayer.height);
            }
            return true;
        }

        public static Item OnPlayer_PickupItem(On_Player.orig_PickupItem orig, Player self, int playerIndex, int worldItemArrayIndex, Item itemToPickUp)
        {
            if (ItemID.Sets.NebulaPickup[itemToPickUp.type])
            {
                SoundEngine.PlaySound(SoundID.Grab, new Vector2((int)self.position.X, (int)self.position.Y));
                //SoundEngine.PlaySound(7, (int)self.position.X, (int)self.position.Y, 1, 1f, 0f);
                int num = itemToPickUp.buffType;
                itemToPickUp = new Item();
                if (Main.netMode == 1)
                {
                    NetMessage.SendData(102, -1, -1, null, playerIndex, (float)num, self.Center.X, self.Center.Y, 0, 0, 0);
                }
                else
                {
                    self.NebulaLevelup(num);
                }
            }
            if (itemToPickUp.type == 58 || itemToPickUp.type == 1734 || itemToPickUp.type == 1867)
            {
                SoundEngine.PlaySound(SoundID.Grab, new Vector2((int)self.position.X, (int)self.position.Y));
                //SoundEngine.PlaySound(7, new Vector2((int)self.position.X, (int)self.position.Y), 1, 1f, 0f);
                self.Heal(20);
                itemToPickUp = new Item();
            }
            else if (itemToPickUp.type == 184 || itemToPickUp.type == 1735 || itemToPickUp.type == 1868)
            {
                SoundEngine.PlaySound(SoundID.Grab, new Vector2((int)self.position.X, (int)self.position.Y));
                //SoundEngine.PlaySound(7, (int)self.position.X, (int)self.position.Y, 1, 1f, 0f);
                self.HealMana(100, true);
                itemToPickUp = new Item();
            }
            else if (itemToPickUp.type == 4143)
            {
                SoundEngine.PlaySound(SoundID.Grab, new Vector2((int)self.position.X, (int)self.position.Y));
                //SoundEngine.PlaySound(7, (int)self.position.X, (int)self.position.Y, 1, 1f, 0f);
                self.HealMana(50, true);
                itemToPickUp = new Item();
            }
            else
            {
                itemToPickUp = self.GetItem(playerIndex, itemToPickUp, GetItemSettings.PickupItemFromWorld);
            }
            Main.item[worldItemArrayIndex] = itemToPickUp;
            if (Main.netMode == 1)
            {
                NetMessage.SendData(21, -1, -1, null, worldItemArrayIndex, 0f, 0f, 0f, 0, 0, 0);
            }
            return itemToPickUp;
        }
    }
}
