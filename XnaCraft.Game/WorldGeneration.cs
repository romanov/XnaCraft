using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using XnaCraft.Engine;
using XnaCraft.Engine.Diagnostics;
using XnaCraft.Engine.Input;
using XnaCraft.Engine.Logic;
using XnaCraft.Engine.World;

namespace XnaCraft.Game
{
    class WorldGeneration : IInitLogic, IUpdateLogic
    {
        private readonly World _world;
        private readonly WorldGenerator _worldGenerator;
        private readonly DiagnosticsService _diagnosticsService;
        private readonly Camera _camera;
        private readonly Player _player;
        private readonly ChunkBuilder _chunkBuilder;

        public WorldGeneration(World world, WorldGenerator worldGenerator, Camera camera, Player player, ChunkBuilder chunkBuilder, DiagnosticsService diagnosticsService)
        {
            _world = world;
            _worldGenerator = worldGenerator;
            _camera = camera;
            _player = player;
            _chunkBuilder = chunkBuilder;
            _diagnosticsService = diagnosticsService;
        }

        public void OnInit()
        {
            StartWorldGeneration();
            SetPlayerStartingPosition();
        }

        public void OnUpdate(GameTime gameTime)
        {
            var cx = (int)Math.Floor(_camera.Position.X / World.ChunkWidth);
            var cy = (int)Math.Floor(_camera.Position.Z / World.ChunkWidth);

            _diagnosticsService.SetInfoValue("Chunk", String.Format("X = {0}, Y = {1}", cx, cy));

            _worldGenerator.GenerateArea(new Point(cx, cy), 15, true);
        }

        public void OnShutdown()
        {
            _worldGenerator.StopGeneration();
        }

        private void StartWorldGeneration()
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
    }
}
