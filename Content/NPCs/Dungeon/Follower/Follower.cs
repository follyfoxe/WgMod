using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using WgMod.Content.Buffs;
using WgMod.Content.Dusts;
using WgMod.Content.Items.Consumables;
using WgMod.Content.Items.Placeable.Furniture.Banners;

namespace WgMod.Content.NPCs.Dungeon.Follower;

[Credit(ProjectRole.Programmer, Contributor.jumpsu2)]
public class Follower : ModNPC
{
    public static int BestiaryColor { get; private set; } = 1;

    public List<(Vector2, int)> FollowingList = [];
    public List<(Vector2, Rectangle)> TrailList = [];

    public float spawnDustAngle = -1f;

    public override void SetStaticDefaults()
    {
        NPCID.Sets.DoesntDespawnToInactivityAndCountsNPCSlots[NPC.type] = true;
        NPCID.Sets.ImmuneToRegularBuffs[NPC.type] = true;
        NPCID.Sets.CantTakeLunchMoney[NPC.type] = true;
        BestiaryColor = Main.rand.Next(1, 19);
    }

    public override void SetDefaults()
    {
        NPC.width = 16;
        NPC.height = 32;
        NPC.noTileCollide = true;
        NPC.noGravity = true;
        NPC.dontTakeDamage = true;
        NPC.knockBackResist = 0f;
        NPC.value = 900;
        NPC.lifeMax = 225;
        NPC.npcSlots = 0.5f;
        NPC.damage = 38;

        NPC.HitSound = SoundID.NPCHit7;
        NPC.DeathSound = SoundID.Zombie5;

        Banner = Type;
        BannerItem = ModContent.ItemType<FollowerBanner>();
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        bestiaryEntry.Info.AddRange([
            BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheDungeon,
            new FlavorTextBestiaryInfoElement("Mods.WgMod.Bestiary.Follower")
        ]);
    }

    static Color GetFollowerColor(int ID)
    {
        int colorIndex = ID % 18;
        if (colorIndex == 0)
            colorIndex = 18;

        Color color = new(1f, 0f, 0f);
        if (colorIndex <= 3)
        {
            color = Color.Lerp(new Color(1f, 0f, 0f), new Color(1f, 1f, 0f), colorIndex / 3f);
        }
        else if (colorIndex <= 6)
        {
            colorIndex -= 3;
            color = Color.Lerp(new Color(1f, 1f, 0f), new Color(0f, 1f, 0f), colorIndex / 3f);
        }
        else if (colorIndex <= 9)
        {
            colorIndex -= 6;
            color = Color.Lerp(new Color(0f, 1f, 0f), new Color(0f, 1f, 1f), colorIndex / 3f);
        }
        else if (colorIndex <= 12)
        {
            colorIndex -= 9;
            color = Color.Lerp(new Color(0f, 1f, 1f), new Color(0f, 0f, 1f), colorIndex / 3f);
        }
        else if (colorIndex <= 15)
        {
            colorIndex -= 12;
            color = Color.Lerp(new Color(0f, 0f, 1f), new Color(1f, 0f, 1f), colorIndex / 3f);
        }
        else if (colorIndex <= 18)
        {
            colorIndex -= 15;
            color = Color.Lerp(new Color(1f, 0f, 1f), new Color(1f, 0f, 0f), colorIndex / 3f);
        }
        return color;
    }

    void SmallDust(Vector2 Position, Vector2 Velocity, float Scale)
    {
        Dust.NewDustPerfect(Position, ModContent.DustType<FollowerDustSmall>(), Velocity, 0, GetFollowerColor((int)NPC.ai[0]), Scale);
    }

    void BigDust(Vector2 Position, Vector2 Velocity, float Scale)
    {
        Dust.NewDustPerfect(Position, ModContent.DustType<FollowerDustBig>(), Velocity, 0, GetFollowerColor((int)NPC.ai[0]), Scale);
    }

    void Poof(float Base, int Max, Vector2? At = null)
    {
        if (At == null)
            At = NPC.Center;
        float degrees = 0;
        while (degrees < 360)
        {
            float power = Main.rand.Next(20, 95) / 10f;
            Vector2 velocity = new Vector2(0, 1).RotatedBy(MathHelper.ToRadians(degrees));
            float scale = Base + Main.rand.Next(0, Max) / 10f;
            BigDust((Vector2)At, velocity * power, scale);
            degrees += 12.76f;
        }
    }

    void TeleportFX(Vector2 At, Vector2 To)
    {
        Poof(1, 30, At);
        Poof(1, 30, To);

        for (int i = 0; i < 10; i++)
        {
            float power = Main.rand.Next(50, 250) / 10f;
            Vector2 velocity = new Vector2(1, 0).RotatedBy(At.AngleTo(To));
            float scale = Main.rand.Next(0, 35) / 10f + 1.5f;
            BigDust(At + new Vector2(Main.rand.Next(-32, 33), Main.rand.Next(-32, 33)), velocity * power, scale);
        }
        for (int i = 0; i < 10; i++)
        {
            float power = Main.rand.Next(50, 250) / 10f;
            Vector2 velocity = new Vector2(1, 0).RotatedBy(To.AngleTo(At));
            float scale = Main.rand.Next(0, 35) / 10f + 1.5f;
            BigDust(To + new Vector2(Main.rand.Next(-32, 33), Main.rand.Next(-32, 33)), velocity * power, scale);
        }
    }

    public override void OnSpawn(IEntitySource source)
    {
        NPC.TargetClosest(false);
        if (!NPC.HasValidTarget)
        {
            NPC.active = false;
            return;
        }
        int indexInCongaLine = -1;
        bool[] takenIndexes = new bool[200];
        foreach (NPC follower in Main.ActiveNPCs)
        {
            if (follower.type != NPC.type)
                continue;
            if (follower.target != NPC.target)
                continue;
            takenIndexes[(int)follower.ai[0]] = true;
        }
        for (int i = 0; i < takenIndexes.Length; i++)
        {
            if (!takenIndexes[i])
            {
                indexInCongaLine = i;
                break;
            }
        }
        if (indexInCongaLine == -1)
        {
            NPC.active = false;
            return;
        }
        NPC.ai[0] = indexInCongaLine;
    }

    public override void AI()
    {
        if (spawnDustAngle == -1f)
            spawnDustAngle = Main.rand.Next(0, 361);
        if (!NPC.HasValidTarget)
        {
            NPC.active = false;
            Poof(1, 30);
            return;
        }

        Player target = Main.player[NPC.target];

        if (FollowingList.Count == 0)
            NPC.Center = target.Center;

        int playerState = 0;
        if (target.velocity.X != 0 && target.velocity.Y == 0)
            playerState = 1;
        else if (target.velocity.Y != 0)
            playerState = 2;

        (Vector2, int) addToList = (target.Center, playerState);

        FollowingList.Add(addToList);

        NPC.ai[3]++;

        if (FollowingList.Count >= 35 * NPC.ai[0])
        {
            if (NPC.dontTakeDamage)
            {
                NPC.dontTakeDamage = false;
                NPC.ai[3] = 0;
            }

            if (NPC.Center.Distance(FollowingList[0].Item1) > 32)
                TeleportFX(NPC.Center, FollowingList[0].Item1);

            NPC.Center = FollowingList[0].Item1;
            if (NPC.ai[1] != FollowingList[0].Item2)
            {
                NPC.frameCounter = 0;
            }
            NPC.ai[1] = FollowingList[0].Item2;
            NPC.direction = NPC.Center.X > FollowingList[1].Item1.X ? 1 : -1;
            FollowingList.RemoveAt(0);
        }

        if (NPC.dontTakeDamage)
        {
            if (NPC.ai[3] % 3 == 0)
            {
                float power = Main.rand.Next(35, 50) / 10f;
                Vector2 velocity = new Vector2(1, 0).RotatedBy(MathHelper.ToRadians(spawnDustAngle));
                BigDust(NPC.Center, velocity * power, 4f);
                spawnDustAngle += 15f;
            }
        }

        if (!NPC.dontTakeDamage && NPC.ai[3] > 900)
            NPC.npcSlots = 0f;
    }

    public override void PostAI()
    {
        NPC.velocity = Vector2.Zero;

        if (Main.rand.NextBool(3))
        {
            float power = Main.rand.Next(0, 5) / 10f;
            Vector2 velocity = new Vector2(0, 1).RotateRandom(MathHelper.ToRadians(360));
            float scale = Main.rand.Next(0, 15) / 10f + 1f;
            SmallDust(PosWithinNPC(), velocity * power, scale);
        }
    }

    public override void FindFrame(int frameHeight)
    {
        NPC.frameCounter++;
        int yOffset = 0;

        int frameToUse;
        switch (NPC.ai[1])
        {
            case 1:
                frameToUse = (int)Math.Floor(NPC.frameCounter / 6f) % 6;
                yOffset = 240;
                break;
            case 2:
                frameToUse = (int)Math.Floor(NPC.frameCounter / 6f) % 4;
                yOffset = 600;
                break;
            default:
                frameToUse = (int)Math.Floor(NPC.frameCounter / 6f) % 4;
                break;
        }

        NPC.frame = new Rectangle(0, 60 * frameToUse + yOffset, 70, 60);

        TrailList.Add((NPC.Center, NPC.frame));
        if (TrailList.Count > 15)
            TrailList.RemoveAt(0);
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        if (NPC.dontTakeDamage && !NPC.IsABestiaryIconDummy)
            return false;

        Texture2D sprite = ModContent.Request<Texture2D>("WgMod/Content/NPCs/Dungeon/Follower/Follower").Value;
        Texture2D sprite2 = ModContent.Request<Texture2D>("WgMod/Content/NPCs/Dungeon/Follower/FollowerAfterimage").Value;

        Color color = GetFollowerColor((int)NPC.ai[0]);

        if (NPC.IsABestiaryIconDummy)
        {
            color = GetFollowerColor(BestiaryColor);
            spriteBatch.Draw(sprite, NPC.position - new Vector2(28, 0), NPC.frame, color, 0f, Vector2.Zero, 1f, SpriteEffects.FlipHorizontally, 0f);
            return false;
        }

        Main.spriteBatch.End();
        Main.spriteBatch.Begin(0, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

        for (int i = TrailList.Count - 1; i > 0; i--)
        {
            SpriteEffects effect2;
            if (i == 0)
                effect2 = TrailList[i].Item1.X > NPC.position.X ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            else
                effect2 = TrailList[i].Item1.X > TrailList[i - 1].Item1.X ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(sprite2, TrailList[i].Item1 - screenPos - new Vector2(36, 35), TrailList[i].Item2, new Color(color.R, color.G, color.B, 6 * i), 0f, Vector2.Zero, 1f, effect2, 0f);
        }

        Main.spriteBatch.End();
        Main.spriteBatch.Begin(0, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

        SpriteEffects effect = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

        spriteBatch.Draw(sprite, NPC.Center - screenPos - new Vector2(36, 35), NPC.frame, color, 0f, Vector2.Zero, 1f, effect, 0f);
        return false;
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        if (Main.netMode == NetmodeID.Server)
            return;

        int damageDone = (int)(hit.Damage / 8f);
        int loops = 0;
        while (damageDone > 0 && loops < 12)
        {
            float power = Main.rand.Next(10, 20 + damageDone) / 10f;
            Vector2 velocity = new Vector2(0, 1).RotateRandom(MathHelper.ToRadians(360));
            float scale = Main.rand.Next(0, 15) / 10f + 2.5f + damageDone / 6f;
            scale = Math.Min(scale, 8f);
            SmallDust(PosWithinNPC(), velocity * power, scale);
            damageDone -= 3;
            loops++;
        }

        if (NPC.life <= 0)
        {
            Poof(3, 70);
        }
    }

    Vector2 PosWithinNPC()
    {
        return NPC.position + new Vector2(Main.rand.Next(0, NPC.width), Main.rand.Next(0, NPC.height));
    }

    public override bool CanHitNPC(NPC target) => !NPC.dontTakeDamage && NPC.ai[3] > 75;

    public override bool CanHitPlayer(Player target, ref int cooldownSlot) => !NPC.dontTakeDamage && NPC.ai[3] > 75;

    public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
    {
        modifiers.FinalDamage *= Math.Min((NPC.ai[3] - 60) / 240, 1f);
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
    {
        FollowerExplode(NPC);
        if (NPC.target == Main.myPlayer)
        {
            target.AddBuff(ModContent.BuffType<GiftOfVoid>(), Utility.TimeToTicks(seconds: 30), false);
        }
        if (Main.netMode == NetmodeID.MultiplayerClient && NPC.target == Main.myPlayer)
        {
            ModPacket packet = Mod.GetPacket();
            packet.Write((byte)WgMod.MessageType.FollowerExplode);
            packet.Write((byte)NPC.whoAmI);
            packet.Send();
        }
    }

    public static Vector2 ExplosionDustPos(NPC npc) => npc.Center + new Vector2(Main.rand.Next(-48, 49), Main.rand.Next(-48, 49));

    public static void FollowerExplode(NPC npc)
    {
        float angleA = 45f;
        float angleB = 9f;

        while (angleA < 360f)
        {
            for (int i = 0; i < 10; i++)
            {
                float power = Main.rand.Next(30, 200) / 10f;
                Vector2 velocity = new Vector2(1, 0).RotatedBy(MathHelper.ToRadians(angleA)).RotatedByRandom(MathHelper.ToRadians(angleB));
                float scale = Main.rand.Next(0, 55) / 10f + 5f;
                Dust.NewDustPerfect(ExplosionDustPos(npc), ModContent.DustType<FollowerDustBig>(), velocity * power, 0, GetFollowerColor((int)npc.ai[0]), scale);
            }
            angleA += 90f;
        }
        npc.active = false;
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<VoidCakes>(), 3));
        npcLoot.Add(ItemDropRule.Common(ItemID.TallyCounter, 100));
    }
}

public class FollowerSpawner : ModNPC
{
    public override string Texture => "WgMod/Assets/Textures/Invisible";

    public override void SetStaticDefaults()
    {
        NPCID.Sets.DoesntDespawnToInactivityAndCountsNPCSlots[NPC.type] = true;
        NPCID.Sets.ImmuneToRegularBuffs[NPC.type] = true;

        NPCID.Sets.NPCBestiaryDrawModifiers bestiaryData = new()
        {
            Hide = true
        };
        NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, bestiaryData);
    }

    public override void SetDefaults()
    {
        NPC.width = 16;
        NPC.height = 16;
        NPC.noTileCollide = true;
        NPC.noGravity = true;
        NPC.dontTakeDamage = true;
        NPC.knockBackResist = 0f;
        NPC.lifeMax = 999;
    }

    public override void AI()
    {
        if (Main.netMode != NetmodeID.MultiplayerClient)
        {
            int amountToSpawn = 1;

            if (Main.expertMode)
                amountToSpawn += Main.rand.Next(0, 3);
            if (Main.getGoodWorld)
                amountToSpawn += Main.rand.Next(2, 6);

            for (int i = 0; i < amountToSpawn; i++)
                NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.position.X, (int)NPC.position.Y, ModContent.NPCType<Follower>());
        }
        NPC.active = false;
    }

    public override float SpawnChance(NPCSpawnInfo spawnInfo)
    {
        return SpawnCondition.Dungeon.Chance * 0.06f;
    }
}
