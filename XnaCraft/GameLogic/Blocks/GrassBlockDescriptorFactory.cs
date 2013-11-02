using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XnaCraft.Engine;

namespace XnaCraft.GameLogic.Blocks
{
    class GrassBlockDescriptorFactory : IBlockDescriptorFactory
    {
        public BlockDescriptor CreateDescriptor()
        {
            return new BlockDescriptor(BlockType.Grass,
                BlockFaceTextures.GrassTop,
                BlockFaceTextures.Dirt,
                BlockFaceTextures.GrassSide);
        }
    }
}
