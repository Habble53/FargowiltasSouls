using FargowiltasSouls.Content.Projectiles.Masomode;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Snow
{
    public class UndeadViking : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.UndeadViking);

        public int AttackTimer;

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            base.SendExtraAI(npc, bitWriter, binaryWriter);
            binaryWriter.Write7BitEncodedInt(AttackTimer);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            base.ReceiveExtraAI(npc, bitReader, binaryReader);
            AttackTimer = binaryReader.Read7BitEncodedInt();
        }

        public override bool SafePreAI(NPC npc)
        {
            if (npc.target == -1)
                return base.SafePreAI(npc);
            Player target = Main.player[npc.target];

            if (npc.Center.Distance(target.Center) > 1500 || !target.active || target.dead)
                return base.SafePreAI(npc);

            AttackTimer++;

            // spawn proj
            if (AttackTimer == 150)
            {
                if (FargoSoulsUtil.HostCheck)
                {
                    Vector2 vel = 10 * Vector2.UnitX.RotatedBy((target.Center - npc.Center).ToRotation());
                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, vel, ModContent.ProjectileType<VikingHook>(), 1, 0f, ai0: npc.whoAmI, ai1: -30 , ai2: -1);
                }
                SoundEngine.PlaySound(SoundID.Unlock);
            }

            // sound cue
            if (AttackTimer == 180)
            {
                AttackTimer = 0;
                SoundEngine.PlaySound(SoundID.Item1);
            }
            return base.SafePreAI(npc);
        }
    }
}
