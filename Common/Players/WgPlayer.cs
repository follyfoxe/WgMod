using System;
using System.IO;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using WgMod.Common.Configs;
using WgMod.Content.Buffs;

namespace WgMod.Common.Players;

public class WgPlayer : ModPlayer
{
    public Weight Weight => _weight;

    internal float _movementFactor;

    Weight _weight;
    Vector2 _prevVel;

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
        float factor = MathF.Abs(Player.velocity.X);
        factor += MathF.Abs(Player.velocity.X - _prevVel.X) * 40f;
        factor *= 0.001f;

        SetWeight(_weight - factor);
        _prevVel = Player.velocity;
    }

    public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
    {
        drawInfo.Position.Y -= WgPlayerDrawLayer.CalculateOffsetY(_weight);
        drawInfo.backShoulderOffset.X += 32f;
    }
}
