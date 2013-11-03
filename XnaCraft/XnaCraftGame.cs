using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using XnaCraft.Engine.Diagnostics;
using XnaCraft.Engine;
using XnaCraft.Engine.Input;
using Autofac;
using System.Reflection;
using XnaCraft.Engine.Logic;
using XnaCraft.Engine.World;
using XnaCraft.Game;
using XnaCraft.Game.Blocks;

namespace XnaCraft
{
    public class XnaCraftGame : Microsoft.Xna.Framework.Game
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

        protected override void LoadContent()
        {
            var builder = new ContainerBuilder();

            builder.RegisterInstance(GraphicsDevice).As<GraphicsDevice>();
            builder.RegisterInstance(Content).As<ContentManager>();
            builder.Register(ctx => new SpriteBatch(GraphicsDevice)).As<SpriteBatch>();

            builder.RegisterType<World>().SingleInstance();
            builder.RegisterType<Player>().SingleInstance();
            builder.RegisterType<Camera>().SingleInstance();
            builder.RegisterType<BlockManager>().SingleInstance();
            builder.RegisterType<InputController>().SingleInstance();
            builder.RegisterType<WorldRenderer>().As<IWorldRenderer>().SingleInstance();
            builder.RegisterType<DiagnosticsService>().SingleInstance();
            builder.RegisterType<WorldGenerator>().SingleInstance();
            builder.RegisterType<BasicChunkGenerator>().As<IChunkGenerator>().InstancePerDependency();
            builder.RegisterType<ChunkBuilder>().As<ChunkBuilder>().SingleInstance();
            builder.RegisterType<ChunkVertexBuilder>().As<IChunkVertexBuilder>().InstancePerDependency();

            var assembliesToScan = new[] {"XnaCraft.Engine", "XnaCraft.Game"}.Select(Assembly.Load).ToArray();

            builder.RegisterAssemblyTypes(assembliesToScan)
                .AssignableTo<ILogic>()
                .As<ILogic>()
                .SingleInstance();

            builder.RegisterAssemblyTypes(assembliesToScan)
                .AssignableTo<IInputHandler>()
                .As<IInputHandler>()
                .SingleInstance();

            builder.RegisterAssemblyTypes(assembliesToScan)
                .AssignableTo<IInputCommand>()
                .As<IInputCommand>()
                .SingleInstance();

            builder.RegisterAssemblyTypes(assembliesToScan)
                .AssignableTo<IBlockDescriptorFactory>()
                .As<IBlockDescriptorFactory>()
                .SingleInstance();

            var container = builder.Build();

            var logicScripts = container.Resolve<IEnumerable<ILogic>>().OrderByDescending(script =>
            {
                var attribute =
                    script.GetType()
                        .GetCustomAttributes(typeof(PriorityAttribute), false)
                        .Cast<PriorityAttribute>()
                        .FirstOrDefault();

                return attribute != null ? attribute.Priority : 0;
            }) .ToArray();

            _initLogicScripts = logicScripts.OfType<IInitLogic>().ToArray();
            _updateLogicScripts = logicScripts.OfType<IUpdateLogic>().ToArray();
            _renderLogicScripts = logicScripts.OfType<IRenderLogic>().ToArray();

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
                Exit();
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
