using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using WgMod.Common.Players;
using WgMod.Content.Buffs.Debuffs;

namespace WgMod.Content.Items.Consumables.Potions.WeightGainPotions;

public class LesserWeightGainPotion : ModItem
{
	public static LocalizedText RestoreLifeText { get; private set; }

	const float WeightEffect = 10;

	public override void SetStaticDefaults()
	{
		Item.ResearchUnlockCount = 30;
	}

	public override void SetDefaults()
	{
		Item.width = 20;
		Item.height = 24;
		Item.useStyle = ItemUseStyleID.DrinkLiquid;
		Item.useAnimation = 17;
		Item.useTime = 17;
		Item.useTurn = true;
		Item.UseSound = SoundID.Item3;
		Item.maxStack = Item.CommonMaxStack;
		Item.consumable = true;
		Item.rare = ItemRarityID.White;
		Item.value = Item.buyPrice(silver: 3);

		Item.buffType = ModContent.BuffType<MilkshakeSickness>();
		Item.buffTime = 1 * 60 * 30;
	}

	public override void UseAnimation(Player player)
	{
		if (!player.TryGetModPlayer(out WgPlayer wg))
			return;
		float weightChange;

		if (player.HasBuff<MilkshakeSickness>())
			weightChange = WeightEffect * 0.1f;
		else
			weightChange = WeightEffect;

		wg.SetWeight(wg.Weight + weightChange);

		CombatText.NewText(player.getRect(), Color.Yellow, weightChange + " kg");
	}

	public override void ModifyTooltips(List<TooltipLine> tooltips)
	{
		tooltips.FormatLines(WeightEffect);
		tooltips.RemoveAt(4);
	}
}

public class WeightGainPotion : ModItem
{
	public static LocalizedText RestoreLifeText { get; private set; }

	const float WeightEffect = 20;

	public override void SetStaticDefaults()
	{
		Item.ResearchUnlockCount = 30;
	}

	public override void SetDefaults()
	{
		Item.width = 30;
		Item.height = 24;
		Item.useStyle = ItemUseStyleID.DrinkLiquid;
		Item.useAnimation = 17;
		Item.useTime = 17;
		Item.useTurn = true;
		Item.UseSound = SoundID.Item3;
		Item.maxStack = Item.CommonMaxStack;
		Item.consumable = true;
		Item.rare = ItemRarityID.Blue;
		Item.value = Item.buyPrice(silver: 6);

		Item.buffType = ModContent.BuffType<MilkshakeSickness>();
		Item.buffTime = 1 * 60 * 30;
	}

	public override void UseAnimation(Player player)
	{
		if (!player.TryGetModPlayer(out WgPlayer wg))
			return;
		float weightChange;

		if (player.HasBuff<MilkshakeSickness>())
			weightChange = WeightEffect * 0.1f;
		else
			weightChange = WeightEffect;

		wg.SetWeight(wg.Weight + weightChange);

		CombatText.NewText(player.getRect(), Color.Yellow, weightChange + " kg");
	}

	public override void ModifyTooltips(List<TooltipLine> tooltips)
	{
		tooltips.FormatLines(WeightEffect);
		tooltips.RemoveAt(4);
	}
}

public class GreaterWeightGainPotion : ModItem
{
	public static LocalizedText RestoreLifeText { get; private set; }

	const float WeightEffect = 30;

	public override void SetStaticDefaults()
	{
		Item.ResearchUnlockCount = 30;
	}

	public override void SetDefaults()
	{
		Item.width = 32;
		Item.height = 32;
		Item.useStyle = ItemUseStyleID.DrinkLiquid;
		Item.useAnimation = 17;
		Item.useTime = 17;
		Item.useTurn = true;
		Item.UseSound = SoundID.Item3;
		Item.maxStack = Item.CommonMaxStack;
		Item.consumable = true;
		Item.rare = ItemRarityID.Orange;
		Item.value = Item.buyPrice(silver: 10);

		Item.buffType = ModContent.BuffType<MilkshakeSickness>();
		Item.buffTime = 1 * 60 * 30;
	}

	public override void UseAnimation(Player player)
	{
		if (!player.TryGetModPlayer(out WgPlayer wg))
			return;
		float weightChange;

		if (player.HasBuff<MilkshakeSickness>())
			weightChange = WeightEffect * 0.1f;
		else
			weightChange = WeightEffect;

		wg.SetWeight(wg.Weight + weightChange);

		CombatText.NewText(player.getRect(), Color.Yellow, weightChange + " kg");
	}

	public override void ModifyTooltips(List<TooltipLine> tooltips)
	{
		tooltips.FormatLines(WeightEffect);
		tooltips.RemoveAt(4);
	}
}
public class SuperWeightGainPotion : ModItem
{
	public static LocalizedText RestoreLifeText { get; private set; }

	const float WeightEffect = 40;

	public override void SetStaticDefaults()
	{
		Item.ResearchUnlockCount = 30;
	}

	public override void SetDefaults()
	{
		Item.width = 64;
		Item.height = 58;
		Item.useStyle = ItemUseStyleID.DrinkLiquid;
		Item.useAnimation = 17;
		Item.useTime = 17;
		Item.useTurn = true;
		Item.UseSound = SoundID.Item3;
		Item.maxStack = Item.CommonMaxStack;
		Item.consumable = true;
		Item.rare = ItemRarityID.Lime;
		Item.value = Item.buyPrice(silver: 30);

		Item.buffType = ModContent.BuffType<MilkshakeSickness>();
		Item.buffTime = 1 * 60 * 30;
	}

	public override void UseAnimation(Player player)
	{
		if (!player.TryGetModPlayer(out WgPlayer wg))
			return;
		float weightChange;

		if (player.HasBuff<MilkshakeSickness>())
			weightChange = WeightEffect * 0.1f;
		else
			weightChange = WeightEffect;

		wg.SetWeight(wg.Weight + weightChange);

		CombatText.NewText(player.getRect(), Color.Yellow, weightChange + " kg");
	}

	public override void ModifyTooltips(List<TooltipLine> tooltips)
	{
		tooltips.FormatLines(WeightEffect);
		tooltips.RemoveAt(4);
	}
}