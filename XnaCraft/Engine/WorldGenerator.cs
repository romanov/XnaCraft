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

        private readonly BlockingCollection<QueueItem> _chunksToGenerate = new BlockingCollection<QueueItem>();
        private readonly List<Chunk> _chunksToEnqueue = new List<Chunk>();

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
            var chunksToGenerate = new List<Chunk>();

            _chunksToEnqueue.Clear();

            TryEnqueueChunk(_chunksToEnqueue, center.X, center.Y);

            for (var r = 0; r <= radius; r++)
            {
                for (var i = r; i > -r; i--)
                {
                    TryEnqueueChunk(_chunksToEnqueue, center.X - i, center.Y + r);
                    TryEnqueueChunk(_chunksToEnqueue, center.X + r, center.Y + i);
                    TryEnqueueChunk(_chunksToEnqueue, center.X + i, center.Y - r);
                    TryEnqueueChunk(_chunksToEnqueue, center.X - r, center.Y - i);
                }
            }

            foreach (var chunk in _chunksToEnqueue)
            {
                _chunksToGenerate.Add(new QueueItem { Chunk = chunk, BuildAdjacent = buildAdjacent });
            }
        }

        private void TryEnqueueChunk(List<Chunk> enqueuedChunks, int x, int y)
        {
            if (!_world.HasChunk(x, y))
            {
                var chunk = new Chunk(_graphicsDevice, x, y);

                _world.AddChunk(x, y, chunk);

                enqueuedChunks.Add(chunk);
            }
        }

        private void ProcessGenerationQueue()
        {
            while (_isRunning)
            {
                var item = _chunksToGenerate.Take();

                _diagnosticsService.SetInfoValue("Queue", _chunksToGenerate.Count);

                var chunk = item.Chunk;

                if (!chunk.IsGenerated)
                {
                    var blocks = GenerateChunk(chunk.X, chunk.Y);
                    chunk.SetBlocks(blocks);
                }

                var adjacentChunks = _world.GetAdjacentChunks(chunk);

                if (adjacentChunks.Values.All(c => c.IsGenerated))
                {
                    chunk.Build(adjacentChunks);

                    if (item.BuildAdjacent)
                    {
                        foreach (var adjacentChunk in adjacentChunks.Values)
                        {
                            _chunksToGenerate.Add(new QueueItem { Chunk = adjacentChunk, BuildAdjacent = false });
                        }
                    }
                }
                else
                {
                    _chunksToGenerate.Add(item);
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

        private class QueueItem
        {
            public Chunk Chunk { get; set; }
            public bool BuildAdjacent { get; set; }
        }
    }
}
