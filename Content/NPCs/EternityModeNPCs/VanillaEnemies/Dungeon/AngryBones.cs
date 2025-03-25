﻿using FargowiltasSouls.Content.Projectiles.Masomode;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Dungeon
{
    public class AngryBones : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            AllBones
        );

        //public int BoneSprayTimer;
        public static int[] AngryBone = [NPCID.AngryBones,
            NPCID.AngryBonesBig,
            NPCID.AngryBonesBigHelmet,
            NPCID.AngryBonesBigMuscle];
        public static int[] BlueBone = [NPCID.BlueArmoredBones,
        NPCID.BlueArmoredBonesMace,
        NPCID.BlueArmoredBonesNoPants,
        NPCID.BlueArmoredBonesSword];
        public static int[] HellBone = [NPCID.HellArmoredBones,
        NPCID.HellArmoredBonesMace,
        NPCID.HellArmoredBonesSpikeShield,
        NPCID.HellArmoredBonesSword];
        public static int[] RustBone = [NPCID.RustyArmoredBonesAxe,
        NPCID.RustyArmoredBonesFlail,
        NPCID.RustyArmoredBonesSword,
        NPCID.RustyArmoredBonesSwordNoArmor];
        public static int[] AllBones = [.. AngryBone, .. BlueBone, .. HellBone, .. RustBone];
        public static Color weaponGlowColor(int npcType)
        {
            if (BlueBone.Contains(npcType))
            {
                return Color.Blue;
            }
            if (HellBone.Contains(npcType))
            {
                return Color.Orange;
            }
            if (RustBone.Contains(npcType))
            {
                return Color.Red;
            }
            return Color.White;
        }
        public int BabyTimer;
        public override void OnSpawn(NPC npc, IEntitySource source)
        {

            if (FargoSoulsUtil.HostCheck && HeldProjectile == -1)
            {
                int weapon = Main.rand.NextFromList([ModContent.ProjectileType<BoneSpear>(), ModContent.ProjectileType<BoneFlail>(), ModContent.ProjectileType<BoneShield>()]);
                //weapon = ModContent.ProjectileType<BoneShield>();
                
                HeldProjectile = Projectile.NewProjectileDirect(source, npc.Center, Vector2.Zero, weapon, FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 1, -1, npc.whoAmI).whoAmI;
                
            }
            base.OnSpawn(npc, source);
        }
        public int HeldProjectile = -1;
        public override void SafeOnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            //hit.SourceDamage = 0;
            if (HeldProjectile >= 0)
            {
                Projectile proj = Main.projectile[HeldProjectile];
                if (proj != null && proj.active && proj.type == ModContent.ProjectileType<BoneShield>() && proj.ai[0] == npc.whoAmI && hit.HitDirection == -npc.direction && projectile.FargoSouls().DeletionImmuneRank == 0)
                {
                    projectile.Kill();
                }
            }
            base.SafeOnHitByProjectile(npc, projectile, hit, damageDone);
        }
        public override void ModifyHitByAnything(NPC npc, Player player, ref NPC.HitModifiers modifiers)
        {
            if (HeldProjectile >= 0)
            {
                Projectile proj = Main.projectile[HeldProjectile];
                if (proj != null && proj.active && proj.type == ModContent.ProjectileType<BoneShield>() && proj.ai[0] == npc.whoAmI && modifiers.HitDirection != npc.spriteDirection)
                {
                    SoundEngine.PlaySound(SoundID.NPCHit4, npc.Center);
                    proj.ai[1] -= player.GetWeaponDamage(player.HeldItem);
                    modifiers.ModifyHitInfo += (ref NPC.HitInfo hitInfo) => hitInfo.Null();
                }
            }
        }
        
        public override void AI(NPC npc)
        {
            //Main.NewText(Main.hardMode);
            base.AI(npc);
            if (HeldProjectile >= 0)
            {
                Projectile held = Main.projectile[HeldProjectile];
                if (held != null && held.active && held.ai[0] == npc.whoAmI)
                {
                    if (held.type == ModContent.ProjectileType<BoneShield>())
                    {
                        //makes them walk through walls dont do this lol
                        //npc.position.X += npc.velocity.X * 0.2f;
                    }
                    if (held.type == ModContent.ProjectileType<BoneFlail>())
                    {
                        npc.position.X -= npc.velocity.X * 0.2f;
                    }
                }
            }
            //if (--BoneSprayTimer > 0 && BoneSprayTimer % 6 == 0) //spray bones
            //{
            //    Vector2 speed = new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));
            //    speed.Normalize();
            //    speed *= 5f;
            //    speed.Y -= Math.Abs(speed.X) * 0.2f;
            //    speed.Y -= 3f;
            //    if (FargoSoulsUtil.HostCheck)
            //        Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, speed, ProjectileID.SkeletonBone, FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0f, Main.myPlayer);
            //}

            //if (npc.justHit)
            //{
            //    //BoneSprayTimer = 120;
            //    BabyTimer += 20;
            //}

            //if (++BabyTimer > 300) //shoot baby guardians
            //{
            //    BabyTimer = 0;
            //    if (FargoSoulsUtil.HostCheck && npc.HasValidTarget && Collision.CanHitLine(npc.Center, 0, 0, Main.player[npc.target].Center, 0, 0))
            //        Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, npc.SafeDirectionTo(Main.player[npc.target].Center), ModContent.ProjectileType<SkeletronGuardian2>(), FargoSoulsUtil.ScaledProjectileDamage(npc.defDamage), 0f, Main.myPlayer);
            //}
        }

        public override void OnKill(NPC npc)
        {
            base.OnKill(npc);

            //if (FargoSoulsUtil.HostCheck)
            //{
            //    if (Main.rand.NextBool(5))
            //        FargoSoulsUtil.NewNPCEasy(npc.GetSource_FromAI(), npc.Center, NPCID.CursedSkull);

            //    for (int i = 0; i < 15; i++)
            //    {
            //        Vector2 speed = new(Main.rand.Next(-50, 51), Main.rand.Next(-100, 1));
            //        speed.Normalize();
            //        speed *= Main.rand.NextFloat(3f, 6f);
            //        speed.Y -= Math.Abs(speed.X) * 0.2f;
            //        speed.Y -= 3f;
            //        speed.Y *= Main.rand.NextFloat(1.5f);
            //        if (FargoSoulsUtil.HostCheck)
            //            Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, speed, ProjectileID.SkeletonBone, FargoSoulsUtil.ScaledProjectileDamage(npc.defDamage), 0f, Main.myPlayer);
            //    }
            //}
        }
    }
}
