using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XnaCraft.Engine.Framework;

namespace XnaCraft.Engine.World
{
    public interface IWorldRenderer
    {
        void Render(World world, Camera camera);
    }
}
