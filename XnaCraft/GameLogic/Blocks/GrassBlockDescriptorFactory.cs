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
            return new BlockDescriptor(BlockTypes.Grass,
                BlockFaceTextures.GrassTop,
                BlockFaceTextures.Dirt,
                BlockFaceTextures.GrassSide);
        }
    }
}
