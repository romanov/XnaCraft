using System.Linq;
using Microsoft.Xna.Framework.Input;
using XnaCraft.Engine;
using XnaCraft.Engine.Input;

namespace XnaCraft.Game.InputCommands
{
    class RemoveBlockCommand : IInputCommand
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
