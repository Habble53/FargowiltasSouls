using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Terraria.GameContent;
using FargowiltasSouls.Content.Projectiles.Deathrays;
using FargowiltasSouls.Content.Projectiles.Minions;
using System.Runtime.InteropServices;
using FargowiltasSouls.Content.Projectiles.BossWeapons;
using FargowiltasSouls.Core.Systems;

namespace FargowiltasSouls.Content.Projectiles.Masomode
{
    public class VikingHook : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_13";

        private static Asset<Texture2D> chainTexture;

        public override void Load()
        {
            chainTexture = TextureAssets.Chain;
        }

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.hostile = true;
            Projectile.tileCollide = true;
            Projectile.penetrate = -1;
        }

        public int maxRange = WorldSavingSystem.MasochistModeReal ? 600 : 400;

        public override void AI()
        {
            NPC owner = Main.npc[(int)Projectile.ai[0]];
            if (!owner.active)
                Projectile.Kill();


            float rot = (owner.Center - Projectile.Center).ToRotation();
            float dist = Projectile.Center.Distance(owner.Center);
            Projectile.rotation = rot - MathHelper.PiOver2;
            if (Projectile.ai[1] < 0)
            {
                rot = (Main.player[owner.target].Center - owner.Center).ToRotation();
                Projectile.ai[1]++;
                if (Projectile.ai[1] == 0)
                {
                    Projectile.velocity = 10 * Vector2.UnitX.RotatedBy(rot);
                }
                else
                {
                    Projectile.Center = owner.Center + 10 * Vector2.UnitX.RotatedBy(rot);
                    Projectile.rotation = rot + MathHelper.PiOver2;
                }
                return;
            }

            Projectile.rotation = rot - MathHelper.PiOver2;

            if (Projectile.ai[1] >= 1)
            {
                if (Projectile.ai[2] != -1)
                {
                    Player target = Main.player[(int)Projectile.ai[2]];
                    // remove grab if immune to damage
                    if (target.immune)
                        Projectile.ai[2] = -1;

                    if (!target.dead && target.active)
                        target.Center = Projectile.Center;
                }
                Projectile.velocity = 10 * Vector2.UnitX.RotatedBy(rot);
                if (dist < 10)
                {
                    Projectile.Kill();
                }
                return;
            }

            for (int i = 0; i < Main.player.Length; i++)
            {
                Player p = Main.player[i];
                if (!p.active || p.dead)
                    continue;

                if (Projectile.Hitbox.Intersects(p.Hitbox))
                {
                    // grab player and go to pull state
                    Projectile.ai[2] = p.whoAmI;
                    Projectile.ai[1] = 2;
                    return;
                }
            }

            if (dist > maxRange)
            {
                // retract state
                Projectile.ai[1] = 1;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            // retract state
            Projectile.ai[1] = 1;
            return false;
        }

        public override bool? CanDamage() => false;

        public override bool PreDraw(ref Color lightColor)
        {
            NPC owner = Main.npc[(int) Projectile.ai[0]];
            if (!owner.active)
                return false;

            Vector2 pos = owner.Center;

            Rectangle? chainSourceRectangle = null;

            float chainHeightAdjustment = 0f;

            Vector2 chainOrigin = chainTexture.Size() / 2f;
            Vector2 chainDrawPosition = Projectile.Center;
            Vector2 vectorFromProjectileToNPC = pos.MoveTowards(chainDrawPosition, 4f) - chainDrawPosition;
            Vector2 unitVectorFromProjectileToNPC = vectorFromProjectileToNPC.SafeNormalize(Vector2.Zero);
            float chainSegmentLength = (chainSourceRectangle.HasValue ? chainSourceRectangle.Value.Height : chainTexture.Height()) + chainHeightAdjustment;
            if (chainSegmentLength == 0)
            {
                chainSegmentLength = 10 * Projectile.scale;
            }
            float chainRotation = unitVectorFromProjectileToNPC.ToRotation() + MathHelper.PiOver2;
            int chainCount = 0;
            float chainLengthRemainingToDraw = vectorFromProjectileToNPC.Length() + chainSegmentLength / 2f;

            while (chainLengthRemainingToDraw > 0f)
            {
                Color chainDrawColor = Lighting.GetColor((int)chainDrawPosition.X / 16, (int)(chainDrawPosition.Y / 16f));

                var chainTextureToDraw = chainTexture;

                Main.spriteBatch.Draw(chainTextureToDraw.Value, chainDrawPosition - Main.screenPosition, chainSourceRectangle, chainDrawColor, chainRotation, chainOrigin, Projectile.scale, SpriteEffects.None, 0f);

                chainDrawPosition += unitVectorFromProjectileToNPC * chainSegmentLength;
                chainCount++;
                chainLengthRemainingToDraw -= chainSegmentLength;
            }
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Rectangle frame = new(0, 0, texture.Width, texture.Height);
            lightColor = Lighting.GetColor(Projectile.Center.ToTileCoordinates());
            float lightLevel = (lightColor.R + lightColor.G + lightColor.B) / 3f / 200f;
            Color glowColor = Color.White with { A = 0 } * 0.7f * lightLevel;
            for (int i = 0; i < 12; i++)
            {
                Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, glowColor, Projectile.rotation, new Vector2(frame.Width / 2, frame.Height / 2), 1.2f * Projectile.scale, SpriteEffects.None);
            }
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, lightColor, Projectile.rotation, new Vector2(frame.Width / 2, frame.Height / 2), Projectile.scale, SpriteEffects.None);
            return false;
        }
    }
}
