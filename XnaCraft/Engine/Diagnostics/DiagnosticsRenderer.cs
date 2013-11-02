using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using XnaCraft.Engine;

namespace XnaCraft.Engine.Diagnostics
{
    class DiagnosticsRenderer : IRenderLogic
    {
        private readonly DiagnosticsService _diagnosticsService;
        private readonly SpriteBatch _spriteBatch;
        private readonly SpriteFont _spriteFont;

        public DiagnosticsRenderer(SpriteBatch spriteBatch, DiagnosticsService diagnosticsService, ContentManager contentManager)
        {
            _diagnosticsService = diagnosticsService;
            _spriteBatch = spriteBatch;

            _spriteFont = contentManager.Load<SpriteFont>("Font");
        }

        public void OnRender(GameTime gameTime)
        {
            _spriteBatch.Begin();

            var sb = new StringBuilder();

            foreach (var counter in _diagnosticsService.GetInfoValues())
            {
                sb.AppendFormat("{0}: {1}", counter.Key, counter.Value).AppendLine();
            }

            _spriteBatch.DrawString(_spriteFont, sb, new Vector2(5, 5), Color.White);

            _spriteBatch.End();
        }
    }
}
