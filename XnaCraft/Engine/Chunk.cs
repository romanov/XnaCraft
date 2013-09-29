using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Threading;

namespace XnaCraft.Engine
{
    class Chunk
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

            var bbMin = new Vector3(x * WorldGenerator.CHUNK_SIZE, 0, y * WorldGenerator.CHUNK_SIZE);
            var bbMax = bbMin + new Vector3(WorldGenerator.CHUNK_SIZE, WorldGenerator.CHUNK_SIZE, WorldGenerator.CHUNK_SIZE);

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
            var faces = new List<VertexPositionNormalTexture>();

            for (var x = 0; x < WorldGenerator.CHUNK_SIZE; x++)
            {
                for (var y = 0; y < WorldGenerator.CHUNK_SIZE; y++)
                {
                    for (var z = 0; z < WorldGenerator.CHUNK_SIZE; z++)
                    {
                        var descriptor = Blocks[x, y, z];

                        if (descriptor != null)
                        {
                            var position = new Vector3(X * WorldGenerator.CHUNK_SIZE + x, y, Y * WorldGenerator.CHUNK_SIZE + z);

                            var top = y < WorldGenerator.CHUNK_SIZE - 1 ? Blocks[x, y + 1, z] : null;
                            var bottom = y > 0 ? Blocks[x, y - 1, z] : null;
                            var front = z > 0 ? Blocks[x, y, z - 1] : null;
                            var back = z < WorldGenerator.CHUNK_SIZE - 1 ? Blocks[x, y, z + 1] : null;
                            var left = x > 0 ? Blocks[x - 1, y, z] : null;
                            var right = x < WorldGenerator.CHUNK_SIZE - 1 ? Blocks[x + 1, y, z] : null;

                            if (top == null)
                            {
                                faces.AddRange(GenerateTopFace(position, descriptor.TextureTop));
                            }
                            if (bottom == null && y != 0)
                            {
                                faces.AddRange(GenerateBottomFace(position, descriptor.TextureBottom));
                            }
                            if (front == null)
                            {
                                var draw = false;

                                if (z == 0)
                                {
                                    var adjacentChunk = default(Chunk);

                                    if (adjacentChunks.TryGetValue(new Point(X, Y - 1), out adjacentChunk))
                                    {
                                        draw = adjacentChunk.Blocks[x, y, WorldGenerator.CHUNK_SIZE - 1] == null;
                                    }
                                }
                                else
                                {
                                    draw = true;
                                }

                                if (draw)
                                {
                                    faces.AddRange(GenerateFrontFace(position, descriptor.TextureFront));
                                }
                            }
                            if (back == null)
                            {
                                var draw = false;

                                if (z == WorldGenerator.CHUNK_SIZE - 1)
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
                                    faces.AddRange(GenerateBackFace(position, descriptor.TextureBack));
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
                                        draw = adjacentChunk.Blocks[WorldGenerator.CHUNK_SIZE - 1, y, z] == null;
                                    }
                                }
                                else
                                {
                                    draw = true;
                                }

                                if (draw)
                                {
                                    faces.AddRange(GenerateLeftFace(position, descriptor.TextureLeft));
                                }
                            }
                            if (right == null)
                            {
                                var draw = false;

                                if (x == WorldGenerator.CHUNK_SIZE - 1)
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
                                    faces.AddRange(GenerateRightFace(position, descriptor.TextureRight));
                                }
                            }
                        }
                    }
                }

                var vertices = faces.ToArray();

                var buffer = new VertexBuffer(_device, VertexPositionNormalTexture.VertexDeclaration, vertices.Length, BufferUsage.WriteOnly);
                buffer.SetData(vertices);
                
                _buffer = buffer;
                _isBuilt = true;
            }
        }

        // TODO: code clean-up
        Vector3 topLeftFront = new Vector3(-0.5f, 0.5f, -0.5f);
        Vector3 topLeftBack = new Vector3(-0.5f, 0.5f, 0.5f);
        Vector3 topRightFront = new Vector3(0.5f, 0.5f, -0.5f);
        Vector3 topRightBack = new Vector3(0.5f, 0.5f, 0.5f);
        Vector3 bottomLeftFront = new Vector3(-0.5f, -0.5f, -0.5f);
        Vector3 bottomLeftBack = new Vector3(-0.5f, -0.5f, 0.5f);
        Vector3 bottomRightFront = new Vector3(0.5f, -0.5f, -0.5f);
        Vector3 bottomRightBack = new Vector3(0.5f, -0.5f, 0.5f);

        Vector3 normalFront = new Vector3(0.0f, 0.0f, 1.0f);
        Vector3 normalBack = new Vector3(0.0f, 0.0f, -1.0f);
        Vector3 normalTop = new Vector3(0.0f, 1.0f, 0.0f);
        Vector3 normalBottom = new Vector3(0.0f, -1.0f, 0.0f);
        Vector3 normalLeft = new Vector3(-1.0f, 0.0f, 0.0f);
        Vector3 normalRight = new Vector3(1.0f, 0.0f, 0.0f);

        private VertexPositionNormalTexture[] GenerateFrontFace(Vector3 position, BlockFaceTexture texture)
        {
            var uvMapping = GetUVMapping(texture);

            return new[] 
            {
                new VertexPositionNormalTexture(topLeftFront + position, normalFront, uvMapping.TopLeft),
                new VertexPositionNormalTexture(bottomLeftFront + position, normalFront, uvMapping.BottomLeft),
                new VertexPositionNormalTexture(topRightFront + position, normalFront, uvMapping.TopRight),
                new VertexPositionNormalTexture(bottomLeftFront + position, normalFront, uvMapping.BottomLeft),
                new VertexPositionNormalTexture(bottomRightFront + position, normalFront, uvMapping.BottomRight),
                new VertexPositionNormalTexture(topRightFront + position, normalFront, uvMapping.TopRight),
            };
        }

        private VertexPositionNormalTexture[] GenerateBackFace(Vector3 position, BlockFaceTexture texture)
        {
            var uvMapping = GetUVMapping(texture);

            return new[] 
            {
                new VertexPositionNormalTexture(topLeftBack + position, normalBack, uvMapping.TopRight),
                new VertexPositionNormalTexture(topRightBack + position, normalBack, uvMapping.TopLeft),
                new VertexPositionNormalTexture(bottomLeftBack + position, normalBack, uvMapping.BottomRight),
                new VertexPositionNormalTexture(bottomLeftBack + position, normalBack, uvMapping.BottomRight),
                new VertexPositionNormalTexture(topRightBack + position, normalBack, uvMapping.TopLeft),
                new VertexPositionNormalTexture(bottomRightBack + position, normalBack, uvMapping.BottomLeft),
            };
        }

        private VertexPositionNormalTexture[] GenerateTopFace(Vector3 position, BlockFaceTexture texture)
        {
            var uvMapping = GetUVMapping(texture);

            return new[] 
            {
                new VertexPositionNormalTexture(topLeftFront + position, normalTop, uvMapping.BottomLeft),
                new VertexPositionNormalTexture(topRightBack + position, normalTop, uvMapping.TopRight),
                new VertexPositionNormalTexture(topLeftBack + position, normalTop, uvMapping.TopLeft),
                new VertexPositionNormalTexture(topLeftFront + position, normalTop, uvMapping.BottomLeft),
                new VertexPositionNormalTexture(topRightFront + position, normalTop, uvMapping.BottomRight),
                new VertexPositionNormalTexture(topRightBack + position, normalTop, uvMapping.TopRight),
            };
        }

        private VertexPositionNormalTexture[] GenerateBottomFace(Vector3 position, BlockFaceTexture texture)
        {
            var uvMapping = GetUVMapping(texture);

            return new[] 
            {
                new VertexPositionNormalTexture(bottomLeftFront + position, normalBottom, uvMapping.TopLeft),
                new VertexPositionNormalTexture(bottomLeftBack + position, normalBottom, uvMapping.BottomLeft),
                new VertexPositionNormalTexture(bottomRightBack + position, normalBottom, uvMapping.BottomRight),
                new VertexPositionNormalTexture(bottomLeftFront + position, normalBottom, uvMapping.TopLeft),
                new VertexPositionNormalTexture(bottomRightBack + position, normalBottom, uvMapping.BottomRight),
                new VertexPositionNormalTexture(bottomRightFront + position, normalBottom, uvMapping.TopRight),
            };
        }

        private VertexPositionNormalTexture[] GenerateLeftFace(Vector3 position, BlockFaceTexture texture)
        {
            var uvMapping = GetUVMapping(texture);

            return new[] 
            {
                new VertexPositionNormalTexture(topLeftFront + position, normalLeft, uvMapping.TopRight),
                new VertexPositionNormalTexture(bottomLeftBack + position, normalLeft, uvMapping.BottomLeft),
                new VertexPositionNormalTexture(bottomLeftFront + position, normalLeft, uvMapping.BottomRight),
                new VertexPositionNormalTexture(topLeftBack + position, normalLeft, uvMapping.TopLeft),
                new VertexPositionNormalTexture(bottomLeftBack + position, normalLeft, uvMapping.BottomLeft),
                new VertexPositionNormalTexture(topLeftFront + position, normalLeft, uvMapping.TopRight),
            };
        }

        private VertexPositionNormalTexture[] GenerateRightFace(Vector3 position, BlockFaceTexture texture)
        {
            var uvMapping = GetUVMapping(texture);

            return new[] 
            {
                new VertexPositionNormalTexture(topRightFront + position, normalRight, uvMapping.TopLeft),
                new VertexPositionNormalTexture(bottomRightFront + position, normalRight, uvMapping.BottomLeft),
                new VertexPositionNormalTexture(bottomRightBack + position, normalRight, uvMapping.BottomRight),
                new VertexPositionNormalTexture(topRightBack + position, normalRight, uvMapping.TopRight),
                new VertexPositionNormalTexture(topRightFront + position, normalRight, uvMapping.TopLeft),
                new VertexPositionNormalTexture(bottomRightBack + position, normalRight, uvMapping.BottomRight),
            };
        }

        struct UVMapping
        {
            public Vector2 TopLeft;
            public Vector2 TopRight;
            public Vector2 BottomLeft;
            public Vector2 BottomRight;
        }

        private UVMapping GetUVMapping(BlockFaceTexture texture)
        {
            var offset = (int)texture;
            var step = 1.0f / Enum.GetValues(typeof(BlockFaceTexture)).Length;
            var uStart = offset * step;
            var uEnd = (offset + 1) * step;

            return new UVMapping
            {
                TopLeft = new Vector2(uEnd, 0.0f),
                TopRight = new Vector2(uStart, 0.0f),
                BottomLeft = new Vector2(uEnd, 1.0f),
                BottomRight = new Vector2(uStart, 1.0f),
            };
        }
    }
}
