using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;

namespace XnaCraft.Engine
{
    public class World
    {
        private readonly Dictionary<Point, Chunk> _chunks = new Dictionary<Point, Chunk>();

        public World()
        {

        }

        public Chunk GetChunk(int x, int y)
        {
            var chunk = default(Chunk);

            if (_chunks.TryGetValue(new Point(x, y), out chunk))
            {
                return chunk.IsGenerated ? chunk : null;
            }
            else
            {
                return null;
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
            _chunks.Add(new Point(x, y), chunk);
        }

        public bool HasChunk(int x, int y)
        {
            return _chunks.ContainsKey(new Point(x, y));
        }

        public bool CheckCollision(BoundingBox boundingBox)
        {
            var blocks = GetBlockRange(boundingBox.Min.ToPoint3(), boundingBox.Max.ToPoint3());

            return blocks.Any(b => b.BoundingBox.Intersects(boundingBox));
        }

        public IEnumerable<Block> RayCast(Ray ray, Point3 center, int radius, bool returnEmptyBlocks = false)
        {
            var blocks = GetBlockRange(center - radius, center + radius, returnEmptyBlocks);

            var hitBlocks = blocks
                .Select(b => new { Result = ray.Intersects(b.BoundingBox), Block = b })
                .Where(r => r.Result.HasValue)
                .OrderBy(r => r.Result.Value)
                .Select(r => r.Block);

            return hitBlocks;
        }

        private IEnumerable<Block> GetBlockRange(Point3 min, Point3 max, bool returnEmptyBlocks = false)
        {
            for (var x = min.X; x <= max.X; x++)
            {
                for (var y = min.Y; y <= max.Y; y++)
                {
                    for (var z = min.Z; z <= max.Z; z++)
                    {
                        var cx = (int)Math.Floor(x / (float)WorldGenerator.CHUNK_WIDTH);
                        var cy = (int)Math.Floor(z / (float)WorldGenerator.CHUNK_WIDTH);

                        var bx = x - cx * WorldGenerator.CHUNK_WIDTH;
                        var by = y;
                        var bz = z - cy * WorldGenerator.CHUNK_WIDTH;

                        if (bx >= 0 && bx < WorldGenerator.CHUNK_WIDTH && by >= 0 && by < WorldGenerator.CHUNK_HEIGHT && bz >= 0 && bz < WorldGenerator.CHUNK_WIDTH)
                        {
                            var chunk = GetChunk(cx, cy);

                            if (chunk != null && chunk.IsGenerated)
                            {
                                var block = chunk.Blocks[bx, by, bz];

                                if (block != null)
                                {
                                    yield return new Block { BlockDescriptor = block, X = x, Y = y, Z = z };
                                }
                                else if (returnEmptyBlocks)
                                {
                                    yield return new Block { X = x, Y = y, Z = z };
                                }
                            }
                        }
                    }
                }
            }
        }

        public IEnumerable<Chunk> GetVisibleChunks(Camera camera)
        {
            var viewFrustrum = new BoundingFrustum(camera.View * camera.Projection);

            var ccx = (int)Math.Floor(camera.Position.X / WorldGenerator.CHUNK_WIDTH);
            var ccy = (int)Math.Floor(camera.Position.Z / WorldGenerator.CHUNK_WIDTH);
            var radius = 14;

            var chunks = new HashSet<Chunk>();
            var radiusSquared = radius * radius;

            for (var cx = ccx - radius; cx <= ccx + radius; cx++)
            {
                for (var cy = ccy - radius; cy <= ccy + radius; cy++)
                {
                    var dx = cx - ccx;
                    var dy = cy - ccy;

                    if (dx * dx + dy * dy <= radiusSquared)
                    {
                        var chunk = GetChunk(cx, cy);

                        if (chunk != null && chunk.IsBuilt && chunk.BoundingBox.Intersects(viewFrustrum))
                        {
                            chunks.Add(chunk);
                        }
                    }
                }
            }

            return chunks;
        }

        public struct Block
        {
            public BlockDescriptor BlockDescriptor;
            public int X;
            public int Y;
            public int Z;

            public bool IsEmpty
            {
                get
                {
                    return BlockDescriptor == null;
                }
            }

            public BoundingBox BoundingBox
            {
                get
                {
                    return new BoundingBox(new Vector3(X - 0.5f, Y - 0.5f, Z - 0.5f), new Vector3(X + 0.5f, Y + 0.5f, Z + 0.5f));
                }
            }
        }

        public void AddBlock(int x, int y, int z, BlockType blockType)
        {
            var cx = (int)Math.Floor(x / (float)WorldGenerator.CHUNK_WIDTH);
            var cy = (int)Math.Floor(z / (float)WorldGenerator.CHUNK_WIDTH);

            var bx = x - cx * WorldGenerator.CHUNK_WIDTH;
            var by = y;
            var bz = z - cy * WorldGenerator.CHUNK_WIDTH;

            var chunk = GetChunk(cx, cy);

            var grassDescriptor = new BlockDescriptor(BlockType.Grass,
                BlockFaceTexture.GrassTop,
                BlockFaceTexture.Dirt,
                BlockFaceTexture.GrassSide);

            chunk.Blocks[bx, by, bz] = grassDescriptor;

            var adjacentChunks = GetAdjacentChunks(chunk);

            chunk.Build(adjacentChunks);

            foreach (var adjacentChunk in adjacentChunks.Values)
            {
                adjacentChunk.Build(GetAdjacentChunks(adjacentChunk));
            }
        }

        public void RemoveBlock(Block block)
        {
            var cx = (int)Math.Floor(block.X / (float)WorldGenerator.CHUNK_WIDTH);
            var cy = (int)Math.Floor(block.Z / (float)WorldGenerator.CHUNK_WIDTH);

            var bx = block.X - cx * WorldGenerator.CHUNK_WIDTH;
            var by = block.Y;
            var bz = block.Z - cy * WorldGenerator.CHUNK_WIDTH;

            var chunk = GetChunk(cx, cy);

            chunk.Blocks[bx, by, bz] = null;

            var adjacentChunks = GetAdjacentChunks(chunk);

            chunk.Build(adjacentChunks);

            foreach (var adjacentChunk in adjacentChunks.Values)
            {
                adjacentChunk.Build(GetAdjacentChunks(adjacentChunk));
            }
        }
    }
}
