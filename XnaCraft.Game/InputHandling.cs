using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using XnaCraft.Engine;
using XnaCraft.Engine.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using XnaCraft.Engine.Input;

namespace XnaCraft.Game
{
    class InputHandling : IInputHandler
    {
        private readonly Camera _camera;
        private readonly Player _player;
        private readonly DiagnosticsService _diagnosticsService;
        private readonly GraphicsDevice _graphicsDevice;

        public InputHandling(Camera camera, Player player, DiagnosticsService diagnosticsService, GraphicsDevice graphicsDevice)
        {
            _camera = camera;
            _player = player;
            _diagnosticsService = diagnosticsService;
            _graphicsDevice = graphicsDevice;
        }

        public void HandleInput(GameTime gameTime, InputState inputState)
        {
            var moveVector = GetMoveVector(inputState);
            var mouseOffset = GetMouseOffset(inputState);

            _camera.MoveTo(_player.Position + new Vector3(0, 0.9f, 0));
            _camera.UpdateRotation(mouseOffset.X, mouseOffset.Y);
            _camera.Update();

            _player.Update(gameTime, moveVector, _camera.LeftRightRotation);

            _diagnosticsService.SetInfoValue("Pos", String.Format("X = {0}, Y = {1}, Z = {2}", _player.Position.X, _player.Position.Y, _player.Position.Z));
        }

        private Vector3 GetMoveVector(InputState inputState)
        {
            var moveVector = Vector3.Zero;

            if (inputState.CurrentKeyboardState.IsKeyDown(Keys.W))
            {
                moveVector.Z = -1;
            }
            if (inputState.CurrentKeyboardState.IsKeyDown(Keys.S))
            {
                moveVector.Z = 1;
            }
            if (inputState.CurrentKeyboardState.IsKeyDown(Keys.A))
            {
                moveVector.X = -1;
            }
            if (inputState.CurrentKeyboardState.IsKeyDown(Keys.D))
            {
                moveVector.X = 1;
            }
            if (inputState.CurrentKeyboardState.IsKeyDown(Keys.LeftShift))
            {
                moveVector.Y = 1;
            }
            if (inputState.CurrentKeyboardState.IsKeyDown(Keys.LeftControl))
            {
                moveVector.Y = -1;
            }

            return moveVector;
        }

        private Vector2 GetMouseOffset(InputState inputState)
        {
            var mouseOffsetX = inputState.CurrentMouseState.X - _graphicsDevice.Viewport.Width / 2;
            var mouseOffsetY = inputState.CurrentMouseState.Y - _graphicsDevice.Viewport.Height / 2;

            Mouse.SetPosition(_graphicsDevice.Viewport.Width / 2, _graphicsDevice.Viewport.Height / 2);

            return new Vector2(mouseOffsetX, mouseOffsetY);
        }
    }
}
