using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using XnaCraft.Engine.Diagnostics;

namespace XnaCraft.Engine
{
    public class WorldGenerator
    {
        public const int CHUNK_WIDTH = 16;
        public const int CHUNK_HEIGHT = 256;
        public const int GRUNT_LEVEL = 128;

        private readonly World _world;
        private readonly GraphicsDevice _graphicsDevice;
        private readonly IChunkGenerator _chunkGenerator;
        private readonly DiagnosticsService _diagnosticsService;


        private readonly BlockingCollection<Batch> _batchQueue = new BlockingCollection<Batch>();

        private volatile bool _isRunning = true;

        public WorldGenerator(World world, GraphicsDevice graphicsDevice, IChunkGenerator chunkGenerator, DiagnosticsService diagnosticsService)
        {
            _world = world;
            _graphicsDevice = graphicsDevice;
            _chunkGenerator = chunkGenerator;
            _diagnosticsService = diagnosticsService;
        }

        public void StartGeneration()
        {
            Task.Factory.StartNew(ProcessGenerationQueue);
        }

        public void StopGeneration()
        {
            _isRunning = false;
        }

        public void GenerateArea(Point center, int radius, bool rebuildAdjacent = true)
        {
            var chunks = new List<Chunk>();
            var chunkPositions = new List<Point>(radius * 8);

            chunks.AddRange(CreateChunkGroup(new[] { center }));

            for (var r = 0; r <= radius; r++)
            {
                for (var i = r; i > -r; i--)
                {
                    chunkPositions.AddRange(new [] {
                        new Point(center.X - i, center.Y + r),
                        new Point(center.X + r, center.Y + i),
                        new Point(center.X + i, center.Y - r),
                        new Point(center.X - r, center.Y - i),
                    }); 
                }

                chunks.AddRange(CreateChunkGroup(chunkPositions));
                chunkPositions.Clear();
            }

            if (chunks.Any())
            {
                _batchQueue.Add(new Batch { Chunks = chunks, RebuildAdjacent = rebuildAdjacent });
            }
        }

        private IEnumerable<Chunk> CreateChunkGroup(IEnumerable<Point> chunkPositions)
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

                    if (batch.RebuildAdjacent)
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
            return _chunkGenerator.Generate(cx, cy);
        }

        private class Batch
        {
            public List<Chunk> Chunks { get; set; }
            public bool RebuildAdjacent { get; set; }
        }
    }
}
