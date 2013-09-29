using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;

namespace XnaCraft.Engine
{
    class World
    {
        private readonly Dictionary<Point, Chunk> _chunks = new Dictionary<Point, Chunk>();

        public World()
        {

        }

        public Chunk GetChunk(int x, int y)
        {
            lock (_chunks)
            {
                var chunk = default(Chunk);

                if (_chunks.TryGetValue(new Point(x, y), out chunk))
                {
                    return chunk;
                }
                else
                {
                    return null;
                }
            }
        }

        public Dictionary<Point, Chunk> GetAdjacentChunks(Chunk chunk)
        {
            var chunks = new[] 
            { 
                GetChunk(chunk.X, chunk.Y - 1),
                GetChunk(chunk.X, chunk.Y + 1),
                GetChunk(chunk.X - 1, chunk.Y),
                GetChunk(chunk.X + 1, chunk.Y),
            };

            return chunks.Where(c => c != null).ToDictionary(c => new Point(c.X, c.Y));
        }

        public void AddChunk(int x, int y, Chunk chunk)
        {
            lock (_chunks)
            {
                _chunks.Add(new Point(x, y), chunk);
            }
        }

        public bool HasChunk(int x, int y)
        {
            lock (_chunks)
            {
                return _chunks.ContainsKey(new Point(x, y));
            }
        }

        public bool CheckCollision(BoundingBox boundingBox)
        {
            //var corners = boundingBox.GetCorners();

            var minX = (int)Math.Floor(boundingBox.Min.X); // corners.Select(c => (int)Math.Floor(c.X)).Min();
            var minY = (int)Math.Floor(boundingBox.Min.Y); // corners.Select(c => (int)Math.Floor(c.Y)).Min();
            var minZ = (int)Math.Floor(boundingBox.Min.Z); // corners.Select(c => (int)Math.Floor(c.Z)).Min();
            var maxX = (int)Math.Floor(boundingBox.Max.X); // corners.Select(c => (int)Math.Floor(c.X)).Max();
            var maxY = (int)Math.Floor(boundingBox.Max.Y); // corners.Select(c => (int)Math.Floor(c.Y)).Max();
            var maxZ = (int)Math.Floor(boundingBox.Max.Z); // corners.Select(c => (int)Math.Floor(c.Z)).Max();

            var blocks = GetBlockRange(minX, minY, minZ, maxX, maxY, maxZ);

            return blocks.Any(b => b.BoundingBox.Intersects(boundingBox));
        }

        private IEnumerable<Block> GetBlockRange(int minX, int minY, int minZ, int maxX, int maxY, int maxZ)
        {
            for (var x = minX; x <= maxX; x++)
            {
                for (var y = minY; y <= maxY; y++)
                {
                    for (var z = minZ; z <= maxZ; z++)
                    {
                        var cx = (int)Math.Floor(x / (float)WorldGenerator.CHUNK_SIZE);
                        var cy = (int)Math.Floor(z / (float)WorldGenerator.CHUNK_SIZE);

                        var bx = x - cx * WorldGenerator.CHUNK_SIZE;
                        var by = y;
                        var bz = z - cy * WorldGenerator.CHUNK_SIZE;

                        if (by < WorldGenerator.CHUNK_SIZE)
                        {
                            var chunk = GetChunk(cx, cy);

                            if (chunk != null && chunk.IsGenerated)
                            {
                                var block = chunk.Blocks[bx, by, bz];

                                if (block != null)
                                {
                                    yield return new Block { BlockDescriptor = block, X = x, Y = y, Z = z };
                                }
                            }
                        }
                    }
                }
            }
        }

        struct Block
        {
            public BlockDescriptor BlockDescriptor;
            public int X;
            public int Y;
            public int Z;

            public BoundingBox BoundingBox
            {
                get
                {
                    return new BoundingBox(new Vector3(X, Y, Z), new Vector3(X + 1, Y + 1, Z + 1));
                }
            }
        }
    }
}
