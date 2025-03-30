using FargowiltasSouls.Content.Projectiles;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static FargowiltasSouls.Content.Items.Accessories.Forces.TimberForce;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
    public class BorealWoodEnchant : BaseEnchant
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

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<BorealEffect>(Item);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.BorealWoodHelmet)
            .AddIngredient(ItemID.BorealWoodBreastplate)
            .AddIngredient(ItemID.BorealWoodGreaves)
            .AddIngredient(ItemID.Shiverthorn)
            .AddIngredient(ItemID.Plum)
            .AddIngredient(ItemID.Snowball, 300)

            .AddTile(TileID.DemonAltar)
            .Register();
        }
    }
    public class BorealEffect : AccessoryEffect
    {

        public override Header ToggleHeader => Header.GetHeader<TimberHeader>();
        public override int ToggleItemType => ModContent.ItemType<BorealWoodEnchant>();
        public override bool ExtraAttackEffect => true;

        public override void TryAdditionalAttacks(Player player, int damage, DamageClass damageType)
        {
            if (!HasEffectEnchant(player))
                return;
            BorealSnowballs(player, damage);
        }
        public override void OnHitNPCEither(Player player, NPC target, NPC.HitInfo hitInfo, DamageClass damageClass, int baseDamage, Projectile projectile, Item item)
        {
            if (player.HasEffect<TimberEffect>())
            {
                if (player.Distance(target.Center) > ShadewoodEffect.Range(player, true))
                    return;
                if (projectile != null && projectile.type == ProjectileID.SnowBallFriendly)
                    return;
                BorealSnowballs(player, baseDamage);
            }
                
        }
        public void BorealSnowballs(Player player, int baseDamage)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (modPlayer.BorealCD <= 0 && player.whoAmI == Main.myPlayer)
            {
                Item item = EffectItem(player);
                bool forceEffect = modPlayer.ForceEffect(item.type);
                Item heldItem = player.HeldItem;
                modPlayer.BorealCD = forceEffect ? 30 : 60;
                if (player.HasEffect<TimberEffect>())
                    modPlayer.BorealCD = 90;

                Vector2 vel = Vector2.Normalize(Main.MouseWorld - player.Center) * 17f;

                float snowballDamage = baseDamage / 2;
                if (!player.HasEffect<TimberEffect>() && heldItem != null && heldItem.IsWeaponWithDamageClass())
                {
                    snowballDamage *= player.ActualClassDamage(DamageClass.Ranged);
                    float softcapMult = forceEffect ? (20f / 3f) : 1f;
                    if (snowballDamage > (12f * softcapMult)) // diminishing returns above 15 snowballDamage for non wiz, 100 for wiz
                        snowballDamage = (float)Math.Round(((24f * softcapMult) + snowballDamage) / 3f); // e.g. non wiz 30 -> 20, wiz 200 -> 150 etc. (https://www.desmos.com/calculator/vyaqqoegxq)
                }
                if (player.HasEffect<TimberEffect>())
                    snowballDamage = 400;
                int p = Projectile.NewProjectile(player.GetSource_Accessory(item), player.Center, vel, ProjectileID.SnowBallFriendly, (int)snowballDamage, 1, Main.myPlayer);

                int numSnowballs = forceEffect ? 7 : 3;
                float angle = MathHelper.Pi / 10;
                FargoSoulsGlobalProjectile.SplitProj(Main.projectile[p], numSnowballs, angle, 1);

            }
        }
    }
}
