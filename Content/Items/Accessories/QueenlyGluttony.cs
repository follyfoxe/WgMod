using System.Security.Cryptography.X509Certificates;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Items.Accessories;

public class QueenlyGluttony : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 22;
        Item.height = 22;

        Item.accessory = true;
        Item.rare = ItemRarityID.Pink;
        Item.value = Item.buyPrice(gold: 1);
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        if (!player.TryGetModPlayer(out WgPlayer wg))
            return;

        float immobility = wg.Weight.ClampedImmobility;

        player.GetDamage(DamageClass.Melee) *= float.Lerp(1.05f, 1.15f, immobility);
        player.GetCritChance(DamageClass.Melee) += (int)float.Lerp(5f, 10f, immobility);
        player.GetArmorPenetration(DamageClass.Melee) += (int)float.Lerp(2f, 12f, immobility);

        wg._queenlyGluttony = true;
    }
}
