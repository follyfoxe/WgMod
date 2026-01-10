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
using WgMod.Content.Projectiles;
using WgMod.Content.Tiles;

namespace WgMod.Common.Players;

public class GeneralPlayer : ModPlayer
{
    internal float JumpSpeedBoost;
    internal bool HasJumped;
    /// <summary>
    /// Similar to player.endurance, but calculated seperately.
    /// </summary>
    internal float AltDR;
    public override void ResetEffects()
    {
        JumpSpeedBoost = 0f;
        if (Player.jump <= 0)
            HasJumped = false;
        AltDR = 0f;
    }
    public override void ModifyHurt(ref Player.HurtModifiers modifiers)
    {
        modifiers.FinalDamage *= 1f - AltDR;
    }
}

