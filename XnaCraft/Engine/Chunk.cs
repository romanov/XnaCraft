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
                        neighbours[x, y, z] = _world.GetBlock(start + new Point3(x, y, z)).IsEmpty ? 0 : 1;
                    }
                }
            }

            return neighbours;
        }

        // TODO: split into smaller methods
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
                            var position = new Vector3(X * WorldGenerator.CHUNK_WIDTH + x, y, Y * WorldGenerator.CHUNK_WIDTH + z);

                            builder.BeginBlock(position, descriptor);

                            var top = y < WorldGenerator.CHUNK_HEIGHT - 1 ? Blocks[x, y + 1, z] : null;
                            var bottom = y > 0 ? Blocks[x, y - 1, z] : null;
                            var front = z > 0 ? Blocks[x, y, z - 1] : null;
                            var back = z < WorldGenerator.CHUNK_WIDTH - 1 ? Blocks[x, y, z + 1] : null;
                            var left = x > 0 ? Blocks[x - 1, y, z] : null;
                            var right = x < WorldGenerator.CHUNK_WIDTH - 1 ? Blocks[x + 1, y, z] : null;

                            if (top == null)
                            {
                                builder.AddTopFace(GetBlockNeighbours(position.ToPoint3()));
                            }
                            if (bottom == null && y != 0)
                            {
                                builder.AddBottomFace(GetBlockNeighbours(position.ToPoint3()));
                            }
                            if (front == null)
                            {
                                var draw = false;

                                if (z == 0)
                                {
                                    var adjacentChunk = _world.GetChunk(X, Y - 1);

                                    if (adjacentChunk != null)
                                    {
                                        draw = adjacentChunk.Blocks[x, y, WorldGenerator.CHUNK_WIDTH - 1] == null;
                                    }
                                }
                                else
                                {
                                    draw = true;
                                }

                                if (draw)
                                {
                                    builder.AddFrontFace(GetBlockNeighbours(position.ToPoint3()));
                                }
                            }
                            if (back == null)
                            {
                                var draw = false;

                                if (z == WorldGenerator.CHUNK_WIDTH - 1)
                                {
                                    var adjacentChunk = _world.GetChunk(X, Y + 1);

                                    if (adjacentChunk != null)
                                    {
                                        draw = adjacentChunk.Blocks[x, y, 0] == null;
                                    }
                                }
                                else
                                {
                                    draw = true;
                                }

                                if (draw)
                                {
                                    builder.AddBackFace(GetBlockNeighbours(position.ToPoint3()));
                                }
                            }
                            if (left == null)
                            {
                                var draw = false;

                                if (x == 0)
                                {
                                    var adjacentChunk = _world.GetChunk(X - 1, Y);

                                    if (adjacentChunk != null)
                                    {
                                        draw = adjacentChunk.Blocks[WorldGenerator.CHUNK_WIDTH - 1, y, z] == null;
                                    }
                                }
                                else
                                {
                                    draw = true;
                                }

                                if (draw)
                                {
                                    builder.AddLeftFace(GetBlockNeighbours(position.ToPoint3()));
                                }
                            }
                            if (right == null)
                            {
                                var draw = false;

                                if (x == WorldGenerator.CHUNK_WIDTH - 1)
                                {
                                    var adjacentChunk = _world.GetChunk(X + 1, Y);

                                    if (adjacentChunk != null)
                                    {
                                        draw = adjacentChunk.Blocks[0, y, z] == null;
                                    }
                                }
                                else
                                {
                                    draw = true;
                                }

                                if (draw)
                                {
                                    builder.AddRightFace(GetBlockNeighbours(position.ToPoint3()));
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
        }
    }
}
