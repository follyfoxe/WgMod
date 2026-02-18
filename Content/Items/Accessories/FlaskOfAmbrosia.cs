using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Players;
using WgMod.Content.Buffs;

namespace WgMod.Content.Items.Accessories;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.trilophyte)]
[Credit(ProjectRole.Artist, Contributor._d_u_m_m_y_)]
public class FlaskOfAmbrosia : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 30;
        Item.height = 36;

        Item.accessory = true;
        Item.rare = ItemRarityID.Orange;
        Item.value = Item.buyPrice(gold: 4);
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        if (!player.TryGetModPlayer(out WgPlayer wg))
            return;
        if (!player.TryGetModPlayer(out AmbrosiaPlayer ap))
            return;
        ap._ambrosiaOnHit = true;
        wg.WeightLossRate += 2f;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.SweetheartNecklace)
            .AddIngredient<WeightLossPendant>()
            .AddTile(TileID.TinkerersWorkbench)
            .Register();
    }
}

public class AmbrosiaPlayer : ModPlayer
{
    internal bool _ambrosiaOnHit;

    public override void ResetEffects()
    {
        _ambrosiaOnHit = false;
    }

    public override void OnHurt(Player.HurtInfo info)
    {
        if (_ambrosiaOnHit)
            Player.AddBuff(ModContent.BuffType<AmbrosiaGorged>(), 8 * 60);
    }
}
