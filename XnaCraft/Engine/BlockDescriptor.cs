using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace XnaCraft.Engine
{
    public class BlockDescriptor
    {
        public BlockType BlockType { get; private set; }
        public Texture2D TextureTop { get; private set; }
        public Texture2D TextureBottom { get; private set; }
        public Texture2D TextureSide { get; private set; }

        public BlockDescriptor(BlockType blockType, Texture2D textureTop, Texture2D textureBottom, Texture2D textureSide)
        {
            BlockType = blockType;
            TextureTop = textureTop;
            TextureBottom = textureBottom;
            TextureSide = textureSide;
        }
    }
}
