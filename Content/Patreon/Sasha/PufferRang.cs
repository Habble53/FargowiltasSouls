using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Patreon.Sasha
{
    public class PufferRang : ModProjectile
    {
        public int timer;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("PufferRang");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
          //Projectile.CloneDefaults(ProjectileID.EnchantedBoomerang);
          //AIType = ProjectileID.EnchantedBoomerang;
            Projectile.friendly = true;
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.scale = 2f;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
          //Projectile.extraUpdates = 1;

            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 7;
            Projectile.FargoSouls().noInteractionWithNPCImmunityFrames = true;
        }

        public override void AI()
        {
            // here's my poor reimplementation of vanilla boomerang AI -Habble
		    Projectile.rotation += 1f;
            if (Projectile.ai[0] == 0f)
            {
                Projectile.ai[1] += 1f;
                if (Projectile.ai[1] >= 30f)
                {
                    Projectile.ai[0] = 1f;
                    Projectile.ai[1] = 0f;
                    Projectile.netUpdate = true;
                }
            }
            else
            {
                Player owner = Main.player[Projectile.owner];
                float returningSpeed = 2f * 15f;
                float projAccel = 3f;
                ref Vector2 vel = ref Projectile.velocity;
                float xDistance = owner.Center.X - Projectile.Center.X;
                float yDistance = owner.Center.Y - Projectile.Center.Y;
                float distance = (float)Math.Sqrt((double)(xDistance * xDistance + yDistance * yDistance));
                if (distance > 3000f)
                {
                    Projectile.Kill();
                }
                distance = returningSpeed / distance;
                xDistance *= distance;
                yDistance *= distance;
                if (vel.X < xDistance)
                {
                    vel.X += projAccel;
                    if (vel.X < 0f && xDistance > 0f)
                        vel.X += projAccel;
                }
                else if (vel.X > xDistance)
                {
                    vel.X -= projAccel;
                    if (vel.X > 0f && xDistance < 0f)
                        vel.X -= projAccel;
                }
                if (vel.Y < yDistance)
                {
                    vel.Y += projAccel;
                    if (vel.Y < 0f && yDistance > 0f)
                        vel.Y += projAccel;
                }
                else if (vel.Y > yDistance)
                {
                    vel.Y -= projAccel;
                    if (vel.Y > 0f && yDistance < 0f)
                        vel.Y -= projAccel;
                }
                if (Projectile.owner == Main.myPlayer)
                    if (Projectile.Hitbox.Intersects(owner.Hitbox))
                        Projectile.Kill();
            }
            //dust!
            int dustId = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y + 2f), Projectile.width, Projectile.height + 5, DustID.Ice, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 100, default, .5f);
            Main.dust[dustId].noGravity = true;

            Projectile.position += Projectile.velocity / 240f * timer++; //gradually speed up
        }

      /*public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            //smaller tile hitbox
            width = 20;
            height = 20;
            return true;
        }*/

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
          //target.immune[Projectile.owner] = 7;
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);

            if (Projectile.owner == Main.myPlayer)
            {
                for (int i = 0; i < 3; i++)
                {
                    int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Main.rand.NextVector2Circular(12f, 12f), ModContent.ProjectileType<PufferSpray>(), (int)(Projectile.damage * 0.15), Projectile.knockBack, Projectile.owner);
                }
            }
            /*
            for (int i = 0; i < 4; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width,
                    Projectile.height, DustID.Smoke, 0f, 0f, 100, default, 3f);
                Main.dust[dust].velocity *= 1.4f;
            }
            */
            for (int i = 0; i < 3; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width,
                    Projectile.height, DustID.Torch, 0f, 0f, 100, default, 3.5f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 7f;
                dust = Dust.NewDust(Projectile.position, Projectile.width,
                    Projectile.height, DustID.Torch, 0f, 0f, 100, default, 1.5f);
                Main.dust[dust].velocity *= 3f;
            }

            float scaleFactor9 = 0.5f;
            for (int j = 0; j < 1; j++)
            {
                int gore = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center,
                    default,
                    Main.rand.Next(61, 64));

                Main.gore[gore].velocity *= scaleFactor9;
                Main.gore[gore].velocity.X += 1f;
                Main.gore[gore].velocity.Y += 1f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = Projectile.GetAlpha(color26);

            SpriteEffects effects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                Color color27 = Color.White * Projectile.Opacity * 0.75f;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 value4 = Projectile.oldPos[i];
                float num165 = Projectile.oldRot[i];
                Main.EntitySpriteDraw(texture2D13, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, Projectile.scale, effects, 0);
            }

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, effects, 0);
            return false;
        }
    }
}
