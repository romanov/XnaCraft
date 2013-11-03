using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XnaCraft.Engine;
using XnaCraft.Engine.Utils;
using XnaCraft.Engine.World;

namespace XnaCraft.Game.Blocks
{
    public class BasicChunkGenerator : IChunkGenerator
    {
        private readonly PerlinGenerator _perlinGenerator = new PerlinGenerator(RandomUtils.GetRandomInteger());
        private readonly BlockManager _blockManager;

        private readonly bool _useDebugTextures = false;

        public BasicChunkGenerator(BlockManager blockManager)
        {
            _blockManager = blockManager;
        }

        public BlockDescriptor[, ,] Generate(int cx, int cy)
        {
            var f = 2;
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
