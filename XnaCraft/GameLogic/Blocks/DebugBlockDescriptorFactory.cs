﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XnaCraft.Engine;

namespace XnaCraft.GameLogic.Blocks
{
    class DebugBlockDescriptorFactory : IBlockDescriptorFactory
    {
        public BlockDescriptor CreateDescriptor()
        {
            return new BlockDescriptor(BlockType.Debug,
                BlockFaceTextures.DebugTop,
                BlockFaceTextures.DebugBottom,
                BlockFaceTextures.DebugFront,
                BlockFaceTextures.DebugBack,
                BlockFaceTextures.DebugLeft,
                BlockFaceTextures.DebugRight);
        }
    }
}