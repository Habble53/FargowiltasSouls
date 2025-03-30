using FargowiltasSouls.Content.Buffs.Souls;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.Projectiles.Masomode;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Souls
{
    public class TungstenShockwave : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/Content/Projectiles/GlowRingHollow";
        public const int Duration = 20;
        public const int BaseRadius = 52;
        public override void SetDefaults()
        {
            Projectile.width = BaseRadius;
            Projectile.height = BaseRadius;
            Projectile.aiStyle = 0;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.penetrate = -1;
            Projectile.timeLeft = Duration;
            Projectile.tileCollide = false;
            Projectile.light = 0.75f;
            Projectile.ignoreWater = true;
            //Projectile.extraUpdates = 1;
            AIType = ProjectileID.Bullet;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.FargoSouls().DeletionImmuneRank = 2;
            Projectile.FargoSouls().CanSplit = false;

            Projectile.scale = 1f;

        }
        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(SoundID.DD2_GhastlyGlaiveImpactGhost with { Pitch = 0.3f }, Projectile.Center);
            //Projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
            => Projectile.Distance(FargoSoulsUtil.ClosestPointInHitbox(targetHitbox, Projectile.Center)) < projHitbox.Width / 2;
        public static readonly Color tungstenColor = new(176, 210, 178);
        public override Color? GetAlpha(Color lightColor)
        {
            return tungstenColor * Projectile.Opacity * (Main.mouseTextColor / 255f) * 0.9f;
        }
        public override void AI()
        {
            Projectile.position = Projectile.Center;
            float scaleModifier = Projectile.ai[1] == 1 ? 3 : 1;
            Projectile.scale += scaleModifier * 5f / Duration;
            Projectile.width = Projectile.height = (int)(BaseRadius * Projectile.scale);
            Projectile.Center = Projectile.position;
            if (Projectile.timeLeft < 8)
                Projectile.Opacity -= 0.15f;
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool PreDraw(ref Color lightColor)
        {
            float rotation = Projectile.rotation;
            Vector2 drawPos = Projectile.Center;
            var texture = TextureAssets.Projectile[Projectile.type].Value;

            int sizeY = texture.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int frameY = Projectile.frame * sizeY;
            Rectangle rectangle = new(0, frameY, texture.Width, sizeY);
            Vector2 origin = rectangle.Size() / 2f;
            float scaleModifier = (float)BaseRadius / sizeY;
            SpriteEffects spriteEffects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.EntitySpriteDraw(texture, drawPos - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), rectangle, Projectile.GetAlpha(lightColor),
                    rotation, origin, Projectile.scale * scaleModifier, spriteEffects, 0);
            return false;
        }
        public override bool? CanHitNPC(NPC target)
        {
            //if (target.whoAmI == Projectile.ai[0])
            //    return false;
            return base.CanHitNPC(target);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.Knockback *= 3;
            //if (target.whoAmI == Projectile.ai[0])
            modifiers.FinalDamage *= 0.5f;
        }
        /*
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            for (int i = 0; i < 50; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width,
                    Projectile.height, DustID.Smoke, 0f, 0f, 100, default, 3f);
                Main.dust[dust].velocity *= 1.4f;
            }
            for (int i = 0; i < 30; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width,
                    Projectile.height, DustID.Torch, 0f, 0f, 100, default, 3.5f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 7f;
                dust = Dust.NewDust(Projectile.position, Projectile.width,
                    Projectile.height, DustID.Torch, 0f, 0f, 100, default, 1.5f);
                Main.dust[dust].velocity *= 3f;
            }
        }
        */
    }
}