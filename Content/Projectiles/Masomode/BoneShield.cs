using FargowiltasSouls.Assets.Sounds;
using FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Dungeon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static FargowiltasSouls.Content.Projectiles.EffectVisual;

namespace FargowiltasSouls.Content.Projectiles.Masomode
{
    public class BoneShield : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 36;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.FargoSouls().DeletionImmuneRank = 1;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            base.SetDefaults();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.hide = false;
            Asset<Texture2D> t = TextureAssets.Projectile[Type];
            lightColor = Lighting.GetColor(Projectile.Center.ToTileCoordinates());
            float lightLevel = (lightColor.R + lightColor.G + lightColor.B) / 3f / 200f;
            SpriteEffects effects = SpriteEffects.None;

            Color glowColorBase = AngryBones.weaponGlowColor((int)Projectile.localAI[0]);
            if (Projectile.ai[0] > -1 && Main.npc[(int)Projectile.ai[0]].spriteDirection == -1)
            {
                effects = SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically;
            }
            for (int j = 0; j < 12; j++)
            {
                Vector2 afterimageOffset = (MathHelper.TwoPi * j / 12f).ToRotationVector2() * 2;
                Color glowColor = glowColorBase with { A = 0 } * 0.7f * lightLevel;


                Main.EntitySpriteDraw(t.Value, Projectile.Center + afterimageOffset - Main.screenPosition, null, glowColor, Projectile.rotation, t.Size() / 2, Projectile.scale, effects);
            }
            Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, t.Size() / 2, 1, effects);
            if (Projectile.ai[0] >= 0)
            {
                Main.instance.DrawHealthBar(Projectile.Bottom.X, Projectile.Bottom.Y, (int)Projectile.ai[1], (int)Projectile.ai[2], 0.3f, 0.5f);
            }
            //Projectile.hide = true;
            return false;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCs.Add(index);
            base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
        }
        public override void OnSpawn(IEntitySource source)
        {
            if (Projectile.ai[0] > -1)
            {
                NPC owner = Main.npc[(int)Projectile.ai[0]];
                if (owner != null && owner.active && AngryBones.AllBones.Contains(owner.type))
                {
                    Projectile.ai[1] = owner.lifeMax * 0.8f;
                    Projectile.ai[2] = Projectile.ai[1];
                }
            }
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
            for (int i = 0; i < 20; i++)
            {
                Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Bone);
                d.velocity *= 0.5f;
            }
            if (AngryBones.HellBone.Contains((int)Projectile.localAI[0]))
            {
                Projectile.NewProjectileDirect(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<FusedExplosion>(), Projectile.damage, 10);
            }
            if (AngryBones.RustBone.Contains((int)Projectile.localAI[0]))
            {
                Projectile.NewProjectileDirect(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<BloodDroplet>(), Projectile.damage, 10);
            }
            base.OnKill(timeLeft);
        }
        public override bool CanHitPlayer(Player target)
        {
            if (Projectile.ai[0] >= 0)
            {
                return false;
            }
            return base.CanHitPlayer(target);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (!AngryBones.BlueBone.Contains((int)Projectile.localAI[0]))
            {
                if (Collision.SolidCollision(Projectile.TopLeft, Projectile.width, -6))
                {
                    Projectile.velocity.Y = 5;

                    Projectile.velocity.X = oldVelocity.X;
                    return false;
                }
                if (Projectile.oldVelocity.Y <= 0)
                {
                    return false;
                }
            }
            else
            {
                if (oldVelocity.Y != Projectile.velocity.Y)
                    Projectile.velocity.Y = -oldVelocity.Y;
                if (oldVelocity.X != Projectile.velocity.X)
                    Projectile.velocity.X = -oldVelocity.X;
                return false;
            }
            return base.OnTileCollide(oldVelocity);
        }
        public override void AI()
        {

            if ((int)Projectile.ai[0] >= 0 && (Main.npc[(int)Projectile.ai[0]] == null || !Main.npc[(int)Projectile.ai[0]].active || !AngryBones.AllBones.Contains(Main.npc[(int)Projectile.ai[0]].type)))
            {
                Projectile.ai[0] = -1;
            }
            if (Projectile.ai[0] == -1)
            {
                Projectile.height = (int)(12 * Projectile.scale);
                Projectile.tileCollide = true;
                Projectile.velocity = new Vector2(0, -8).RotatedByRandom(MathHelper.ToRadians(10));
                Projectile.ai[0] = -2;
            }
            else if (Projectile.ai[0] == -2)
            {

                Projectile.velocity += new Vector2(0, 0.2f);
                Projectile.rotation += MathHelper.ToRadians(Projectile.velocity.Length() * 2);
            }
            else
            {
                NPC owner = Main.npc[(int)Projectile.ai[0]];
                Projectile.localAI[0] = owner.type;
                owner.knockBackResist = 0;
                Projectile.tileCollide = false;
                Projectile.timeLeft = 200;
                if (owner.spriteDirection == 1)
                {
                    Projectile.Center = owner.Left + new Vector2(20, 0);
                }
                else
                {
                    Projectile.Center = owner.Right + new Vector2(-20, 0);
                }
                //Projectile.Center += new Vector2(15 * owner.spriteDirection, 0);
                Projectile.velocity = Vector2.Zero;
                float downAngle = owner.spriteDirection == 1 ? 20 : -20;
                float upAngle = owner.spriteDirection == 1 ? -15 : 15;
                if ((owner.frame.Y >= 5 * owner.frame.Height && owner.frame.Y <= 7 * owner.frame.Height) || (owner.frame.Y >= 12 * owner.frame.Height && owner.frame.Y <= 14 * owner.frame.Height))
                {
                    Projectile.rotation = Utils.AngleLerp(Projectile.rotation, MathHelper.ToRadians(downAngle), 0.08f);
                }
                else
                {
                    Projectile.rotation = Utils.AngleLerp(Projectile.rotation, MathHelper.ToRadians(upAngle), 0.08f);
                }
                if (Projectile.ai[1] <= 0)
                {
                    if (FargoSoulsUtil.HostCheck)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center + new Vector2(0, Main.rand.Next(-10, 16)), new Vector2(Main.rand.NextFloat(2, 5) * owner.spriteDirection, 0), ProjectileID.SkeletonBone, FargoSoulsUtil.ScaledProjectileDamage(owner.damage), 1);
                        }
                    }
                    SoundEngine.PlaySound(SoundID.NPCDeath43, Projectile.Center);
                    owner.knockBackResist = 0.3f;
                    owner.GetGlobalNPC<AngryBones>().HeldProjectile = -1;
                    Projectile.Kill();
                    
                }
                base.AI();
            }
        }
    }
}
