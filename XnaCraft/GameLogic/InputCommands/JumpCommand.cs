using Microsoft.Xna.Framework.Input;
using XnaCraft.Engine;
using XnaCraft.Engine.Input;

namespace XnaCraft.GameLogic.InputCommands
{
    class JumpCommand : IInputCommand
    {
        private readonly Player _player;

        public JumpCommand(Player player)
        {
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
