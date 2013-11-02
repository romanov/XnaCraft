using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XnaCraft.Engine
{
    public class BlockManager
    {
        private readonly Dictionary<BlockType, BlockDescriptor> _descriptors;

        public BlockManager(IEnumerable<IBlockDescriptorFactory> factories)
        {
            _descriptors = factories.Select(f => f.CreateDescriptor()).ToDictionary(d => d.BlockType);
        }

        public BlockDescriptor GetDescriptor(BlockType blockType)
        {
            return _descriptors[blockType];
        }
    }
}
