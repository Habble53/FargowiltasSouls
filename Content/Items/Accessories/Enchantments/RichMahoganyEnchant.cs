﻿
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
    public class RichMahoganyEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override Color nameColor => new(181, 108, 100);


        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Blue;
            Item.value = 10000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<MahoganyEffect>(Item);
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ItemID.RichMahoganyHelmet)
            .AddIngredient(ItemID.RichMahoganyBreastplate)
            .AddIngredient(ItemID.RichMahoganyGreaves)
            .AddIngredient(ItemID.Moonglow)
            .AddIngredient(ItemID.Pineapple)
            .AddIngredient(ItemID.GrapplingHook)

            .AddTile(TileID.DemonAltar)
            .Register();
        }
    }
    public class MahoganyEffect : AccessoryEffect
    {

        public override Header ToggleHeader => Header.GetHeader<TimberHeader>();
        public override int ToggleItemType => ModContent.ItemType<RichMahoganyEnchant>();
        
        public override void PostUpdateEquips(Player player)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            bool forceEffect = modPlayer.ForceEffect<RichMahoganyEnchant>();

            if (player.grapCount > 0)
            {
                player.thorns += forceEffect ? 5.0f : 0.5f;
                modPlayer.MahoganyTimer = 0;
            }
            else //when not grapple, refresh DR
            {
                if (modPlayer.MahoganyTimer >= 60)
                {
                    modPlayer.MahoganyCanUseDR = true;
                }
                else
                    modPlayer.MahoganyTimer++;
            }
            if (modPlayer.MahoganyCanUseDR && modPlayer.MahoganyTimer < 60)
                player.endurance += forceEffect ? 0.3f : 0.1f;
        }
        public static void MahoganyHookAI(Projectile projectile, FargoSoulsPlayer modPlayer)
        {
            int cap = projectile.type == ProjectileID.QueenSlimeHook ? 4 : 1;
            if (projectile.extraUpdates < cap)
                projectile.extraUpdates += cap;
        }
    }
}
