using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Configs;
using WgMod.Content.Buffs;

namespace WgMod.Common.Players;

public partial class WgPlayer : ModPlayer
{
    /// <summary> The player's weight </summary>
    public Weight Weight { get; private set; } = Weight.Base;

    /// <summary> How much movement will be reduced because of the player's weight, multiply this </summary>
    public StatModifier MovementPenalty;

    /// <summary> How fast the player will lose weight when walking, add or subtract to this </summary>
    public StatModifier WeightLossRate;

    public readonly int[] BuffDuration = new int[Player.MaxBuffs];
    internal int _ignoreWgBuffTimer = 2;

    internal float _finalKnockbackResistance;
    internal float _finalMovementFactor;
    
    internal float _buffTotalGain;
    internal int _iceBreakTimer;
    internal bool _displayWeight;

    // TODO: Split these into separate ModPlayers
    internal bool _ambrosiaOnHit; // FlaskOfAmbrosia effect
    internal bool _queenlyGluttony; // QueenlyGluttony effect
    internal bool _bottomlessAppetite; //BottomlessAppetite effect
    internal int _bottomlessAppetiteGrabRange; // How much BottomlessAppetite increases grab range
    internal bool _championsBelt;
    internal float _championsBeltMeleeScale;

    float _lastGfxOffY;
    Vector2 _prevVel;

    public override void Initialize()
    {
        SetWeight(Weight.Base, false);
        InitializeVisuals();
    }

    public override void OnEnterWorld()
    {
        _ignoreWgBuffTimer = 2;
    }

    public void SetWeight(Weight weight, bool effects = true)
    {
        int prevStage = Weight.GetStage();
        Weight = Weight.Clamp(weight);
        if (Weight.GetStage() != prevStage && effects)
        {
            SoundEngine.PlaySound(new SoundStyle("WgMod/Assets/Sounds/Belly_", 3, SoundType.Sound));
            _squishPos += 0.06f;
        }
    }

    public override void ResetEffects()
    {
        // Resets some effects
        _ambrosiaOnHit = false;
        _queenlyGluttony = false;
        _bottomlessAppetite = false;
        _championsBelt = false;

        // Custom stats
        MovementPenalty = StatModifier.Default;
        WeightLossRate = StatModifier.Default;
    }

    public override void PreUpdateBuffs()
    {
        // Ensure fat buff
        int type = ModContent.BuffType<FatBuff>();
        if (!Player.HasBuff(type))
            Player.AddBuff(type, 60);
    }

    public override void PostUpdateRunSpeeds()
    {
        if (WgServerConfig.Instance.DisableFatBuffs || Player.mount.Active)
            return;

        int stage = Weight.GetStage();
        if (stage >= Weight.DamageReductionStage)
        {
            if (stage < Weight.ImmobileStage)
                _finalKnockbackResistance = float.Lerp(0f, 0.6f, Weight.GetClampedFactor(Weight.FromStage(Weight.DamageReductionStage), Weight.Immobile));
            else
                _finalKnockbackResistance = 1f;
        }
        else
            _finalKnockbackResistance = 0f;

        float basePenalty;
        if (stage < Weight.ImmobileStage)
        {
            float immobility = Weight.ClampedImmobility;
            basePenalty = float.Lerp(0f, 0.7f, immobility * immobility);
        }
        else
            basePenalty = 1f;
        _finalMovementFactor = Math.Clamp(1f - MovementPenalty.ApplyTo(basePenalty), 0f, 1f);

        Player.runAcceleration *= _finalMovementFactor;
        Player.maxRunSpeed *= _finalMovementFactor;
        Player.accRunSpeed *= _finalMovementFactor;

        if (Player.whoAmI == Main.myPlayer) // If it's the local player
            Player.jumpSpeed = float.Lerp(Player.jumpSpeed * 0.2f, Player.jumpSpeed, _finalMovementFactor);
    }

    public override void PostUpdateMiscEffects()
    {
        Vector2 acc = Player.velocity - _prevVel;
        _prevVel = Player.velocity;
        _squishRest = 1f;

        int stage = Weight.GetStage();
        ResizeHitbox(stage);

        // Weight loss
        float factor = MathF.Abs(Player.velocity.X);
        factor += MathF.Abs(acc.X) * 20f;
        factor *= 0.0002f;
        SetWeight(Weight - WeightLossRate.ApplyTo(factor));

        // Ice break
        if (stage >= Weight.HeavyStage)
        {
            const int IceBreakTime = 60;
            if (Player.velocity.Y > -0.01f && HasIceBelow())
            {
                if (_iceBreakTimer == IceBreakTime / 2)
                    SoundEngine.PlaySound(SoundID.Item127);
                _iceBreakTimer++;
                if (_iceBreakTimer > IceBreakTime)
                    ThinIceBreak();
            }
            else
                _iceBreakTimer = 0;
        }
    }

    void ResizeHitbox(int stage)
    {
        int targetWidth = Player.defaultWidth;
        if (!WgServerConfig.Instance.DisableFatHitbox && !Player.mount.Active && !Player.isLockedToATile)
            targetWidth = WeightValues.GetHitboxWidthInTiles(stage) * 16 - 12;
        if (Player.width != targetWidth)
        {
            float centerX = Player.position.X + Player.width * 0.5f;
            float targetX = centerX - targetWidth * 0.5f;
            // Make sure we have enough space... otherwise we'd be able to walk through walls
            if (!Collision.SolidCollision(new Vector2(targetX, Player.position.Y), targetWidth, Player.height))
            {
                Player.width = targetWidth;
                Player.position.X = targetX;
            }
            else
                _squishRest = 1.2f;
        }
    }

    public override void PreUpdate()
    {
        PreUpdateVisuals();
    }

    public override void PostUpdate()
    {
        UpdateJiggle();
        PostUpdateVisuals();

        if (_ignoreWgBuffTimer > 0)
            _ignoreWgBuffTimer--;
    }

    public override void UpdateDead()
    {
        _ignoreWgBuffTimer = 2;
    }

    // Taken from CheckIceBreak() in Player.cs
    void ThinIceBreak()
    {
        Vector2 pos = Player.position + Player.velocity;
        int xStart = (int)(pos.X / 16.0);
        int xEnd = (int)(((double)pos.X + Player.width) / 16.0);
        int yStart = (int)(((double)Player.position.Y + Player.height + 1.0) / 16.0);
        for (int x = xStart; x <= xEnd; x++)
        {
            for (int y = yStart; y <= yStart + 1 && Main.tile[x, y] != null; y++)
            {
                Tile tile = Main.tile[x, y];
                if (tile.HasUnactuatedTile && tile.TileType == TileID.BreakableIce && !WorldGen.SolidTile(x, y - 1))
                {
                    WorldGen.KillTile(x, y);
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                        NetMessage.SendData(MessageID.TileManipulation, number2: x, number3: y);
                }
            }
        }
    }

    // Not exactly proud of this...
    bool HasIceBelow()
    {
        Vector2 pos = Player.position + Player.velocity;
        int xStart = (int)(pos.X / 16.0);
        int xEnd = (int)(((double)pos.X + Player.width) / 16.0);
        int yStart = (int)(((double)Player.position.Y + Player.height + 1.0) / 16.0);
        for (int x = xStart; x <= xEnd; x++)
        {
            for (int y = yStart; y <= yStart + 2; y++)
            {
                Tile tile = Main.tile[x, y];
                if (tile.HasUnactuatedTile && tile.TileType == TileID.BreakableIce && !WorldGen.SolidTile(x, y - 1))
                    return true;
            }
        }
        return false;
    }

    public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
    {
        SetWeight(new Weight(Weight.Mass * WeightValues.GetDeathPenalty(Player.difficulty)));
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (_queenlyGluttony && (hit.DamageType == DamageClass.Melee || hit.DamageType == DamageClass.MeleeNoSpeed)) // If QueenlyGluttony is equipped and player is using melee
        {
            if (Main.rand.NextBool(50))
                target.AddBuff(BuffID.Shimmer, 2 * 60); // 1/50 chance to apply shimmer to enemy for 2 seconds
            else
                target.AddBuff(BuffID.GelBalloonBuff, 2 * 60); // 49/50 chance to apply Sparkle Slime Balloon effect to enemy for 2 seconds
        }
    }

    public override void OnHurt(Player.HurtInfo info)
    {
        if (_ambrosiaOnHit) // If FlaskOfAmbrosia is equipped
            Player.AddBuff(ModContent.BuffType<AmbrosiaGorged>(), 8 * 60); // Apply AmbrosiaGorged to player for 8 seconds when taking damage
    }

    public override void ModifyHurt(ref Player.HurtModifiers modifiers)
    {
        if (Player.noKnockback)
            return;
        Player.noKnockback = true;
        modifiers.KnockbackImmunityEffectiveness *= _finalKnockbackResistance;
    }

    public override void PostHurt(Player.HurtInfo info)
    {
        Player.noKnockback = false;
    }

    public override void ResetInfoAccessories()
    {
        _displayWeight = false;
    }

    public override void RefreshInfoAccessoriesFromTeamPlayers(Player otherPlayer)
    {
        if (otherPlayer.Wg()._displayWeight)
            _displayWeight = true;
    }
}
