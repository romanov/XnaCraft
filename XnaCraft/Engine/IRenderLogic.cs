using Microsoft.Xna.Framework;

namespace XnaCraft.Engine
{
    public interface IRenderLogic : ILogic
    {
        void OnRender(GameTime gameTime);
    }
}