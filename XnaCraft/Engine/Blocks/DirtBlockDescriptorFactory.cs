using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XnaCraft.Engine.Blocks
{
    class DirtBlockDescriptorFactory : IBlockDescriptorFactory
    {
        public BlockDescriptor CreateDescriptor()
        {
            return new BlockDescriptor(BlockType.Dirt,
                BlockFaceTexture.Dirt,
                BlockFaceTexture.Dirt,
                BlockFaceTexture.Dirt);
        }
    }
}
