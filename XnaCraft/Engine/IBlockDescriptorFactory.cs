using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XnaCraft.Engine
{
    interface IBlockDescriptorFactory
    {
        BlockDescriptor CreateDescriptor();
    }
}
