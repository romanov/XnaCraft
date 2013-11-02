using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace XnaCraft.Engine
{
    public interface ILogic
    {

    }

    public interface IInitLogic : ILogic
    {
        void OnInit();
        void OnShutdown();
    }

    public interface IUpdateLogic : ILogic
    {
        void OnUpdate(GameTime gameTime);
    }

    public interface IRenderLogic : ILogic
    {
        void OnRender(GameTime gameTime);
    }
}
