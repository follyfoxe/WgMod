using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Items.Accessories
{
    [AutoloadEquip(EquipType.Wings)]
    [Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
    public class LiftingTome : ModItem
    {
        public const int _minionCount = 1;
        WgStat _damage = new(0.04f, 0.08f);
        WgStat _magicDamage = new(0.02f, 0.04f);
        WgStat _manaCost = new(0.96f, 0.92f);
        WgStat _maxMana = new(20, 70);

        public override void SetStaticDefaults()
        {
            ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new WingStats(60, 5f, 1f);
        }

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 40;
            Item.value = Item.buyPrice(gold: 1);
            Item.rare = ItemRarityID.Orange;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (!player.TryGetModPlayer(out LiftingTomePlayer lt))
                return;
            if (!player.TryGetModPlayer(out WgPlayer wg))
                return;
            float immobility = wg.Weight.ClampedImmobility;

            _damage.Lerp(immobility);
            _magicDamage.Lerp(immobility);
            _manaCost.Lerp(immobility);
            _maxMana.Lerp(immobility);
            _maxMana.Value = MathF.Floor(_maxMana.Value / 20f) * 20f;

            player.GetDamage(DamageClass.Generic) += _damage;
            player.GetDamage(DamageClass.Magic) += _magicDamage;
            player.manaCost *= _manaCost;
            player.statManaMax2 += _maxMana;
            player.maxMinions += _minionCount;

            lt._active = true;
            lt._onHit = true;

            if (lt._cooldownBolt < lt._cooldownBoltMax)
                lt._cooldownBolt++;

            if (lt._cooldownSkull < lt._cooldownSkullMax)
                lt._cooldownSkull++;

            if (lt._cooldownScythe < lt._cooldownScytheMax)
                lt._cooldownScythe++;

            lt.SpawnHallucination(Item);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.BoneHelm)
                .AddIngredient(ItemID.WaterBolt)
                .AddIngredient(ItemID.BookofSkulls)
                .AddIngredient(ItemID.DemonScythe)
                .AddTile(TileID.DemonAltar)
                .SortBefore(Main.recipe.First(recipe => recipe.createItem.wingSlot != -1))
                .Register();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.FormatLines(_damage.Percent(), (1 - _manaCost).Percent(), _maxMana - 10, _minionCount, _magicDamage.Percent());
        }
    }

    public class LiftingTomePlayer : ModPlayer
    {
        public bool _active;
        public bool _onHit;
        public int _cooldownBolt;
        public int _cooldownSkull;
        public int _cooldownScythe;
        public int _cooldownBoltMax = 90;
        public int _cooldownSkullMax = 120;
        public int _cooldownScytheMax = 180;

        WgStat damageModifier = new(1f, 2f);
        WgStat velocityModifier = new(1f, 0.8f);

        public override void ResetEffects()
        {
            _active = false;
            _onHit = false;
        }

        public override void OnHitAnything(float x, float y, Entity victim)
        {
            if (!Player.TryGetModPlayer(out WgPlayer wg) || !_onHit)
                return;
            float immobility = wg.Weight.ClampedImmobility;

            damageModifier.Lerp(immobility);
            velocityModifier.Lerp(immobility);

            Vector2 mousePosition = Main.MouseWorld;
            float angle = Utils.AngleTo(Player.Center, mousePosition);
            Vector2 velocity = new Vector2(MathF.Cos(angle), MathF.Sin(angle));

            if (_cooldownScythe == _cooldownScytheMax)
            {
                _cooldownScythe = (int)float.Lerp(30, 0, immobility);

                Projectile.NewProjectile(
                    Player.GetSource_FromThis(),
                    Player.Center,
                    velocity * 0.2f * velocityModifier,
                    ProjectileID.DemonScythe,
                    17 * damageModifier,
                    5
                );

                return;
            }

            if (_cooldownSkull == _cooldownSkullMax)
            {
                _cooldownSkull = (int)float.Lerp(30, 0, immobility);

                Projectile.NewProjectile(
                    Player.GetSource_FromThis(),
                    Player.Center,
                    velocity * 3.5f * velocityModifier,
                    ProjectileID.BookOfSkullsSkull,
                    14 * damageModifier,
                    3.5f
                );

                return;
            }

            if (_cooldownBolt == _cooldownBoltMax)
            {
                _cooldownBolt = (int)float.Lerp(30, 0, immobility);

                Projectile.NewProjectile(
                    Player.GetSource_FromThis(),
                    Player.Center,
                    velocity * 4.5f * velocityModifier,
                    ProjectileID.WaterBolt,
                    9 * damageModifier,
                    5
                );
            }
        }

        public void SpawnHallucination(Item item)
        {
            if (Player.whoAmI != Main.myPlayer)
                return;
            Player.insanityShadowCooldown = Utils.Clamp<int>(
                Player.insanityShadowCooldown - 1,
                0,
                100
            );
            if (Player.insanityShadowCooldown > 0)
                return;
            Player.insanityShadowCooldown = Main.rand.Next(20, 101);
            float num = 500f;
            int Damage = 18;
            _hallucinationCandidates.Clear();
            for (int index = 0; index < 200; ++index)
            {
                NPC npc = Main.npc[index];
                if (
                    npc.CanBeChasedBy(this)
                    && (double)Player.Distance(npc.Center) <= (double)num
                    && Collision.CanHitLine(
                        Player.position,
                        Player.width,
                        Player.height,
                        npc.position,
                        npc.width,
                        npc.height
                    )
                )
                    _hallucinationCandidates.Add(npc);
            }
            if (_hallucinationCandidates.Count == 0)
                return;
            Vector2 spawnposition;
            Vector2 spawnvelocity;
            float ai0;
            float ai1;
            Projectile.RandomizeInsanityShadowFor(
                Main.rand.NextFromCollection(_hallucinationCandidates),
                false,
                out spawnposition,
                out spawnvelocity,
                out ai0,
                out ai1
            );
            Projectile.NewProjectile(
                new EntitySource_ItemUse(Player, item),
                spawnposition,
                spawnvelocity,
                ProjectileID.InsanityShadowFriendly,
                Damage,
                0.0f,
                Player.whoAmI,
                ai0,
                ai1
            );
        }

        private static List<NPC> _hallucinationCandidates = new List<NPC>();
    }
}
