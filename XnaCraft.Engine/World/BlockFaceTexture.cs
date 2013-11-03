using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XnaCraft.Engine.World
{
    public class BlockFaceTexture
    {
        public int Offset { get; private set; }

        public BlockFaceTexture(int offset)
        {
            Offset = offset;
        }
    }
}
