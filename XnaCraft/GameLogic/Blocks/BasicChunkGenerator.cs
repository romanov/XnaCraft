using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XnaCraft.Engine;

namespace XnaCraft.GameLogic.Blocks
{
    class BasicChunkGenerator : IChunkGenerator
    {
        private readonly PerlinGenerator _perlinGenerator = new PerlinGenerator(Utils.GetRandomInteger());
        private readonly BlockManager _blockManager;

        private readonly bool _useDebugTextures = false;

        public BasicChunkGenerator(BlockManager blockManager)
        {
            _blockManager = blockManager;
        }

        public BlockDescriptor[, ,] Generate(int cx, int cy)
        {
            var f = 2;
            var chunk = new BlockDescriptor[WorldGenerator.CHUNK_WIDTH, WorldGenerator.CHUNK_HEIGHT, WorldGenerator.CHUNK_WIDTH];

            for (var x = 0; x < WorldGenerator.CHUNK_WIDTH; x++)
            {
                for (var z = 0; z < WorldGenerator.CHUNK_WIDTH; z++)
                {
                    var height = WorldGenerator.GRUNT_LEVEL + (int)(((_perlinGenerator.Noise(
                        f * (cx * WorldGenerator.CHUNK_WIDTH + x) / (float)64,
                        f * (cy * WorldGenerator.CHUNK_WIDTH + z) / (float)64, 0) + 1) / 2) * (64));

                    for (var y = 0; y < WorldGenerator.CHUNK_HEIGHT; y++)
                    {
                        if (y <= height)
                        {
                            if (_useDebugTextures)
                            {
                                chunk[x, y, z] = _blockManager.GetDescriptor(BlockTypes.Debug);
                            }
                            else
                            {
                                chunk[x, y, z] = y == height ? _blockManager.GetDescriptor(BlockTypes.Grass) : _blockManager.GetDescriptor(BlockTypes.Dirt);
                            }
                        }
                    }
                }
            }

            return chunk;
        }
    }
}
