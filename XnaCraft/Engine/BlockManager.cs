using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace XnaCraft.Engine
{
    class BlockManager
    {
        private readonly Dictionary<BlockType, BlockDescriptor> _descriptors;

        public BlockManager()
        {
            var factories = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => typeof(IBlockDescriptorFactory).IsAssignableFrom(t) && t.IsClass)
                .Select(t => (IBlockDescriptorFactory)Activator.CreateInstance(t));

            _descriptors = factories.Select(f => f.CreateDescriptor()).ToDictionary(d => d.BlockType);
        }

        public BlockDescriptor GetDescriptor(BlockType blockType)
        {
            return _descriptors[blockType];
        }
    }
}
