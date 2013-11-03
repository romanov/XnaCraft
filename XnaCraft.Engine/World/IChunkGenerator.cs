using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XnaCraft.Engine.World
{
    public interface IChunkGenerator
    {
        BlockDescriptor[, ,] Generate(int cx, int cy);
    }
}
