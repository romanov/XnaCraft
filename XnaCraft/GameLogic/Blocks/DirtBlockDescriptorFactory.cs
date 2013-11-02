﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XnaCraft.Engine;

namespace XnaCraft.GameLogic.Blocks
{
    class DirtBlockDescriptorFactory : IBlockDescriptorFactory
    {
        public BlockDescriptor CreateDescriptor()
        {
            return new BlockDescriptor(BlockType.Dirt,
                BlockFaceTextures.Dirt,
                BlockFaceTextures.Dirt,
                BlockFaceTextures.Dirt);
        }
    }
}