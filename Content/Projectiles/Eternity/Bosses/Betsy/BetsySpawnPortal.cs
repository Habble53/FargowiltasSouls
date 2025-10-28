using FargowiltasSouls.Assets.Textures;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Eternity.Bosses.Betsy
{
    public class BetsySpawnPortal : ModProjectile
    {
        public override string Texture => FargoAssets.GetAssetString("Content/NPCs", "TavernkeepPortal");

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 9;
        }
        public override void SetDefaults()
        {
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.width = 60;
            Projectile.height = 100;
            Projectile.hide = true;
        }

        public override void AI()
        {
            ref float target = ref Projectile.ai[1];
            ref float type = ref Projectile.ai[0];

            if (type == 0 || !Main.player[(int)target].Alive())
            {
                Projectile.Kill();
                return;
            }

            if (Projectile.ai[2] == 0)
            {
                SoundEngine.PlaySound(SoundID.Item104, Projectile.Center);
                FargoSoulsUtil.DustRing(Projectile.Center, 20, DustID.Shadowflame, 5f);
            }

            if (++Projectile.ai[2] % 60 == 0)
            {
                switch(type)
                {
                    case NPCID.DD2WyvernT3:
                        SoundEngine.PlaySound(SoundID.DD2_WyvernScream, Projectile.Center);
                        if (FargoSoulsUtil.HostCheck)
                            Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<BetsyWyvernClone>(), Projectile.damage, 1f, ai0: target);
                        break;
                    default:
                        Projectile.Kill();
                        return;
                }
            }

            if (Projectile.ai[2] >= 60)
            {
                Projectile.Kill();
                return;
            }

            if (Projectile.frameCounter++ > 5)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame > 8)
                    Projectile.frame = 0;
            }
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
            behindProjectiles.Add(index);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            FargoSoulsUtil.GenericProjectileDraw(Projectile, Color.Pink);
            return false;
        }
    }
}
