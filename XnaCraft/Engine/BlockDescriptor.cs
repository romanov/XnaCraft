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
        public BlockFaceTexture TextureTop { get; private set; }
        public BlockFaceTexture TextureBottom { get; private set; }
        public BlockFaceTexture TextureSide { get; private set; }

        public BlockDescriptor(BlockType blockType, BlockFaceTexture textureTop, BlockFaceTexture textureBottom, BlockFaceTexture textureSide)
        {
            BlockType = blockType;
            TextureTop = textureTop;
            TextureBottom = textureBottom;
            TextureSide = textureSide;
        }
    }
}
