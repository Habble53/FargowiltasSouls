using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.ID;

namespace FargowiltasSouls.Content.Items.Summons
{
    public class DraconicCrystal : SoulsItem
    {
        public override string Texture => "Terraria/Images/Item_3828";

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.DD2ElderCrystal);
            Item.width = 20;
            Item.height = 20;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.rare = ItemRarityID.Blue;
            Item.useStyle = ItemUseStyleID.HoldUp;
        }

        public override bool CanUseItem(Player player)
        {
            if (DD2Event.Ongoing || !DD2Event.ReadyForTier3)
                return false;

            return base.CanUseItem(player);
        }

        public override bool? UseItem(Player player)
        {
            Point mousePos = Main.MouseWorld.ToTileCoordinates();
            if (Main.tile[mousePos].TileType != TileID.ElderCrystalStand)
                return false;
            DD2Event.SummonCrystal(mousePos.X, mousePos.Y, player.whoAmI);
            DD2Event.TimeLeftBetweenWaves = 0;
            NPC.waveNumber = 6;
            NPC.waveKills = 220;
            DD2Event.CheckProgress(NPCID.DD2GoblinT3);
            foreach (var i in Main.ActiveItems)
            {
                // kill defender medal drop
                if (i.type == ItemID.DefenderMedal && i.timeSinceItemSpawned == 0)
                    i.active = false;
            }
            player.QuickSpawnItem(Item.GetSource_FromThis(), ItemID.DD2EnergyCrystal, 140); // give all missing crystals

            return true;
        }
    }
}
