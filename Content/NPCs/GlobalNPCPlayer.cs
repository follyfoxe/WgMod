using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Players;
using WgMod.Content.Buffs;

namespace WgMod.Content.NPCs;

public class GlobalNPCPlayer : ModPlayer
{
    HashSet<int> Slimes = new HashSet<int>
    {
        NPCID.BlueSlime,
        NPCID.GreenSlime,
        NPCID.RedSlime,
        NPCID.PurpleSlime,
        NPCID.RedSlime,
        NPCID.Pinky,
        NPCID.Slimer,
        NPCID.Slimer2,
        NPCID.YellowSlime,
        NPCID.BlackSlime,
        NPCID.IceSlime,
        NPCID.SandSlime,
        NPCID.JungleSlime,
        NPCID.SpikedIceSlime,
        NPCID.SpikedJungleSlime,
        NPCID.MotherSlime,
        NPCID.BabySlime,
        NPCID.LavaSlime,
        NPCID.DungeonSlime,
        NPCID.GoldenSlime,
        NPCID.KingSlime,
        NPCID.SlimeSpiked,
        NPCID.UmbrellaSlime,
        NPCID.ShimmerSlime,
        NPCID.ToxicSludge,
        NPCID.CorruptSlime,
        NPCID.Slimeling,
        NPCID.Crimslime,
        NPCID.IlluminantSlime,
        NPCID.RainbowSlime,
        NPCID.QueenSlimeBoss,
        NPCID.SlimeRibbonGreen,
        NPCID.SlimeRibbonRed,
        NPCID.SlimeRibbonWhite,
        NPCID.SlimeRibbonYellow,
        NPCID.QueenSlimeMinionBlue,
        NPCID.QueenSlimeMinionPink,
        NPCID.QueenSlimeMinionPurple,
        NPCID.HoppinJack,
        NPCID.SlimedZombie,
    };

    HashSet<int> Bees = new HashSet<int>
    {
        NPCID.Bee,
        NPCID.BeeSmall,
        NPCID.QueenBee,
        NPCID.Hornet,
        NPCID.HornetFatty,
        NPCID.HornetHoney,
        NPCID.HornetLeafy,
        NPCID.HornetStingy,
        NPCID.BigHornetFatty,
        NPCID.BigHornetFatty,
        NPCID.BigHornetLeafy,
        NPCID.BigHornetSpikey,
        NPCID.BigHornetStingy,
        NPCID.MossHornet,
        NPCID.BigMossHornet,
        NPCID.TinyMossHornet,
        NPCID.GiantMossHornet,
        NPCID.LittleMossHornet,
        NPCID.LittleHornetFatty,
        NPCID.LittleHornetHoney,
        NPCID.LittleHornetLeafy,
        NPCID.LittleHornetSpikey,
        NPCID.LittleHornetStingy,
        NPCID.VortexHornet,
        NPCID.VortexHornetQueen,
    };

    HashSet<int> Feeders = new HashSet<int>
    {
        NPCID.Demon,
        NPCID.FireImp,
        NPCID.Nymph,
        NPCID.VoodooDemon,
        NPCID.CultistArcherBlue,
        NPCID.CultistBoss,
        NPCID.CultistDragonHead,
        NPCID.CultistDragonBody1,
        NPCID.CultistDragonBody2,
        NPCID.CultistDragonBody3,
        NPCID.CultistDragonBody4,
        NPCID.CultistDragonTail,
        NPCID.AncientCultistSquidhead,
        NPCID.FloatyGross,
        NPCID.DesertLamiaDark,
        NPCID.DesertLamiaLight,
        NPCID.RedDevil,
        NPCID.TheBride,
        NPCID.TheGroom,
        NPCID.SandElemental,
        NPCID.GoblinSummoner,
        NPCID.GrayGrunt,
        NPCID.BrainScrambler,
        NPCID.GigaZapper,
        NPCID.MartianDrone,
        NPCID.MartianEngineer,
        NPCID.MartianOfficer,
        NPCID.MartianWalker,
        NPCID.RayGunner,
        NPCID.ScutlixRider,
        NPCID.MartianTurret,
        NPCID.MartianProbe,
        NPCID.MartianSaucer,
        NPCID.MartianSaucerCore,
        NPCID.MartianSaucerTurret,
        NPCID.Poltergeist,
        NPCID.Plantera,
        NPCID.PlanterasHook,
        NPCID.PlanterasTentacle,
        NPCID.Harpy,
        NPCID.ShadowFlameApparition,
        NPCID.AncientLight,
        NPCID.AncientDoom,
        NPCID.BurningSphere,
        NPCID.PresentMimic,
    };

    void AddNpcs(HashSet<int> table, string mod, params string[] npcs)
    {
        if (!ModLoader.TryGetMod(mod, out Mod foundMod))
            return;
        foreach (string npc in npcs)
            table.Add(foundMod.Find<ModNPC>(npc).Type);
    }

    public override void Load()
    {
        AddNpcs(Slimes, "Consolaria", "ShadowSlime");
        AddNpcs(Bees, "Consolaria", "DragonHornet");
        AddNpcs(
            Feeders,
            "Consolaria",
            "TurkortheUngrateful",
            "TurkorNeck",
            "TurkortheUngratefulHead",
            "ArchDemon"
        );

        AddNpcs(
            Slimes,
            "CalamityMod",
            "PerennialSlime",
            "AeroSlime",
            "CorruptSlimeSpawn",
            "CorruptSlimeSpawn2",
            "CrimsonSlimeSpawn",
            "CrimsonSlimeSpawn2",
            "CrimulanPaladin",
            "EbonianPaladin",
            "SplitCrimulanPaladin",
            "SplitEbonianPaladin",
            "SlimeGodCore",
            "AstralSlime",
            "InfernalCongealment",
            "CryoSlime",
            "BloomSlime",
            "IrradiatedSlime"
        );

        AddNpcs(
            Feeders,
            "CalamityMod",
            "WulfrumAmplifier",
            "WulfrumDrone",
            "WulfrumRover",
            "WulfrumGyrator",
            "WulfrumAmplifier",
            "WulfrumHovercraft",
            "SupremeCalamitas",
            "SepulcherArm",
            "BrimstoneHeart",
            "SepulcherBody",
            "SepulcherBodyEnergyBall",
            "SepulcherHead",
            "SepulcherTail",
            "SoulSeekerSupreme",
            "SupremeCataclysm",
            "SupremeCatastrophe",
            "CloudElemental",
            "Brimling",
            "BrimstoneElemental",
            "Anahita",
            "AnahitaIceShield",
            "AquaticAberration",
            "Leviathan",
            "LeviathanStart",
            "Eidolist",
            "OverloadedSoldier",
            "RenegadeWarlock"
        );

        AddNpcs(
            Feeders,
            "CalamityFables",
            "WulfrumGrappler",
            "WulfrumMagnetizer",
            "WulfrumMortar",
            "WulfrumNexus",
            "WulfrumRoller",
            "WulfrumRover"
        );
    }

    public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
    {
        if (!Player.TryGetModPlayer(out WgPlayer wg))
            return;

        if (Slimes.Contains(npc.type))
        {
            Player.AddBuff(BuffID.Slimed, 60);
            SoundEngine.PlaySound(new SoundStyle("WgMod/Assets/Sounds/Gulp_", 4, SoundType.Sound), Player.Center);
        }

        if (Bees.Contains(npc.type))
        {
            Player.AddBuff(BuffID.Honey, 60);
            SoundEngine.PlaySound(new SoundStyle("WgMod/Assets/Sounds/Gulp_", 4, SoundType.Sound), Player.Center);
        }

        if (Feeders.Contains(npc.type))
        {
            Player.AddBuff(ModContent.BuffType<ForceFed>(), 10 * hurtInfo.Damage);
            SoundEngine.PlaySound(new SoundStyle("WgMod/Assets/Sounds/Gulp_", 4, SoundType.Sound), Player.Center);

            wg.SetWeight(wg.Weight + hurtInfo.Damage / 5);
        }
    }
}
