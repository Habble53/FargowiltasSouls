using FargowiltasSouls.Content.Buffs.Masomode;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Masomode
{
    public class BloodDroplet : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.tileCollide = true;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 600;
            Projectile.scale = Main.rand.NextFloat(0.5f, 0.9f);
        }

        public override void AI()
        {
            Projectile.velocity.Y += 0.2f;
            Projectile.velocity.X += Main.windSpeedCurrent/100f;
            if (Projectile.ai[1] == 1 && Main.projectile[(int)Projectile.ai[0]] != null && Main.projectile[(int)Projectile.ai[0]].active)
            {
                Projectile.tileCollide = false;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.AngleTo(Main.projectile[(int)Projectile.ai[0]].Center).ToRotationVector2() * 18, 0.08f);
                if (Projectile.Distance(Main.projectile[(int)Projectile.ai[0]].Center) < 5)
                {
                    DeathState();
                }
            }
            else
            {
                Projectile.tileCollide = true;
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Collision.SolidCollision(Projectile.Bottom, 1, 6))
            {
                DeathState();
            }
            return false;
            return base.OnTileCollide(oldVelocity);
        }
        public void DeathState()
        {
            Projectile.velocity = Vector2.Zero;
            if (Projectile.timeLeft > 30)
            {
                Projectile.timeLeft = 30;
                if (Projectile.ai[1] == 0)
                {
                    Projectile.NewProjectileDirect(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<BloodPuddle>(), 0, 1);
                    SoundEngine.PlaySound(SoundID.NPCDeath9, Projectile.Center);
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> t = TextureAssets.Projectile[Type];
            if (Projectile.timeLeft > 30)
            Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, t.Size() / 2, Projectile.scale, SpriteEffects.None);
            float length = ProjectileID.Sets.TrailCacheLength[Type];
            for (int i = 0; i < length; i++)
            {
                if (30 - i < Projectile.timeLeft)
                {
                    float opacity = 1 - (i / length);
                    if (opacity > 1) opacity = 1;
                    Color color = lightColor;
                    color = color.MultiplyRGB(new Color(opacity, opacity, opacity));
                    Vector2 oldPosMinus1 = Projectile.position;
                    if (i > 0) oldPosMinus1 = Projectile.oldPos[i - 1];
                    Main.EntitySpriteDraw(t.Value, Projectile.oldPos[i] - Main.screenPosition + Projectile.Size / 2, null, color * opacity, Projectile.oldRot[i], t.Size() / 2, Projectile.scale * (opacity), SpriteEffects.None);
                    Main.EntitySpriteDraw(t.Value, Vector2.Lerp(Projectile.oldPos[i], oldPosMinus1, 0.5f) - Main.screenPosition + Projectile.Size / 2, null, color * opacity, Projectile.oldRot[i], t.Size() / 2, Projectile.scale * (opacity), SpriteEffects.None);
                }
            }
            return false;   
        }
    }
}