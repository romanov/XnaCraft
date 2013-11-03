using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XnaCraft.Engine;
using XnaCraft.Engine.World;

namespace XnaCraft.Game.Blocks
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
