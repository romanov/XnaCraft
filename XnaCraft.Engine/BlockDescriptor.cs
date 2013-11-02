using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace XnaCraft.Engine
{
    [DebuggerDisplay("{BlockType}")]
    public class BlockDescriptor
    {
        public BlockType BlockType { get; private set; }

        public BlockFaceTexture TextureTop { get; private set; }
        public BlockFaceTexture TextureBottom { get; private set; }
        public BlockFaceTexture TextureFront { get; private set; }
        public BlockFaceTexture TextureBack { get; private set; }
        public BlockFaceTexture TextureLeft { get; private set; }
        public BlockFaceTexture TextureRight { get; private set; }

        public BlockDescriptor(BlockType blockType, BlockFaceTexture textureTop, BlockFaceTexture textureBottom, BlockFaceTexture textureSide)
        {
            BlockType = blockType;
            TextureTop = textureTop;
            TextureBottom = textureBottom;
            TextureFront = textureSide;
            TextureBack = textureSide;
            TextureLeft = textureSide;
            TextureRight = textureSide;
        }

        public BlockDescriptor(BlockType blockType, BlockFaceTexture textureTop, BlockFaceTexture textureBottom,
            BlockFaceTexture textureFront, BlockFaceTexture textureBack, BlockFaceTexture textureLeft, BlockFaceTexture textureRight)
        {
            BlockType = blockType;
            TextureTop = textureTop;
            TextureBottom = textureBottom;
            TextureFront = textureFront;
            TextureBack = textureBack;
            TextureLeft = textureLeft;
            TextureRight = textureRight;
        }
    }
}
