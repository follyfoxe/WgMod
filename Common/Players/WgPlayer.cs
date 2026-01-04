using System;
using System.IO;
using System.Runtime.InteropServices;
using Humanizer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using WgMod.Common.Configs;
using WgMod.Content.Buffs;
using WgMod.Content.Tiles;

namespace WgMod.Common.Players;

public class WgPlayer : ModPlayer
{
    /// <summary> The player's weight </summary>
    public Weight Weight { get; private set; }

    /// <summary> How much movement will be reduced because of the player's weight, multiply this </summary>
    public StatModifier MovementPenalty;

    /// <summary> How fast the player will lose weight when walking, add or subtract to this </summary>
    public StatModifier WeightLossFactor;

    public readonly int[] BuffDuration = new int[Player.MaxBuffs];

    internal bool _onTreadmill;
    internal float _treadmillX;

    internal float _squishRest = 1f;
    internal float _squishPos = 1f;
    internal float _squishVel;
    internal float _bellyOffset;

    internal float _finalMovementFactor;
    internal float _buffTotalGain;
    internal int _iceBreakTimer;
    internal bool _ambrosiaOnHit;
    internal bool _queenlyGluttony;

    float _lastGfxOffY;
    Vector2 _prevVel;

    public override void Load()
    {
        if (Main.netMode != NetmodeID.Server)
            WgArms.Load(Mod);
    }

    public override void SetStaticDefaults()
    {
        if (Main.netMode != NetmodeID.Server)
            WgArms.SetupDrawing(Mod);
    }

    public override void Initialize()
    {
        SetWeight(Weight.Base, false);
    }

    public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
    {
        ModPacket packet = Mod.GetPacket();
        packet.Write((byte)WgMod.MessageType.WgPlayerSync);
        packet.Write((byte)Player.whoAmI);
        packet.Write(Weight.Mass);
        packet.Send(toWho, fromWho);
    }

    public void ReceivePlayerSync(BinaryReader reader)
    {
        SetWeight(new Weight(reader.ReadSingle()));
    }

    public override void CopyClientState(ModPlayer targetCopy)
    {
        WgPlayer clone = (WgPlayer)targetCopy;
        clone.SetWeight(Weight, false);
    }

    public override void SendClientChanges(ModPlayer clientPlayer)
    {
        WgPlayer clone = (WgPlayer)clientPlayer;
        if (Weight != clone.Weight)
            SyncPlayer(-1, Main.myPlayer, false);
    }

    public override void LoadData(TagCompound tag)
    {
        if (tag.TryGet("Weight", out float w))
            SetWeight(new Weight(w), false);
    }

    public override void SaveData(TagCompound tag)
    {
        tag["Weight"] = Weight.Mass;
    }

    public void SetWeight(Weight weight, bool effects = true)
    {
        int prevStage = Weight.GetStage();
        Weight = Weight.Clamp(weight);
        if (Weight.GetStage() != prevStage && effects)
            SoundEngine.PlaySound(new SoundStyle("WgMod/Assets/Sounds/Belly_", 3, SoundType.Sound));
    }

    public override void PreUpdateBuffs()
    {
        // Ensure fat buff
        int type = ModContent.BuffType<FatBuff>();
        if (!Player.HasBuff(type))
            Player.AddBuff(type, 60);

        // Custom stats
        MovementPenalty = StatModifier.Default;
        WeightLossFactor = StatModifier.Default;
    }

    public override void PostUpdateRunSpeeds()
    {
        if (ModContent.GetInstance<WgServerConfig>().DisableFatBuffs || Player.mount.Active)
            return;

        float basePenalty;
        int stage = Weight.GetStage();
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

    public override void PreUpdateMovement()
    {
        if (_onTreadmill)
            WeightLossFactor += Treadmill.WeightLoss;

        Vector2 acc = Player.velocity - _prevVel;
        _prevVel = Player.velocity;
        _squishRest = 1f;

        // Weight loss
        float factor = MathF.Abs(Player.velocity.X);
        factor += MathF.Abs(acc.X) * 20f;
        factor *= 0.0002f;
        SetWeight(Weight - WeightLossFactor.ApplyTo(factor));

        // Hitbox
        int stage = Weight.GetStage();
        int targetWidth = Player.defaultWidth;
        if (!ModContent.GetInstance<WgServerConfig>().DisableFatHitbox && !Player.mount.Active && !Player.isLockedToATile)
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
                _squishRest = 1.25f;
        }

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

    public override void PreUpdate()
    {
        Player.gfxOffY = _lastGfxOffY;
        if (!Player.sitting.isSitting)
            _onTreadmill = false;
    }

    public override void PostUpdate()
    {
        const float dt = 1f / 60f;
        if (Main.dedServ || ModContent.GetInstance<WgClientConfig>().DisableJiggle)
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

        // Can't find a better way to change the draw position
        _lastGfxOffY = Player.gfxOffY;
        Player.gfxOffY -= WeightValues.DrawOffsetY(Weight.GetStage());

        if (_onTreadmill)
            Player.Center = new Vector2(_treadmillX, Player.Center.Y);
    }

    public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
    {
        int stage = Weight.GetStage();
        int armStage = WeightValues.GetArmStage(stage);
        if (armStage >= 0)
        {
            Player.body = WgArms.GetArmEquipSlot(Mod, armStage);
            drawInfo.armorHidesArms = true;
        }
        if (_onTreadmill)
        {
            drawInfo.isSitting = false;
            drawInfo.torsoOffset = 0f;
            drawInfo.seatYOffset = 0f;
        }
    }

    public override void TransformDrawData(ref PlayerDrawSet drawInfo)
    {
        // Couldn't think of a better solution
        int stage = Weight.GetStage();
        int armStage = WeightValues.GetArmStage(stage);
        if (armStage >= 0)
        {
            Texture2D armTexture = WgArms.ArmTextures[armStage].Value;
            foreach (ref DrawData data in CollectionsMarshal.AsSpan(drawInfo.DrawDataCache))
            {
                if (data.texture == armTexture)
                    data.color = drawInfo.colorBodySkin;
            }
        }
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

    // Resets some effects
    public override void ResetEffects()
    {
        _ambrosiaOnHit = false;
        _queenlyGluttony = false;
    }

    // For Flask of Ambrosia :3
    public override void OnHurt(Player.HurtInfo info)
    {
        if (_ambrosiaOnHit)
            Player.AddBuff(ModContent.BuffType<AmbrosiaGorged>(), 8 * 60);
    }

    // On hit effects NPCs
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (_queenlyGluttony && (hit.DamageType == DamageClass.Melee || hit.DamageType == DamageClass.MeleeNoSpeed))
        {
            if (Main.rand.NextBool(50))
                target.AddBuff(BuffID.Shimmer, 120);
            else
                target.AddBuff(BuffID.GelBalloonBuff, 120);
        }
    }
}
