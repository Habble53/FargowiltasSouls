using Fargowiltas.Items.Ammos;
using Fargowiltas.NPCs;
using FargowiltasSouls.Common.Graphics.Particles;
using FargowiltasSouls.Content.Bosses.MutantBoss;
using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Content.Buffs.Souls;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.Items.Accessories.Forces;
using FargowiltasSouls.Content.Items.Accessories.Masomode;
using FargowiltasSouls.Content.Items.Misc;
using FargowiltasSouls.Content.Items.Summons;
using FargowiltasSouls.Content.Items.Weapons.BossDrops;
using FargowiltasSouls.Content.Items.Weapons.Misc;
using FargowiltasSouls.Content.NPCs.Critters;
using FargowiltasSouls.Content.NPCs.EternityModeNPCs;
using FargowiltasSouls.Content.Projectiles.ChallengerItems;
using FargowiltasSouls.Content.Projectiles.Masomode;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.ItemDropRules;
using FargowiltasSouls.Core.ModPlayers;
using FargowiltasSouls.Core.Systems;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using static FargowiltasSouls.Content.Items.Accessories.Forces.TimberForce;
using static tModPorter.ProgressUpdate;

namespace FargowiltasSouls.Core.Globals
{
    public class FargoSoulsGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

#pragma warning disable CA2211

        public static int boss = -1;
#pragma warning restore CA2211

        public int originalDefense;
        public bool BrokenArmor;

        public bool CanHordeSplit = true;

        public bool FirstTick;
        //        //debuffs
        public bool OriPoison;
        public bool EarthPoison;
        public int EarthDoTValue; //value to base Earth Poison DoT on.
        public bool SBleed;
        public bool TimberBleed;
        //        public bool Shock;
        public bool Rotting;
        public bool LeadPoison;
        public bool Needled;
        public bool SolarFlare;
        public bool TimeFrozen;
        public bool HellFire;
        public bool HellFireMarked;
        // public bool Corrupted;
        // public bool CorruptedForce;
        public bool Infested;
        public int MaxInfestTime;
        public float InfestedDust;
        public bool Electrified;
        public bool CurseoftheMoon;
        public int lightningRodTimer;
        public bool Sadism;
        public bool OceanicMaul;
        public bool MutantNibble;
        public int LifePrevious = -1;
        public bool GodEater;
        public bool Suffocation;
        public int SuffocationTimer;
        public bool DeathMarked;
        //        public bool Villain;
        public bool FlamesoftheUniverse;
        public bool Lethargic;
        public int LethargicCounter;
        //        public bool ExplosiveCritter = false;
        //        private int critterCounter = 120;
        public bool Sublimation;

        public bool SnowChilled;
        public int SnowChilledTimer;

        public bool Chilled;
        public bool Smite;
        public bool MoltenAmplify;
        public bool Anticoagulation;
        public bool BloodDrinker;
        public bool MagicalCurse;

        public int NecroDamage;

        public bool PungentGazeWasApplied;
        public int PungentGazeTime;
        public bool SinisterIconFullFight;

        public int GrazeCD;

        //        public static bool Revengeance => CalamityMod.World.CalamityWorld.revenge;

        static HashSet<int> RareNPCs = [];

        public override void Unload()
        {
            base.Unload();
            RareNPCs = null;
        }

        public override void ResetEffects(NPC npc)
        {
            BrokenArmor = false;
            TimeFrozen = false;
            SBleed = false;
            TimberBleed = false;
            //            Shock = false;
            Rotting = false;
            LeadPoison = false;
            SolarFlare = false;
            HellFire = false;
            HellFireMarked = false;
            // Corrupted = false;
            // CorruptedForce = false;
            OriPoison = false;
            EarthPoison = false;
            Infested = false;
            Electrified = false;
            CurseoftheMoon = false;
            Sadism = false;
            OceanicMaul = false;
            MutantNibble = false;
            GodEater = false;
            Suffocation = false;
            Sublimation = false;
            DeathMarked = false;
            //            //SnowChilled = false;
            Chilled = false;
            Smite = false;
            MoltenAmplify = false;
            Anticoagulation = false;
            BloodDrinker = false;
            FlamesoftheUniverse = false;
            MagicalCurse = false;
            PungentGazeTime = 0;
        }
        public override void SetStaticDefaults()
        {
            //blightus deletus
            if (ModContent.TryFind("CalamityMod", "MiracleBlight", out ModBuff miracleBlight))
            {
                foreach (ModNPC npc in Mod.GetContent<ModNPC>())
                {
                    NPCID.Sets.SpecificDebuffImmunity[npc.Type][miracleBlight.Type] = true;
                }
            }


        }
        public override void SetDefaults(NPC npc)
        {
            if (npc.rarity > 0 && !RareNPCs.Contains(npc.type))
                RareNPCs.Add(npc.type);
        }
        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            base.OnSpawn(npc, source);
            if (SinisterIconBoss(npc))
            {
                foreach (var player in Main.ActivePlayers)
                {
                    if (player.Alive() && player.HasEffect<SinisterIconDropsEffect>())
                        SinisterIconFullFight = true;
                }
            }
        }
        public override bool PreAI(NPC npc)
        {
            if (npc.boss || npc.type == NPCID.EaterofWorldsHead)
                boss = npc.whoAmI;
            if (!LumUtils.AnyBosses())
                boss = -1;

            bool retval = base.PreAI(npc);
            if (TimeFrozen)
            {
                npc.position = npc.oldPosition;
                npc.frameCounter = 0;
                retval = false;
            }

            if (SinisterIconBoss(npc))
            {
                foreach (var player in Main.ActivePlayers)
                {
                    if (!(player.Alive() && player.HasEffect<SinisterIconDropsEffect>()))
                        SinisterIconFullFight = false;
                }
            }

            if (!FirstTick)
            {
                originalDefense = npc.defense;


                //                switch (npc.type)
                //                {
                //                    case NPCID.TheDestroyer:
                //                    case NPCID.TheDestroyerBody:
                //                    case NPCID.TheDestroyerTail:
                //                        npc.buffImmune[ModContent.BuffType<TimeFrozen>()] = false;
                //                        npc.buffImmune[ModContent.BuffType<Frozen>()] = false;
                //                        //npc.buffImmune[BuffID.Darkness] = false;
                //                        break;

                //                    /*case NPCID.WallofFlesh:
                //                    case NPCID.WallofFleshEye:
                //                    case NPCID.MoonLordCore:
                //                    case NPCID.MoonLordHand:
                //                    case NPCID.MoonLordHead:
                //                    case NPCID.MoonLordLeechBlob:
                //                    case NPCID.TargetDummy:
                //                    case NPCID.GolemFistLeft:
                //                    case NPCID.GolemFistRight:
                //                    case NPCID.GolemHead:
                //                    case NPCID.DungeonGuardian:
                //                    case NPCID.DukeFishron:
                //                        SpecialEnchantImmune = true;
                //                        break;*/

                //                    default:
                //                        break;
                //                }

                //                //critters
                //                if (npc.damage == 0 && !npc.townNPC && npc.lifeMax == 5)
                //                {
                //                    Player player = Main.player[Main.myPlayer];

                //                    /*if ( npc.releaseOwner == player.whoAmI && player.FargoSouls().WoodEnchant)
                //                    {
                //                        switch (npc.type)
                //                        {
                //                            case NPCID.Bunny:


                //                                npc.active = false;
                //                                break;
                //                        }



                //                        ExplosiveCritter = true;
                //                    }*/
                //                }

                FirstTick = true;
            }

            if (Lethargic && ++LethargicCounter > 3)
            {
                LethargicCounter = 0;
                retval = false;
            }

            //            if (ExplosiveCritter)
            //            {
            //                critterCounter--;

            //                if (critterCounter <= 0)
            //                {
            //                    Player player = Main.player[npc.releaseOwner];
            //                    FargoSoulsPlayer modPlayer = player.FargoSouls();

            //                    int damage = 25;

            //                    if (modPlayer.WoodForce || modPlayer.WizardEnchant)
            //                    {
            //                        damage *= 5;
            //                    }

            //                    Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<ExplosionSmall>(), modPlayer.HighestDamageTypeScaling(damage), 4, npc.releaseOwner);
            //                    //gold critters make coin value go up of hit enemy, millions of other effects eeech
            //                }

            //            }
            if (SnowChilled)
            {
                SnowChilledTimer--;

                if (SnowChilledTimer <= 0)
                    SnowChilled = false;

                if (SnowChilledTimer % 2 == 1)
                {
                    npc.position = npc.oldPosition;
                    retval = false;
                }
            }
            return retval;
        }


        public override void PostAI(NPC npc)
        {
            if (BrokenArmor)
            {
                npc.defense = originalDefense - 10;
                if (npc.defense < 0)
                    npc.defense = 0;
            }

            if (Sublimation)
            {
                npc.defense = originalDefense - 15; //ichor 2
                if (npc.defense < 0)
                    npc.defense = 0;
            }

            if (SnowChilled)
            {
                int dustId = Dust.NewDust(npc.position, npc.width, npc.height, DustID.Snow, npc.velocity.X, npc.velocity.Y, 100, default, 1f);
                Main.dust[dustId].noGravity = true;
            }

            SuffocationTimer += Suffocation ? 1 : -3;
            if (SuffocationTimer < 0)
                SuffocationTimer = 0;

            if (!npc.friendly && npc.damage > 0
                && Main.LocalPlayer.active && !Main.LocalPlayer.dead)
            {
                if (--GrazeCD < 0) //managed by the npc itself so worm segments dont make it count down faster
                    GrazeCD = 6;

                NPC realLifeNPC = FargoSoulsUtil.NPCExists(npc.realLife);
                FargoSoulsGlobalNPC npcForGrazeCD = realLifeNPC is not null ? realLifeNPC.FargoSouls() : npc.FargoSouls();

                if (npcForGrazeCD.GrazeCD == 0)
                {
                    FargoSoulsPlayer fargoPlayer = Main.LocalPlayer.FargoSouls();
                    if (fargoPlayer.Graze && !Main.LocalPlayer.immune && Main.LocalPlayer.hurtCooldowns[0] <= 0 && Main.LocalPlayer.hurtCooldowns[1] <= 0)
                    {
                        Vector2 point = FargoSoulsUtil.ClosestPointInHitbox(npc.Hitbox, Main.LocalPlayer.Center);
                        int dummy = -1;
                        if (Main.LocalPlayer.Distance(point) < fargoPlayer.GrazeRadius
                            && NPCLoader.CanHitPlayer(npc, Main.LocalPlayer, ref dummy)
                            && (npc.ModNPC == null || npc.ModNPC.CanHitPlayer(Main.LocalPlayer, ref dummy))
                            && (npc.noTileCollide || Collision.CanHitLine(point, 0, 0, Main.LocalPlayer.Center, 0, 0)))
                        {
                            npcForGrazeCD.GrazeCD = 30;

                            if (fargoPlayer.DeviGraze)
                                SparklingAdoration.OnGraze(fargoPlayer, npc.damage);
                            if (fargoPlayer.CirnoGraze)
                                IceQueensCrown.OnGraze(fargoPlayer, npc.damage);
                        }
                    }
                }
            }
        }

        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
            if (LeadPoison)
            {
                if (Main.rand.Next(4) < 3)
                {
                    int dust = Dust.NewDust(new Vector2(npc.position.X - 2f, npc.position.Y - 2f), npc.width + 4, npc.height + 4, DustID.Lead, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 100, default, 1f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.8f;
                    Dust d = Main.dust[dust];
                    d.velocity.Y -= 0.5f;
                    if (Main.rand.NextBool(4))
                    {
                        d.noGravity = false;
                        d.scale *= 0.5f;
                    }
                }
            }

            /*
            if (Corrupted || CorruptedForce)
            {
                if (Main.rand.Next(8) < 9)
                {
                    int dust = Dust.NewDust(new Vector2(npc.position.X - 2f, npc.position.Y - 2f), npc.width + 4, npc.height + 4, DustID.Shadowflame, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 100);
                    Main.dust[dust].noGravity = true;

                    Dust d = Main.dust[dust];
                    d.velocity.Y -= 10f;
                }
            }
            */

            if (OriPoison)
            {
                if (Main.rand.Next(4) < 3)
                {
                    int dust = Dust.NewDust(new Vector2(npc.position.X - 2f, npc.position.Y - 2f), npc.width + 4, npc.height + 4, DustID.PinkTorch, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 100, default, 1f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.8f;
                    Dust d = Main.dust[dust];
                    d.velocity.Y -= 0.5f;
                    if (Main.rand.NextBool(4))
                    {
                        d.noGravity = false;
                        d.scale *= 0.5f;
                    }
                }
            }

            if (MagicalCurse)
            {
                if (Main.rand.NextBool(4))
                {
                    int d = Dust.NewDust(npc.position, npc.width, npc.height, Main.rand.NextBool() ? 107 : 157);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity *= 0.2f;
                    Main.dust[d].scale = 1.5f;
                }
            }

            if (Sublimation)
            {
                if (Main.rand.NextBool(4))
                {
                    int d = Dust.NewDust(npc.position, npc.width, npc.height, DustID.PortalBolt, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 0, new Color(220, 255, 220), 2.5f);
                    Main.dust[d].velocity.Y -= 1;
                    Main.dust[d].velocity *= 1.5f;
                    Main.dust[d].noGravity = true;
                }
            }

            if (HellFire)
            {
                if (Main.rand.Next(4) < 3)
                {
                    int dust = Dust.NewDust(new Vector2(npc.position.X - 2f, npc.position.Y - 2f), npc.width + 4, npc.height + 4, DustID.SolarFlare, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 100);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].shader = GameShaders.Armor.GetSecondaryShader(56, Main.LocalPlayer);

                    Dust d = Main.dust[dust];
                    d.velocity.Y -= 0.5f;
                    if (Main.rand.NextBool(4))
                    {
                        d.noGravity = false;
                        d.scale *= 0.5f;
                    }
                }
            }

            if (HellFireMarked)
            {
                if (Main.rand.NextBool(3))
                {
                    int dust = Dust.NewDust(npc.position, npc.width, npc.height, DustID.SolarFlare, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, Scale: 4f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].shader = GameShaders.Armor.GetSecondaryShader(56, Main.LocalPlayer);

                    Dust d = Main.dust[dust];
                    d.velocity.Y -= 0.5f;
                    if (Main.rand.NextBool(4))
                    {
                        d.noGravity = false;
                        d.scale *= 0.5f;
                    }

                    d.velocity *= 3;
                }
            }

            if (SBleed || TimberBleed)
            {
                if (Main.rand.Next(4) < 3)
                {
                    int dust = Dust.NewDust(new Vector2(npc.position.X - 2f, npc.position.Y - 2f), npc.width + 4, npc.height + 4, DustID.Blood, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 100);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].shader = GameShaders.Armor.GetSecondaryShader(56, Main.LocalPlayer);

                    Dust d = Main.dust[dust];
                    d.velocity.Y -= 0.5f;
                    if (Main.rand.NextBool(4))
                    {
                        d.noGravity = false;
                        d.scale *= 0.5f;
                    }
                }
            }

            //            /*if (Infested)
            //            {
            //                if (Main.rand.Next(4) < 3)
            //                {
            //                    int dust = Dust.NewDust(new Vector2(npc.position.X - 2f, npc.position.Y - 2f), npc.width + 4, npc.height + 4, 44, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 100, Color.LimeGreen, InfestedDust);
            //                    Main.dust[dust].noGravity = true;
            //                    Main.dust[dust].velocity *= 1.8f;
            //                    Dust expr_1CCF_cp_0 = Main.dust[dust];
            //                    expr_1CCF_cp_0.velocity.Y = expr_1CCF_cp_0.velocity.Y - 0.5f;
            //                    if (Main.rand.NextBool(4))
            //                    {
            //                        Main.dust[dust].noGravity = false;
            //                        Main.dust[dust].scale *= 0.5f;
            //                    }
            //                }

            //                Lighting.AddLight((int)(npc.position.X / 16f), (int)(npc.position.Y / 16f + 1f), 1f, 0.3f, 0.1f);
            //            }*/

            if (Suffocation)
                drawColor = Colors.RarityPurple;

            //            if (Villain)
            //            {
            //                if (Main.rand.Next(4) < 3)
            //                {
            //                    int dust = Dust.NewDust(new Vector2(npc.position.X - 2f, npc.position.Y - 2f), npc.width + 4, npc.height + 4, DustID.AncientLight, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 100);
            //                    Main.dust[dust].noGravity = true;
            //                    Main.dust[dust].velocity *= 1.8f;
            //                    Dust expr_1CCF_cp_0 = Main.dust[dust];
            //                    expr_1CCF_cp_0.velocity.Y = expr_1CCF_cp_0.velocity.Y - 0.5f;
            //                    if (Main.rand.NextBool(4))
            //                    {
            //                        Main.dust[dust].noGravity = false;
            //                        Main.dust[dust].scale *= 0.5f;
            //                    }
            //                }

            //                Lighting.AddLight((int)(npc.position.X / 16f), (int)(npc.position.Y / 16f + 1f), 1f, 0.3f, 0.1f);
            //            }

            if (Electrified)
            {
                if (Main.rand.Next(4) < 3)
                {
                    int dust = Dust.NewDust(new Vector2(npc.position.X - 2f, npc.position.Y - 2f), npc.width + 4, npc.height + 4, DustID.Vortex, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.8f;
                    if (Main.rand.NextBool(3))
                    {
                        Main.dust[dust].noGravity = false;
                        Main.dust[dust].scale *= 0.5f;
                    }
                }

                Lighting.AddLight((int)npc.Center.X / 16, (int)npc.Center.Y / 16, 0.3f, 0.8f, 1.1f);
            }

            if (CurseoftheMoon)
            {
                int d = Dust.NewDust(npc.Center, 0, 0, DustID.Vortex, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity *= 3f;
                Main.dust[d].scale += 0.5f;

                if (Main.rand.Next(4) < 3)
                {
                    d = Dust.NewDust(npc.position, npc.width, npc.height, DustID.Vortex, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity.Y -= 1f;
                    Main.dust[d].velocity *= 2f;
                }
            }

            if (Sadism)
            {
                if (Main.rand.NextBool(7))
                {
                    int d = Dust.NewDust(npc.position, npc.width, npc.height, DustID.UltraBrightTorch, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 0, Color.White, 4f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity *= 2f;
                }
            }

            if (GodEater)
            {
                if (Main.rand.NextBool(7))
                {
                    int dust = Dust.NewDust(npc.position - new Vector2(2f, 2f), npc.width + 4, npc.height + 4, DustID.GemAmethyst, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 0, Color.White, 4f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.2f;
                    Main.dust[dust].velocity.Y -= 0.15f;
                }
                Lighting.AddLight(npc.position, 0.15f, 0.03f, 0.09f);
            }

            if (Chilled)
            {
                int d = Dust.NewDust(npc.Center, 0, 0, DustID.MagicMirror, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity *= 3f;
                Main.dust[d].scale += 0.5f;

                if (Main.rand.Next(4) < 3)
                {
                    d = Dust.NewDust(npc.position, npc.width, npc.height, DustID.MagicMirror, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity.Y -= 1f;
                    Main.dust[d].velocity *= 2f;
                }
            }

            if (FlamesoftheUniverse)
            {
                if (!Main.rand.NextBool(3))
                {
                    int d = Dust.NewDust(npc.position, npc.width, npc.height, DustID.Scorpion, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 0, new Color(50 * Main.rand.Next(6) + 5, 50 * Main.rand.Next(6) + 5, 50 * Main.rand.Next(6) + 5, 0), 2.5f);
                    Main.dust[d].velocity.Y -= 1;
                    Main.dust[d].velocity *= 1.5f;
                    Main.dust[d].noGravity = true;
                }
            }

            if (Smite)
            {
                if (!Main.rand.NextBool(4))
                {
                    Color color = Main.DiscoColor;
                    int d = Dust.NewDust(npc.position, npc.width, npc.height, DustID.GemDiamond, 0.0f, 0.0f, 100, color, 2.5f);
                    Main.dust[d].velocity *= 2f;
                    Main.dust[d].noGravity = true;
                }
            }

            if (Anticoagulation)
            {
                if (!Main.rand.NextBool(4))
                {
                    int d = Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood);
                    Main.dust[d].velocity *= 2f;
                    Main.dust[d].scale += 1f;
                }
            }

            if (BloodDrinker)
            {
                if (!Main.rand.NextBool(3))
                {
                    int d = Dust.NewDust(npc.position, npc.width, npc.height, DustID.LifeDrain, npc.velocity.X * 0.2f, npc.velocity.Y * 0.2f, 0, Color.White, 2.5f);
                    Main.dust[d].noGravity = true;
                }
            }

            if (PungentGazeTime > 0)
            {
                if (Main.rand.NextBool(3))
                {
                    float ratio = (float)PungentGazeTime / PungentGazeBuff.MAX_TIME;
                    Vector2 sparkDir = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi);
                    float sparkDistance = 20 * Main.rand.NextFloat(0.6f, 1.3f);
                    Vector2 sparkCenter = npc.Center + sparkDir * sparkDistance * 2;
                    float sparkTime = 15;
                    Vector2 sparkVel = (npc.Center - sparkCenter) / sparkTime;
                    float sparkScale = MathHelper.Lerp(0.25f, 1.5f,ratio);
                    Particle spark = new SmallSparkle(npc.Center, sparkVel, Color.Red, sparkScale, (int)sparkTime);
                    spark.Spawn();
                }
            }
            if (DeathMarked)
            {
                drawColor.R = (byte)(drawColor.R * 0.7f);
                drawColor.G = (byte)(drawColor.G * 0.6f);
                drawColor.B = (byte)(drawColor.B * 0.7f);
            }

        }
        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            int shrapnel = 0;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if (proj.TypeAlive<BaronTuskShrapnel>() && proj.owner == Main.myPlayer)
                {
                    if (proj.As<BaronTuskShrapnel>().EmbeddedNPC == npc)
                    {
                        shrapnel++;
                    }
                }
            }
            if (shrapnel >= 15)
            {
                Texture2D texture = (Texture2D)ModContent.Request<Texture2D>("FargowiltasSouls/Content/Projectiles/GlowRing", AssetRequestMode.ImmediateLoad);
                Rectangle rectangle = texture.Bounds;
                Vector2 origin2 = rectangle.Size() / 2f;
                Color color = Color.Red;
                float ringScale = npc.scale / 3f;
                spriteBatch.Draw(texture, npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color, npc.rotation, origin2, ringScale, SpriteEffects.None, 0);
            }
            base.PostDraw(npc, spriteBatch, screenPos, drawColor);
        }
        public override Color? GetAlpha(NPC npc, Color drawColor)
        {
            if (Chilled)
            {
                drawColor = Color.LightBlue;
                return drawColor;
            }

            return null;
        }

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            Player player = Main.player[Main.myPlayer];
            FargoSoulsPlayer modPlayer = player.FargoSouls();

            if (Rotting)
            {
                if (npc.lifeRegen > 0)
                    npc.lifeRegen = 0;

                npc.lifeRegen -= 100;

                if (damage < 5)
                    damage = 5;
            }

            if (LeadPoison)
            {
                if (npc.lifeRegen > 0)
                {
                    npc.lifeRegen = 0;
                }

                int dot = npc.type == NPCID.EaterofWorldsBody ? 4 : 20;

                //calamity worms mod compat
                if (ModLoader.HasMod("CalamityMod"))
                {
                    if (ModContent.TryFind("CalamityMod", "DesertScourgeBody", out ModNPC scourgeBody) && npc.type == scourgeBody.Type)
                    {
                        dot = 4;
                    }
                }
                bool forceEffect = Main.player.Any(p => p.Alive() && p.HasEffect<LeadEffect>() && p.FargoSouls() is FargoSoulsPlayer pF && pF != null && pF.ForceEffect<LeadEnchant>());
                if (forceEffect)
                {
                    dot *= 3;
                }

                bool terraEffect = Main.player.Any(p => p.Alive() && p.HasEffect<TerraLightningEffect>());
                if (terraEffect)
                    dot = 250;

                npc.lifeRegen -= dot;
                if (damage < (int)(dot / 10f))
                    damage = (int)(dot / 10f);
            }


            //50 dps
            if (SolarFlare)
            {
                if (npc.lifeRegen > 0)
                {
                    npc.lifeRegen = 0;
                }

                npc.lifeRegen -= 100;

                if (damage < 10)
                {
                    damage = 10;
                }
            }

            //100 dps
            if (HellFire)
            {
                if (npc.lifeRegen > 0)
                    npc.lifeRegen = 0;

                int hellfireMarkedMultiplier = HellFireMarked ? 10 : 1;

                npc.lifeRegen -= 200 * hellfireMarkedMultiplier;

                int shownDamage = 50 * hellfireMarkedMultiplier;
                if (damage < shownDamage)
                {
                    damage = shownDamage;
                }
            }

            //25 dps which is 1 more than  
            if (Sublimation)
            {
                if (npc.lifeRegen > 0)
                    npc.lifeRegen = 0;

                npc.lifeRegen -= 50;

                if (damage < 5)
                    damage = 5;
            }

            //12 dps 
            if (OriPoison)
            {
                if (npc.lifeRegen > 0)
                    npc.lifeRegen = 0;

                npc.lifeRegen -= 24;

                if (damage < 2)
                    damage = 2;
            }
            if (EarthPoison)
            {
                int EarthDamage = EarthDoTValue;

                if (npc.lifeRegen > 0)
                    npc.lifeRegen = 0;

                npc.lifeRegen -= EarthDamage;
                if (damage < EarthDamage / 8)
                    damage = EarthDamage / 8;
            }

            if (Infested)
            {
                if (npc.lifeRegen > 0)
                    npc.lifeRegen = 0;

                npc.lifeRegen -= InfestedExtraDot(npc);

                if (damage < 8)
                    damage = 8;
            }
            else
            {
                MaxInfestTime = 0;
            }

            if (Electrified)
            {
                if (npc.lifeRegen > 0)
                    npc.lifeRegen = 0;

                npc.lifeRegen -= 4;
                if (npc.velocity != Vector2.Zero)
                    npc.lifeRegen -= 16;
                if (npc.wet)
                    npc.lifeRegen -= 16;

                if (damage < 4)
                    damage = 4;
            }

            if (CurseoftheMoon)
            {
                if (npc.lifeRegen > 0)
                    npc.lifeRegen = 0;

                npc.lifeRegen -= 24;

                if (damage < 6)
                    damage = 6;
            }

            if (OceanicMaul)
            {
                if (npc.lifeRegen > 0)
                    npc.lifeRegen = 0;

                npc.lifeRegen -= 48;

                if (damage < 12)
                    damage = 12;
            }

            if (Sadism)
            {
                if (npc.lifeRegen > 0)
                    npc.lifeRegen = 0;

                npc.lifeRegen -= 170 + 48 + 60 + 8 + 4 + 16;

                if (damage < 70)
                    damage = 70;
            }

            if (MutantNibble)
            {
                if (npc.lifeRegen > 0)
                    npc.lifeRegen = 0;
                if (npc.lifeRegenCount > 0)
                    npc.lifeRegenCount = 0;

                if (npc.life > 0 && LifePrevious > 0) //trying to prevent some wack despawn stuff
                {
                    if (npc.life > LifePrevious)
                        npc.life = LifePrevious;
                    else
                        LifePrevious = npc.life;
                }
            }
            else
            {
                LifePrevious = npc.life;
            }

            if (GodEater)
            {
                if (npc.lifeRegen > 0)
                    npc.lifeRegen = 0;

                npc.lifeRegen -= 4200;

                if (damage < 777)
                    damage = 777;
            }

            if (Suffocation)
            {
                if (npc.lifeRegen > 0)
                    npc.lifeRegen = 0;
                npc.lifeRegen -= (int)(40f * Math.Min(1f, 1f * SuffocationTimer / 480));
                if (damage < 5)
                    damage = 5;
            }

            if (FlamesoftheUniverse)
            {
                if (npc.lifeRegen > 0)
                    npc.lifeRegen = 0;
                npc.lifeRegen -= (30 + 50 + 48 + 30) / 2;
                if (damage < 20)
                    damage = 20;
            }

            if (TimberBleed)
            {
                npc.lifeRegen -= 400;
                if (damage < 40)
                    damage = 40;
            }

            if (Anticoagulation)
            {
                if (npc.lifeRegen > 0)
                    npc.lifeRegen = 0;
                npc.lifeRegen -= 16;
                if (damage < 6)
                    damage = 6;
            }

            float dotMultiplier = DoTMultiplier(npc, modPlayer.Player);
            if (dotMultiplier != 1 && npc.lifeRegen < 0)
            {
                npc.lifeRegen = (int)(npc.lifeRegen * dotMultiplier);
                damage = (int)(damage * dotMultiplier);
            }

            if (TimeFrozen && npc.life == 1)
            {
                if (npc.lifeRegen < 0)
                    npc.lifeRegen = 0;
            }
        }
        public static float DoTMultiplier(NPC npc, Player player)
        {
            float multiplier = 1;
            if (npc.lifeRegen >= 0)
                return multiplier;

            if (player.HasEffect<OrichalcumEffect>())
                multiplier += OrichalcumEffect.OriDotModifier(npc, player.FargoSouls()) - 1;

            if (npc.FargoSouls().MagicalCurse)
                multiplier += 1;

            //half as effective if daybreak applied
            if (npc.daybreak && multiplier > 1)
                multiplier -= (multiplier - 1) / 2;

            return multiplier;
        }
        private int InfestedExtraDot(NPC npc)
        {
            int buffIndex = npc.FindBuffIndex(ModContent.BuffType<InfestedBuff>());
            if (buffIndex == -1)
                return 0;

            int timeLeft = npc.buffTime[buffIndex];
            if (MaxInfestTime <= 0)
                MaxInfestTime = timeLeft;
            float baseVal = (MaxInfestTime - timeLeft) / 20f; //change the denominator to adjust max power of DOT
            int dmg = (int)(baseVal * baseVal + 8);

            InfestedDust = baseVal / 15 + .5f;
            if (InfestedDust > 5f)
                InfestedDust = 5f;

            return dmg;
        }

        public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();

            if (modPlayer.Bloodthirsty)
            {
                //100x spawn rate
                spawnRate = (int)(spawnRate * 0.01);
                //2x max spawn
                maxSpawns *= 3;
            }

            if (modPlayer.Illuminated)
            {
                spawnRate = (int)(spawnRate / 1.5f);
                maxSpawns = (int)(maxSpawns * 1.5f);
                /*
                Color light = Lighting.GetColor(player.Center.ToTileCoordinates());
                float modifier = (light.R + light.G + light.B) / 700f;
                modifier = MathHelper.Clamp(modifier, 0, 1);
                modifier += 1;

                spawnRate = (int)(spawnRate / modifier);
                maxSpawns = (int)(maxSpawns * modifier);
                Main.NewText(spawnRate);
                Main.NewText(maxSpawns);
                */
            }

            if (player.HasEffect<SinisterIconEffect>())
            {
                spawnRate /= 2;
                maxSpawns *= 2;
            }

            //if (modPlayer.BuilderMode) maxSpawns = 0;
        }

        public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.FargoSouls().PungentEyeball)
            {
                foreach (var entry in pool)
                {
                    if (RareNPCs.Contains(entry.Key))
                    {
                        pool[entry.Key] = entry.Value * 5;
                    }
                }
            }

            int y = spawnInfo.SpawnTileY;
            bool day = Main.dayTime;
            bool surface = y < Main.worldSurface && !spawnInfo.Sky;
            if (day && surface && spawnInfo.PlayerInTown && FargowiltasSouls.NoBiome(spawnInfo) && FargowiltasSouls.NoZone(spawnInfo))
            {
                pool[ModContent.NPCType<TophatSquirrelCritter>()] = 0.03f;
            }
        }

        private bool lootMultiplierCheck;
        public static bool SinisterIconBoss(NPC npc) => npc.boss || IllegalLootMultiplierNPCs.Contains(npc.type);
        public static int[] IllegalLootMultiplierNPCs => [
            NPCID.DD2Betsy,
            NPCID.EaterofWorldsBody,
            NPCID.EaterofWorldsHead,
            NPCID.EaterofWorldsTail
        ];

        public override void OnKill(NPC npc)
        {
            Player player = Main.player[npc.lastInteraction];
            FargoSoulsPlayer modPlayer = player.FargoSouls();

            if (player.HasEffect<NecroEffect>() && !npc.boss)
            {
                NecroEffect.NecroSpawnGraveEnemy(npc, player, modPlayer);
            }

            if (!lootMultiplierCheck)
            {
                lootMultiplierCheck = true;

                if (player.HasEffect<SinisterIconDropsEffect>() && npc.type != ModContent.NPCType<MutantBoss>())// && !npc.boss && !IllegalLootMultiplierNPCs.Contains(npc.type))
                {
                    if (SinisterIconBoss(npc))
                    {
                        if (npc.FargoSouls().SinisterIconFullFight)
                            npc.NPCLoot();
                    }
                    else
                        npc.NPCLoot();
                }

                if (player.FargoSouls().PlatinumEffect != null && !npc.boss)
                {
                    bool isForcePlatinum = player.FargoSouls().ForceEffect(player.FargoSouls().PlatinumEffect.type);

                    if (Main.rand.NextBool(isForcePlatinum ? 3 : 5) && !IllegalLootMultiplierNPCs.Contains(npc.type))
                    {
                        int repeats = 5;

                        npc.extraValue /= repeats;

                        for (int i = 0; i < repeats - 1; i++)
                            npc.NPCLoot();
                    }
                }
            }

            if (npc.boss && !WorldSavingSystem.DownedAnyBoss)
            {
                WorldSavingSystem.DownedAnyBoss = true;
                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.WorldData);
            }
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            static IItemDropRule BossDrop(int item)
            {
                return new DropBasedOnEMode(ItemDropRule.Common(item, 3), ItemDropRule.Common(item, 10));
            }

            switch (npc.type)
            {
                case NPCID.KingSlime:
                    npcLoot.Add(BossDrop(ModContent.ItemType<SlimeKingsSlasher>()));
                    break;

                case NPCID.EyeofCthulhu:
                    npcLoot.Add(BossDrop(ModContent.ItemType<LeashOfCthulhu>()));
                    break;

                case NPCID.EaterofWorldsHead:
                case NPCID.EaterofWorldsBody:
                case NPCID.EaterofWorldsTail:
                    {
                        LeadingConditionRule lastEater = new(new Conditions.LegacyHack_IsABoss());
                        lastEater.OnSuccess(BossDrop(ModContent.ItemType<EaterLauncherJr>()));
                        npcLoot.Add(lastEater);
                    }
                    break;

                case NPCID.BrainofCthulhu:
                    npcLoot.Add(BossDrop(ModContent.ItemType<BrainStaff>()));
                    break;

                case NPCID.DD2DarkMageT1:
                case NPCID.DD2DarkMageT3:
                    npcLoot.Add(BossDrop(ModContent.ItemType<DarkTome>()));
                    break;

                case NPCID.QueenBee:
                    npcLoot.Add(BossDrop(ModContent.ItemType<TheSmallSting>()));
                    break;

                case NPCID.SkeletronHead:
                    var drop = new DropBasedOnEMode(ItemDropRule.Common(ModContent.ItemType<BoneZone>(), 3), ItemDropRule.Common(ModContent.ItemType<BoneZone>(), 10));
                    drop.OnSuccess(ItemDropRule.Common(ModContent.ItemType<BrittleBone>(), 1, 200, 200));
                    npcLoot.Add(drop);
                    break;

                case NPCID.WallofFlesh:
                    npcLoot.Add(BossDrop(ModContent.ItemType<FleshHand>()));
                    break;

                case NPCID.TheDestroyer:
                    npcLoot.Add(BossDrop(ModContent.ItemType<ElectricWhip>()));
                    break;

                case NPCID.SkeletronPrime:
                    npcLoot.Add(BossDrop(ModContent.ItemType<RefractorBlaster>()));
                    break;

                case NPCID.Retinazer:
                case NPCID.Spazmatism:
                    {
                        LeadingConditionRule noTwin = new(new Conditions.MissingTwin());
                        noTwin.OnSuccess(BossDrop(ModContent.ItemType<TwinRangs>()));
                        npcLoot.Add(noTwin);
                    }
                    break;

                case NPCID.Plantera:
                    npcLoot.Add(BossDrop(ModContent.ItemType<Dicer>()));
                    break;

                case NPCID.Golem:
                    npcLoot.Add(BossDrop(ModContent.ItemType<RockSlide>()));
                    break;

                case NPCID.DukeFishron:
                    npcLoot.Add(BossDrop(ModContent.ItemType<FishStick>()));
                    break;

                case NPCID.HallowBoss:
                    npcLoot.Add(BossDrop(ModContent.ItemType<PrismaRegalia>()));
                    break;

                case NPCID.DD2Betsy:
                    npcLoot.Add(BossDrop(ModContent.ItemType<DragonBreath>()));
                    break;

                case NPCID.MoonLordCore:
                    npcLoot.Add(BossDrop(ModContent.ItemType<MoonBow>()));
                    break;

                case NPCID.BigMimicJungle:
                    npcLoot.Add(ItemDropRule.OneFromOptions(1,
                        ModContent.ItemType<Vineslinger>(),
                        ModContent.ItemType<Mahoguny>(),
                        ModContent.ItemType<OvergrownKey>()));
                    break;

                default:
                    break;
            }

            //if (Fargowiltas.Instance.CalamityLoaded && Revengeance && WorldSavingSystem.EternityMode && Main.bloodMoon && Main.moonPhase == 0 && Main.raining && Main.rand.NextBool(10))
            //{
            //    Mod calamity = ModLoader.GetMod("CalamityMod");

            //    if (npc.type == calamity.NPCType("DevourerofGodsHeadS"))
            //    {
            //        Item.NewItem(npc.Hitbox, calamity.ItemType("CosmicPlushie"));
            //    }
            //}
        }

        public override bool CheckDead(NPC npc)
        {
            if (TimeFrozen)
            {
                npc.life = 1;
                return false;
            }

            Player player = FargoSoulsUtil.PlayerExists(npc.lastInteraction);
            if (player == null)
                return base.CheckDead(npc);

            FargoSoulsPlayer modPlayer = player.FargoSouls();

            //            /*if (npc.boss && FargoSoulsUtil.BossIsAlive(ref mutantBoss, ModContent.NPCType<MutantBoss.MutantBoss>()) && npc.type != ModContent.NPCType<MutantBoss.MutantBoss>())
            //            {
            //                npc.active = false;
            //                SoundEngine.PlaySound(npc.DeathSound, npc.Center);
            //                return false;
            //            }*/

            if (player.HasEffect<WoodCompletionEffect>())
            {
                WoodCompletionEffect.WoodCheckDead(modPlayer, npc);
            }

            if (player.HasEffect<CactusEffect>() && npc.lifeMax > 10 && !npc.townNPC && npc.lifeMax != int.MaxValue) //super dummy
            {
                CactusEffect.CactusProc(npc, player);
            }

            return base.CheckDead(npc);
        }
        public override void OnHitByItem(NPC npc, Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            OnHitByEither(npc, player, damageDone);
        }

        public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            OnHitByEither(npc, Main.player[projectile.owner], damageDone);
            
        }

        // TODO: damageDone or hitInfo.Damage ?
        public void OnHitByEither(NPC npc, Player player, int damageDone)
        {
            if (Anticoagulation && player.whoAmI == Main.myPlayer)
            {
                int type = ModContent.ProjectileType<Bloodshed>();
                if (Main.rand.NextBool(player.ownedProjectileCounts[type] + 2))
                {
                    const float speed = 12f;
                    Projectile.NewProjectile(npc.GetSource_OnHurt(player), npc.Center, Main.rand.NextVector2Circular(speed, speed), type, 0, 0f, Main.myPlayer, 1f);
                }
            }
            

            if (damageDone > 0 && player.HasEffect<NecroEffect>() && npc.boss)
            {
                NecroEffect.NecroSpawnGraveBoss(this, npc, player, damageDone);
            }
        }

        public override bool CanHitPlayer(NPC npc, Player target, ref int CooldownSlot)
        {
            if (TimeFrozen)
                return false;
            return true;
        }

        public override void ModifyHitNPC(NPC npc, NPC target, ref NPC.HitModifiers modifiers)
        {
            Player player = Main.player[Main.myPlayer];
            FargoSoulsPlayer modPlayer = player.FargoSouls();

            if (target.type == ModContent.NPCType<CreeperGutted>())
                modifiers.FinalDamage /= 20;
        }

        public override bool? CanBeHitByItem(NPC npc, Player player, Item item)
        {
            if (TimeFrozen && npc.life == 1)
                return false;
            return null;
        }

        public override bool? CanBeHitByProjectile(NPC npc, Projectile projectile)
        {
            if (TimeFrozen && npc.life == 1)
                return false;
            return null;
        }

        public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
        {
            Player player = Main.player[Main.myPlayer];
            FargoSoulsPlayer modPlayer = player.FargoSouls();

            if (player.HasEffect<EbonwoodEffect>())
            {
                float divisor = 50;
                if (player.HasEffect<TimberEffect>())
                    divisor = 35;
                modifiers.FlatBonusDamage += (int) (modPlayer.EbonwoodCharge / divisor);
            }

            if (OceanicMaul)
                modifiers.ArmorPenetration += 20;
            if (CurseoftheMoon)
                modifiers.ArmorPenetration += 10;
            if (Rotting)
                modifiers.ArmorPenetration += 10;
            if (DeathMarked)
                modifiers.FinalDamage *= 1.15f;
            if (Smite)
            {
                modifiers.FinalDamage *= 1.2f;
            }

            if (MoltenAmplify)
            {
                float modifier = 1.2f;
                if (!player.HasEffectEnchant<MoltenEffect>())
                    modifier = 1.15f;
                else if (player.ForceEffect<MoltenEffect>())
                    modifier = 1.3f;
                modifiers.FinalDamage *= modifier;
            }

            if (PungentGazeTime > 0)
            {
                modifiers.FinalDamage *= 1.0f + 0.15f * PungentGazeTime / PungentGazeBuff.MAX_TIME;
            }

            //            //if (modPlayer.KnightEnchant && Villain && !npc.boss)
            //            //{
            //            //    damage *= 1.5;
            //            //}

            //            if (crit && modPlayer.ShroomEnchant && !modPlayer.TerrariaSoul && player.stealth == 0)
            //            {
            //                damage *= 1.5;
            //            }

            if (modPlayer.DeviGraze)
            {
                modifiers.FinalDamage *= 1.0f + (float)modPlayer.DeviGrazeBonus;
            }

            //            //normal damage calc
        }

        public override void ModifyShop(NPCShop shop)
        {
            if (shop.NpcType == ModContent.NPCType<Deviantt>())
            {
                shop.Add(new Item(ModContent.ItemType<EternityAdvisor>()) { shopCustomPrice = Item.buyPrice(copper: 10000) });
            }
        }
        public override void ModifyActiveShop(NPC npc, string shopName, Item[] items)
        {
            Player player = Main.player[Main.myPlayer];
            FargoSoulsPlayer modPlayer = player.FargoSouls();

            if (modPlayer.WoodEnchantDiscount)
            {
                WoodEnchant.WoodDiscount(items);
            }
        }
        public override void SetupTravelShop(int[] shop, ref int nextSlot)
        {
            if (Main.hardMode && Main.moonPhase == 0)
            {
                shop[nextSlot] = ModContent.ItemType<MechLure>();
                nextSlot++;
            }
        }

    }
}
