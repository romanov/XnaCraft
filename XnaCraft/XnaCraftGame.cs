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

            _world = new World();
            _player = new Player(_world);
            _camera = new Camera(GraphicsDevice);

            GenerateWorld();

            _player.Position = GetPlayerStartingPosition();

            _inputController = new InputController(this, _world, _camera, _player);
            _worldRenderer = new WorldRenderer(this);
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _crosshairTexture = Content.Load<Texture2D>("crosshair");
        }

        protected override void OnActivated(object sender, EventArgs args)
        {
            Mouse.SetPosition(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);

            base.OnActivated(sender, args);
        }

        private void GenerateWorld()
        {
            _worldGenerator = new WorldGenerator(_world, GraphicsDevice, Content, _diagnosticsService);

            var startChunk = _worldGenerator.GenerateChunk(0, 0);

            var chunk = new Chunk(GraphicsDevice, _world, 0, 0);
            chunk.SetBlocks(startChunk);

            _world.AddChunk(0, 0, chunk);

            chunk.Build();

            _worldGenerator.GenerateArea(new Point(0, 0), 15, true);
            _worldGenerator.StartGeneration();
        }

        private Vector3 GetPlayerStartingPosition()
        {
            var startHeight = WorldGenerator.CHUNK_HEIGHT;
            var blocks = _world.GetChunk(0, 0).Blocks;

            while (blocks[WorldGenerator.CHUNK_WIDTH / 2 - 1, startHeight - 1, WorldGenerator.CHUNK_WIDTH / 2 - 1] == null)
            {
                startHeight--;
            }

            return new Vector3(WorldGenerator.CHUNK_WIDTH / 2 - 0.5f, startHeight + 1.41f, WorldGenerator.CHUNK_WIDTH / 2 - 0.5f);
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

                var cx = (int)Math.Floor(_camera.Position.X / WorldGenerator.CHUNK_WIDTH);
                var cy = (int)Math.Floor(_camera.Position.Z / WorldGenerator.CHUNK_WIDTH);

                _diagnosticsService.SetInfoValue("Chunk", String.Format("X = {0}, Y = {1}", cx, cy));

                _worldGenerator.GenerateArea(new Point(cx, cy), 15, true);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _worldRenderer.Render(_world, _camera);

            _spriteBatch.Begin();

            var crosshairPosition = new Vector2(
                (GraphicsDevice.Viewport.Width - _crosshairTexture.Width) / 2, 
                (GraphicsDevice.Viewport.Height - _crosshairTexture.Height) / 2);

            _spriteBatch.Draw(_crosshairTexture, crosshairPosition, Color.White);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
