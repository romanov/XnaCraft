using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using XnaCraft.Engine.Framework;
using XnaCraft.Engine.Messaging;

namespace XnaCraft.Engine.World
{
    public class World
    {
        private readonly BlockManager _blockManager;
        private readonly IEventManager _eventManager;
        private readonly Dictionary<Point, Chunk> _chunks = new Dictionary<Point, Chunk>();

        public const int ChunkWidth = 16;
        public const int ChunkHeight = 256;
        public const int GroundLevel = 128;

        public World(BlockManager blockManager, IEventManager eventManager)
        {
            _blockManager = blockManager;
            _eventManager = eventManager;
        }

        public Chunk GetChunk(int x, int y)
        {
            Chunk chunk;

            if (_chunks.TryGetValue(new Point(x, y), out chunk))
            {
                return chunk.IsGenerated ? chunk : null;
            }
                
            return null;
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

        public BlockDescriptor GetBlock(Point3 position)
        {
            var cx = (int)Math.Floor(position.X / (float)ChunkWidth);
            var cy = (int)Math.Floor(position.Z / (float)ChunkWidth);

            var bx = position.X - cx * ChunkWidth;
            var by = position.Y;
            var bz = position.Z - cy * ChunkWidth;

            if (by >= 0 && by < ChunkHeight)
            {
                var chunk = GetChunk(cx, cy);

                if (chunk != null)
                {
                    var block = chunk.Blocks[bx, by, bz];

                    if (block != null)
                    {
                        return block;
                    }
                }
            }

            return null;
        }

        private IEnumerable<Block> GetBlockRange(Point3 min, Point3 max, bool returnEmptyBlocks = false)
        {
            for (var x = min.X; x <= max.X; x++)
            {
                for (var y = min.Y; y <= max.Y; y++)
                {
                    for (var z = min.Z; z <= max.Z; z++)
                    {
                        var cx = (int)Math.Floor(x / (float)ChunkWidth);
                        var cy = (int)Math.Floor(z / (float)ChunkWidth);

                        var bx = x - cx * ChunkWidth;
                        var by = y;
                        var bz = z - cy * ChunkWidth;

                        if (by >= 0 && by < ChunkHeight)
                        {
                            var chunk = GetChunk(cx, cy);

                            if (chunk != null)
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

        public IList<Chunk> GetVisibleChunks(Camera camera)
        {
            var viewFrustrum = new BoundingFrustum(camera.View * camera.Projection);

            var ccx = (int)Math.Floor(camera.Position.X / ChunkWidth);
            var ccy = (int)Math.Floor(camera.Position.Z / ChunkWidth);
            var radius = 14;

            var chunks = new List<Chunk>();
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
            var cx = (int)Math.Floor(x / (float)ChunkWidth);
            var cy = (int)Math.Floor(z / (float)ChunkWidth);

            var bx = x - cx * ChunkWidth;
            var by = y;
            var bz = z - cy * ChunkWidth;

            var chunk = GetChunk(cx, cy);
            var blockDescriptor = _blockManager.GetDescriptor(blockType);

            chunk.SetBlock(bx, by, bz, blockDescriptor);

            _eventManager.Publish(new BlockAddedEvent { Chunk = chunk });
        }

        public void RemoveBlock(Block block)
        {
            var cx = (int)Math.Floor(block.X / (float)ChunkWidth);
            var cy = (int)Math.Floor(block.Z / (float)ChunkWidth);

            var bx = block.X - cx * ChunkWidth;
            var by = block.Y;
            var bz = block.Z - cy * ChunkWidth;

            var chunk = GetChunk(cx, cy);

            chunk.SetBlock(bx, by, bz, null);

            _eventManager.Publish(new BlockRemovedEvent { Chunk = chunk });
        }
    }

    public class BlockAddedEvent : IEvent
    {
        public Chunk Chunk { get; set; }
    }

    public class BlockRemovedEvent : IEvent
    {
        public Chunk Chunk { get; set; }
    }
}
