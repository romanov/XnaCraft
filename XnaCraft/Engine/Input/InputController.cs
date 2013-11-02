using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using XnaCraft.Engine.Input.Commands;
using Microsoft.Xna.Framework.Graphics;
using XnaCraft.Engine.Diagnostics;

namespace XnaCraft.Engine.Input
{
    class InputController
    {
        private readonly GraphicsDevice _graphicsDevice;
        private readonly World _world;
        private readonly Camera _camera;
        private readonly Player _player;
        private readonly DiagnosticsService _diagnosticsService;

        private readonly List<ICommand> _commands = new List<ICommand>();
        private readonly InputState _inputState = new InputState();

        public InputController(GraphicsDevice graphicsDevice, World world, Camera camera, Player player, DiagnosticsService diagnosticsService)
        {
            _graphicsDevice = graphicsDevice;
            _world = world;
            _camera = camera;
            _player = player;
            _diagnosticsService = diagnosticsService;

            _commands.AddRange(new ICommand[]
            {
                new AddBlockCommand(_world, _camera, _player),
                new RemoveBlockCommand(_world, _camera, _player),
                new JumpCommand(_world, _camera, _player),
            });
        }

        public void Update(GameTime gameTime)
        {
            _inputState.CurrentKeyboardState = Keyboard.GetState();
            _inputState.CurrentMouseState = Mouse.GetState();

            var moveVector = GetMoveVector();
            var mouseOffset = GetMouseOffset();

            _camera.Update(mouseOffset.X, mouseOffset.Y, _player.Position + new Vector3(0, 0.9f, 0));
            _player.Update(gameTime, moveVector, _camera.LeftRightRotation);

            _diagnosticsService.SetInfoValue("Pos", String.Format("X = {0}, Y = {1}, Z = {2}", _player.Position.X, _player.Position.Y, _player.Position.Z));

            ExecuteCommands();

            _inputState.PreviousKeyboardState = _inputState.CurrentKeyboardState;
            _inputState.PreviousMouseState = _inputState.CurrentMouseState;
        }

        private void ExecuteCommands()
        {
            foreach (var command in _commands)
            {
                if (command.WasInvoked(_inputState))
                {
                    command.Execute();
                }
            }
        }

        private Vector3 GetMoveVector()
        {
            var moveVector = Vector3.Zero;

            if (_inputState.CurrentKeyboardState.IsKeyDown(Keys.W))
            {
                moveVector.Z = -1;
            }
            if (_inputState.CurrentKeyboardState.IsKeyDown(Keys.S))
            {
                moveVector.Z = 1;
            }
            if (_inputState.CurrentKeyboardState.IsKeyDown(Keys.A))
            {
                moveVector.X = -1;
            }
            if (_inputState.CurrentKeyboardState.IsKeyDown(Keys.D))
            {
                moveVector.X = 1;
            }
            if (_inputState.CurrentKeyboardState.IsKeyDown(Keys.LeftShift))
            {
                moveVector.Y = 1;
            }
            if (_inputState.CurrentKeyboardState.IsKeyDown(Keys.LeftControl))
            {
                moveVector.Y = -1;
            }

            return moveVector;
        }

        private Vector2 GetMouseOffset()
        {
            var mouseOffsetX = _inputState.CurrentMouseState.X - _graphicsDevice.Viewport.Width / 2;
            var mouseOffsetY = _inputState.CurrentMouseState.Y - _graphicsDevice.Viewport.Height / 2;

            Mouse.SetPosition(_graphicsDevice.Viewport.Width / 2, _graphicsDevice.Viewport.Height / 2);

            return new Vector2(mouseOffsetX, mouseOffsetY);
        }
    }
}
