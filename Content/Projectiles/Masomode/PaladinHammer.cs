using FargowiltasSouls.Assets.ExtraTextures;
using FargowiltasSouls.Content.Buffs.Masomode;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Masomode
{
    public class PaladinHammer : ModProjectile, IPixelatedPrimitiveRenderer
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.tileCollide = true;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 600;
            Projectile.scale = 1;
            Projectile.hostile = true;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Pi;
            if (Projectile.timeLeft < 30 && Projectile.Opacity == 0)
            {
                if (Projectile.timeLeft > 15)
                FargoSoulsUtil.ScreenshakeRumble(1);
                Vector2 pos = Projectile.Center + new Vector2(Main.rand.NextFloat(-100, 100), 0);

                for (int j = 0; j < 100; j++)
                {
                    if (!Collision.SolidCollision(pos, 1, 1) && Collision.SolidCollision(pos - new Vector2(0, 8), 1, 1))
                    {
                        j = 100;
                    }
                    else
                    {
                        pos = pos - new Vector2(0, 8);
                    }
                }
                Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), pos, Vector2.Zero, ModContent.ProjectileType<DungeonDebris>(), Projectile.damage, 1);
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.Opacity = 0;
            
            if (Projectile.timeLeft > 30)
            {
                Projectile.timeLeft = 30;
                SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
                for (int i = 0; i < 20; i++)
                {
                    Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, new Vector2(Main.rand.NextFloat(2, 5), 0).RotatedByRandom(MathHelper.TwoPi), Main.rand.Next(61, 64));
                }
                
            }
            Projectile.velocity = Vector2.Zero;
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> t = TextureAssets.Projectile[Type];
            Asset<Texture2D> glow = ModContent.Request<Texture2D>("FargowiltasSouls/Content/Projectiles/GlowRingHollow");
            if (Projectile.timeLeft < 30)
            {
                float x = 1 - Projectile.timeLeft / 30f;
                float lerp = 1 - MathF.Pow(1 - x, 3);
                float size = MathHelper.Lerp(0f, 0.3f, lerp);

                float lerp2 = x * x * x;
                float opacity = MathHelper.Lerp(1, 0, lerp2);
                Main.EntitySpriteDraw(glow.Value, Projectile.Center - Main.screenPosition, null, Color.Yellow * opacity, 0, glow.Size() / 2, size * Projectile.scale, SpriteEffects.None);
            }
            Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition, null, lightColor * Projectile.Opacity, Projectile.rotation, t.Size() / 2, Projectile.scale, SpriteEffects.None);
            return false;   
        }



        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.width * 0.8f;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.White, Color.Yellow, completionRatio);
        }
        public void RenderPixelatedPrimitives(SpriteBatch spriteBatch)
        {
            ManagedShader shader = ShaderManager.GetShader("FargowiltasSouls.BlobTrail");
            FargoSoulsUtil.SetTexture1(FargosTextureRegistry.Techno1Noise.Value);
            PrimitiveRenderer.RenderTrail(Projectile.oldPos, new(WidthFunction, ColorFunction, _ => Projectile.Size * 0.5f, Pixelate: true, Shader: shader), 25);

        }
    }
}