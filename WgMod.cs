using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Creative;
using Terraria.Graphics.Renderers;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common;
using WgMod.Common.Players;
using WgMod.Content.Buffs;

namespace WgMod;

// TODO: Use calories instead
public record struct GainOptions(float TotalGain, float Time = 0f)
{
    public readonly bool IsInstant => Time < 0.01f;
    public static implicit operator GainOptions(float mass) => new(mass);
}

// Please read https://github.com/tModLoader/tModLoader/wiki/Basic-tModLoader-Modding-Guide#mod-skeleton-contents for more information about the various files in a mod.
public partial class WgMod : Mod
{
    public static readonly Dictionary<int, GainOptions> _buffTable = [];

    internal static WgMod Instance;

    public WgMod()
    {
        Instance = this;
    }

    // Vanilla
    void AddBuffs((int id, GainOptions gain)[] table)
    {
        foreach (var (id, gain) in table)
            _buffTable[id] = gain;
    }

    // Mods
    void AddBuffs(string mod, (string name, GainOptions gain)[] table)
    {
        if (!ModLoader.TryGetMod(mod, out Mod foundMod))
            return;
        foreach (var (name, gain) in table)
        {
            if (!foundMod.TryFind(name, out ModBuff foundBuff))
            {
                Logger.Warn($"Couldn't find buff '{name}' for mod '{mod}'");
                return;
            }
            _buffTable[foundBuff.Type] = gain;
        }
    }

    public override void Load()
    {
        RegisterBuffs();

        On_PlayerSittingHelper.UpdateSitting += Detours.OnUpdateSitting;
        On_Mount.Draw += Detours.OnMountDraw;

        On_Player.GetItemGrabRange += Detours.OnPlayer_GetItemGrabRange;

        On_Player.StatusToPlayerPvP += Detours.OnPlayer_StatusToPlayerPvP;

        //On_LegacyPlayerRenderer.DrawPlayerFull += Detours.OnLegacyPlayerRenderer_DrawPlayerFull;

        On_PlayerDrawSet.HeadOnlySetup += Detours.OnPlayerDrawSet_HeadOnlySetup;

        On_Player.ResizeHitbox += Detours.OnPlayer_ResizeHitbox;

        On_Player.Heal += Detours.OnPlayer_Heal;
        On_Player.ApplyLifeAndOrMana += Detours.OnPlayer_ApplyLifeAndOrMana;

        On_Player.UpdateJumpHeight += Detours.OnPlayer_UpdateJumpHeight;

        On_Main.DrawProj_DrawExtras += Detours.OnMain_DrawProj_DrawExtras;

        On_Main.GetPlayerArmPosition += Detours.OnMain_GetPlayerArmPosition;

        On_PlayerDrawLayers.DrawStarboardRainbowTrail += Detours.OnPlayerDrawLayers_DrawStarboardRainbowTrail;

        On_EntityShadowInfo.CopyPlayer += Detours.OnEntityShadowInfo_CopyPlayer;

        On_Player.UpdateSocialShadow += Detours.OnPlayer_UpdateSocialShadow;

        On_PlayerDrawLayers.DrawPlayer_03_PortableStool += Detours.OnPlayerDrawLayers_DrawPlayer_03_PortableStool;

        On_Player.DashMovement += Detours.OnPlayer_DashMovement;

        On_Mount.Hover += Detours.OnMount_Hover;

        On_Player.PickupItem += Detours.OnPlayer_PickupItem;

        //On_Player.QuickHeal_GetItemToUse += Detours.OnPlayer_QuickHeal_GetItemToUse;
        //On_Player.GetHealLife += Detours.OnPlayer_GetHealLife;
    }

    public override void Unload()
    {
        On_Mount.Draw -= Detours.OnMountDraw;
    }
}
