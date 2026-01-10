using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using WgMod.Common.Configs;
using WgMod.Content.Buffs;
using WgMod.Content.Items.Accessories;
using WgMod.Content.Projectiles;
using WgMod.Content.Tiles;

namespace WgMod.Common.Players;

public class WgPlayer : ModPlayer
{
    /// <summary> The player's weight </summary>
    public Weight Weight = new Weight();

    //public float MovementPenalty;

    /// <summary> How fast the player will lose weight when walking, add or subtract to this number </summary>
    //public float WeightLossFactor;

    public readonly int[] BuffDuration = new int[Player.MaxBuffs];

    internal bool _onTreadmill;
    internal float _treadmillX;
    
    internal float _squishRest = 1f;
    internal float _squishPos = 1f;
    internal float _squishVel;
    internal float _bellyOffset;

    internal float _finalMovementFactor;

    internal int _iceBreakTimer;

    internal bool _ambrosiaOnHit; // FlaskOfAmbrosia effect
    internal bool _queenlyGluttony; // QueenlyGluttony effect
    internal bool _bottomlessAppetite; //BottomlessAppetite effect
    internal int _bottomlessAppetiteGrabRange; // How much BottomlessAppetite increases grab range
    internal bool _crushEffect;
    public bool _vacuumSetBonus;

    internal float _buffTotalGain;

    internal bool _displayWeight;

    internal bool _squishedCauseNoSpace;

    internal int _guideToLiftingTimer;
    internal bool _enabledGuideToLifting;

    /// <summary> How much movement will be reduced because of the player's weight, multiply this value. <br/>
    /// You can also subtract from MovementPenaltyReduction.Flat to reduce how much the player's weight in kg affects movement.
    /// </summary>
    public StatModifier MovementPenaltyReduction;

    /// <summary> How much attack speed will be reduced because of the player's weight, multiply this value. <br/>
    /// You can also subtract from AttackSpeedPenaltyReduction.Flat to reduce how much the player's weight in kg affects attack speed.
    /// </summary>
    public StatModifier AttackSpeedPenaltyReduction;

    /// <summary> How much extra max life this player gains from obesity, add to this value.</summary>
    public StatModifier BonusMaxLifeIncrease;

    /// <summary> How fast the player loses weight while doing things like walking, jumping, using tools, etc., add to this value. <br/>
    /// </summary>
    public StatModifier WeightLossRate;

    /// <summary> How fast the player gains weight, add to this value. <br/>
    /// </summary>
    public StatModifier WeightGainRate;

    /// <summary>
    /// How much the player gains every second. add to this value.
    /// </summary>
    public float PassiveWeightGain;

    /// <summary>
    /// How much the player loses every second. add to this value.
    /// </summary>
    public float PassiveWeightLoss;


    public float YVelocityOfLastTick = 0;

    public int AddedWidth = 0;
    public int AddedHeight = 0;
    public int AddedVisualHeight = 0;

    //these values are for displaying on the fat buff description.
    public float FatBuffMovementPenalty = 0;
    public int FatBuffMaxLife = 0;
    public float FatBuffAttackSpeedPenalty = 0;
    public float FatBuffDamageReduction = 0;

    /// <summary> If the player is on the ground or not.<br/>
    /// 0 = The player is in the air. <br/>
    /// 1 = The player just landed. <br/>
    /// 2 = The player is on the ground. <br/>
    /// </summary>
    public int LandState = 2;

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
    public override void ResetEffects()
    {
        _ambrosiaOnHit = false;
        _queenlyGluttony = false;
        _bottomlessAppetite = false;
        _crushEffect = false;
        _enabledGuideToLifting = false;

        FatBuffMovementPenalty = 0;
        FatBuffMaxLife = 0;
        FatBuffAttackSpeedPenalty = 0;
        FatBuffDamageReduction = 0;

        MovementPenaltyReduction = StatModifier.Default;
        AttackSpeedPenaltyReduction = StatModifier.Default;
        BonusMaxLifeIncrease = StatModifier.Default;

        WeightLossRate = StatModifier.Default;
        WeightGainRate = StatModifier.Default;

        PassiveWeightGain = 0f;
        PassiveWeightLoss = 0f;

        /*
        float Mult = ModContent.GetInstance<WgServerConfig>().WeightChangeMult;
        WeightGainRate *= Mult;
        WeightLossRate *= Mult;
        */
        
        BonusMaxLifeIncrease += 0.03f * Player.ConsumedLifeFruit;
    }

    /*public override void ModifyHurt(ref Player.HurtModifiers modifiers)
    {
        modifiers.KnockbackImmunityEffectiveness
    }*/

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
        SetWeight(reader.ReadSingle());
    }

    public override void CopyClientState(ModPlayer targetCopy)
    {
        WgPlayer clone = (WgPlayer)targetCopy;
        clone.SetWeight(Weight.Mass, false);
    }

    public override void SendClientChanges(ModPlayer clientPlayer)
    {
        WgPlayer clone = (WgPlayer)clientPlayer;
        if (Weight.Mass != clone.Weight.Mass)
            SyncPlayer(-1, Main.myPlayer, false);
    }

    public override void LoadData(TagCompound tag)
    {
        if (tag.TryGet("Weight", out float w))
            SetWeight(w, false);
        else
            SetWeight(Weight.Base, false);
    }

    public override void SaveData(TagCompound tag)
    {
        tag["Weight"] = Weight.Mass;
    }

    public void SetWeight(float weight, bool effects = true)
    {
        int lastStage = Weight.GetStage();
        Weight.SetWeight(weight);
        if (Weight.GetStage() != lastStage && effects)
            SoundEngine.PlaySound(new SoundStyle("WgMod/Assets/Sounds/Belly_", 3, SoundType.Sound));
    }
    public void AddWeight(float amount, bool effects = true)
    {
        SetWeight(Weight.Mass + WeightGainRate.ApplyTo(amount), effects);
    }
    public void ReduceWeight(float amount, bool effects = true)
    {
        SetWeight(Weight.Mass - WeightLossRate.ApplyTo(amount), effects);
    }
    public float SetMobilityMultiplier()
    {
        if (ModContent.GetInstance<WgServerConfig>().DisableFatBuffs)
            return 1f;
        float mass = Math.Max(MovementPenaltyReduction.ApplyTo(Player.WG().Weight.Mass), Player.WG().Weight.Base);
        float lerpAmount = Math.Clamp((mass - Player.WG().Weight.Base) / (Player.WG().Weight.Immobile - Player.WG().Weight.Base), 0f, 1f);
        float lerpAmount2 = Math.Clamp(lerpAmount * 1.25f, 0f, 1f);
        return float.Lerp(1f, 0f, lerpAmount * lerpAmount2);
    }
    public float SetAttackSpeedPercentage()
    {
        if (ModContent.GetInstance<WgServerConfig>().DisableFatBuffs)
            return 1f;
        float mass = Math.Max(AttackSpeedPenaltyReduction.ApplyTo(Player.WG().Weight.Mass), Player.WG().Weight.Base);
        float lerpAmount = Math.Clamp((mass - Player.WG().Weight.Base) / (Player.WG().Weight.Immobile * 1.5f - Player.WG().Weight.Base), 0f, 1f);
        float lerpAmount2 = Math.Clamp(lerpAmount * 1.25f, 0f, 1f);
        return Math.Min(float.Lerp(0f, 1f, lerpAmount * lerpAmount2) - 0.03f, 0.67f);
    }

    public List<int> ItemUseStylesForWeightLoss = new List<int>
    {
        ItemUseStyleID.Swing,
        ItemUseStyleID.Thrust,
        ItemUseStyleID.GolfPlay,
        ItemUseStyleID.Rapier,
    };
    public bool IsRunningAgainstConveyor()
    {
        List<Point> tiles = Collision.GetTilesIn(Player.Hitbox.BottomLeft() - new Vector2(-2, -2), Player.Hitbox.BottomRight() + new Vector2(2, 10));
        int RightConveyors = 0;
        int LeftConveyors = 0;
        foreach (var point in tiles)
        {
            Tile tile = Framing.GetTileSafely(point);
            if (tile.HasTile)
            {
                if (!tile.IsActuated && tile.TileType == TileID.ConveyorBeltRight)
                    RightConveyors++;
                else if (!tile.IsActuated && tile.TileType == TileID.ConveyorBeltLeft)
                    LeftConveyors++;
            }
        }
        if (RightConveyors == LeftConveyors) return false;
        if (RightConveyors > LeftConveyors && Player.controlRight)
            return true;
        if (RightConveyors < LeftConveyors && Player.controlLeft)
            return true;
        return false;
    }
    public void DoWeightLoss()
    {
        float amount = 0.05f + PassiveWeightLoss;
        Item heldItem = Player.HeldItem;

        if (!heldItem.IsAir && Player.ItemAnimationJustStarted)
            if (ItemUseStylesForWeightLoss.Contains(heldItem.useStyle))
                amount += 2f; //using a swung or thrusted weapon or tool

        if (!Player.General().HasJumped && Player.jump > 0)
        {
            Player.General().HasJumped = true;
            amount += 12.5f; //from jumping
        }
        if (!Player.IsAirborne() && !Player.mount.Active && !Player.sleeping.isSleeping && !Player.sitting.isSitting && Player.grappling[0] < 0 && !Player.pulley) //make sure the player is ACTUALLY on the ground
        {
            if ((Player.controlRight && Player.velocity.X > 0) || (Player.controlLeft && Player.velocity.X < 0))
            {
                amount += 0.5f; //running

            }
            if (IsRunningAgainstConveyor())
            {
                amount += 9.5f; //running against a conveyor
            }
        }
        else if (_onTreadmill)
        {
            if ((Player.controlRight && Player.velocity.X > 0) || (Player.controlLeft && Player.velocity.X < 0))
            {
                amount += 25f; //using a treadmill

            }
        }
        if (amount > 0)
        {
            amount /= 60f; //ticks to seconds
            amount /= 60f; //seconds to minutes
            ReduceWeight(amount);
        }
    }

    public override void PreUpdateBuffs()
    {
        // Ensure fat buff
        int type = ModContent.BuffType<FatBuff>();
        if (!Player.HasBuff(type))
            Player.AddBuff(type, 60);
    }
    public override void PostUpdateEquips()
    {
        if (ModContent.GetInstance<WgServerConfig>().DisableFatBuffs)
            return;

        if (_enabledGuideToLifting)
            GuideToLiftingYourFatAss.LiftYourFatAss(Player);

        float attackSpeed = SetAttackSpeedPercentage();

        int lifeIncrease = Weight.GetStatModifierFromWeight(-5, 1, 7f, min: 0);
        lifeIncrease = (int)BonusMaxLifeIncrease.ApplyTo(lifeIncrease);
        lifeIncrease = (int)Math.Floor(lifeIncrease / 5f) * 5;

        float damageReduction = Weight.GetStatModifierFromWeight(-0.05f, 0.01f, 20f, 5f, 0f, 0.2f);

        Player.General().AltDR += damageReduction;
        Player.statLifeMax2 += lifeIncrease;
        Player.GetAttackSpeed(DamageClass.Generic) -= attackSpeed;

        FatBuffDamageReduction = damageReduction;
        FatBuffMaxLife = lifeIncrease;
        FatBuffAttackSpeedPenalty = attackSpeed;
    }
    public override void PostUpdateRunSpeeds()
    {
        if (ModContent.GetInstance<WgServerConfig>().DisableFatBuffs) //|| Player.mount.Active)
            return;

        float MovementPenalty = SetMobilityMultiplier();
        float totalBonusMass = Player.WG().Weight.Mass - Player.WG().Weight.Base;

        if (Player.mount.Active)
        {
            MovementPenalty = Math.Clamp(MovementPenalty * 1.5f, 0.3f, 1f);
        }
            

        if (MovementPenalty <= 0.1f)
            MovementPenalty = 0f;

        FatBuffMovementPenalty = 1f - MovementPenalty;

        Player.runAcceleration *= MovementPenalty;
        Player.maxRunSpeed *= MovementPenalty;
        Player.accRunSpeed *= MovementPenalty;
        Player.maxRunSpeed *= MovementPenalty;
        Player.jumpSpeed *= Math.Max(MovementPenalty, 0.01f);

        Player.gravity += totalBonusMass / 1280;
        Player.maxFallSpeed += (totalBonusMass / 128);

    }

    public override void PostUpdateMiscEffects()
    {
        _squishRest = 1f;

        ResizeHitbox(Player);
        if (Player.statLife >= Player.statLifeMax2) //overheal effects
        {
            if (Player.HasBuff(BuffID.Honey))
                PassiveWeightGain += 0.025f; //gain 0.025kg every second
            if (Player.HasBuff(BuffID.SoulDrain) && Player.soulDrain > 0)
                PassiveWeightGain += 0.03f * Player.soulDrain; //gain 0.03kg every second, amount is multiplied by every enemy being hit by life drain
            if (Player.HasBuff(BuffID.HeartyMeal))
                PassiveWeightGain += 0.035f;
        }
        AddWeight(PassiveWeightGain / 60f);
        DoWeightLoss();
    }

    public override void PreUpdateMovement()
    {
        _prevVel = Player.velocity;
        int stage = Weight.GetStage();
        if (stage >= 4)
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
        Player.gfxOffY -= WeightValues.DrawOffsetY(Weight.GetStage()) * Player.gravDir;
        if (Player.gravDir == -1f && Weight.GetStage() == 7)
            Player.gfxOffY -= 5f;
        if (_onTreadmill)
            Player.Center = new Vector2(_treadmillX, Player.Center.Y);


        CheckForSolidGround(Player);

        if (_crushEffect)
        {
            Projectile hitbox = null;
            foreach (var proj in Main.ActiveProjectiles)
            {
                if (proj.active && proj.type == ModContent.ProjectileType<Girth>() && proj.ai[0] == Player.whoAmI)
                {
                    hitbox = proj;
                    break;
                }
            }
            if (hitbox == null)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(Player.GetSource_FromThis(), Player.position, Vector2.Zero, ModContent.ProjectileType<Girth>(), 0, 0, Player.whoAmI);
                else if (Player == Main.LocalPlayer)
                    Projectile.NewProjectile(Player.GetSource_FromThis(), Player.position, Vector2.Zero, ModContent.ProjectileType<Girth>(), 0, 0, Player.whoAmI);
            }
            if (LandState == 1)
            {
                float crushPower = YVelocityOfLastTick * Player.WG().Weight.Mass / 120f - 10;

                if (crushPower > 10)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, ModContent.ProjectileType<Girthquake>(), (int)crushPower, 0, Player.whoAmI, 0, 8f + crushPower / 12f);
                    else if (Player == Main.LocalPlayer)
                        Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, ModContent.ProjectileType<Girthquake>(), (int)crushPower, 0, Player.whoAmI, 0, 8f + crushPower / 12f);
                }
            }
        }
        YVelocityOfLastTick = Player.velocity.Y;

        Player.itemLocation += new Vector2(0, AddedHeight);
    }
   
    public bool CheckForSolidGround(Player player)
    {
        List<Point> tiles = Collision.GetTilesIn(player.Hitbox.BottomLeft() - new Vector2(-2, -2), player.Hitbox.BottomRight() + new Vector2(2, 6));
        bool HasSolidTile = false;
        foreach (var point in tiles)
        {
            Tile tile = Framing.GetTileSafely(point);
            if (tile.HasTile)
            {
                if (Main.tileSolid[tile.TileType])
                    HasSolidTile = true;
                if (Main.tileSolidTop[tile.TileType])
                    HasSolidTile = true;
            }
        }
        if (HasSolidTile)
        {
            if (LandState == 0)
                LandState = 1;
            else
                LandState = 2;
        }
        else
            LandState = 0;
        return HasSolidTile;
    }

    public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
    {
        if (_onTreadmill)
        {
            drawInfo.isSitting = false;
            drawInfo.torsoOffset = 0f;
            drawInfo.seatYOffset = 0f;
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

    public void ResizeHitbox(Player self)
    {
        int stage = Weight.GetStage();
        int targetWidth = Player.defaultWidth;
        int targetHeight = Player.defaultHeight;
        if (!ModContent.GetInstance<WgServerConfig>().DisableFatHitbox && !Player.mount.Active && !Player.isLockedToATile)
        {
            Vector2 newSize = WeightValues.GetHitboxSize(stage);
            targetWidth = (int)newSize.X;
            targetHeight = Math.Max((int)newSize.Y - 11, Player.defaultHeight);

            AddedWidth = targetWidth - Player.defaultWidth;
            AddedHeight = targetHeight - Player.defaultHeight;
            AddedVisualHeight = Math.Clamp(Math.Max((int)newSize.Y, Player.defaultHeight) - Player.defaultHeight, 0, 11);
        }
        if (Player.width != targetWidth)
        {
            float centerX = Player.position.X + Player.width * 0.5f;
            float targetX = centerX - targetWidth * 0.5f;
            // Make sure we have enough space... otherwise we'd be able to walk through walls
            if (!Collision.SolidCollision(new Vector2(targetX, Player.position.Y), targetWidth, Player.height))
            {
                Player.width = targetWidth;
                Player.position.X = targetX;
                _squishedCauseNoSpace = false;
                _squishRest = 1.5f;
                //Main.NewText("Updated Width!");
            }
            else
                _squishedCauseNoSpace = true;
        }

        self.position.Y = self.position.Y + (float)self.height;
        //self.position.X = self.position.X + (float)self.width;
        self.height = 42 + self.HeightOffsetBoost + self.WG().AddedHeight;
        //self.width = 20 + self.WG().AddedWidth;
        self.position.Y = self.position.Y - (float)self.height;
        //self.position.X = self.position.X - (float)self.width;
        
    }

    public override void HideDrawLayers(PlayerDrawSet drawInfo)
    {
        int stage = Player.WG().Weight.GetStage();
        int armstage = WeightValues.GetArmStage(stage);
        foreach (PlayerDrawLayer drawLayer in PlayerDrawLayerLoader.Layers)
        {
            if (drawLayer == PlayerDrawLayers.ArmOverItem && armstage > 0)
                drawLayer.Hide();
            else if ((drawLayer == PlayerDrawLayers.Skin || drawLayer == PlayerDrawLayers.Torso || drawLayer == PlayerDrawLayers.Leggings) && stage >= 5)
                drawLayer.Hide();
        }
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

