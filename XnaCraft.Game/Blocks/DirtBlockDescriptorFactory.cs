﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XnaCraft.Engine;
using XnaCraft.Engine.World;

namespace XnaCraft.Game.Blocks
{
    class DirtBlockDescriptorFactory : IBlockDescriptorFactory
    {
        public BlockDescriptor CreateDescriptor()
        {
            return new BlockDescriptor(BlockTypes.Dirt,
                BlockFaceTextures.Dirt,
                BlockFaceTextures.Dirt,
                BlockFaceTextures.Dirt);
        }
    }
}
