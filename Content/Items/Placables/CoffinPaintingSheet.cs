﻿using Terraria.ID;
using Terraria.ObjectData;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Placables
{
    public class CoffinPaintingSheet : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
            TileObjectData.newTile.Width = 6;
            TileObjectData.newTile.Height = 4;
            TileObjectData.newTile.CoordinateHeights = [16, 16, 16, 16];
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.StyleWrapLimit = 36;
            TileObjectData.addTile(Type);

            DustType = DustID.Sand;
            TileID.Sets.DisableSmartCursor[Type] = true;
        }
    }
}

