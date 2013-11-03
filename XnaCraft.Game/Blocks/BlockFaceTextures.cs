using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XnaCraft.Engine;
using XnaCraft.Engine.World;

namespace XnaCraft.Game.Blocks
{
    public static class BlockFaceTextures
    {
        public static readonly BlockFaceTexture GrassTop = new BlockFaceTexture(0);
        public static readonly BlockFaceTexture Dirt = new BlockFaceTexture(1);
        public static readonly BlockFaceTexture GrassSide = new BlockFaceTexture(2);
        public static readonly BlockFaceTexture DebugTop = new BlockFaceTexture(3);
        public static readonly BlockFaceTexture DebugBottom = new BlockFaceTexture(4);
        public static readonly BlockFaceTexture DebugFront = new BlockFaceTexture(5);
        public static readonly BlockFaceTexture DebugBack = new BlockFaceTexture(6);
        public static readonly BlockFaceTexture DebugLeft = new BlockFaceTexture(7);
        public static readonly BlockFaceTexture DebugRight = new BlockFaceTexture(8);
    }
}
