﻿using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Content.Items.Accessories.Forces;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
    public class AshWoodEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override Color nameColor => new(139, 116, 100);


        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Blue;
            Item.value = 10000;
        }
        public static void PassiveEffect(Player player)
        {
            //player.FargoSouls().fireNoDamage = true;
        }
        public override void UpdateInventory(Player player) => PassiveEffect(player);
        public override void UpdateVanity(Player player) => PassiveEffect(player);
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            PassiveEffect(player);
            player.AddEffect<AshWoodEffect>(Item);
            player.AddEffect<AshWoodFireballs>(Item);
        }



        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.AshWoodHelmet)
            .AddIngredient(ItemID.AshWoodBreastplate)
            .AddIngredient(ItemID.AshWoodGreaves)
            .AddIngredient(ItemID.Fireblossom)
            .AddIngredient(ItemID.SpicyPepper)
            .AddIngredient(ItemID.Gel, 50)
            .AddTile(TileID.DemonAltar)
            .Register();
        }
    }
    public class AshWoodEffect : AccessoryEffect
    {
        public override Header ToggleHeader => null;

        public override void PostUpdateEquips(Player player)
        {
            AshWoodEnchant.PassiveEffect(player);
            player.buffImmune[ModContent.BuffType<OiledBuff>()] = true;
            player.ashWoodBonus = true;
        }
    }
    public class AshWoodFireballs : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<TerraHeader>();
        public override int ToggleItemType => ModContent.ItemType<AshWoodEnchant>();
        public override bool ExtraAttackEffect => true;
        public override void PostUpdateEquips(Player player)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (modPlayer.AshwoodCD > 0)
                modPlayer.AshwoodCD--;
        }
        public override void TryAdditionalAttacks(Player player, int damage, DamageClass damageType)
        {
            if (!HasEffectEnchant(player))
                return;
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            bool debuffed = false;
            for (int i = 0; i < Player.MaxBuffs; i++)
            {
                int type = player.buffType[i];
                if (type > 0 && type is not BuffID.PotionSickness or BuffID.ManaSickness or BuffID.WaterCandle && Main.debuff[type] && FargowiltasSouls.DebuffIDs.Contains(type))
                    debuffed = true;
            }
            if (modPlayer.AshwoodCD <= 0 && (debuffed || player.HasEffect<ObsidianProcEffect>()))
            {
                modPlayer.AshwoodCD = modPlayer.ForceEffect<AshWoodEnchant>() ? 20 : player.HasEffect<ObsidianProcEffect>() ? 25 : 35;

                float softcapMult = 2f;
                int effectItemType = EffectItem(player).type;
                Item heldItem = player.HeldItem;
                int ashwood = ModContent.ItemType<AshWoodEnchant>();
                int obsidian = ModContent.ItemType<ObsidianEnchant>();
                if (!player.ForceEffect<AshWoodFireballs>() && (effectItemType == ashwood || effectItemType == obsidian))
                    softcapMult = 1f;

                float fireballDamage = damage;
                if (!modPlayer.TerrariaSoul && heldItem != null && heldItem.IsWeaponWithDamageClass())
                {
                    fireballDamage *= player.ActualClassDamage(DamageClass.Magic);
                    if (fireballDamage > 24f * softcapMult)
                    fireballDamage = (float)Math.Round(((48f * softcapMult) + fireballDamage) / 3f);
                }
                Vector2 vel = Vector2.Normalize(Main.MouseWorld - player.Center) * 17f;
                vel = vel.RotatedByRandom(Math.PI / 10);

                if (player.whoAmI == Main.myPlayer)
                    Projectile.NewProjectile(GetSource_EffectItem(player), player.Center, vel, ProjectileID.BallofFire, (int)fireballDamage, 1, Main.myPlayer);
            }
        }
    }
}
