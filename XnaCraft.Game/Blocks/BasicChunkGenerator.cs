using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XnaCraft.Engine;
using XnaCraft.Engine.Framework;
using XnaCraft.Engine.World;

namespace XnaCraft.Game.Blocks
{
    public class BasicChunkGenerator : IChunkGenerator
    {
        private readonly PerlinGenerator _perlinGenerator = new PerlinGenerator(RandomUtils.GetRandomInteger());
        private readonly BlockManager _blockManager;

        // TODO: move to configuration
        private const bool UseDebugTextures = false;

        public BasicChunkGenerator(BlockManager blockManager)
        {
            _blockManager = blockManager;
        }

        public BlockDescriptor[, ,] Generate(int cx, int cy)
        {
            const int f = 2;

            var chunk = new BlockDescriptor[World.ChunkWidth, World.ChunkHeight, World.ChunkWidth];

            for (var x = 0; x < World.ChunkWidth; x++)
            {
                for (var z = 0; z < World.ChunkWidth; z++)
                {
                    var height = World.GroundLevel + (int)(((_perlinGenerator.Noise(
                        f * (cx * World.ChunkWidth + x) / (float)64,
                        f * (cy * World.ChunkWidth + z) / (float)64, 0) + 1) / 2) * (64));

                    for (var y = 0; y < World.ChunkHeight; y++)
                    {
                        if (y <= height)
                        {
                            if (UseDebugTextures)
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
