using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XnaCraft.Engine.World
{
    public class Chunk
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public BlockDescriptor[, ,] Blocks { get; private set; }
        public VertexBuffer Buffer { get { return _buffer; } }
        public BoundingBox BoundingBox { get; private set; }

        private volatile VertexBuffer _buffer;
        private volatile bool _isGenerated;
        private volatile bool _isBuilt;
        private volatile bool _isDirty;

        public bool IsGenerated { get { return _isGenerated; } }
        public bool IsBuilt { get { return _isBuilt; } }
        public bool IsDirty { get { return _isDirty; } }

        public Chunk(int x, int y)
        {
            X = x;
            Y = y;

            var bbMin = new Vector3(x * World.ChunkWidth, 0, y * World.ChunkWidth);
            var bbMax = bbMin + new Vector3(World.ChunkWidth, World.ChunkHeight, World.ChunkWidth);

            BoundingBox = new BoundingBox(bbMin, bbMax);
        }

        public void SetBlocks(BlockDescriptor[, ,] blocks)
        {
            Blocks = blocks;
            _isGenerated = true;
        }

        public void SetVertexBuffer(VertexBuffer buffer)
        {
            var oldBuffer = _buffer;

            _buffer = buffer;
            _isBuilt = true;

            if (oldBuffer != null)
            {
                oldBuffer.Dispose();
            }
        }

        public void SetBlock(int bx, int by, int bz, BlockDescriptor blockDescriptor)
        {
            Blocks[bx, by, bz] = blockDescriptor;
            _isDirty = true;
        }
    }
}
