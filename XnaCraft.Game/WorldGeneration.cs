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
    class WorldGeneration : IUpdateLogic
    {
        private readonly WorldGenerator _worldGenerator;
        private readonly DiagnosticsService _diagnosticsService;
        private readonly Camera _camera;
        private readonly InputController _inputController;

        public WorldGeneration(WorldGenerator worldGenerator, Camera camera, DiagnosticsService diagnosticsService, InputController inputController)
        {
            _worldGenerator = worldGenerator;
            _camera = camera;
            _diagnosticsService = diagnosticsService;
            _inputController = inputController;
        }

        public void OnUpdate(GameTime gameTime)
        {
            var cx = (int)Math.Floor(_camera.Position.X / World.ChunkWidth);
            var cy = (int)Math.Floor(_camera.Position.Z / World.ChunkWidth);

            _diagnosticsService.SetInfoValue("Chunk", String.Format("X = {0}, Y = {1}", cx, cy));

            _worldGenerator.GenerateArea(new Point(cx, cy), 15, true);
        }
    }
}
