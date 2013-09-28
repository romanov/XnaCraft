using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using XnaCraft.Diagnostics;
using XnaCraft.Engine;

namespace XnaCraft
{
    public class XnaCraftGame : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager _graphics;

        private Camera _camera;

        private World _world;
        private WorldGenerator _worldGenerator;
        private WorldRenderer _worldRenderer;
        private DiagnosticsService _diagnosticsService;

        public XnaCraftGame()
        {
            _graphics = new GraphicsDeviceManager(this);

            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;

            Content.RootDirectory = "Content";

#if DEBUG
            IsFixedTimeStep = false;
            _graphics.SynchronizeWithVerticalRetrace = false;
#endif

            _diagnosticsService = new DiagnosticsService();
            Services.AddService(typeof(DiagnosticsService), _diagnosticsService);
            Components.Add(new DiagnosticsRenderer(this));
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void OnActivated(object sender, EventArgs args)
        {
            Mouse.SetPosition(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);

            base.OnActivated(sender, args);
        }

        protected override void LoadContent()
        {
            _camera = new Camera(GraphicsDevice);

            _world = new World();
            _worldGenerator = new WorldGenerator(Content);
            _worldRenderer = new WorldRenderer(this, GraphicsDevice);
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            if (IsActive)
            {
                var keyboardState = Keyboard.GetState();
                var mouseState = Mouse.GetState();

                var moveVector = Vector3.Zero;

                if (keyboardState.IsKeyDown(Keys.W))
                {
                    moveVector.Z = -1;
                }
                if (keyboardState.IsKeyDown(Keys.S))
                {
                    moveVector.Z = 1;
                }
                if (keyboardState.IsKeyDown(Keys.A))
                {
                    moveVector.X = -1;
                }
                if (keyboardState.IsKeyDown(Keys.D))
                {
                    moveVector.X = 1;
                }
                if (keyboardState.IsKeyDown(Keys.LeftShift))
                {
                    moveVector.Y = 1;
                }
                if (keyboardState.IsKeyDown(Keys.LeftControl))
                {
                    moveVector.Y = -1;
                }

                var mouseOffsetX = mouseState.X - GraphicsDevice.Viewport.Width / 2;
                var mouseOffsetY = mouseState.Y - GraphicsDevice.Viewport.Height / 2;

                _camera.Update(gameTime, mouseOffsetX, mouseOffsetY, moveVector);

                Mouse.SetPosition(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);

                var cx = (int)Math.Floor(_camera.Position.X / WorldGenerator.CHUNK_SIZE);
                var cy = (int)Math.Floor(_camera.Position.Z / WorldGenerator.CHUNK_SIZE);

                _diagnosticsService.SetInfoValue("Chunk", String.Format("X = {0}, Y = {1}", cx, cy));

                var radius = 5;

                for (var x = cx - radius; x <= cx + radius; x++)
                {
                    for (var y = cy - radius; y <= cy + radius; y++)
                    {
                        if (!_world.HasChunk(x, y))
                        {
                            _world.AddChunk(x, y, _worldGenerator.GenerateChunk(x, y));
                        }
                    }
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _worldRenderer.Render(_world, _camera);

            base.Draw(gameTime);
        }
    }
}
