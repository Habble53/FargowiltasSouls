using FargowiltasSouls.Assets.Sounds;
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
using static FargowiltasSouls.Content.Projectiles.EffectVisual;

namespace FargowiltasSouls.Content.Projectiles.Masomode
{
    public class BoneFlail : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.Mace;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 2;
            ProjectileID.Sets.TrailingMode[Type] = 0;
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 6;
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
            Asset<Texture2D> chain = ModContent.Request<Texture2D>("FargowiltasSouls/Content/Projectiles/BossWeapons/LeashFlailChain");
            lightColor = Lighting.GetColor(Projectile.Center.ToTileCoordinates());
            float lightLevel = (lightColor.R + lightColor.G + lightColor.B) / 3f / 200f;
            Color glowColorBase = AngryBones.weaponGlowColor((int)Projectile.localAI[0]);
            if (Projectile.ai[2] > 0 || Projectile.ai[0] < 0)
            {
                for (int j = 0; j < 12; j++)
                {
                    Vector2 afterimageOffset = (MathHelper.TwoPi * j / 12f).ToRotationVector2() * 2;
                    Color glowColor = glowColorBase with { A = 0 } * 0.7f * lightLevel;


                    Main.EntitySpriteDraw(t.Value, Projectile.Center + afterimageOffset - Main.screenPosition, null, glowColor, Projectile.rotation, t.Size() / 2, Projectile.scale, SpriteEffects.None);
                }
            }
            if ((int)Projectile.ai[0] >= 0 && (Main.npc[(int)Projectile.ai[0]] == null || !Main.npc[(int)Projectile.ai[0]].active || !AngryBones.AllBones.Contains(Main.npc[(int)Projectile.ai[0]].type)))
            {

                Projectile.ai[0] = -1;
            }
            else if (Projectile.ai[0] >= 0) {
                NPC owner = Main.npc[(int)Projectile.ai[0]];
                float scale = Projectile.scale * 0.7f;
                for (int i = 0; i < Projectile.Distance(owner.Center); i += (int)(chain.Height() * scale))
                {
                    Main.EntitySpriteDraw(chain.Value, Projectile.Center + new Vector2(i, 0).RotatedBy(Projectile.AngleTo(owner.Center)) - Main.screenPosition, null, lightColor, Projectile.AngleTo(owner.Center) + MathHelper.PiOver2, chain.Size() / 2, scale, SpriteEffects.None);
                }
             }
            Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, t.Size() / 2, 1, SpriteEffects.None);
            //Projectile.hide = true;
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
            base.OnKill(timeLeft);
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
        }
        
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (oldVelocity.Y != Projectile.velocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y;
            if (oldVelocity.X != Projectile.velocity.X)
                Projectile.velocity.X = -oldVelocity.X;
            if (!AngryBones.BlueBone.Contains((int)Projectile.localAI[0]))
            {
                Projectile.velocity *= 0.7f;
            }

            return false;
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
                Projectile.velocity = (Projectile.position - Projectile.oldPos[1]).SafeNormalize(Vector2.Zero) * 8;
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
                if (Projectile.ai[2] == 0)
                {
                    owner.knockBackResist = 0.3f;
                    Projectile.tileCollide = false;
                    Projectile.rotation += MathHelper.ToRadians(15);
                    Projectile.timeLeft = 200;
                    
                    Vector2 displacement = Projectile.rotation.ToRotationVector2() * 30;
                    if (displacement.Y > 0) displacement.Y *= 0.3f;
                    displacement.X *= 0.8f;
                    Projectile.Center = owner.Center + displacement;
                    Projectile.velocity = Vector2.Zero;

                    Player target = Main.player[owner.target];
                    if (target != null && target.active && !target.dead && Collision.CanHitLine(owner.Center, 1, 1, target.Center, 1, 1))
                    {
                        Projectile.ai[1]++;
                        if (Projectile.ai[1] >= 200)
                        {
                            Projectile.ai[1] = 0;
                            Projectile.ai[2] = 1;
                            SoundEngine.PlaySound(FargosSoundRegistry.LeashBreak, Projectile.Center);
                        }
                    }
                }
                else if (Projectile.ai[2] == 1)
                {
                    Projectile.tileCollide = true;
                    Player target = Main.player[owner.target];
                    if (Projectile.ai[1] == 0)
                    {
                        Projectile.velocity = Projectile.AngleTo(target.Center).ToRotationVector2() * 8;
                    }
                    if (Projectile.ai[1] > 20)
                    {
                        Projectile.velocity *= 0.99f;
                    }
                    Projectile.ai[1]++;
                    if (Projectile.ai[1] >= 30)
                    {
                        Projectile.ai[1] = 0;
                        Projectile.ai[2] = 2;
                    }
                    Projectile.rotation += MathHelper.ToRadians(Projectile.velocity.Length() * 2);
                    owner.velocity.X = 0;
                    owner.knockBackResist = 0;
                }
                else if (Projectile.ai[2] == 2)
                {
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.AngleTo(owner.Center).ToRotationVector2() * 10, 0.08f);
                    if (Projectile.Distance(owner.Center) <= 20)
                    {
                        Projectile.ai[2] = 0;
                    }
                    Projectile.rotation += MathHelper.ToRadians(Projectile.velocity.Length() * 2);
                    owner.velocity.X = 0;
                    owner.knockBackResist = 0;
                }
            }
            base.AI();
        }
    }
}
