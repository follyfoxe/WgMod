using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using WgMod.Common.Players;

namespace WgMod.Content.Buffs;

public class GiftOfVoid : ModBuff
{
    float _movementSpeed = 0.3f;
    float _jumpHeight = 1.5f;
    float _movementPenalty = 0.33f;
    float _stale = 1f;
    public override bool ReApply(Player player, int time, int buffIndex)
    {
        player.buffTime[buffIndex] = Math.Min(player.buffTime[buffIndex] + time, Utility.TimeToTicks(minutes: 10));
        return true;
    }

    public override void Update(Player player, ref int buffIndex)
    {
        if (!player.TryGetModPlayer(out WgPlayer wg))
            return;
        if (!player.TryGetModPlayer(out GiftOfVoidPlayer gov))
            return;

        gov._giftOfVoidTimer++;
        wg.SetWeight(wg.Weight + 0.01f);
        if (gov._giftOfVoidTimer >= Utility.TimeToTicks(minutes: 1))
        {
            _stale = MathHelper.Lerp(1f, 0.25f, Math.Clamp((float)(gov._giftOfVoidTimer - Utility.TimeToTicks(minutes: 1)) / Utility.TimeToTicks(minutes: 3), 0f, 1f));
        }
        Main.NewText(_stale);
        player.moveSpeed += _movementSpeed * _stale;
        player.jumpSpeedBoost += _jumpHeight * _stale;
        wg.MovementPenalty *= 1f - _movementPenalty * _stale;
    }
    public override bool PreDraw(SpriteBatch spriteBatch, int buffIndex, ref BuffDrawParams drawParams)
    {
        drawParams.DrawColor = new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, (int)(Main.buffAlpha[buffIndex] * 255));
        return true;
    }
    public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
    {
        tip = this.GetLocalization("Description").Format((int)Math.Round(_movementSpeed * _stale * 100), (int)Math.Round(Utility.GetJumpSpeedIncrease(_jumpHeight * _stale) * 100), (int)Math.Round(_movementPenalty * _stale * 100));
    }
}

public class CrashDown : ModBuff
{
    public override void SetStaticDefaults()
    {
        Main.debuff[Type] = true;
    }

    public override void Update(Player player, ref int buffIndex)
    {
        if (!player.TryGetModPlayer(out WgPlayer wg))
            return;


        player.moveSpeed -= 0.15f;
        wg.MovementPenalty *= 1.33f;
    }
    public override bool PreDraw(SpriteBatch spriteBatch, int buffIndex, ref BuffDrawParams drawParams)
    {
        drawParams.DrawColor = new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, (int)(Main.buffAlpha[buffIndex] * 255));
        return true;
    }
    public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
    {
        rare = ItemRarityID.Expert;
    }
}

public class GiftOfVoidPlayer : ModPlayer
{
    public int _giftOfVoidTimer = 0;
    public override void PostUpdateBuffs()
    {
        if (_giftOfVoidTimer > 0)
        {
            if (!Player.HasBuff<GiftOfVoid>())
            {
                Player.AddBuff(ModContent.BuffType<CrashDown>(), (int)Math.Clamp(_giftOfVoidTimer / 2f, Utility.TimeToTicks(seconds: 30), Utility.TimeToTicks(minutes: 5)));
                _giftOfVoidTimer = 0;
            }
        }
    }
    public override void UpdateDead()
    {
        _giftOfVoidTimer = 0;
    }
    public override void SaveData(TagCompound tag)
    {
        tag["GiftOfVoidCounter"] = _giftOfVoidTimer;
    }
    public override void LoadData(TagCompound tag)
    {
        _giftOfVoidTimer = tag.GetInt("GiftOfVoidCounter");
    }
}
