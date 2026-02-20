using System.Numerics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Items.Accessories;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.trilophyte)]
public class WeightLossPendant : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 24;
        Item.height = 32;

        Item.accessory = true;
        Item.rare = ItemRarityID.Orange;
        Item.value = Item.buyPrice(gold: 1, silver: 50);
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        if (!player.TryGetModPlayer(out WgPlayer wg))
            return;
        if (!player.TryGetModPlayer(out WeightLossPendantPlayer wp))
            return;
        wg.WeightLossRate += 5f;
        wp._active = true;
        wp._hidden = hideVisual;
    }
}

public class WeightLossPendantPlayer : ModPlayer
{
    internal bool _active;
    internal bool _hidden;
    internal int _dustRate;

    public override void ResetEffects()
    {
        _active = false;
        _hidden = false;
    }

    public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
    {
        _dustRate = 30;

        if (Main.rand.NextBool(_dustRate) && _active == true && _hidden == false)
        {
            Dust.NewDust(Player.position, Player.width, Player.height - 1, DustID.Shadowflame, 0f, 0f, 150, default, 0.7f);
        }
    }
}
