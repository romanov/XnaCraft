using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XnaCraft.Engine.Blocks
{
    class DebugBlockDescriptorFactory : IBlockDescriptorFactory
    {
        public BlockDescriptor CreateDescriptor()
        {
            return new BlockDescriptor(BlockType.Debug,
                BlockFaceTexture.DebugTop,
                BlockFaceTexture.DebugBottom,
                BlockFaceTexture.DebugFront,
                BlockFaceTexture.DebugBack,
                BlockFaceTexture.DebugLeft,
                BlockFaceTexture.DebugRight);
        }
    }
}
