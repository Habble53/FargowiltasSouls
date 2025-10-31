using Fargowiltas.Content.Items.Tiles;
using FargowiltasSouls.Content.Projectiles.Accessories.Souls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
    public class JungleEnchant : BaseEnchant
    {
        public override Color nameColor => new(113, 151, 31);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Blue;
            Item.value = 50000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<JungleJumpEffect>(Item);
            player.jumpBoost = true;
            player.extraFall += 10;
            player.AddEffect<JungleHerbEffect>(Item);
            //player.AddEffect<JungleDashEffect>(Item);
        }

        public override void AddRecipes()
        {
            CreateRecipe()

                .AddIngredient(ItemID.JungleHat)
                .AddIngredient(ItemID.JungleShirt)
                .AddIngredient(ItemID.JunglePants)
                .AddIngredient(ItemID.ThornChakram)
                .AddIngredient(ItemID.IvyWhip)
                .AddRecipeGroup("FargowiltasSouls:AnyStaffofRegrowth")
                //.AddIngredient(ItemID.Buggy);
                //panda pet

                .AddTile<EnchantedTreeSheet>()
                .Register();
        }
    }
    /*public class JungleDashEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<NatureHeader>();
        public override int ToggleItemType => ModContent.ItemType<JungleEnchant>();
        
        public static void AddDash(Player player)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (modPlayer.HasDash)
                return;
            modPlayer.HasDash = true;
            modPlayer.FargoDash = DashManager.DashType.Jungle;
        }
        public static void JungleDash(Player player, int direction)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            float dashSpeed = modPlayer.ChlorophyteEnchantActive ? 12f : 9f;
            player.velocity.X = dashSpeed * direction;
            if (modPlayer.IsDashingTimer < 10)
                modPlayer.IsDashingTimer = 10;
            player.dashDelay = 60;
            if (Main.netMode == NetmodeID.MultiplayerClient)
                NetMessage.SendData(MessageID.PlayerControls, number: player.whoAmI);
        }
    }*/
    public class JungleJumpEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<NatureHeader>();
        public override int ToggleItemType => ModContent.ItemType<JungleEnchant>();
        public override bool ExtraJumpEffect => true;
        public static int BaseDamage(Player player)
        {
            int dmg = 8;
            if (player.FargoSouls().ChlorophyteEnchantActive || player.ForceEffect<JungleJumpEffect>())
                dmg = 48;
            if (player.FargoSouls().ChlorophyteEnchantActive && player.ForceEffect<JungleJumpEffect>())
                dmg = 120;
            return FargoSoulsUtil.HighestDamageTypeScaling(player, dmg);
        }
        public override void PostUpdateEquips(Player player)
        {
            player.GetJumpState<JungleJump>().Enable();
            player.GetJumpState<JungleJump2>().Enable();
            if (player.ForceEffect<JungleJumpEffect>())
                player.GetJumpState<JungleJump3>().Enable();

            if (player.FargoSouls().ChlorophyteEnchantActive)
            {
                player.GetJumpState<JungleJump3>().Enable();
                if (player.ForceEffect<JungleJumpEffect>())
                    player.GetJumpState<JungleJump4>().Enable();

                foreach (ExtraJump jump in ExtraJumpLoader.OrderedJumps)
                {
                    if (jump.Name.Contains("JungleJump") && player.GetJumpState(jump).Active)
                    {
                        player.jumpSpeedBoost += 1f; //not really any other way to buff ascent speed
                        break;
                    }
                }
            }

            /*if (player.whoAmI != Main.myPlayer)
                return;
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (player.grapCount > 0)
            {
                modPlayer.CanJungleJump = true;
                modPlayer.JungleJumping = false;
            }
            else if (player.controlJump)
            {
                if (player.GetJumpState(ExtraJump.BlizzardInABottle).Available || player.GetJumpState(ExtraJump.SandstormInABottle).Available || player.GetJumpState(ExtraJump.CloudInABottle).Available || player.GetJumpState(ExtraJump.FartInAJar).Available || player.GetJumpState(ExtraJump.TsunamiInABottle).Available || player.GetJumpState(ExtraJump.UnicornMount).Available)
                {
                }
                else if (!modPlayer.ChlorophyteEnchantActive)
                {
                    if (player.jump == 0 && player.releaseJump && player.velocity.Y != 0f && !player.mount.Active && modPlayer.CanJungleJump)
                    {
                        player.jump = (int)((double)Player.jumpHeight * 3);

                        modPlayer.JungleJumping = true;
                        modPlayer.JungleCD = 0;
                        modPlayer.CanJungleJump = false;

                        if (Main.netMode == NetmodeID.MultiplayerClient)
                            NetMessage.SendData(MessageID.PlayerControls, number: player.whoAmI);
                    }
                }
            }

            if (modPlayer.JungleJumping)
            {
                if (player.rocketBoots > 0)
                {
                    modPlayer.savedRocketTime = player.rocketTimeMax;
                    player.rocketTime = 0;
                }

                player.runAcceleration *= 3f;
                //Player.maxRunSpeed *= 2f;

                //spwn cloud
                if (modPlayer.JungleCD == 0)
                {
                    int tier = 1;
                    if (modPlayer.ChlorophyteEnchantActive)
                        tier++;
                    bool jungleForceEffect = modPlayer.ForceEffect<JungleEnchant>();
                    if (jungleForceEffect)
                        tier++;

                    modPlayer.JungleCD = 18 - tier * tier;
                    int dmg = 12 * tier * tier - 5;

                    SoundEngine.PlaySound(SoundID.Item62 with { Volume = 0.5f }, player.Center);

                    if (player.whoAmI == Main.myPlayer)
                    {
                        foreach (Projectile p in FargoSoulsUtil.XWay(10, GetSource_EffectItem(player), player.Bottom, ProjectileID.SporeCloud, 4f, FargoSoulsUtil.HighestDamageTypeScaling(player, dmg), 0f))
                        {
                            if (p == null)
                                continue;
                            p.usesIDStaticNPCImmunity = true;
                            p.idStaticNPCHitCooldown = 10;
                            p.FargoSouls().noInteractionWithNPCImmunityFrames = true;
                            p.extraUpdates += 1;
                            p.velocity = p.velocity.RotatedByRandom(MathHelper.PiOver2 * 0.15f);
                            p.velocity *= Main.rand.NextFloat(0.8f, 1.2f);
                            p.DamageType = DamageClass.Default;
                        }
                    }
                }

                if (player.jump == 0 || player.velocity == Vector2.Zero)
                {
                    modPlayer.JungleJumping = false;
                    player.rocketTime = modPlayer.savedRocketTime;
                }
            }
            else if (player.jump <= 0 && player.velocity.Y == 0f)
            {
                modPlayer.CanJungleJump = true;
            }

            if (modPlayer.JungleCD != 0)
            {
                modPlayer.JungleCD--;
            }*/
        }
    }

    public class JungleHerbEffect : AccessoryEffect
    {
        public override Header ToggleHeader => null;
        public override int ToggleItemType => ModContent.ItemType<JungleEnchant>();
    }

    public class JungleJump : ExtraJump
    {
        public static bool Buff(Player player) => player.FargoSouls().ChlorophyteEnchantActive || player.ForceEffect<JungleJumpEffect>();
        public override Position GetDefaultPosition() => new After(SandstormInABottle);
        public override float GetDurationMultiplier(Player player) => Buff(player) ? 2f : 1.2f;
        public override void UpdateHorizontalSpeeds(Player player)
        {
            player.runAcceleration *= Buff(player) ? 3.5f : 2f;
            player.maxRunSpeed *= Buff(player) ? 2f : 1.75f;
        }

        public override void OnStarted(Player player, ref bool playSound)
        {
            playSound = false;
            if (!Main.dedServ) SoundEngine.PlaySound(SoundID.Item62 with { Volume = 0.5f }, player.Center);

            if (player.whoAmI == Main.myPlayer)
            {
                if (!player.FargoSouls().ChlorophyteEnchantActive) //spores
                {
                    foreach (Projectile p in FargoSoulsUtil.XWay(4, player.GetSource_EffectItem<JungleJumpEffect>(), player.Bottom, ProjectileID.SporeCloud, 4f, JungleJumpEffect.BaseDamage(player), 0f))
                    {
                        if (p == null)
                            continue;
                        p.penetrate = 10;
                        p.usesIDStaticNPCImmunity = true;
                        p.idStaticNPCHitCooldown = 10;
                        p.FargoSouls().noInteractionWithNPCImmunityFrames = true;
                        p.velocity.Y = -player.velocity.Y;
                        p.velocity = p.velocity.RotatedByRandom(MathHelper.PiOver4);
                        p.velocity *= Main.rand.NextFloat(0.2f, 1.2f);
                        p.DamageType = DamageClass.Default;
                    }
                }
            }
        }

        public override void ShowVisuals(Player player)
        {
            int offsetY = player.gravDir == -1f ? 6 : player.height - 6;
            Vector2 pos = new(player.position.X, player.position.Y + offsetY);

            if (player.miscCounter % 8 > 4 && player.itemAnimation == 0 && ((player.gravDir == 1f && player.velocity.Y < 0f) || (player.gravDir == -1f && player.velocity.Y > 0f)))
                player.ChangeDir(-player.direction);

            //dust
            for (int i = 0; i < 6; i++)
            {
                float vel = (i % 2 == 0) ? -0.8f : 0.8f;
                bool type = Main.rand.NextBool();
                int dusttype = type ? (player.FargoSouls().ChlorophyteEnchantActive ? DustID.ChlorophyteWeapon : DustID.JungleSpore) : DustID.JungleTorch;
                float scale = player.FargoSouls().ChlorophyteEnchantActive ? (type ? 1.7f : 1) : (type ? 1 : 2);
                Dust dust = Dust.NewDustDirect(pos, player.width, 12, dusttype, player.velocity.X * 0.3f, player.velocity.Y * 0.3f, 100, Scale: scale);
                dust.fadeIn = type ? player.FargoSouls().ChlorophyteEnchantActive ? 2 : 1.5f : 1f;
                dust.velocity *= i < 3 ? 0.1f : 0.6f;
                dust.velocity += player.velocity * vel;
                dust.noGravity = true;
            }

            //leaves
            if (!Main.dedServ)
            {
                Vector2 pos2 = new(pos.X += 4, pos.Y -= 4);
                Gore gore = Gore.NewGoreDirect(player.GetSource_FromThis(), pos2, -player.velocity, GoreID.TreeLeaf_Jungle);
                gore.timeLeft = 1;
                gore.alpha = 50;
            }

            //chloro bomb
            if (player.FargoSouls().ChlorophyteEnchantActive)
            {
                if (player.FargoSouls().ChloroTimer-- <= 0)
                {
                    player.FargoSouls().ChloroTimer = 17;
                    Vector2 vel = -player.velocity;
                    Projectile.NewProjectile(player.GetSource_EffectItem<JungleJumpEffect>(), player.Bottom, vel, ModContent.ProjectileType<ChloroBomb>(), JungleJumpEffect.BaseDamage(player), 2f);
                }
            }
        }

        public override void OnEnded(Player player)
        {
            player.FargoSouls().ChloroTimer = 0;
        }
    }

    public class JungleJump2 : JungleJump { }
    public class JungleJump3 : JungleJump { } //wiz jung or chloro exclusive
    public class JungleJump4 : JungleJump { } //wiz chloro exclusive
}
