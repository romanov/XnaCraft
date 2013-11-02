using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace XnaCraft.Engine.Diagnostics
{
    class FrameCounter : IUpdateLogic, IRenderLogic
    {
        private int _frameCounter = 0;
        private TimeSpan _elapsedTime = TimeSpan.Zero;

        private readonly DiagnosticsService _diagnosticsService;

        public FrameCounter(DiagnosticsService diagnosticsService)
        {
            _diagnosticsService = diagnosticsService;

            _diagnosticsService.SetInfoValue("FPS", 0);
        }

        public void OnUpdate(GameTime gameTime)
        {
            _elapsedTime += gameTime.ElapsedGameTime;

            if (_elapsedTime > TimeSpan.FromSeconds(1))
            {
                _elapsedTime -= TimeSpan.FromSeconds(1);
                _diagnosticsService.SetInfoValue("FPS", _frameCounter);
                _frameCounter = 0;
            }
        }

        public void OnRender(GameTime gameTime)
        {
            _frameCounter++;
        }
    }
}
