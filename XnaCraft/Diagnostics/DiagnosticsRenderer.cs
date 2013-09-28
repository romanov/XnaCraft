using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace XnaCraft.Diagnostics
{
    // based on http://blogs.msdn.com/b/shawnhar/archive/2007/06/08/displaying-the-framerate.aspx
    public class DiagnosticsRenderer : DrawableGameComponent
    {
        private SpriteBatch _spriteBatch;
        private SpriteFont _spriteFont;

        private int _frameRate = 0;
        private int _frameCounter = 0;
        private TimeSpan _elapsedTime = TimeSpan.Zero;


        private readonly DiagnosticsService _diagnosticsService;

        public DiagnosticsRenderer(Game game)
            : base(game)
        {
            _diagnosticsService = (DiagnosticsService)game.Services.GetService(typeof(DiagnosticsService));
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _spriteFont = Game.Content.Load<SpriteFont>("Font");
        }

        public override void Update(GameTime gameTime)
        {
            _elapsedTime += gameTime.ElapsedGameTime;

            if (_elapsedTime > TimeSpan.FromSeconds(1))
            {
                _elapsedTime -= TimeSpan.FromSeconds(1);
                _frameRate = _frameCounter;
                _frameCounter = 0;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            _frameCounter++;



            _spriteBatch.Begin();


            var sb = new StringBuilder();

            sb.AppendFormat("FPS: {0}", _frameRate);

            foreach (var counter in _diagnosticsService.GetInfoValues())
            {
                sb.AppendLine().AppendFormat("{0}: {1}", counter.Key, counter.Value);
            }

            _spriteBatch.DrawString(_spriteFont, sb, new Vector2(5, 5), Color.White);

            _spriteBatch.End();
        }
    }
}
