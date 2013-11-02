using Microsoft.Xna.Framework;

namespace XnaCraft.Engine
{
    public interface IUpdateLogic : ILogic
    {
        void OnUpdate(GameTime gameTime);
    }
}