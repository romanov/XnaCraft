using System.Linq;
using Microsoft.Xna.Framework.Input;
using XnaCraft.Engine;
using XnaCraft.Engine.Input;

namespace XnaCraft.GameLogic.InputCommands
{
    class AddBlockCommand : IInputCommand
    {
        private readonly World _world;
        private readonly Camera _camera;
        private readonly Player _player;

        public AddBlockCommand(World world, Camera camera, Player player)
        {
            _world = world;
            _camera = camera;
            _player = player;
        }

        public bool WasInvoked(InputState context)
        {
            return context.PreviousMouseState.RightButton == ButtonState.Pressed && context.CurrentMouseState.RightButton == ButtonState.Released;
        }

        public void Execute()
        {
            var blocks = _world.RayCast(_camera.Ray, _player.Position.ToPoint3(), 5, true).ToList();
            var emptyBlocks = blocks.TakeWhile(x => x.IsEmpty).ToList();

            if (emptyBlocks.Any() && blocks.Count != emptyBlocks.Count)
            {
                var block = emptyBlocks.Last();

                if (!_player.BoundingBox.Intersects(block.BoundingBox))
                {
                    _world.AddBlock(block.X, block.Y, block.Z, BlockType.Grass);
                }
            }
        }
    }
}
