using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Buffs;

public class GiftOfVoid : ModBuff
{
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

        float staleMultiplier = 1f;

        if (gov._giftOfVoidTimer >= Utility.TimeToTicks(minutes: 1))
        {
            staleMultiplier = MathHelper.Lerp(1f, 0.25f, (gov._giftOfVoidTimer - Utility.TimeToTicks(minutes: 1)) / Utility.TimeToTicks(minutes: 3));
        }

        player.moveSpeed += 0.3f * staleMultiplier;
        player.jumpSpeedBoost += 0.1f * staleMultiplier;
        wg.MovementPenalty *= 1f - 0.33f * staleMultiplier;
    }
    public override bool PreDraw(SpriteBatch spriteBatch, int buffIndex, ref BuffDrawParams drawParams)
    {
        drawParams.DrawColor = new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, (int)(Main.buffAlpha[buffIndex] * 255));
        return true;
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
}
