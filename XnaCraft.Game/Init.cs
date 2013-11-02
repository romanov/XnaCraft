using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XnaCraft.Engine;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace XnaCraft.Game
{
    class Init : IInitLogic
    {
        private readonly WorldGenerator _worldGenerator;
        private readonly World _world;
        private readonly GraphicsDevice _graphicsDevice;
        private readonly Player _player;

        public Init(WorldGenerator worldGenerator, World world, GraphicsDevice graphicsDevice, Player player)
        {
            _worldGenerator = worldGenerator;
            _world = world;
            _graphicsDevice = graphicsDevice;
            _player = player;
        }

        public void OnInit()
        {
            GenerateWorld();

            _player.Position = GetPlayerStartingPosition();
        }

        private void GenerateWorld()
        {
            var startChunk = _worldGenerator.GenerateChunk(0, 0);

            var chunk = new Chunk(_graphicsDevice, _world, 0, 0);
            chunk.SetBlocks(startChunk);

            _world.AddChunk(0, 0, chunk);

            chunk.Build();

            _worldGenerator.GenerateArea(new Point(0, 0), 15, true);
            _worldGenerator.StartGeneration();
        }

        private Vector3 GetPlayerStartingPosition()
        {
            var startHeight = WorldGenerator.CHUNK_HEIGHT;
            var blocks = _world.GetChunk(0, 0).Blocks;

            while (blocks[WorldGenerator.CHUNK_WIDTH / 2 - 1, startHeight - 1, WorldGenerator.CHUNK_WIDTH / 2 - 1] == null)
            {
                startHeight--;
            }

            return new Vector3(WorldGenerator.CHUNK_WIDTH / 2 - 0.5f, startHeight + 1.41f, WorldGenerator.CHUNK_WIDTH / 2 - 0.5f);
        }


        public void OnShutdown()
        {
            _worldGenerator.StopGeneration();
        }
    }
}
