using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Content.Projectiles.Masomode;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Dungeon
{
    public class Paladin : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.Paladin);

        public int Counter;
        //public bool IsSmallPaladin;
        public bool FinishedSpawning;

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            base.SendExtraAI(npc, bitWriter, binaryWriter);

            //bitWriter.WriteBit(IsSmallPaladin);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            base.ReceiveExtraAI(npc, bitReader, binaryReader);

            //IsSmallPaladin = bitReader.ReadBit();
        }
        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Asset<Texture2D> armless = ModContent.Request<Texture2D>("FargowiltasSouls/Content/NPCs/EternityModeNPCs/VanillaEnemies/Dungeon/PaladinAmputee");
            Asset<Texture2D> arm = ModContent.Request<Texture2D>("FargowiltasSouls/Content/NPCs/EternityModeNPCs/VanillaEnemies/Dungeon/PaladinHand");
            Asset<Texture2D> hammer = ModContent.Request<Texture2D>("FargowiltasSouls/Content/Projectiles/Masomode/PaladinHammer");
            if (Counter > 0)
            {
                SpriteEffects effects = SpriteEffects.None;
                if (npc.spriteDirection == 1) effects = SpriteEffects.FlipHorizontally;
                Main.EntitySpriteDraw(armless.Value, npc.Center - Main.screenPosition + new Vector2(6 * npc.spriteDirection, 0), null, drawColor, npc.rotation, armless.Size() / 2, npc.scale, effects);

                if (Counter < 75)
                {
                    int counter = Counter;
                    if (counter > 60) counter = 60;
                    //dont know why this fucks up so bad with diff sprite direction byeah behold my fucked up draw code
                    //go my ternary expression

                    float x = counter / 60f;
                    float lerp = 1 - MathF.Pow(1 - x, 4);

                    float rotation = npc.rotation - npc.spriteDirection * Utils.AngleLerp(MathHelper.ToRadians(180), MathHelper.ToRadians(360), lerp);
                    rotation = MathHelper.WrapAngle(rotation);
                    Vector2 offset = new Vector2(npc.spriteDirection == 1 ? -10 : 10, -4);

                    Main.EntitySpriteDraw(hammer.Value, npc.Center - Main.screenPosition + offset, null, drawColor, rotation, new Vector2(npc.spriteDirection == 1 ? 34 : 14, 30), npc.scale, effects == SpriteEffects.None ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
                    Main.EntitySpriteDraw(arm.Value, npc.Center - Main.screenPosition + offset, null, drawColor, rotation, npc.spriteDirection == 1 ? new Vector2(12, 25) : new Vector2(6, 25), npc.scale, effects);
                   
                }
                if (Counter > 75)
                {
                    int counter = Counter - 75;
                    if (counter > 30) counter = 30;
                    //dont know why this fucks up so bad with diff sprite direction byeah behold my fucked up draw code
                    //go my ternary expression

                    float x = counter / 30f;
                    float lerp = x == 1 ? 1 : 1 - MathF.Pow(2, -10 * x);

                    float rotation = npc.rotation + npc.spriteDirection * Utils.AngleLerp(MathHelper.ToRadians(360), MathHelper.ToRadians(180), lerp);
                    rotation = MathHelper.WrapAngle(rotation);
                    Vector2 offset = new Vector2(npc.spriteDirection == 1 ? -10 : 10, -4);

                    if (counter < 5)
                    Main.EntitySpriteDraw(hammer.Value, npc.Center - Main.screenPosition + offset, null, drawColor, rotation, new Vector2(npc.spriteDirection == 1 ? 34 : 14, 30), npc.scale, effects == SpriteEffects.None ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
                    Main.EntitySpriteDraw(arm.Value, npc.Center - Main.screenPosition + offset, null, drawColor, rotation, npc.spriteDirection == 1 ? new Vector2(12, 25) : new Vector2(6, 25), npc.scale, effects);
                }

                return false;
            }
            return base.PreDraw(npc, spriteBatch, screenPos, drawColor);
        }
        public override bool SafePreAI(NPC npc)
        {
            Player target = null;
            if (npc.target == -1 || npc.target == 255)
            {
                npc.TargetClosest();
                return false;
            }
            target = Main.player[npc.target];
            if (!target.active || target.dead) return false;

            if ((Collision.CanHitLine(npc.position, 1, 1, target.position, 1, 1) && npc.velocity.Y == 0) || Counter > 0)
            {
                npc.velocity.X = 0;
                if (target.Center.X < npc.Center.X)
                {
                    npc.direction = -1;
                }
                else
                {
                    npc.direction = 1;
                }
                Counter++;
                if (Counter == 80)
                {
                    SoundEngine.PlaySound(SoundID.Item1, npc.Center);
                    Projectile.NewProjectileDirect(npc.GetSource_FromAI(), npc.Center, npc.AngleTo(target.Center).ToRotationVector2() * 20, ModContent.ProjectileType<PaladinHammer>(), FargoSoulsUtil.ScaledProjectileDamage(100), 1);
                }
                if (Counter > 100)
                {
                    Counter = 0;
                }
                return false;
            }
            else
            {
                Counter = 0;
            }

                return base.SafePreAI(npc);
        }
        public override void AI(NPC npc)
        {
            npc.ai[2] = 0;
            //base.AI(npc);

            //if (IsSmallPaladin && Main.netMode == NetmodeID.Server && ++Counter <= 65 && Counter % 15 == 5) //mp sync
            //{
            //    npc.netUpdate = true;
            //    NetSync(npc);
            //}

            //if (IsSmallPaladin && !FinishedSpawning)
            //{
            //    FinishedSpawning = true;

            //    npc.Center = npc.Bottom;

            //    npc.width = (int)(npc.width * .65f);
            //    npc.height = (int)(npc.height * .65f);
            //    npc.scale = .65f;
            //    npc.lifeMax /= 2;
            //    if (npc.life > npc.lifeMax)
            //        npc.life = npc.lifeMax;

            //    npc.Bottom = npc.Center;
            //}

            //EModeGlobalNPC.Aura(npc, 800f, false, 246, default, BuffID.BrokenArmor, ModContent.BuffType<LowGroundBuff>());
            //foreach (NPC n in Main.npc.Where(n => n.active && !n.friendly && n.type != NPCID.Paladin && n.Distance(npc.Center) < 800f))
            //{
            //    n.Eternity().PaladinsShield = true;
            //    if (Main.rand.NextBool())
            //    {
            //        int d = Dust.NewDust(n.position, n.width, n.height, DustID.GoldCoin, 0f, -1.5f, 0, new Color());
            //        Main.dust[d].velocity *= 0.5f;
            //        Main.dust[d].noLight = true;
            //    }
            //}
        }

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);

            //target.AddBuff(ModContent.BuffType<LethargicBuff>(), 600);
        }

        public override void OnKill(NPC npc)
        {
            if (FargoSoulsUtil.HostCheck)
            {
                for (int i = 0; i < 5; i++)
                {
                    FargoSoulsUtil.NewNPCEasy(npc.GetSource_FromThis(), npc.Center, NPCID.DungeonSpirit,
                        velocity: Main.rand.NextVector2Circular(16f, 16f));
                }
            }
        }
    }
}
