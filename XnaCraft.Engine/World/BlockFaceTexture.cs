using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XnaCraft.Engine.World
{
    public class BlockFaceTexture
    {
        public string Name { get; private set; }

        public int Offset { get; private set; }

        public BlockFaceTexture(string name, int offset)
        {
            Name = name;
            Offset = offset;
        }
    }
}
