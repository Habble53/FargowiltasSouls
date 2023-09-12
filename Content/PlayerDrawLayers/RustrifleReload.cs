using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework.Graphics;
using FargowiltasSouls.Content.Items.Weapons.Challengers;

namespace FargowiltasSouls.Content.PlayerDrawLayers
{
	public class RustrifleReload : PlayerDrawLayer
    {
        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) => drawInfo.drawPlayer.GetModPlayer<FargoSoulsPlayer>().RustRifleReloading && drawInfo.drawPlayer.HeldItem.type == ModContent.ItemType<NavalRustrifle>();
        public override Position GetDefaultPosition() => new Between();
        protected override void Draw(ref PlayerDrawSet drawInfo)
        {

            if (drawInfo.shadow == 0f)
            {
                float scale = 2f;

                Player player = drawInfo.drawPlayer;
                FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();

                Vector2 pos = player.Center - Main.screenPosition;

                Texture2D barTexture = ModContent.Request<Texture2D>("FargowiltasSouls/Content/PlayerDrawLayers/RustrifleReloadBar", AssetRequestMode.ImmediateLoad).Value;
                Rectangle barRectangle = barTexture.Bounds;

                Vector2 offset = (Vector2.UnitY * 150 * player.gravDir).RotatedBy(-drawInfo.rotation);

                int barWidth = (int)((barRectangle.Width - 16) * scale);

                Vector2 barPos = pos + offset;
                Vector2 barPos0 = barPos - (Vector2.UnitX * barWidth / 2).RotatedBy(-drawInfo.rotation);


                DrawData bar = new(barTexture, barPos, barRectangle, Color.White, player.gravDir < 0 ? MathHelper.Pi - drawInfo.rotation : 0f - drawInfo.rotation, barRectangle.Size() / 2, scale, SpriteEffects.None, 0);
                drawInfo.DrawDataCache.Add(bar);

                Texture2D zoneTexture = ModContent.Request<Texture2D>("FargowiltasSouls/Content/PlayerDrawLayers/RustrifleReloadZone", AssetRequestMode.ImmediateLoad).Value;
                Rectangle zoneRectangle = zoneTexture.Bounds;


                Vector2 zonePos = barPos0 + (Vector2.UnitX * barWidth * modPlayer.RustRifleReloadZonePos).RotatedBy(-drawInfo.rotation);
                DrawData zone = new(zoneTexture, zonePos, zoneRectangle, Color.White, player.gravDir < 0 ? MathHelper.Pi - drawInfo.rotation : 0f - drawInfo.rotation, zoneRectangle.Size() / 2, scale, SpriteEffects.None, 0);
                drawInfo.DrawDataCache.Add(zone);

                Texture2D sliderTexture = ModContent.Request<Texture2D>("FargowiltasSouls/Content/PlayerDrawLayers/RustrifleReloadSlider", AssetRequestMode.ImmediateLoad).Value;
                Rectangle sliderRectangle = sliderTexture.Bounds;


                Vector2 sliderPos = barPos0 + (Vector2.UnitX * barWidth * modPlayer.RustRifleReloadProgress).RotatedBy(-drawInfo.rotation);
                DrawData slider = new(sliderTexture, sliderPos, sliderRectangle, Color.White, player.gravDir < 0 ? MathHelper.Pi - drawInfo.rotation : 0f - drawInfo.rotation, sliderRectangle.Size() / 2, scale, SpriteEffects.None, 0);
                drawInfo.DrawDataCache.Add(slider);
            }
        }
    }
}