using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Content.Buffs;

namespace WgMod.Content.Items.Consumables;

[Credit(ProjectRole.Programmer, Contributor.jumpsu2)]
public class VoidCakes : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 5;
        //Main.RegisterItemAnimation(Type, new DrawAnimationVertical(int.MaxValue, 3));
        ItemID.Sets.FoodParticleColors[Type] =
        [
            new Color(0, 0, 0)
        ];

        //TODO: add sprites for held and on plate
        //ItemID.Sets.IsFood[Type] = true;
    }

    public override bool CanUseItem(Player player) => !player.HasBuff<CrashDown>();

    public override void SetDefaults()
    {
        Item.DefaultToFood(30, 26, ModContent.BuffType<GiftOfVoid>(), Utility.TimeToTicks(minutes: 2));
        //not letting you bums use your infinite buff stack duration shenanigans on this
        Item.buffTime = 0;
        Item.buffType = 0;
        Item.value = Item.sellPrice(silver: 33, copper: 33);
        Item.rare = ItemRarityID.Green;
    }
    public override bool? UseItem(Player player)
    {
        if (player.whoAmI == Main.myPlayer)
        {
            player.AddBuff(ModContent.BuffType<GiftOfVoid>(), Utility.TimeToTicks(minutes: 2), false);
        }
        return true;
    }
    public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) => false;
    public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
    {
        drawColor = Main.DiscoColor;

        spriteBatch.Draw(TextureAssets.Item[Item.type].Value, position, frame, drawColor, 0, origin, scale, SpriteEffects.None, 0);
    }
    public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI) => false;
    public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
    {
        Main.GetItemDrawFrame(Item.type, out var itemTexture, out var itemFrame);

        Vector2 drawOrigin = itemFrame.Size() / 2f;
        Vector2 drawPosition = Item.Bottom - Main.screenPosition - new Vector2(0, drawOrigin.Y);

        lightColor = Main.DiscoColor;

        spriteBatch.Draw(TextureAssets.Item[Item.type].Value, drawPosition, itemFrame, lightColor, rotation, drawOrigin, scale, SpriteEffects.None, 0);
    }
}
