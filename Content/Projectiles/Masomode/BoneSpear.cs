using FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Dungeon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Masomode
{
    public class BoneSpear : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 6;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.FargoSouls().DeletionImmuneRank = 1;
            Projectile.aiStyle = -1;
            Projectile.hide = true;
            Projectile.penetrate = -1;
            base.SetDefaults();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if ((int)Projectile.ai[0] >= 0 && (Main.npc[(int)Projectile.ai[0]] == null || !Main.npc[(int)Projectile.ai[0]].active || !AngryBones.AllBones.Contains(Main.npc[(int)Projectile.ai[0]].type)))
            {

                Projectile.ai[0] = -1;
            }
            Asset<Texture2D> t = TextureAssets.Projectile[Type];
            Asset<Texture2D> head = ModContent.Request<Texture2D>("FargowiltasSouls/Content/Projectiles/Masomode/BoneSpearHead");
            Vector2 origin = t.Size() - new Vector2(16, 16);
            lightColor = Lighting.GetColor(Projectile.Center.ToTileCoordinates());
            float lightLevel = (lightColor.R + lightColor.G + lightColor.B) / 3f / 200f;
            if (Projectile.ai[0] < 0)
            {
                origin = t.Size() / 2;
            }

            SpriteEffects effects = SpriteEffects.None;
            Color glowColorBase = AngryBones.weaponGlowColor((int)Projectile.localAI[0]);
            if (Projectile.ai[0] > -1 && Main.npc[(int)Projectile.ai[0]].spriteDirection == -1)
            {
                
                effects = SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically;
                origin = t.Size() - new Vector2(36, 36);
            }

            for (int j = 0; j < 12; j++)
            {
                Vector2 afterimageOffset = (MathHelper.TwoPi * j / 12f).ToRotationVector2() * 2 ;
                Color glowColor = glowColorBase with { A = 0 } * 0.7f * lightLevel;


                Main.EntitySpriteDraw(head.Value, Projectile.Center + afterimageOffset - Main.screenPosition, null, glowColor, Projectile.rotation, origin, Projectile.scale, effects);
            }
            Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, origin, 1, effects);
            return false;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCs.Add(index);
            base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
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
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 vec = Projectile.Center - new Vector2(29, 29).RotatedBy(Projectile.rotation) * Projectile.scale;
            if (Projectile.ai[0] < 0)
            {
                vec = Projectile.Center - new Vector2(19, 19).RotatedBy(Projectile.rotation) * Projectile.scale;
            }
            if (Projectile.ai[0] > -1 && Main.npc[(int)Projectile.ai[0]].spriteDirection == -1)
            {
                vec = Projectile.Center + new Vector2(29, 29).RotatedBy(Projectile.rotation) * Projectile.scale;
            }
            //Dust d = Dust.NewDustDirect(vec, 1, 1, DustID.Terra);
            //d.velocity = Vector2.Zero;
            Point center = vec.ToPoint();
            
            
            Rectangle hitbox = new Rectangle((int)vec.X - 3, (int)vec.Y - 3, 6, 6);
            return targetHitbox.Intersects(hitbox);
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
                Projectile.tileCollide = true;
                Projectile.velocity = new Vector2(0, -8).RotatedByRandom(MathHelper.ToRadians(10));
                Projectile.ai[0] = -2;
            }else if (Projectile.ai[0] == -2)
            {
                
                Projectile.velocity += new Vector2(0, 0.2f);
                Projectile.rotation += MathHelper.ToRadians(Projectile.velocity.Length() * 2);
            }
            else
            {
                NPC owner = Main.npc[(int)Projectile.ai[0]];
                Projectile.localAI[0] = owner.type;
                Projectile.timeLeft = 200;

                if (owner.spriteDirection == 1)
                {
                    Projectile.Center = owner.Left + new Vector2(5, 0);
                }
                else
                {
                    Projectile.Center = owner.Right + new Vector2(-5, 0);
                }
                    Projectile.velocity = Vector2.Zero;

                //if (owner.spriteDirection == 1)
                //{
                //  Projectile.rotation = MathHelper.ToRadians(135);
                //}
                //else
                //{
                //    Projectile.rotation = MathHelper.ToRadians(315);
                //}
                float downAngle = owner.spriteDirection == 1 ? 155 : 115;
                float upAngle = owner.spriteDirection == 1 ? 125 : 145;
                //Main.NewText(owner.frame.Y);
                if ((owner.frame.Y >= 5 * owner.frame.Height && owner.frame.Y <= 7 * owner.frame.Height) || (owner.frame.Y >= 12 * owner.frame.Height && owner.frame.Y <= 14 * owner.frame.Height))
                {
                    Projectile.rotation = Utils.AngleLerp(Projectile.rotation, MathHelper.ToRadians(downAngle), 0.08f);
                }
                else
                {
                    Projectile.rotation = Utils.AngleLerp(Projectile.rotation, MathHelper.ToRadians(upAngle), 0.08f);
                }
                
            }

            
            base.AI();
        }
    }
}
