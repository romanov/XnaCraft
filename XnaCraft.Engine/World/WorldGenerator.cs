using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using XnaCraft.Engine.Diagnostics;
using XnaCraft.Engine.Messaging;

namespace XnaCraft.Engine.World
{
    public class WorldGenerator
    {
        private readonly World _world;
        private readonly IChunkGenerator _chunkGenerator;
        private readonly ChunkBuilder _chunkBuilder;
        private readonly IEventManager _eventManager;
        private readonly DiagnosticsService _diagnosticsService;

        private readonly BlockingCollection<Batch> _batchQueue = new BlockingCollection<Batch>();
        private readonly BlockingCollection<Chunk> _chunksQueue = new BlockingCollection<Chunk>();
        private readonly List<ISubscription> _subscriptions = new List<ISubscription>();

        private volatile bool _isRunning = true;

        public WorldGenerator(World world, IChunkGenerator chunkGenerator, ChunkBuilder chunkBuilder, IEventManager eventManager, DiagnosticsService diagnosticsService)
        {
            _world = world;
            _chunkGenerator = chunkGenerator;
            _chunkBuilder = chunkBuilder;
            _eventManager = eventManager;
            _diagnosticsService = diagnosticsService;
        }

        public void StartGeneration()
        {
            _subscriptions.Add(_eventManager.Subscribe<BlockAddedEvent>(e => _chunksQueue.Add(e.Chunk)));
            _subscriptions.Add(_eventManager.Subscribe<BlockRemovedEvent>(e => _chunksQueue.Add(e.Chunk)));

            Task.Factory.StartNew(ProcessGenerationQueue);
            Task.Factory.StartNew(ProcessChunkRebuildQueue);
        }

        public void StopGeneration()
        {
            foreach (var subscription in _subscriptions)
            {
                subscription.Cancel();
            }

            _isRunning = false;
        }

        public BlockDescriptor[, ,] GenerateChunk(int cx, int cy)
        {
            return _chunkGenerator.Generate(cx, cy);
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
                    chunkPositions.AddRange(new[] {
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
                    var chunk = new Chunk(chunkPosition.X, chunkPosition.Y);

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

                    _chunkBuilder.Build(chunk);

                    if (batch.RebuildAdjacent)
                    {
                        foreach (var adjacentChunk in adjacentChunks.Values)
                        {
                            if (!lookup.Contains(adjacentChunk))
                            {
                                _chunkBuilder.Build(adjacentChunk);
                            }
                        }
                    }

                    _diagnosticsService.SetInfoValue("Queue", --queueLength);
                }
            }
        }

        private void ProcessChunkRebuildQueue()
        {
            while (_isRunning)
            {
                var chunk = _chunksQueue.Take();

                _chunkBuilder.Build(chunk);

                var adjacentChunks = _world.GetAdjacentChunks(chunk);

                foreach (var adjacentChunk in adjacentChunks.Values)
                {
                    _chunkBuilder.Build(adjacentChunk);
                }
            }
        }

        private class Batch
        {
            public List<Chunk> Chunks { get; set; }
            public bool RebuildAdjacent { get; set; }
        }
    }
}
