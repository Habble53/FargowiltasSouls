using FargowiltasSouls.Content.Buffs.Souls;
using FargowiltasSouls.Content.Items.Accessories.Forces;
using FargowiltasSouls.Content.Projectiles;
using FargowiltasSouls.Content.UI.Elements;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static FargowiltasSouls.Content.Items.Accessories.Forces.TimberForce;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
    public class EbonwoodEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public static readonly Color NameColor = new(100, 90, 141);
        public override Color nameColor => NameColor;


        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Blue;
            Item.value = 10000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<EbonwoodEffect>(Item);

        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ItemID.EbonwoodHelmet)
            .AddIngredient(ItemID.EbonwoodBreastplate)
            .AddIngredient(ItemID.EbonwoodGreaves)
            .AddIngredient(ItemID.VileMushroom)
            .AddIngredient(ItemID.BlackCurrant)
            .AddIngredient(ItemID.LightlessChasms)


            .AddTile(TileID.DemonAltar)
            .Register();
        }
    }
    public class EbonwoodEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<TimberHeader>();
        public override int ToggleItemType => ModContent.ItemType<EbonwoodEnchant>();

        public override void PostUpdateEquips(Player player)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (player.whoAmI != Main.myPlayer)
                return;

            if (modPlayer.EbonwoodCharge < 0)
                modPlayer.EbonwoodCharge = 0;

            bool forceEffect = modPlayer.ForceEffect<EbonwoodEnchant>();
            float chargeCap = forceEffect ? 500 : 250;
            float chargeSpeed = forceEffect ? 2f : 1f;
            float decaySpeed = chargeSpeed / 2.5f;
            if (player.HasEffect<TimberEffect>())
                decaySpeed /= 3;
            bool hasIncreased = false; // If charge has increased this frame
            int dist = ShadewoodEffect.Range(player, forceEffect);
            float LCharge = modPlayer.EbonwoodCharge;

            foreach (NPC npc in Main.npc.Where(n => n.active && !n.friendly && n.lifeMax > 10 && !n.dontTakeDamage && (n.damage > 0 || n.defDamage > 0)))
            {
                Vector2 npcComparePoint = FargoSoulsUtil.ClosestPointInHitbox(npc, player.Center);
                // npc is in aura
                if (player.Distance(npcComparePoint) < dist && Collision.CanHitLine(player.Center, 0, 0, npcComparePoint, 0, 0))
                {
                    if (modPlayer.EbonwoodCharge < chargeCap)
                        modPlayer.EbonwoodCharge += chargeSpeed;
                    hasIncreased = true;
                    break;
                }
            }

            if (!hasIncreased && modPlayer.EbonwoodCharge > 0)
                modPlayer.EbonwoodCharge -= decaySpeed;

            player.endurance += 0.01f * modPlayer.EbonwoodCharge / 50;

            
            CooldownBarManager.Activate("EbonwoodEnchantCharge", ModContent.Request<Texture2D>("FargowiltasSouls/Content/Items/Accessories/Enchantments/EbonwoodEnchant").Value, EbonwoodEnchant.NameColor,
                () => Main.LocalPlayer.FargoSouls().EbonwoodCharge / chargeCap, true, activeFunction: () => player.HasEffect<EbonwoodEffect>());
            
            // charge visual/sound effects
            if (hasIncreased && modPlayer.EbonwoodCharge % (chargeCap / 5) == 0 && modPlayer.EbonwoodCharge != LCharge)
            {
                SoundEngine.PlaySound(SoundID.NPCDeath55, player.Center);
                for (int i = 0; i < 60; i++)
                {
                    Vector2 offset = new();
                    double angle = Main.rand.NextDouble() * 2d * Math.PI;
                    offset.X += (float)(Math.Sin(angle));
                    offset.Y += (float)(Math.Cos(angle));
                    Vector2 spawnPos = player.Center + offset - new Vector2(4, 4);
                    Dust dust = Main.dust[Dust.NewDust(spawnPos, 0, 0, DustID.Shadowflame, 0, 0, 100, Color.White, 1f)];
                    dust.velocity = player.velocity;
                    if (Main.rand.NextBool(3))
                        dust.velocity += Vector2.Normalize(offset) * -5f;
                    dust.noGravity = true;
                }
            }

            //dust
            if (!MoltenAuraProj.CombinedAura(player))
            {
                int visualProj = ModContent.ProjectileType<EbonwoodAuraProj>();
                if (player.ownedProjectileCounts[visualProj] <= 0)
                {
                    Projectile.NewProjectile(GetSource_EffectItem(player), player.Center, Vector2.Zero, visualProj, 0, 0, Main.myPlayer);
                }
            }
        }

        /*
        public static void EbonwoodProc(Player player, NPC npc, int AoE, bool force, int limit)
        {
            //corrupt all in vicinity
            foreach (NPC npcToProcOn in Main.npc.Where(n => n.active && !n.friendly && n.lifeMax > 5 && !n.dontTakeDamage))
            {
                Vector2 npcComparePoint = FargoSoulsUtil.ClosestPointInHitbox(npcToProcOn, npc.Center);
                if (npc.Distance(npcComparePoint) < AoE && !npc.HasBuff<WitheredWizardBuff>() && !npc.HasBuff<WitheredBuff>() && limit > 0)
                {
                    EbonwoodProc(player, npc, AoE, force, limit - 1); //yes this chains (up to 3 times deep)
                }
            }

            Corrupt(npc, force);
            SoundEngine.PlaySound(SoundID.NPCDeath55, npc.Center);
            npc.FargoSouls().EbonCorruptionTimer = 0;

            //dust
            for (int i = 0; i < 60; i++)
            {
                Vector2 offset = new();
                double angle = Main.rand.NextDouble() * 2d * Math.PI;
                offset.X += (float)(Math.Sin(angle) * AoE);
                offset.Y += (float)(Math.Cos(angle) * AoE);
                Vector2 spawnPos = npc.Center + offset - new Vector2(4, 4);
                Dust dust = Main.dust[Dust.NewDust(spawnPos, 0, 0, DustID.Shadowflame, 0, 0, 100, Color.White, 1f)];
                dust.velocity = npc.velocity;
                if (Main.rand.NextBool(3))
                    dust.velocity += Vector2.Normalize(offset) * -5f;
                dust.noGravity = true;
            }
        }
        private static void Corrupt(NPC npc, bool force)
        {
            if (npc.HasBuff<WitheredWizardBuff>() || npc.HasBuff<WitheredBuff>()) //don't stack the buffs under any circumstances
            {
                return;
            }
            if (force)
            {
                npc.AddBuff(ModContent.BuffType<WitheredWizardBuff>(), 60 * 4);
                return;
            }
            npc.AddBuff(ModContent.BuffType<WitheredBuff>(), 60 * 4);
        }
        */
    }
}
