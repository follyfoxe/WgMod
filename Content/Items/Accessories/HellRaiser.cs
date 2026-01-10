using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Items.Accessories;

public class HellRaiser : ModItem
{
    int _hellRaiserMinionCount;
    float _hellRaiserMinionDamage;
    float _hellRaiserWhipSpeed;

    public override void SetDefaults()
    {
        Item.width = 26;
        Item.height = 32;

        Item.accessory = true;
        Item.rare = ItemRarityID.LightRed;
        Item.value = Item.buyPrice(gold: 1);
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        if (!player.TryGetModPlayer(out WgPlayer wg))
            return;
        float immobility = wg.Weight.ClampedImmobility;

        _hellRaiserMinionCount = 3;
        _hellRaiserMinionDamage = float.Lerp(-0.1f, 0.1f, immobility);
        _hellRaiserWhipSpeed = float.Lerp(0.9f, 0.8f, immobility);

        player.maxMinions += _hellRaiserMinionCount;
        player.GetDamage(DamageClass.Summon) += _hellRaiserMinionDamage;
        player.GetAttackSpeed(DamageClass.SummonMeleeSpeed) *= _hellRaiserWhipSpeed;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.HellstoneBar, 12)
            .AddIngredient(ItemID.SoulofNight, 6)
            .AddTile(TileID.MythrilAnvil)
            .Register();
    }
}
