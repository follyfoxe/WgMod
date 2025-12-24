using System;
using System.Collections.Generic;
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
    public static readonly Dictionary<int, float> BuffTable = new()
    {
        [BuffID.WellFed] = 1f,
        [BuffID.WellFed2] = 3f,
        [BuffID.WellFed3] = 6f
    };

    // TODO: Show in UI
    public Weight Weight => _weight;

    private Weight _weight;
    private Vector2 _prevVel;

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
        if (Player.mount.Active || ModContent.GetInstance<WgServerConfig>().DisableBuffs)
            return;
        float factor = _weight.ClampedImmobility;
        factor = 1f - factor * factor;
        Player.runAcceleration *= factor;
        Player.maxRunSpeed *= factor;
        Player.accRunSpeed *= factor;
    }

    public override void PreUpdateBuffs()
    {
        if (ModContent.GetInstance<WgServerConfig>().DisableBuffs)
        {
            SetBuff<ImmobileBuff>(false);
            SetBuff<FatBuff>(false);
            return;
        }
        int stage = _weight.GetStage();
        bool immobile = stage >= Weight.ImmobileStage;
        SetBuff<ImmobileBuff>(immobile);
        SetBuff<FatBuff>(stage >= Weight.BuffStage && !immobile);
    }

    void SetBuff<T>(bool set) where T : ModBuff
    {
        int type = ModContent.BuffType<T>();
        bool has = Player.HasBuff(type);
        if (set && !has)
            Player.AddBuff(type, 60);
        else if (!set && has)
            Player.ClearBuff(type);
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
