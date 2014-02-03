using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using XnaCraft.Engine;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using XnaCraft.Engine.Framework;
using XnaCraft.Engine.Logic;
using XnaCraft.Engine.World;

namespace XnaCraft.Game
{
    class WorldRendering : IRenderLogic
    {
        private readonly IWorldRenderer _worldRenderer;
        private readonly SpriteBatch _spriteBatch;
        private readonly GraphicsDevice _graphicsDevice;
        private readonly World _world;
        private readonly Camera _camera;

        private readonly Texture2D _crosshairTexture;

        public WorldRendering(IWorldRenderer worldRenderer, SpriteBatch spriteBatch, 
            GraphicsDevice graphicsDevice, ContentManager contentManager, World world, Camera camera)
        {
            _worldRenderer = worldRenderer;
            _spriteBatch = spriteBatch;
            _graphicsDevice = graphicsDevice;
            _world = world;
            _camera = camera;

            _crosshairTexture = contentManager.Load<Texture2D>("crosshair");
        }

        public void OnRender(GameTime gameTime)
        {
            _worldRenderer.Render(_world, _camera);

            _spriteBatch.Begin();

            var crosshairPosition = new Vector2(
                (_graphicsDevice.Viewport.Width - _crosshairTexture.Width) / 2,
                (_graphicsDevice.Viewport.Height - _crosshairTexture.Height) / 2);

            _spriteBatch.Draw(_crosshairTexture, crosshairPosition, Color.White);

            _spriteBatch.End();
        }
    }
}
