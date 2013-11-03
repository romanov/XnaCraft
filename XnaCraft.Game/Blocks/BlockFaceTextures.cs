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
        public static readonly BlockFaceTexture GrassTop = new BlockFaceTexture("GrassTop", 0);
        public static readonly BlockFaceTexture Dirt = new BlockFaceTexture("Dirt", 1);
        public static readonly BlockFaceTexture GrassSide = new BlockFaceTexture("GrassSide", 2);
        public static readonly BlockFaceTexture DebugTop = new BlockFaceTexture("DebugTop", 3);
        public static readonly BlockFaceTexture DebugBottom = new BlockFaceTexture("DebugBottom", 4);
        public static readonly BlockFaceTexture DebugFront = new BlockFaceTexture("DebugFront", 5);
        public static readonly BlockFaceTexture DebugBack = new BlockFaceTexture("DebugBack", 6);
        public static readonly BlockFaceTexture DebugLeft = new BlockFaceTexture("DebugLeft", 7);
        public static readonly BlockFaceTexture DebugRight = new BlockFaceTexture("DebugRight", 8);
    }
}
