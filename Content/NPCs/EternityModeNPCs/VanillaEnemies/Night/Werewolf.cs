using FargowiltasSouls.Assets.Sounds;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Night
{
    public class Werewolf : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.Werewolf);

        public override void SetDefaults(NPC npc)
        {
            npc.lifeMax *= 3;
            npc.knockBackResist = 0f;
        }

        public int JumpTimer = 140;
        public override bool SafePreAI(NPC npc)
        {
            npc.knockBackResist = 0f;
            //EModeGlobalNPC.Aura(npc, 200, ModContent.BuffType<BerserkedBuff>(), false, 60);
            JumpTimer--;
            if (JumpTimer <= 0)
            {
                if (JumpTimer == 0)
                {
                    FargoSoulsUtil.DustRing(npc.Center, 32, DustID.ViciousPowder, 5f, default, 2f);
                    SoundEngine.PlaySound(new SoundStyle("FargowiltasSouls/Assets/Sounds/NPC_Hit_6") with { Pitch = -0.5f }, npc.Center);
                }
                npc.velocity *= 0.9f;
                if (JumpTimer <= -60)
                {
                    JumpTimer = 60 * 9;
                    if (npc.HasPlayerTarget && Collision.CanHitLine(npc.Center, 1, 1, Main.player[npc.target].Center, 1, 1))
                    {
                        Vector2 targetPoint = Main.player[npc.target].Center - Vector2.UnitY * 200;
                        float distanceScale = MathHelper.Clamp(npc.Distance(targetPoint) / 1000f, 0f, 1f);
                        float vel = 5f + 15f * distanceScale;
                        //Main.NewText($"{vel}");
                        npc.velocity = npc.DirectionTo(targetPoint) * vel;
                        SoundEngine.PlaySound(FargosSoundRegistry.ThrowShort with { Pitch = 0.5f }, npc.Center);
                    }
                        
                }
            }
            else
            {
                if (JumpTimer < 10 && !(npc.HasPlayerTarget && Collision.CanHitLine(npc.Center, 1, 1, Main.player[npc.target].Center, 1, 1)))
                    JumpTimer++;
            }
            
            return base.SafePreAI(npc);
        }

        public override void OnHitNPC(NPC npc, NPC target, NPC.HitInfo hit)
        {
            base.OnHitNPC(npc, target, hit);

            if (target.townNPC && (hit.InstantKill || target.life < hit.Damage))
            {
                target.Transform(npc.type);
                //SoundEngine.PlaySound(SoundID.);
            }
        }

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);

            target.AddBuff(BuffID.Rabies, 1800);
        }
    }
}
