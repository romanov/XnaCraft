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
using System.Threading.Tasks;

namespace XnaCraft
{
    public class XnaCraftGame : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager _graphics;

        private Camera _camera;
        private World _world;
        private Player _player;        
        
        private WorldGenerator _worldGenerator;
        private IWorldRenderer _worldRenderer;
        private DiagnosticsService _diagnosticsService;

        private SpriteBatch _spriteBatch;
        private Texture2D _crosshairTexture;

        private readonly bool _isFullScreen = false;

        public XnaCraftGame()
        {
            _graphics = new GraphicsDeviceManager(this);

            if (_isFullScreen)
            {
                _graphics.PreferredBackBufferWidth = 1920;
                _graphics.PreferredBackBufferHeight = 1080;
                _graphics.IsFullScreen = true;
            }
            else
            {
                _graphics.PreferredBackBufferWidth = 1280;
                _graphics.PreferredBackBufferHeight = 720;
                _graphics.IsFullScreen = false;
            }

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
            _worldGenerator = new WorldGenerator(_world, GraphicsDevice, Content, _diagnosticsService);
            _worldRenderer = new WorldRenderer(this);

            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _crosshairTexture = Content.Load<Texture2D>("crosshair");

            var startChunk = _worldGenerator.GenerateChunk(0, 0);
            var startHeight = WorldGenerator.CHUNK_HEIGHT;

            while (startChunk[WorldGenerator.CHUNK_WIDTH / 2 - 1, startHeight - 1, WorldGenerator.CHUNK_WIDTH / 2 - 1] == null)
            {
                startHeight--;
            }

            var chunk = new Chunk(GraphicsDevice, 0, 0);
            chunk.SetBlocks(startChunk);

            _world.AddChunk(0, 0, chunk);

            chunk.Build(new Dictionary<Point,Chunk>());

            _player = new Player(_world, new Vector3(WorldGenerator.CHUNK_WIDTH / 2 - 0.5f, startHeight + 1.4f, WorldGenerator.CHUNK_WIDTH / 2 - 0.5f));

            GenerateArea(false);
        }

        protected override void UnloadContent()
        {
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            _worldGenerator.StopGeneration();

            base.OnExiting(sender, args);
        }

        private KeyboardState _previousKeyboardState = new KeyboardState();
        private MouseState _previousMouseState = new MouseState();

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

                var jump = keyboardState.IsKeyDown(Keys.Space);
                var mouseOffsetX = mouseState.X - GraphicsDevice.Viewport.Width / 2;
                var mouseOffsetY = mouseState.Y - GraphicsDevice.Viewport.Height / 2;

                _camera.Update(mouseOffsetX, mouseOffsetY, _player.Position + new Vector3(0, 0.9f, 0));

                _player.Move(gameTime, moveVector * (float)gameTime.ElapsedGameTime.TotalSeconds, _camera.LeftRightRotation, jump);

                _diagnosticsService.SetInfoValue("Pos", String.Format("X = {0}, Y = {1}, Z = {2}", _player.Position.X, _player.Position.Y, _player.Position.Z));

                Mouse.SetPosition(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);

                if (_previousMouseState.RightButton == ButtonState.Pressed && mouseState.RightButton == ButtonState.Released)
                {
                    var blocks = _world.RayCast(_camera.Ray, _player.Position.ToPoint3(), 5, true);
                    var emptyBlocks = blocks.TakeWhile(x => x.IsEmpty);

                    if (emptyBlocks.Any() && blocks.Count() != emptyBlocks.Count())
                    {
                        var block = emptyBlocks.Last();

                        if (!_player.BoundingBox.Intersects(block.BoundingBox))
                        {
                            _world.AddBlock(block.X, block.Y, block.Z, BlockType.Grass);
                        }
                    }
                }

                if (_previousMouseState.LeftButton == ButtonState.Pressed && mouseState.LeftButton == ButtonState.Released)
                {
                    var block = _world.RayCast(_camera.Ray, _player.Position.ToPoint3(), 5).FirstOrDefault();

                    if (!block.IsEmpty)
                    {
                        _world.RemoveBlock(block);
                    }
                }

                _previousKeyboardState = keyboardState;
                _previousMouseState = mouseState;

                GenerateArea(true);
            }

            base.Update(gameTime);
        }

        private void GenerateArea(bool buildAdjacent)
        {
            var cx = (int)Math.Floor(_camera.Position.X / WorldGenerator.CHUNK_WIDTH);
            var cy = (int)Math.Floor(_camera.Position.Z / WorldGenerator.CHUNK_WIDTH);

            _diagnosticsService.SetInfoValue("Chunk", String.Format("X = {0}, Y = {1}", cx, cy));

            _worldGenerator.GenerateArea(new Point(cx, cy), 15, buildAdjacent);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _worldRenderer.Render(_world, _camera);

            _spriteBatch.Begin();
            _spriteBatch.Draw(_crosshairTexture, new Vector2((GraphicsDevice.Viewport.Width - _crosshairTexture.Width) / 2, (GraphicsDevice.Viewport.Height - _crosshairTexture.Height) / 2), Color.White);
            _spriteBatch.End();


            base.Draw(gameTime);
        }
    }
}
