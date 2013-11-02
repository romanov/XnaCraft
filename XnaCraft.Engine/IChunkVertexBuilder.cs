using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XnaCraft.Engine
{
    public interface IChunkVertexBuilder
    {
        void BeginBlock(Vector3 position, BlockDescriptor descriptor, int[, ,] neighbours);

        void AddFrontFace();
        void AddBackFace();
        void AddTopFace();
        void AddBottomFace();
        void AddLeftFace();
        void AddRightFace();

        VertexBuffer Build(GraphicsDevice device);
    }
}
