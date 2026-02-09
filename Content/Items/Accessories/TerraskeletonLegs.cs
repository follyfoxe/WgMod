using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Items.Accessories;

[AutoloadEquip(EquipType.Shoes)]
public class TerraskeletonLegs : ModItem
{
    public const int MoveSpeedBonus = 8;
    public const int LavaImmunityTime = 2;

    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MoveSpeedBonus, LavaImmunityTime);

    public override void SetDefaults()
    {
        Item.width = 38;
        Item.height = 28;

        Item.accessory = true;
        Item.rare = ItemRarityID.Lime;
        Item.value = Item.buyPrice(gold: 1);
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.moveSpeed += MoveSpeedBonus / 100f;
        player.accRunSpeed = 6.75f;
        player.rocketBoots = 2;
        player.vanityRocketBoots = 4;

        player.waterWalk2 = true;
        player.waterWalk = true;
        player.iceSkate = true;
        player.fireWalk = true;
        player.lavaRose = true;
        player.lavaMax += LavaImmunityTime * 60;

        if (!player.TryGetModPlayer(out WgPlayer wg))
            return;
        float immobility = wg.Weight.ClampedImmobility;
        wg.MovementPenalty *= float.Lerp(1f, 0.6f, immobility);
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.TerrasparkBoots)
            .AddIngredient<ExoskeletonLegs>()
            .AddIngredient(ItemID.HallowedBar, 12)
            .AddTile(TileID.MythrilAnvil)
            .Register();
    }
}
