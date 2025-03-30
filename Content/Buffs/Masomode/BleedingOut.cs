using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Masomode
{
    public class BleedingOut : ModBuff
    {
        public override string Texture => "Terraria/Images/Buff_" + BuffID.Bleeding;
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.FargoSouls().BleedingOut = true;
        }
    }
}