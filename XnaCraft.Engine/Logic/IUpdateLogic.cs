using Microsoft.Xna.Framework;

namespace XnaCraft.Engine.Logic
{
    public interface IUpdateLogic : ILogic
    {
        void OnUpdate(GameTime gameTime);
    }
}