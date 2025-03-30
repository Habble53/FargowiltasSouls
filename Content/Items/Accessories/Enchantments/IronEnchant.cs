using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
    public class IronEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override Color nameColor => new(152, 142, 131);


        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Blue;
            Item.value = 40000;
        }
        public override void UpdateInventory(Player player) => PassiveEffects(player, Item);
        public override void UpdateVanity(Player player) => PassiveEffects(player, Item);
        public override void UpdateAccessory(Player player, bool hideVisual) => AddEffects(player, Item);
        public static void PassiveEffects(Player player, Item item)
        {
            player.AddEffect<IronPassiveEffect>(item);
            player.AddEffect<IronEffect>(item);
        }
        public static void AddEffects(Player player, Item item)
        {
            PassiveEffects(player, item);
            player.AddEffect<IronPickupEffect>(item);
            player.AddEffect<IronEquippedEffect>(item);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.IronHelmet)
            .AddIngredient(ItemID.IronChainmail)
            .AddIngredient(ItemID.IronGreaves)
            .AddIngredient(ItemID.IronHammer)
            .AddIngredient(ItemID.IronAnvil)
            .AddIngredient(ItemID.Apricot) //(high in iron pog)

            .AddTile(TileID.DemonAltar)
            .Register();
        }
    }
    public class IronPassiveEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<TerraHeader>();
        public override int ToggleItemType => ModContent.ItemType<IronEnchant>();

    }
    public class IronEquippedEffect : AccessoryEffect
    {
        public override Header ToggleHeader => null;
        public override int ToggleItemType => ModContent.ItemType<IronEnchant>();

    }
    public class IronEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<TerraHeader>();
        public override int ToggleItemType => ModContent.ItemType<IronEnchant>();
        
    }
    public class IronPickupEffect : AccessoryEffect
    {
        public override Header ToggleHeader => null;
        public override int ToggleItemType => ModContent.ItemType<IronEnchant>();

        public override void PostUpdateEquips(Player player)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (modPlayer.IronReductionDuration > 0)
            {
                player.endurance += player.HasEffectEnchant<IronPickupEffect>() && player.ForceEffect<IronPickupEffect>() ? 0.28f : 0.14f;
                modPlayer.IronReductionDuration--;
            }
        }
        public static void OnPickup(Player player)
        {
            if (!player.HasEffect<IronPickupEffect>())
                return;
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            modPlayer.IronReductionDuration = 60 * 5;
        }

    }
}
