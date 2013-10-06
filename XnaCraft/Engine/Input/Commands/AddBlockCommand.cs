using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace XnaCraft.Engine.Input.Commands
{
    class AddBlockCommand : ICommand
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
            var blocks = _world.RayCast(_camera.Ray, _player.Position.ToPoint3(), 5, true);
            var emptyBlocks = blocks.TakeWhile(x => x.IsEmpty);

            if (emptyBlocks.Any() && blocks.Count() != emptyBlocks.Count())
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
