using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using XnaCraft.Diagnostics;

namespace XnaCraft.Engine
{
    class WorldGenerator
    {
        public const int CHUNK_WIDTH = 16;
        public const int CHUNK_HEIGHT = 256;
        public const int GRUNT_LEVEL = 128;

        private readonly BlockDescriptor _grassDescriptor;
        private readonly BlockDescriptor _dirtDescriptor;
        private readonly BlockDescriptor _debugDescriptor;

        private readonly World _world;
        private readonly GraphicsDevice _graphicsDevice;
        private readonly DiagnosticsService _diagnosticsService;
        private readonly PerlinGenerator _perlinGenerator = new PerlinGenerator(Utils.GetRandomInteger());

        private readonly bool _useDebugTextures = false;

        private readonly BlockingCollection<Batch> _batchQueue = new BlockingCollection<Batch>();

        private volatile bool _isRunning = true;

        public WorldGenerator(World world, GraphicsDevice graphicsDevice, ContentManager contentManager, DiagnosticsService diagnosticsService)
        {
            _world = world;
            _graphicsDevice = graphicsDevice;
            _diagnosticsService = diagnosticsService;

            _grassDescriptor = new BlockDescriptor(BlockType.Grass,
                BlockFaceTexture.GrassTop,
                BlockFaceTexture.Dirt,
                BlockFaceTexture.GrassSide);

            _dirtDescriptor = new BlockDescriptor(BlockType.Dirt,
                BlockFaceTexture.Dirt,
                BlockFaceTexture.Dirt,
                BlockFaceTexture.Dirt);

            _debugDescriptor = new BlockDescriptor(BlockType.Dirt,
                BlockFaceTexture.DebugTop,
                BlockFaceTexture.DebugBottom,
                BlockFaceTexture.DebugFront,
                BlockFaceTexture.DebugBack,
                BlockFaceTexture.DebugLeft,
                BlockFaceTexture.DebugRight);

            Task.Factory.StartNew(ProcessGenerationQueue);
        }

        public void StopGeneration()
        {
            _isRunning = false;
        }

        public void GenerateArea(Point center, int radius, bool buildAdjacent = true)
        {
            var chunks = new List<Chunk>();

            chunks.AddRange(CreateChunkGroup(new[] { center }));

            for (var r = 0; r <= radius; r++)
            {
                var chunkPositions = new Point[r * 8];

                for (var i = r; i > -r; i--)
                {
                    chunkPositions[(r - i) * 4 + 0] = new Point(center.X - i, center.Y + r);
                    chunkPositions[(r - i) * 4 + 1] = new Point(center.X + r, center.Y + i);
                    chunkPositions[(r - i) * 4 + 2] = new Point(center.X + i, center.Y - r);
                    chunkPositions[(r - i) * 4 + 3] = new Point(center.X - r, center.Y - i);
                }

                chunks.AddRange(CreateChunkGroup(chunkPositions));
            }

            _batchQueue.Add(new Batch { Chunks = chunks, BuildAdjacent = buildAdjacent });
        }

        private IEnumerable<Chunk> CreateChunkGroup(Point[] chunkPositions)
        {
            foreach (var chunkPosition in chunkPositions)
            {
                if (!_world.HasChunk(chunkPosition.X, chunkPosition.Y))
                {
                    var chunk = new Chunk(_graphicsDevice, _world, chunkPosition.X, chunkPosition.Y);

                    _world.AddChunk(chunk.X, chunk.Y, chunk);

                    yield return chunk;   
                }
            }
        }

        private void ProcessGenerationQueue()
        {
            while (_isRunning)
            {
                var batch = _batchQueue.Take();

                var queueLength = 2 * (batch.Chunks.Count + _batchQueue.Sum(x => x.Chunks.Count));
                _diagnosticsService.SetInfoValue("Queue", queueLength);

                foreach (var chunk in batch.Chunks)
                {
                    var blocks = GenerateChunk(chunk.X, chunk.Y);
                    chunk.SetBlocks(blocks);

                    _diagnosticsService.SetInfoValue("Queue", --queueLength);
                }

                var lookup = new HashSet<Chunk>(batch.Chunks);

                foreach (var chunk in batch.Chunks)
                {
                    var adjacentChunks = _world.GetAdjacentChunks(chunk);

                    chunk.Build();

                    if (batch.BuildAdjacent)
                    {
                        foreach (var adjacentChunk in adjacentChunks.Values)
                        {
                            if (!batch.Chunks.Contains(adjacentChunk))
                            {
                                adjacentChunk.Build();
                            }
                        }
                    }

                    _diagnosticsService.SetInfoValue("Queue", --queueLength);
                }
            }
        }

        public BlockDescriptor[, ,] GenerateChunk(int cx, int cy)
        {
            var f = 2;

            var chunk = new BlockDescriptor[CHUNK_WIDTH, CHUNK_HEIGHT, CHUNK_WIDTH];

            for (var x = 0; x < CHUNK_WIDTH; x++)
            {
                for (var z = 0; z < CHUNK_WIDTH; z++)
                {
                    var height = GRUNT_LEVEL + (int)(((_perlinGenerator.Noise(
                        f * (cx * CHUNK_WIDTH + x) / (float)64,
                        f * (cy * CHUNK_WIDTH + z) / (float)64, 0) + 1) / 2) * (64));

                    //var height = (x * z % 3) + 3;

                    for (var y = 0; y < CHUNK_HEIGHT; y++)
                    {
                        if (y <= height)
                        {
                            if (_useDebugTextures)
                            {
                                chunk[x, y, z] = _debugDescriptor;
                            }
                            else
                            {
                                chunk[x, y, z] = y == height ? _grassDescriptor : _dirtDescriptor;
                            }
                        }
                    }
                }
            }

            return chunk;
        }

        private class Batch
        {
            public List<Chunk> Chunks { get; set; }
            public bool BuildAdjacent { get; set; }
        }
    }
}
