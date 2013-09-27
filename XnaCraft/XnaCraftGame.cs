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

        public XnaCraftGame()
        {
            _graphics = new GraphicsDeviceManager(this);

            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;

            Content.RootDirectory = "Content";

            IsFixedTimeStep = false;
            _graphics.SynchronizeWithVerticalRetrace = false;

            Services.AddService(typeof(DiagnosticsService), new DiagnosticsService());
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

            _world.AddChunk(0, 0, _worldGenerator.GenerateChunk(0, 0));
            _world.AddChunk(0, 1, _worldGenerator.GenerateChunk(0, 1));
            _world.AddChunk(0, 2, _worldGenerator.GenerateChunk(0, 2));
            _world.AddChunk(1, 0, _worldGenerator.GenerateChunk(1, 0));
            _world.AddChunk(1, 1, _worldGenerator.GenerateChunk(1, 1));
            _world.AddChunk(1, 2, _worldGenerator.GenerateChunk(1, 2));
            _world.AddChunk(2, 0, _worldGenerator.GenerateChunk(2, 0));
            _world.AddChunk(2, 1, _worldGenerator.GenerateChunk(2, 1));
            _world.AddChunk(2, 2, _worldGenerator.GenerateChunk(2, 2));
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

                var center = new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
                var mousePos = new Vector2(mouseState.X, mouseState.Y);
                var offset = mousePos - center;

                Mouse.SetPosition((int)center.X, (int)center.Y);

                _camera.Update(gameTime, offset.X, offset.Y, moveVector);
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
