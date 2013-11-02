using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XnaCraft.Engine
{
    public class BlockType
    {
        public string Name { get; private set; }

        public BlockType(string name)
        {
            Name = name;
        }
    }
}
