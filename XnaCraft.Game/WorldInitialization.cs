using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XnaCraft.Engine;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using XnaCraft.Engine.Logic;
using XnaCraft.Engine.World;

namespace XnaCraft.Game
{
    class WorldInitialization : IInitLogic
    {
        private readonly WorldGenerator _worldGenerator;
        private readonly World _world;
        private readonly ChunkBuilder _chunkBuilder;
        private readonly Player _player;

        public WorldInitialization(WorldGenerator worldGenerator, World world, ChunkBuilder chunkBuilder, Player player)
        {
            _worldGenerator = worldGenerator;
            _world = world;
            _chunkBuilder = chunkBuilder;
            _player = player;
        }

        public void OnInit()
        {
            GenerateWorld();
            SetPlayerStartingPosition();
        }

        private void GenerateWorld()
        {
            var startChunk = _worldGenerator.GenerateChunk(0, 0);

            var chunk = new Chunk(0, 0);
            chunk.SetBlocks(startChunk);

            _world.AddChunk(0, 0, chunk);

            _chunkBuilder.Build(chunk);

            _worldGenerator.GenerateArea(new Point(0, 0), 15, true);
            _worldGenerator.StartGeneration();
        }

        private void SetPlayerStartingPosition()
        {
            var startHeight = World.ChunkHeight;
            var blocks = _world.GetChunk(0, 0).Blocks;

            while (blocks[World.ChunkWidth / 2 - 1, startHeight - 1, World.ChunkWidth / 2 - 1] == null)
            {
                startHeight--;
            }

            _player.Position = new Vector3(World.ChunkWidth / 2 - 0.5f, startHeight + 1.41f, World.ChunkWidth / 2 - 0.5f);
        }


        public void OnShutdown()
        {
            _worldGenerator.StopGeneration();
        }
    }
}
