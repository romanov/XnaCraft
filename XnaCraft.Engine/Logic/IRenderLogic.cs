using Microsoft.Xna.Framework;

namespace XnaCraft.Engine.Logic
{
    public interface IRenderLogic : ILogic
    {
        void OnRender(GameTime gameTime);
    }
}