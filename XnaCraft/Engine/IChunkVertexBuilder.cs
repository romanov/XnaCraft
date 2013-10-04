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
        void BeginBlock(Vector3 position, BlockDescriptor descriptor);

        void AddFrontFace(int[, ,] neighbours);
        void AddBackFace(int[, ,] neighbours);
        void AddTopFace(int[, ,] neighbours);
        void AddBottomFace(int[, ,] neighbours);
        void AddLeftFace(int[, ,] neighbours);
        void AddRightFace(int[, ,] neighbours);

        VertexBuffer Build(GraphicsDevice device);
    }
}
