using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace XnaCraft.Engine.Input.Commands
{
    class JumpCommand : ICommand
    {
        private readonly World _world;
        private readonly Camera _camera;
        private readonly Player _player;

        public JumpCommand(World world, Camera camera, Player player)
        {
            _world = world;
            _camera = camera;
            _player = player;
        }

        public bool WasInvoked(InputState context)
        {
            return context.CurrentKeyboardState.IsKeyDown(Keys.Space);
        }

        public void Execute()
        {
            _player.Jump();
        }
    }
}
