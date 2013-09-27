using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace XnaCraft.Engine
{
    class World
    {
        private readonly Dictionary<Point, BlockDescriptor[, ,]> _chunks = new Dictionary<Point, BlockDescriptor[, ,]>();

        public struct Chunk
        {
            public int X;
            public int Y;
            public BlockDescriptor[, ,] Blocks;

            public static Chunk Empty = new Chunk { X = 0, Y = 0, Blocks = null };
        }

        public World()
        {

        }

        public Chunk GetChunk(int x, int y)
        {
            var blocks = default(BlockDescriptor[, ,]);

            if (_chunks.TryGetValue(new Point(x, y), out blocks))
            {
                return new Chunk { X = x, Y = y, Blocks = blocks };
            }
            else
            {
                return Chunk.Empty;
            }
        }

        public void AddChunk(int x, int y, BlockDescriptor[, ,] blocks)
        {
            _chunks.Add(new Point(x, y), blocks);
        }
    }
}
