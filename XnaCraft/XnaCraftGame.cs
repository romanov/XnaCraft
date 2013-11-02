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
using XnaCraft.Engine.Diagnostics;
using XnaCraft.Engine;
using System.Threading.Tasks;
using XnaCraft.Engine.Input;
using Autofac;
using System.Reflection;

namespace XnaCraft
{
    public class XnaCraftGame : Game
    {
        private readonly bool _isFullScreen = false;

        private IEnumerable<IInitLogic> _initLogicScripts;
        private IEnumerable<IUpdateLogic> _updateLogicScripts;
        private IEnumerable<IRenderLogic> _renderLogicScripts;

        public XnaCraftGame()
        {
            var graphics = new GraphicsDeviceManager(this);

            if (_isFullScreen)
            {
                graphics.PreferredBackBufferWidth = 1920;
                graphics.PreferredBackBufferHeight = 1080;
                graphics.IsFullScreen = true;
            }
            else
            {
                graphics.PreferredBackBufferWidth = 1280;
                graphics.PreferredBackBufferHeight = 720;
                graphics.IsFullScreen = false;
            }

            Content.RootDirectory = "Content";

#if DEBUG
            IsFixedTimeStep = false;
            graphics.SynchronizeWithVerticalRetrace = false;
#endif
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            var builder = new ContainerBuilder();

            builder.RegisterInstance(this).As<Game>();
            builder.RegisterInstance(GraphicsDevice).As<GraphicsDevice>();
            builder.RegisterInstance(Content).As<ContentManager>();
            builder.Register(ctx => new SpriteBatch(GraphicsDevice)).As<SpriteBatch>();

            builder.RegisterType<World>().SingleInstance();
            builder.RegisterType<Player>().SingleInstance();
            builder.RegisterType<Camera>().SingleInstance();
            builder.RegisterType<BlockManager>().SingleInstance();
            builder.RegisterType<InputController>().SingleInstance();
            builder.RegisterType<WorldRenderer>().SingleInstance();
            builder.RegisterType<DiagnosticsService>().SingleInstance();
            builder.RegisterType<WorldGenerator>().SingleInstance();

            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .AssignableTo<ILogic>()
                .As<ILogic>()
                .SingleInstance();

            var container = builder.Build();

            var logicScripts = container.Resolve<IEnumerable<ILogic>>();

            _initLogicScripts = logicScripts.Where(s => typeof(IInitLogic).IsAssignableFrom(s.GetType())).Cast<IInitLogic>().ToArray();
            _updateLogicScripts = logicScripts.Where(s => typeof(IUpdateLogic).IsAssignableFrom(s.GetType())).Cast<IUpdateLogic>().ToArray();
            _renderLogicScripts = logicScripts.Where(s => typeof(IRenderLogic).IsAssignableFrom(s.GetType())).Cast<IRenderLogic>().ToArray();

            foreach (var script in _initLogicScripts)
            {
                script.OnInit();
            }
        }

        protected override void OnActivated(object sender, EventArgs args)
        {
            Mouse.SetPosition(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);

            base.OnActivated(sender, args);
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            foreach (var script in _initLogicScripts)
            {
                script.OnShutdown();
            }

            base.OnExiting(sender, args);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                this.Exit();
            }

            if (IsActive)
            {
                foreach (var script in _updateLogicScripts)
                {
                    script.OnUpdate(gameTime);
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            foreach (var script in _renderLogicScripts)
            {
                script.OnRender(gameTime);
            }

            base.Draw(gameTime);
        }
    }
}
