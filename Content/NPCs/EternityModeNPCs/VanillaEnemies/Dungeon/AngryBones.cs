using FargowiltasSouls.Content.Projectiles.Masomode;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using System.Collections.Generic;
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
        public override void OnFirstTick(NPC npc)
        {
            // Main.NewText(HeldProjectile);
            List<int> weapons = [ModContent.ProjectileType<BoneSpear>(), ModContent.ProjectileType<BoneFlail>(), ModContent.ProjectileType<BoneShield>()];
            if (FargoSoulsUtil.HostCheck && HeldProjectile == -1)
            {
                int weapon = Main.rand.NextFromList(weapons.ToArray());
                HeldProjectile = Projectile.NewProjectileDirect(npc.GetSource_FromAI(), npc.Center, Vector2.Zero, weapon, FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 1, -1, npc.whoAmI).whoAmI;
                weapons.Remove(weapon);
                if (WorldSavingSystem.MasochistModeReal)
                {
                    
                    int weapon2 = Main.rand.NextFromList(weapons.ToArray());
                    MasoHeldProjectile = Projectile.NewProjectileDirect(npc.GetSource_FromAI(), npc.Center, Vector2.Zero, weapon2, FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 1, -1, npc.whoAmI).whoAmI;
                }

            }
        }

        public int HeldProjectile = -1;
        public int MasoHeldProjectile = -1;
        public override void SafeOnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            //hit.SourceDamage = 0;
            if (HeldProjectile >= 0 || MasoHeldProjectile >= 0)
            {

                Projectile proj = null;
                if (MasoHeldProjectile >= 0 && Main.projectile[MasoHeldProjectile].type == ModContent.ProjectileType<BoneShield>())
                {
                    proj = Main.projectile[MasoHeldProjectile];
                }
                else if (HeldProjectile >= 0)
                {
                    proj = Main.projectile[HeldProjectile];
                }
                if (proj != null && proj.active && proj.type == ModContent.ProjectileType<BoneShield>() && proj.ai[0] == npc.whoAmI && hit.HitDirection == -npc.direction && projectile.FargoSouls().DeletionImmuneRank == 0)
                {
                    projectile.Kill();
                }
            }
            base.SafeOnHitByProjectile(npc, projectile, hit, damageDone);
        }
        public override void ModifyHitByAnything(NPC npc, Player player, ref NPC.HitModifiers modifiers)
        {
            if (HeldProjectile >= 0 || MasoHeldProjectile >= 0)
            {

                Projectile proj = null;
                if (MasoHeldProjectile >= 0 && Main.projectile[MasoHeldProjectile].type == ModContent.ProjectileType<BoneShield>())
                {
                    proj = Main.projectile[MasoHeldProjectile];
                }
                else if (HeldProjectile >= 0)
                {
                    proj = Main.projectile[HeldProjectile];
                }
                if (proj != null && proj.active && proj.type == ModContent.ProjectileType<BoneShield>() && proj.ai[0] == npc.whoAmI && modifiers.HitDirection != npc.spriteDirection)
                {
                    SoundEngine.PlaySound(SoundID.NPCHit4, npc.Center);
                    proj.ai[1] -= player.GetWeaponDamage(player.HeldItem);
                    modifiers.ModifyHitInfo += (ref NPC.HitInfo hitInfo) => hitInfo.Null();
                }
            }
        }
    }
}
