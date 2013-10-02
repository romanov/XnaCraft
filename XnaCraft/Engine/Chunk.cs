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

        private volatile VertexBuffer _buffer;
        private volatile bool _isGenerated = false;
        private volatile bool _isBuilt = false;

        public bool IsGenerated { get { return _isGenerated; } }
        public bool IsBuilt { get { return _isBuilt; } }

        public Chunk(GraphicsDevice device, int x, int y)
        {
            X = x;
            Y = y;

            var bbMin = new Vector3(x * WorldGenerator.CHUNK_WIDTH, 0, y * WorldGenerator.CHUNK_WIDTH);
            var bbMax = bbMin + new Vector3(WorldGenerator.CHUNK_WIDTH, WorldGenerator.CHUNK_HEIGHT, WorldGenerator.CHUNK_WIDTH);

            BoundingBox = new Microsoft.Xna.Framework.BoundingBox(bbMin, bbMax);

            _device = device;
        }

        public void SetBlocks(BlockDescriptor[, ,] blocks)
        {
            Blocks = blocks;
            _isGenerated = true;
        }

        // TODO: split into smaller methods
        public void Build(Dictionary<Point, Chunk> adjacentChunks)
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
                                builder.AddTopFace();
                            }
                            if (bottom == null && y != 0)
                            {
                                builder.AddBottomFace();
                            }
                            if (front == null)
                            {
                                var draw = false;

                                if (z == 0)
                                {
                                    var adjacentChunk = default(Chunk);

                                    if (adjacentChunks.TryGetValue(new Point(X, Y - 1), out adjacentChunk))
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
                                    builder.AddFrontFace();
                                }
                            }
                            if (back == null)
                            {
                                var draw = false;

                                if (z == WorldGenerator.CHUNK_WIDTH - 1)
                                {
                                    var adjacentChunk = default(Chunk);

                                    if (adjacentChunks.TryGetValue(new Point(X, Y + 1), out adjacentChunk))
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
                                    builder.AddBackFace();
                                }
                            }
                            if (left == null)
                            {
                                var draw = false;

                                if (x == 0)
                                {
                                    var adjacentChunk = default(Chunk);

                                    if (adjacentChunks.TryGetValue(new Point(X - 1, Y), out adjacentChunk))
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
                                    builder.AddLeftFace();
                                }
                            }
                            if (right == null)
                            {
                                var draw = false;

                                if (x == WorldGenerator.CHUNK_WIDTH - 1)
                                {
                                    var adjacentChunk = default(Chunk);

                                    if (adjacentChunks.TryGetValue(new Point(X + 1, Y), out adjacentChunk))
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
                                    builder.AddRightFace();
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
