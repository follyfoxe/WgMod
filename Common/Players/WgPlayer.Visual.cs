using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using WgMod.Common.Configs;

namespace WgMod.Common.Players;

public partial class WgPlayer
{
    internal float _squishRest = 1f;
    internal float _squishPos = 1f;
    internal float _squishVel;
    internal float _bellyOffset;

    internal readonly WgArmor.Layer[] _armorLayers = new WgArmor.Layer[2];
    internal RenderTarget2D _armorTarget;
    internal int _lastBodySlot;

    void InitializeVisuals()
    {
        if (!WgClientConfig.Instance.DisableUVClothes)
        {
            Main.RunOnMainThread(() =>
            {
                WgPlayerDrawLayer.SetupArmorLayers(this);
                WgArmor.Render(ref _armorTarget, _armorLayers);
            });
        }
    }

    void PreUpdateVisuals()
    {
        Player.gfxOffY = _lastGfxOffY;
    }

    void PostUpdateVisuals()
    {
        if (!WgClientConfig.Instance.DisableUVClothes)
            WgArmor.Render(ref _armorTarget, _armorLayers);

        // Can't find a better way to change the draw position
        _lastGfxOffY = Player.gfxOffY;
        Player.gfxOffY -= WeightValues.DrawOffsetY(Weight.GetStage()) * Player.gravDir;
    }

    void UpdateJiggle()
    {
        const float dt = 1f / 60f;
        if (Main.dedServ || WgClientConfig.Instance.DisableJiggle)
        {
            _squishVel = 0f;
            _squishPos = 1f;
        }
        else
        {
            Vector2 vel = Player.velocity;
            vel.Y += _bellyOffset * 0.6f;

            _squishPos += MathF.Abs(vel.X) * 0.005f;
            _squishPos += vel.Y * 0.005f;

            _squishVel += (_squishRest - _squishPos) * 400f * dt;
            _squishVel = float.Lerp(_squishVel, 0f, 1f - MathF.Exp(-6f * dt));
            _squishPos += _squishVel * dt;
            _squishPos = Math.Clamp(_squishPos, 0.5f, 1.5f);
        }
    }

    public override void HideDrawLayers(PlayerDrawSet drawInfo)
    {
        int stage = Weight.GetStage();
        if (stage >= 5)
        {
            foreach (PlayerDrawLayer drawLayer in PlayerDrawLayerLoader.Layers)
            {
                if (drawLayer == PlayerDrawLayers.Skin || drawLayer == PlayerDrawLayers.Torso || drawLayer == PlayerDrawLayers.Leggings)
                    drawLayer.Hide();
            }
        }
    }

    // Being used as a PreDraw kind of thing
    public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
    {
        if (drawInfo.shadow == 0f)
        {
            _lastBodySlot = Player.body;
            int stage = Weight.GetStage();
            int armStage = WeightValues.GetArmStage(stage);
            if (armStage >= 0)
            {
                Player.body = WgArms.GetArmEquipSlot(Mod, armStage);
                drawInfo.armorHidesArms = true;
                drawInfo.armorHidesHands = true;
            }
        }
    }

    public override void TransformDrawData(ref PlayerDrawSet drawInfo)
    {
        // Sticking with this for now...
        int stage = Weight.GetStage();
        int armStage = WeightValues.GetArmStage(stage);
        if (armStage >= 0)
        {
            Texture2D armTexture = WgArms.ArmTextures[armStage].Value;
            foreach (ref DrawData data in CollectionsMarshal.AsSpan(drawInfo.DrawDataCache))
            {
                if (data.texture == armTexture)
                {
                    data.color = drawInfo.colorBodySkin;
                    data.shader = 0;
                }
            }
        }
        if (drawInfo.shadow == 0f)
            Player.body = _lastBodySlot;
    }
}
