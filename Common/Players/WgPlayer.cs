using System;
using System.IO;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using WgMod.Common.Configs;
using WgMod.Content.Buffs;

namespace WgMod.Common.Players;

public class WgPlayer : ModPlayer
{
    public Weight Weight => _weight;

    public readonly int[] buffDuration = new int[Player.MaxBuffs];

    internal float _buffTotalGain;
    internal float _movementFactor;

    internal float _squishPos = 1f;
    internal float _squishVel;
    internal float _bellyOffset;

    Weight _weight;
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
        _weight = new Weight(Weight.Base);
    }

    public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
    {
        ModPacket packet = Mod.GetPacket();
        packet.Write((byte)WgMod.MessageType.WgPlayerSync);
        packet.Write((byte)Player.whoAmI);
        packet.Write(_weight.Mass);
        packet.Send(toWho, fromWho);
    }

    public void ReceivePlayerSync(BinaryReader reader)
    {
        SetWeight(new Weight(reader.ReadSingle()));
    }

    public override void CopyClientState(ModPlayer targetCopy)
    {
        WgPlayer clone = (WgPlayer)targetCopy;
        clone.SetWeight(_weight, false);
    }

    public override void SendClientChanges(ModPlayer clientPlayer)
    {
        WgPlayer clone = (WgPlayer)clientPlayer;
        if (_weight != clone._weight)
            SyncPlayer(-1, Main.myPlayer, false);
    }

    public override void LoadData(TagCompound tag)
    {
        if (tag.TryGet("Weight", out float w))
            SetWeight(new Weight(w), false);
    }

    public override void SaveData(TagCompound tag)
    {
        tag["Weight"] = _weight.Mass;
    }

    public void SetWeight(Weight weight, bool effects = true)
    {
        int prevStage = _weight.GetStage();
        _weight = Weight.Clamp(weight);
        if (_weight.GetStage() != prevStage && effects)
            SoundEngine.PlaySound(new SoundStyle("WgMod/Assets/Sounds/Belly_", 3, SoundType.Sound));
    }

    public override void PostUpdateRunSpeeds()
    {
        if (ModContent.GetInstance<WgServerConfig>().DisableFatBuffs || Player.mount.Active)
            return;
        Player.runAcceleration *= _movementFactor;
        Player.maxRunSpeed *= _movementFactor;
        Player.accRunSpeed *= _movementFactor;
    }

    public override void PreUpdateBuffs()
    {
        int type = ModContent.BuffType<FatBuff>();
        if (!Player.HasBuff(type))
            Player.AddBuff(type, 60);
    }

    public override void PostUpdateBuffs()
    {
        if (_movementFactor < 0.01f)
            Player.jumpSpeedBoost = -4f;
    }

    public override void PreUpdateMovement()
    {
        Vector2 acc = Player.velocity - _prevVel;
        _prevVel = Player.velocity;

        // Weight loss
        float factor = MathF.Abs(Player.velocity.X);
        factor += MathF.Abs(acc.X) * 30f;
        factor *= 0.0002f;
        SetWeight(_weight - factor);
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

            _squishVel += (1f - _squishPos) * 400f * dt;
            _squishVel = float.Lerp(_squishVel, 0f, 1f - MathF.Exp(-6f * dt));
            _squishPos += _squishVel * dt;
            _squishPos = Math.Clamp(_squishPos, 0.5f, 1.5f);
        }
    }

    public override void FrameEffects()
    {
        int stage = Weight.GetStage();
        int armStage = WgArms.GetArmStage(stage);
        if (armStage >= 0)
            Player.body = WgArms.GetArmEquipSlot(Mod, armStage);
    }

    public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
    {
        drawInfo.Position.Y -= WgPlayerDrawLayer.CalculateOffsetY(_weight);
        drawInfo.backShoulderOffset.X += 32f;
    }
}
