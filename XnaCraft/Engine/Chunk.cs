using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Threading;

namespace XnaCraft.Engine
{
    public class Chunk
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public BlockDescriptor[, ,] Blocks { get; private set; }
        public VertexBuffer Buffer { get { return _buffer; } }
        public BoundingBox BoundingBox { get; private set; }

        private readonly GraphicsDevice _device;
        private readonly World _world;

        private volatile VertexBuffer _buffer;
        private volatile bool _isGenerated = false;
        private volatile bool _isBuilt = false;

        public bool IsGenerated { get { return _isGenerated; } }
        public bool IsBuilt { get { return _isBuilt; } }

        public Chunk(GraphicsDevice device, World world, int x, int y)
        {
            X = x;
            Y = y;

            var bbMin = new Vector3(x * WorldGenerator.CHUNK_WIDTH, 0, y * WorldGenerator.CHUNK_WIDTH);
            var bbMax = bbMin + new Vector3(WorldGenerator.CHUNK_WIDTH, WorldGenerator.CHUNK_HEIGHT, WorldGenerator.CHUNK_WIDTH);

            BoundingBox = new BoundingBox(bbMin, bbMax);

            _device = device;
            _world = world;
        }

        public void SetBlocks(BlockDescriptor[, ,] blocks)
        {
            Blocks = blocks;
            _isGenerated = true;
        }

        public void Build()
        {
            var builder = new ChunkVertexBuilder();

            for (var x = 0; x < WorldGenerator.CHUNK_WIDTH; x++)
            {
                for (var y = 0; y < WorldGenerator.CHUNK_HEIGHT; y++)
                {
                    for (var z = 0; z < WorldGenerator.CHUNK_WIDTH; z++)
                    {
                        var descriptor = Blocks[x, y, z];

                        if (descriptor != null)
                        {
                            var position = new Point3(X * WorldGenerator.CHUNK_WIDTH + x, y, Y * WorldGenerator.CHUNK_WIDTH + z);

                            var top = y == WorldGenerator.CHUNK_HEIGHT - 1 || Blocks[x, y + 1, z] == null;
                            var bottom = y != 0 && Blocks[x, y - 1, z] == null;
                            var front = (z > 0 ? Blocks[x, y, z - 1] : _world.GetBlock(position + new Point3(0, 0, -1))) == null;
                            var back = (z < WorldGenerator.CHUNK_WIDTH - 1 ? Blocks[x, y, z + 1] : _world.GetBlock(position + new Point3(0, 0, 1))) == null;
                            var left = (x > 0 ? Blocks[x - 1, y, z] : _world.GetBlock(position + new Point3(-1, 0, 0))) == null;
                            var right = (x < WorldGenerator.CHUNK_WIDTH - 1 ? Blocks[x + 1, y, z] : _world.GetBlock(position + new Point3(1, 0, 0))) == null;

                            if (top || bottom || front || back || left || right)
                            {
                                builder.BeginBlock(position.ToVector3(), descriptor, GetBlockNeighbours(position));

                                if (top)
                                {
                                    builder.AddTopFace();
                                }
                                if (bottom)
                                {
                                    builder.AddBottomFace();
                                }
                                if (front)
                                {
                                    builder.AddFrontFace();
                                }
                                if (back)
                                {
                                    builder.AddBackFace();
                                }
                                if (left)
                                {
                                    builder.AddLeftFace();
                                }
                                if (right)
                                {
                                    builder.AddRightFace();
                                }
                            }
                        }
                    }
                }
            }

            if (!_device.IsDisposed)
            {
                _buffer = builder.Build(_device);
            }

            _isBuilt = true;
        }

        private int[, ,] GetBlockNeighbours(Point3 blockPosition)
        {
            var neighbours = new int[3, 3, 3];

            var start = blockPosition - 1;

            for (var x = 0; x < 3; x++)
            {
                for (var y = 0; y < 3; y++)
                {
                    for (var z = 0; z < 3; z++)
                    {
                        neighbours[x, y, z] = _world.GetBlock(start + new Point3(x, y, z)) == null ? 0 : 1;
                    }
                }
            }

            return neighbours;
        }
    }
}
