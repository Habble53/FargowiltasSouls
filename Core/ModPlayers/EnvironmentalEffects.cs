using FargowiltasSouls.Core.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.Buffs.Masomode;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Localization;
using FargowiltasSouls.Content.Projectiles.Masomode;
using Fargowiltas.Content.Buffs;
using Terraria.GameContent.Events;

namespace FargowiltasSouls.Core.ModPlayers
{
    public partial class EModePlayer : ModPlayer
    {
        public int MasomodeFreezeTimer;
        public int MasomodeSpaceBreathTimer;
        public int LightningCounter;
        public int LightLevelCounter;
        public int HallowFlipCheckTimer;


        public override void PreUpdate()
        {
            if (!WorldSavingSystem.EternityMode)
                return;

            if (!Player.Alive())
                return;

            if (LumUtils.AnyBosses())
                return;

            FargoSoulsPlayer fargoSoulsPlayer = Player.FargoSouls();
            bool waterEffectImmune = fargoSoulsPlayer.BaronsBurden;

            Tile currentTile = getPlayerTile();

            if (!NPC.downedBoss3 && Player.ZoneDungeon && !NPC.AnyNPCs(NPCID.DungeonGuardian) && !Main.drunkWorld && !Main.zenithWorld)
            {
                NPC.SpawnOnPlayer(Player.whoAmI, NPCID.DungeonGuardian);
            }


            
            //water biome effects
            if (WaterWet && !waterEffectImmune)
            {
                if (!(Player.GetJumpState(ExtraJump.Flipper).Enabled || Player.gills || fargoSoulsPlayer.MutantAntibodies))
                    Player.AddBuff(ModContent.BuffType<LethargicBuff>(), 2);

                //quicksand alpha, theres literally no water in ug... unless?
                //if (Player.ZoneUndergroundDesert)
                //{
                //    FargoSoulsUtil.AddDebuffFixedDuration(Player, BuffID.Shimmer, 2);
                //}

                if (Player.ZoneSnow)
                {
                    if (Player.chilled)
                    {
                        Player.AddBuff(ModContent.BuffType<HypothermiaBuff>(), 2);

                        /*
                        MasomodeFreezeTimer++;
                        if (MasomodeFreezeTimer >= 600)
                        {
                            FargoSoulsUtil.AddDebuffFixedDuration(Player, BuffID.Frozen, 120);
                            MasomodeFreezeTimer = -300;
                        }
                        */
                    }
                    else
                    {
                        MasomodeFreezeTimer = 0;
                    }
                }

                if (Player.ZoneJungle)
                {
                    FargoSoulsUtil.AddDebuffFixedDuration(Player, BuffID.Poisoned, 2);
                }

                if (Player.ZoneCrimson)
                {
                    FargoSoulsUtil.AddDebuffFixedDuration(Player, BuffID.Ichor, 300);
                }

                if (Player.ZoneCorrupt)
                {
                    FargoSoulsUtil.AddDebuffFixedDuration(Player, BuffID.CursedInferno, 2);
                }

                if (Player.ZoneHallow)
                {
                    Player.AddBuff(ModContent.BuffType<SmiteBuff>(), 120);
                }
            }


            // Pure Heart-affected biome debuffs
            if (!fargoSoulsPlayer.PureHeart) 
            {
                if (Player.ZoneDesert)
                {
                    DesertDebuffs(currentTile);
                }

                if (currentTile.TileType == TileID.Cactus)
                {
                    CactusDamage(currentTile, fargoSoulsPlayer);
                }

                if (Player.ZoneSnow)
                {
                    //if (!Main.dayTime && Framing.GetTileSafely(Player.Center).WallType == WallID.None)
                    //    Player.AddBuff(BuffID.Chilled, Main.expertMode && Main.expertDebuffTime > 1 ? 1 : 2);
                }

                if (Player.ZoneJungle)
                {
                    JungleStorming(); 
                }

                if (Player.ZoneCorrupt)
                {
                    FargoSoulsUtil.AddDebuffFixedDuration(Player, BuffID.Darkness, 2);
                }

                if (Player.ZoneHallow)
                {
                    HallowedIlluminated();
                }

                // spider biome
                if (currentTile.WallType == WallID.SpiderUnsafe)
                {
                    SpiderWebbed();
                }

                if (Player.ZoneMarble)
                {
                    //Main.NewText("ech");
                }

                if (Player.ZoneGranite)
                {
                    //Main.NewText("ech");
                }

                if (Player.ZoneUnderworldHeight)
                {
                    UnderworldFire();
                }

                if (Player.ZoneSkyHeight)
                {
                    SpaceBreathMeter();
                }

                if (Main.raining)
                {
                    RainLightning(currentTile);
                }

                if (Main.bloodMoon)
                {
                    Player.AddBuff(BuffID.WaterCandle, 2);
                }
                    
                //boss environs??
                //deerclops
                if (!NPC.downedDeerclops && Player.ZoneRockLayerHeight && Player.ZoneSnow && !LumUtils.AnyBosses())
                {
                    DeerclopsHands();
                }

                // hallow lifelight sparks
                if (!WorldSavingSystem.DownedBoss[(int)WorldSavingSystem.Downed.Lifelight] && Player.ZoneHallow && Player.ZoneRockLayerHeight && !LumUtils.AnyBosses())
                {
                    LifelightSparkles();
                }
            }

            //other stuff that does not get disabled by pure heart
            if (Player.ZoneMeteor)
            {
                MeteorFallenStars();
            }


        }

        private void FallDamageDebuff()
        {
            //falling gives you dazed. wings save you
            /*if (Player.velocity.Y == 0f && Player.wingsLogic == 0 && !Player.noFallDmg && !Player.ghost && !Player.dead)
            {
                int num21 = 25;
                num21 += Player.extraFall;
                int num22 = (int)(Player.position.Y / 16f) - Player.fallStart;
                if (Player.mount.CanFly)
                {
                    num22 = 0;
                }
                if (Player.mount.Cart && Minecart.OnTrack(Player.position, Player.width, Player.height))
                {
                    num22 = 0;
                }
                if (Player.mount.Type == 1)
                {
                    num22 = 0;
                }
                Player.mount.FatigueRecovery();

                if (((Player.gravDir == 1f && num22 > num21) || (Player.gravDir == -1f && num22 < -num21)))
                {
                    Player.immune = false;
                    int dmg = (int)(num22 * Player.gravDir - num21) * 10;
                    if (Player.mount.Active)
                        dmg = (int)(dmg * Player.mount.FallDamage);

                    Player.Hurt(PlayerDeathReason.ByOther(0), dmg, 0);
                    Player.AddBuff(BuffID.Dazed, 120);
                }
                Player.fallStart = (int)(Player.position.Y / 16f);
            }*/
        }

        private void JungleStorming()
        {
            if (Main.netMode == NetmodeID.SinglePlayer && Player.ZoneOverworldHeight)
            {
                int day = 86400;
                int hour = day / 24;

                //rain increases if alredy raining
                if (Main.IsItRaining)
                {
                    if (Main.maxRaining < 0.9f && Main.windSpeedCurrent < 0.8f && Main.rand.NextBool(600))
                    {
                        //rain
                        Main.raining = true;
                        Main.maxRaining = Main.cloudAlpha = Math.Min(Main.maxRaining + 0.02f, 0.9f);
                        //wind
                        Main.windSpeedTarget = Main.windSpeedCurrent = Math.Min(Main.windSpeedCurrent + 0.02f, 0.8f);

                        if (Main.netMode == NetmodeID.Server)
                        {
                            NetMessage.SendData(MessageID.WorldData);
                            Main.SyncRain();
                        }

                        //Main.NewText("storm increased.." + Main.maxRaining);
                    }
                }
                //rain increased chnce to start
                else if (WorldUpdatingSystem.rainCD == 0)
                {
                    if (Main.rand.NextBool(7200))
                    {
                        //rain
                        Main.rainTime = hour * 4;
                        Main.raining = true;
                        Main.maxRaining = Main.cloudAlpha = 0.02f;

                        //Main.NewText("rain started");

                        WorldUpdatingSystem.rainCD = 43200;// 1/2 day cooldown
                    }
                }
            }
        }

        private void MeteorFallenStars()
        {
            //5x star rate
            Star.starfallBoost = 5;
            //manually spawn day stars during day
            if (Main.dayTime)
            {
                int starProj = ModContent.ProjectileType<FallenStarDay>();

                for (int m = 0; m < Main.dayRate; m++)
                {
                    double num7 = (double)Main.maxTilesX / 4200.0;
                    num7 *= (double)Star.starfallBoost;
                    if (!((double)Main.rand.Next(8000) < 10.0 * num7))
                    {
                        continue;
                    }
                    int num8 = 12;

                    int randDist = Main.rand.Next(1, 200);
                    float posX = Player.position.X + (float)Main.rand.Next(-randDist, randDist + 1);
                    float posY = Main.rand.Next((int)((double)Main.maxTilesY * 0.05));
                    posY *= 16;
                    Vector2 position = new Vector2(posX, posY);
                    int num11 = -1;//whether or not star travels towards the player

                    if (!Collision.SolidCollision(position, 16, 16))
                    {
                        //Main.NewText("star spawn");

                        float speedX = Main.rand.Next(-100, 101);
                        float speedY = Main.rand.Next(200) + 100;
                        float num16 = (float)Math.Sqrt(speedX * speedX + speedY * speedY);
                        num16 = (float)num8 / num16;
                        speedX *= num16;
                        speedY *= num16;
                        Projectile.NewProjectile(Terraria.Entity.GetSource_NaturalSpawn(), position.X, position.Y, speedX, speedY, starProj, 100, 0f, Main.myPlayer, 0f, num11);
                    }
                }
            }
        }

        private void DesertDebuffs(Tile currentTile)
        {
            if (Player.ZoneOverworldHeight && currentTile.WallType == WallID.None)
            {
                if (Main.dayTime)
                {
                    if (!Player.wet && Main.IsItRaining && !hasUmbrella())
                    {
                        FargoSoulsUtil.AddDebuffFixedDuration(Player, BuffID.Weak, 2);
                    }
                }
                else
                {
                    if (!Player.resistCold && !Player.HasBuff(BuffID.Campfire) && !Player.HasBuff<Semistation>() && !Player.HasBuff<Omnistation>() && !Main.SceneMetrics.HasCampfire && !ItemID.Sets.Torches[Player.HeldItem.type])
                    {
                        FargoSoulsUtil.AddDebuffFixedDuration(Player, BuffID.Chilled, 2);
                    }
                }
            }
        }

        private void CactusDamage(Tile currentTile, FargoSoulsPlayer fargoSoulsPlayer)
        {
            if (currentTile.HasUnactuatedTile && !fargoSoulsPlayer.CactusImmune && !Player.cactusThorns)
            {
                int damage = 10;
                if (WorldSavingSystem.MasochistModeReal)
                {
                    if (Player.ZoneCorrupt)
                    {
                        damage *= 2;
                        Player.AddBuff(BuffID.CursedInferno, 150);
                    }
                    if (Player.ZoneCrimson)
                    {
                        damage *= 2;
                        Player.AddBuff(BuffID.Ichor, 150);
                    }
                    if (Player.ZoneHallow)
                    {
                        damage *= 2;
                        Player.AddBuff(BuffID.Confused, 150);
                    }
                }

                if (Main.hardMode)
                    damage *= 2;

                if (Player.hurtCooldowns[0] <= 0) //same i-frames as spike tiles
                    Player.Hurt(PlayerDeathReason.ByCustomReason(Language.GetTextValue("Mods.FargowiltasSouls.DeathMessage.Cactus", Player.name)), damage, 0, false, false, 0, false);
            }
        }

        private void HallowedIlluminated()
        {
            if (Player.ZoneRockLayerHeight)
            {
                if (++HallowFlipCheckTimer > 6) //reduce computation
                {
                    HallowFlipCheckTimer = 0;

                    float playerAbove = Player.Center.Y - 16 * 50;
                    float playerBelow = Player.Center.Y + 16 * 50;
                    if (playerAbove / 16 < Main.maxTilesY && playerBelow / 16 < Main.maxTilesY
                        && !Collision.CanHitLine(new Vector2(Player.Left.X, playerAbove), 0, 0, new Vector2(Player.Left.X, playerBelow), 0, 0)
                        && !Collision.CanHitLine(new Vector2(Player.Right.X, playerAbove), 0, 0, new Vector2(Player.Right.X, playerBelow), 0, 0))
                    {
                        if (!Main.wallHouse[Framing.GetTileSafely(Player.Center).WallType]
                            && !Main.wallHouse[Framing.GetTileSafely(Player.TopLeft).WallType]
                            && !Main.wallHouse[Framing.GetTileSafely(Player.TopRight).WallType]
                            && !Main.wallHouse[Framing.GetTileSafely(Player.BottomLeft).WallType]
                            && !Main.wallHouse[Framing.GetTileSafely(Player.BottomRight).WallType])
                        {
                            Player.AddBuff(ModContent.BuffType<HallowIlluminatedBuff>(), 90);
                        }
                    }
                }
            }
        }

        private void UnderworldFire()
        {
            bool anyAshwoodEffect = Player.HasEffect<AshWoodEffect>() || Player.HasEffect<ObsidianEffect>();

            if (anyAshwoodEffect || !(Player.fireWalk || Player.lavaMax > 0))
                FargoSoulsUtil.AddDebuffFixedDuration(Player, BuffID.OnFire, 2);
        }

        private void RainLightning(Tile currentTile)
        {
            if (Player.ZoneOverworldHeight
                && !hasUmbrella() && currentTile.WallType == WallID.None)
            {
                if (Player.ZoneSnow)
                    Player.AddBuff(ModContent.BuffType<HypothermiaBuff>(), 2);
                else
                    Player.AddBuff(BuffID.Wet, 2);

                LightningCounter++;

                int lighntningMinSeconds = WorldSavingSystem.MasochistModeReal ? 10 : 17;
                if (LightningCounter >= LumUtils.SecondsToFrames(lighntningMinSeconds))
                {
                    Point tileCoordinates = Player.Top.ToTileCoordinates();

                    tileCoordinates.X += Main.rand.Next(-25, 25);
                    tileCoordinates.Y -= Main.rand.Next(4, 8);


                    bool foundMetal = false;
                    if (WorldSavingSystem.MasochistModeReal)
                        foundMetal = true;

                    /* TODO: make this work
                    for (int x = -5; x < 5; x++)
                    {
                        for (int y = -5; y < 5; y++)
                        {
                            int testX = tileCoordinates.X + x;
                            int testY = tileCoordinates.Y + y;
                            Tile tile = Main.tile[testX, testY];
                            if (IronTiles.Contains(tile.TileType) ||IronTiles.Contains(tile.WallType))
                            {
                                foundMetal = true;
                                tileCoordinates.X = testX;
                                tileCoordinates.Y = testY;
                                Main.NewText("found metal");
                                break;
                            }
                        }
                    }
                    */

                    if (LumUtils.AnyBosses() && !WorldSavingSystem.MasochistModeReal)
                    {
                        LightningCounter = 0;
                    }
                    else if (Main.rand.NextBool(300) || foundMetal)
                    {
                        //tends to spawn in ceilings if the Player goes indoors/underground


                        //for (int index = 0; index < 10 && !WorldGen.SolidTile(tileCoordinates.X, tileCoordinates.Y) && tileCoordinates.Y > 10; ++index) 
                        //    tileCoordinates.Y -= 1;

                        float ai1 = Player.Center.Y;
                        LightningCounter = 0;
                        int projType = ModContent.ProjectileType<RainLightning>();
                        Vector2 pos = new(tileCoordinates.X * 16 + 8, tileCoordinates.Y * 16 + 17 - 900);
                        if (Main.netMode == NetmodeID.MultiplayerClient)
                        {
                            if (Player.whoAmI == Main.myPlayer)
                            {
                                var netMessage = Mod.GetPacket();
                                netMessage.Write((byte)FargowiltasSouls.PacketID.RequestEnvironmentalProjectile);
                                netMessage.Write(projType);
                                netMessage.WriteVector2(pos);
                                netMessage.Write(ai1);
                                netMessage.Send();
                            }
                        }
                        else
                        {
                            int damage = (Main.hardMode ? 120 : 60) / 4;
                            Projectile.NewProjectile(Terraria.Entity.GetSource_NaturalSpawn(), pos, Vector2.Zero, projType, damage, 2f, Main.myPlayer, Vector2.UnitY.ToRotation(), ai1);
                        }
                    }
                }
            }
        }

        private Tile getPlayerTile()
        {
            Vector2 tileCenter = Player.Center;
            tileCenter.X /= 16;
            tileCenter.Y /= 16;
            Tile currentTile = Framing.GetTileSafely((int)tileCenter.X, (int)tileCenter.Y);

            return currentTile;
        }

        private void SpaceBreathMeter()
        {
            if (!Player.buffImmune[BuffID.Suffocation] && Player.whoAmI == Main.myPlayer)
            {
                bool immunity = !Player.armor[0].IsAir && (Player.armor[0].type == ItemID.FishBowl || Player.armor[0].type == ItemID.GoldGoldfishBowl);
                if (Player.accDivingHelm)
                    immunity = true;

                bool inLiquid = Collision.DrownCollision(Player.position, Player.width, Player.height, Player.gravDir);
                if (!inLiquid && !immunity)
                {
                    Player.breath -= 3;
                    if (++MasomodeSpaceBreathTimer > 10)
                    {
                        MasomodeSpaceBreathTimer = 0;
                        Player.breath--;
                    }
                    if (Player.breath == 0)
                        SoundEngine.PlaySound(SoundID.Drown);
                    if (Player.breath <= 0)
                        Player.AddBuff(BuffID.Suffocation, 2);

                    if (Player.breath < -10) //don't stack far into negatives
                    {
                        Player.breath = -10;
                    }
                }
            }
        }

        private void SpiderWebbed()
        {
            if (!Player.buffImmune[BuffID.Webbed] && Player.stickyBreak > 0)
            {
                Player.AddBuff(BuffID.Webbed, 30);
                Player.AddBuff(BuffID.Slow, 90);
                Player.stickyBreak = 0;

                Vector2 vector = Collision.StickyTiles(Player.position, Player.velocity, Player.width, Player.height);
                if (vector.X != -1 && vector.Y != -1)
                {
                    int num3 = (int)vector.X;
                    int num4 = (int)vector.Y;
                    WorldGen.KillTile(num3, num4, false, false, false);
                    if (Main.netMode == NetmodeID.MultiplayerClient && !Main.tile[num3, num4].HasTile)
                        NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 0, num3, num4, 0f, 0, 0, 0);
                }
            }
        }

        private void DeerclopsHands()
        {
            if (Player.ZoneHallow)
                return;

            if (Player.townNPCs >= 2f)
                return;

            Color light = Lighting.GetColor(Player.Center.ToTileCoordinates());
            float lightLevel = light.R + light.G + light.B;

            if (lightLevel >= 500)
                return;

            LightLevelCounter++;
            if (LightLevelCounter > LumUtils.SecondsToFrames(20) && Main.rand.NextBool(600))
            {
                Vector2 pos = Player.Center + Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * 270;
                bool failed = false;
                for (int i = 0; i < 200; i++) // try to find a dark spot
                {
                    pos = Player.Center + Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * 270;
                    Color lightAtPos = Lighting.GetColor(pos.ToTileCoordinates());
                    float lightLevelAtPos = lightAtPos.R + lightAtPos.G + lightAtPos.B;
                    if (lightLevelAtPos < 500)
                        break;
                    if (i == 199) // failed
                        failed = true;
                }
                if (!failed)
                {
                    LightLevelCounter = 0;

                    int projType = ModContent.ProjectileType<DeerclopsDarknessHand>();
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                    {
                        if (Player.whoAmI == Main.myPlayer)
                        {
                            var netMessage = Mod.GetPacket();
                            netMessage.Write((byte)FargowiltasSouls.PacketID.RequestEnvironmentalProjectile);
                            netMessage.Write(projType);
                            netMessage.WriteVector2(pos);
                            netMessage.Send();
                        }
                    }
                    else
                    {
                        int damage = (Main.hardMode ? 120 : 60) / 4;
                        int p = Projectile.NewProjectile(Terraria.Entity.GetSource_NaturalSpawn(), pos, Vector2.Zero, projType, damage, 2f, Main.myPlayer);
                        if (p.IsWithinBounds(Main.maxProjectiles))
                        {
                            Main.projectile[p].light = 1f;
                        }
                    }
                    Lighting.AddLight(pos, 1f, 1f, 1f);
                }
            }
        }

        private void LifelightSparkles()
        {
            if (Player.townNPCs >= 2f)
                return;

            Color light = Lighting.GetColor(Player.Center.ToTileCoordinates());
            float lightLevel = light.R + light.G + light.B;

            if (lightLevel < 500)
                return;

            LightLevelCounter++;
            if (LightLevelCounter > LumUtils.SecondsToFrames(10) && Main.rand.NextBool(300))
            {
                LightLevelCounter = 0;
                Vector2 pos = Player.Center;
                int projType = ModContent.ProjectileType<LifelightEnvironmentStar>();
                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    if (Player.whoAmI == Main.myPlayer)
                    {
                        var netMessage = Mod.GetPacket();
                        netMessage.Write((byte)FargowiltasSouls.PacketID.RequestEnvironmentalProjectile);
                        netMessage.Write(projType);
                        netMessage.WriteVector2(pos);
                        netMessage.Send();
                    }
                }
                else
                {
                    int damage = (Main.hardMode ? 120 : 60) / 4;
                    Projectile.NewProjectile(Terraria.Entity.GetSource_NaturalSpawn(), pos, Vector2.Zero, projType, damage, 2f, Main.myPlayer, -120);
                }
            }
        }




        private bool hasUmbrella()
        {
            return Player.HeldItem.type == ItemID.Umbrella || Player.HeldItem.type == ItemID.TragicUmbrella
                || Player.armor[0].type == ItemID.UmbrellaHat || Player.armor[0].type == ItemID.Eyebrella
                || Player.HasEffect<RainUmbrellaEffect>();
        }

    }
}
