using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Eternity.Bosses.Betsy
{
    public class BetsyCurveFireBreath : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_687";

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 7;
        }

        public override void SetDefaults()
        {
            Projectile.timeLeft = 360;
            Projectile.tileCollide = false;
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.hostile = true;
            Projectile.aiStyle = -1;
            Projectile.extraUpdates = 2;
        }

        public override void AI()
        {
            Main.projFrames[Type] = 7;
            Lighting.AddLight(Projectile.Center, TorchID.Purple);
            if (Projectile.ai[0] == -1)
            {
                Projectile.velocity.Y += 0.09f;
            }
            else if (Projectile.ai[0] == 1)
            {
                Projectile.velocity.Y -= 0.09f;
            }

            if (Projectile.frameCounter++ > 16)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame > Main.projFrames[Type])
                    Projectile.frame = 0;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            FargoSoulsUtil.GenericProjectileDraw(Projectile, lightColor);
            return false;
        }
    }
}
