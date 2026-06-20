using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Common.GlobalItems;

public class WgItem : GlobalItem
{
    public override void ModifyShootStats(Item item, Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        if (!player.TryGetModPlayer(out WgPlayer wg))
            return;
        position.Y += wg._addedGfxOffY;
    }
}
