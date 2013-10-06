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
using XnaCraft.Engine.Input;

namespace XnaCraft
{
    public class XnaCraftGame : Game
    {
        private GraphicsDeviceManager _graphics;

        private Camera _camera;
        private World _world;
        private Player _player;        
        
        private WorldGenerator _worldGenerator;
        private IWorldRenderer _worldRenderer;
        private InputController _inputController;
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

            var chunk = new Chunk(GraphicsDevice, _world, 0, 0);
            chunk.SetBlocks(startChunk);

            _world.AddChunk(0, 0, chunk);

            chunk.Build();

            _player = new Player(_world, new Vector3(WorldGenerator.CHUNK_WIDTH / 2 - 0.5f, startHeight + 1.41f, WorldGenerator.CHUNK_WIDTH / 2 - 0.5f));

            _inputController = new InputController(this, _world, _camera, _player);

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

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            if (IsActive)
            {
                _inputController.Update(gameTime);

                GenerateArea(true);
            }

            base.Update(gameTime);
        }

        private void GenerateArea(bool rebuildAdjacent)
        {
            var cx = (int)Math.Floor(_camera.Position.X / WorldGenerator.CHUNK_WIDTH);
            var cy = (int)Math.Floor(_camera.Position.Z / WorldGenerator.CHUNK_WIDTH);

            _diagnosticsService.SetInfoValue("Chunk", String.Format("X = {0}, Y = {1}", cx, cy));

            _worldGenerator.GenerateArea(new Point(cx, cy), 15, rebuildAdjacent);
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
