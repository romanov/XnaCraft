using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XnaCraft.Engine.Blocks
{
    class GrassBlockDescriptorFactory : IBlockDescriptorFactory
    {
        public BlockDescriptor CreateDescriptor()
        {
            return new BlockDescriptor(BlockType.Grass,
                BlockFaceTexture.GrassTop,
                BlockFaceTexture.Dirt,
                BlockFaceTexture.GrassSide);
        }
    }
}
