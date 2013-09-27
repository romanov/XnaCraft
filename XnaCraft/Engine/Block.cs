using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XnaCraft.Engine
{
    public struct Block
    {
        public BlockDescriptor Descriptor;
        public Vector3 Position;

        public Block(BlockDescriptor descriptor, Vector3 position)
        {
            Descriptor = descriptor;
            Position = position;
        }
    }
}
