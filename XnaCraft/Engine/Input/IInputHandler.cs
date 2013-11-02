using Microsoft.Xna.Framework;

namespace XnaCraft.Engine.Input
{
    public interface IInputHandler
    {
        void HandleInput(GameTime gameTime, InputState inputState);
    }
}