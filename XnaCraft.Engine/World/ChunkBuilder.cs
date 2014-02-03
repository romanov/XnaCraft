using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XnaCraft.Engine.Framework;

namespace XnaCraft.Engine.World
{
    public class ChunkBuilder
    {
        private readonly World _world;
        private readonly Func<IChunkVertexBuilder> _builderFactory;

        public ChunkBuilder(World world, Func<IChunkVertexBuilder> builderFactory)
        {
            _world = world;
            _builderFactory = builderFactory;
        }

        public void Build(Chunk chunk)
        {
            var builder = _builderFactory();

            for (var x = 0; x < World.ChunkWidth; x++)
            {
                for (var y = 0; y < World.ChunkHeight; y++)
                {
                    for (var z = 0; z < World.ChunkWidth; z++)
                    {
                        BuildChunk(chunk, builder, x, y, z);
                    }
                }
            }

            chunk.SetVertexBuffer(builder.Build());
        }

        private void BuildChunk(Chunk chunk, IChunkVertexBuilder builder, int x, int y, int z)
        {
            var descriptor = chunk.Blocks[x, y, z];

            if (descriptor != null)
            {
                var position = new Point3(chunk.X * World.ChunkWidth + x, y, chunk.Y * World.ChunkWidth + z);

                var top = y == World.ChunkHeight - 1 || chunk.Blocks[x, y + 1, z] == null;
                var bottom = y != 0 && chunk.Blocks[x, y - 1, z] == null;
                var front = (z > 0 ? chunk.Blocks[x, y, z - 1] : _world.GetBlock(position + new Point3(0, 0, -1))) == null;
                var back = (z < World.ChunkWidth - 1 ? chunk.Blocks[x, y, z + 1] : _world.GetBlock(position + new Point3(0, 0, 1))) == null;
                var left = (x > 0 ? chunk.Blocks[x - 1, y, z] : _world.GetBlock(position + new Point3(-1, 0, 0))) == null;
                var right = (x < World.ChunkWidth - 1 ? chunk.Blocks[x + 1, y, z] : _world.GetBlock(position + new Point3(1, 0, 0))) == null;

                if (top || bottom || front || back || left || right)
                {
                    builder.BeginBlock(position.ToVector3(), descriptor, GetBlockNeighbours(position));

                    if (top)
                    {
                        builder.AddTopFace();
                    }
                    if (bottom)
                    {
                        builder.AddBottomFace();
                    }
                    if (front)
                    {
                        builder.AddFrontFace();
                    }
                    if (back)
                    {
                        builder.AddBackFace();
                    }
                    if (left)
                    {
                        builder.AddLeftFace();
                    }
                    if (right)
                    {
                        builder.AddRightFace();
                    }
                }
            }
        }

        private int[, ,] GetBlockNeighbours(Point3 blockPosition)
        {
            var neighbours = new int[3, 3, 3];

            var start = blockPosition - 1;

            for (var x = 0; x < 3; x++)
            {
                for (var y = 0; y < 3; y++)
                {
                    for (var z = 0; z < 3; z++)
                    {
                        neighbours[x, y, z] = _world.GetBlock(start + new Point3(x, y, z)) == null ? 0 : 1;
                    }
                }
            }

            return neighbours;
        }
    }
}
