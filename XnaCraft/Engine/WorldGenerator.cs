using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace XnaCraft.Engine
{
    class WorldGenerator
    {
        public const int CHUNK_SIZE = 16;

        private readonly BlockDescriptor _grassDescriptor;
        private readonly BlockDescriptor _dirtDescriptor;

        private readonly PerlinGenerator _perlinGenerator = new PerlinGenerator(Utils.GetRandomInteger());

        public WorldGenerator(ContentManager contentManager)
        {
            _grassDescriptor = new BlockDescriptor(BlockType.Grass,
                BlockFaceTexture.GrassTop,
                BlockFaceTexture.Dirt,
                BlockFaceTexture.GrassSide);

            _dirtDescriptor = new BlockDescriptor(BlockType.Dirt,
                BlockFaceTexture.Dirt,
                BlockFaceTexture.Dirt,
                BlockFaceTexture.Dirt);
        }

        public BlockDescriptor[, ,] GenerateChunk(int cx, int cy)
        {
            var f = 1;

            var chunk = new BlockDescriptor[CHUNK_SIZE, CHUNK_SIZE, CHUNK_SIZE];

            for (var x = 0; x < CHUNK_SIZE; x++)
            {
                for (var z = 0; z < CHUNK_SIZE; z++)
                {
                    var height = (int)(((_perlinGenerator.Noise(
                        f * (cx * CHUNK_SIZE + x) / (float)CHUNK_SIZE,
                        f * (cy * CHUNK_SIZE + z) / (float)CHUNK_SIZE, 0) + 1) / 2) * CHUNK_SIZE);

                    for (var y = 0; y < CHUNK_SIZE; y++)
                    {
                        if (y <= height)
                        {
                            chunk[x, y, z] = y == height ? _grassDescriptor : _dirtDescriptor;
                        }
                    }
                }
            }

            return chunk;
        }
    }
}
