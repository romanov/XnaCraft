using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace XnaCraft.Engine.Input.Commands
{
    class RemoveBlockCommand : ICommand
    {
        private readonly World _world;
        private readonly Camera _camera;
        private readonly Player _player;

        public RemoveBlockCommand(World world, Camera camera, Player player)
        {
            _world = world;
            _camera = camera;
            _player = player;
        }

        public bool WasInvoked(InputState context)
        {
            return context.PreviousMouseState.LeftButton == ButtonState.Pressed && context.CurrentMouseState.LeftButton == ButtonState.Released;
        }

        public void Execute()
        {
            var block = _world.RayCast(_camera.Ray, _player.Position.ToPoint3(), 5).FirstOrDefault();

            if (!block.IsEmpty)
            {
                _world.RemoveBlock(block);
            }
        }
    }
}
