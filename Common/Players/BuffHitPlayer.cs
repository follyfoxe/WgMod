using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Content.Buffs;

namespace WgMod.Common.Players;

public class BuffHitPlayer : ModPlayer
{
    readonly HashSet<int> _slimes =
    [
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
    ];

    readonly HashSet<int> _bees =
    [
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
    ];

    readonly HashSet<int> _feeders =
    [
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
    ];

    void AddNPCs(HashSet<int> table, string mod, params string[] npcs)
    {
        if (!ModLoader.TryGetMod(mod, out Mod foundMod))
            return;
        foreach (string npc in npcs)
        {
            if (!foundMod.TryFind(npc, out ModNPC foundNpc))
            {
                Mod.Logger.Warn($"Couldn't find buff '{npc}' for mod '{mod}'");
                continue;
            }
            table.Add(foundNpc.Type);
        }
    }

    public override void Load()
    {
        AddNPCs(_slimes, "Consolaria", "ShadowSlime");
        AddNPCs(_bees, "Consolaria", "DragonHornet");
        AddNPCs(
            _feeders,
            "Consolaria",
            "TurkortheUngrateful",
            "TurkorNeck",
            "TurkortheUngratefulHead",
            "ArchDemon"
        );

        AddNPCs(
            _slimes,
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

        AddNPCs(
            _feeders,
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

        AddNPCs(
            _feeders,
            "CalamityFables",
            "WulfrumGrappler",
            "WulfrumMagnetizer",
            "WulfrumMortar",
            "WulfrumNexus",
            "WulfrumRoller",
            "WulfrumRover"
        );
    }

    void AddBuff(int type, int timeToAdd, float weightGain)
    {
        if (!Player.TryGetModPlayer(out WgPlayer wg))
            return;
        Player.AddBuff(type, timeToAdd);
        wg.SetWeight(wg.Weight + weightGain);
        SoundEngine.PlaySound(new SoundStyle("WgMod/Assets/Sounds/Gulp_", 4, SoundType.Sound), Player.Center);
    }

    public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
    {
        if (_slimes.Contains(npc.type))
            AddBuff(BuffID.Slimed, 10 * hurtInfo.Damage, hurtInfo.Damage / 10);
        if (_bees.Contains(npc.type))
            AddBuff(BuffID.Slimed, 10 * hurtInfo.Damage, hurtInfo.Damage / 8);
        if (_feeders.Contains(npc.type))
            AddBuff(ModContent.BuffType<ForceFed>(), 10 * hurtInfo.Damage, hurtInfo.Damage / 6);
    }
}
