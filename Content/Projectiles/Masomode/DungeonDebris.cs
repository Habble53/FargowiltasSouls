using FargowiltasSouls.Content.Buffs.Masomode;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using static FargowiltasSouls.Content.Projectiles.EffectVisual;

namespace FargowiltasSouls.Content.Projectiles.Masomode
{
    public class DungeonDebris : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.BlueDungeonDebris;
        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 1000;
            Projectile.hostile = true;
        }
        public override void OnSpawn(IEntitySource source)
        {
            if (GenVars.crackedType == TileID.CrackedGreenDungeonBrick) Projectile.ai[0] = ProjectileID.GreenDungeonDebris;
            else if (GenVars.crackedType == TileID.CrackedPinkDungeonBrick) Projectile.ai[0] = ProjectileID.PinkDungeonDebris;
            else Projectile.ai[0] = ProjectileID.BlueDungeonDebris;
            Projectile.localAI[2] = Main.rand.Next(0, 3);
            base.OnSpawn(source);
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item127, Projectile.Center);
            base.OnKill(timeLeft);
        }
        public override void AI()
        {
            

            if (Main.rand.NextBool(5))
            {
                Dust d = Dust.NewDustDirect(Projectile.Center, 1, 1, Projectile.ai[0] == ProjectileID.GreenDungeonDebris ? DustID.DungeonGreen : Projectile.ai[0] == ProjectileID.PinkDungeonDebris ? DustID.DungeonPink : DustID.DungeonBlue);
                d.velocity *= 0.2f;
            }
            if (Projectile.velocity.Y < 10) Projectile.velocity.Y += 0.2f;
            Projectile.rotation += MathHelper.ToRadians(5);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.instance.LoadProjectile((int)Projectile.ai[0]);
            Asset<Texture2D> t = TextureAssets.Projectile[(int)Projectile.ai[0]];
            Rectangle source = new Rectangle(0, t.Height() / 3 * (int)(Projectile.localAI[2]), t.Width(), t.Height()/3);

            //for (int j = 0; j < 12; j++)
            //{
            //    Vector2 afterimageOffset = (MathHelper.TwoPi * j / 12f).ToRotationVector2() * 4;
            //    Color glowColor = new Color(255, 255, 100) with { A = 0 } * 0.7f;


                //Main.EntitySpriteDraw(t.Value, Projectile.Center + afterimageOffset - Main.screenPosition, source, glowColor, Projectile.rotation, new Vector2(t.Width(), t.Height() / 3) / 2, Projectile.scale, SpriteEffects.None);
            //}
            lightColor *= 2;

            Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition, source, lightColor, Projectile.rotation, new Vector2(t.Width(), t.Height() / 3) / 2, Projectile.scale, SpriteEffects.None);
            return false;
        }
    }
}