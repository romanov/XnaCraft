using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XnaCraft.Engine
{
    class ChunkVertexBuilder : IChunkVertexBuilder
    {
        // TODO: code clean-up
        Vector3 topLeftFront = new Vector3(-0.5f, 0.5f, -0.5f);
        Vector3 topLeftBack = new Vector3(-0.5f, 0.5f, 0.5f);
        Vector3 topRightFront = new Vector3(0.5f, 0.5f, -0.5f);
        Vector3 topRightBack = new Vector3(0.5f, 0.5f, 0.5f);
        Vector3 bottomLeftFront = new Vector3(-0.5f, -0.5f, -0.5f);
        Vector3 bottomLeftBack = new Vector3(-0.5f, -0.5f, 0.5f);
        Vector3 bottomRightFront = new Vector3(0.5f, -0.5f, -0.5f);
        Vector3 bottomRightBack = new Vector3(0.5f, -0.5f, 0.5f);

        Vector3 normalFront = new Vector3(0.0f, 0.0f, 1.0f);
        Vector3 normalBack = new Vector3(0.0f, 0.0f, -1.0f);
        Vector3 normalTop = new Vector3(0.0f, 1.0f, 0.0f);
        Vector3 normalBottom = new Vector3(0.0f, -1.0f, 0.0f);
        Vector3 normalLeft = new Vector3(-1.0f, 0.0f, 0.0f);
        Vector3 normalRight = new Vector3(1.0f, 0.0f, 0.0f);

        private readonly List<VertexPositionNormalTexture> _faces = new List<VertexPositionNormalTexture>();

        private Vector3 _position;
        private BlockDescriptor _descriptor;

        public void BeginBlock(Vector3 position, BlockDescriptor descriptor)
        {
            _position = position;
            _descriptor = descriptor;
        }

        public void AddFrontFace()
        {
            var uvMapping = GetUVMapping(_descriptor.TextureFront);

            _faces.AddRange(new[] 
            {
                new VertexPositionNormalTexture(topLeftFront + _position, normalFront, uvMapping.TopLeft),
                new VertexPositionNormalTexture(bottomLeftFront + _position, normalFront, uvMapping.BottomLeft),
                new VertexPositionNormalTexture(topRightFront + _position, normalFront, uvMapping.TopRight),
                new VertexPositionNormalTexture(bottomLeftFront + _position, normalFront, uvMapping.BottomLeft),
                new VertexPositionNormalTexture(bottomRightFront + _position, normalFront, uvMapping.BottomRight),
                new VertexPositionNormalTexture(topRightFront + _position, normalFront, uvMapping.TopRight),
            });
        }

        public void AddBackFace()
        {
            var uvMapping = GetUVMapping(_descriptor.TextureBack);

            _faces.AddRange(new[] 
            {
                new VertexPositionNormalTexture(topLeftBack + _position, normalBack, uvMapping.TopRight),
                new VertexPositionNormalTexture(topRightBack + _position, normalBack, uvMapping.TopLeft),
                new VertexPositionNormalTexture(bottomLeftBack + _position, normalBack, uvMapping.BottomRight),
                new VertexPositionNormalTexture(bottomLeftBack + _position, normalBack, uvMapping.BottomRight),
                new VertexPositionNormalTexture(topRightBack + _position, normalBack, uvMapping.TopLeft),
                new VertexPositionNormalTexture(bottomRightBack + _position, normalBack, uvMapping.BottomLeft),
            });
        }

        public void AddTopFace()
        {
            var uvMapping = GetUVMapping(_descriptor.TextureTop);

            _faces.AddRange(new[] 
            {
                new VertexPositionNormalTexture(topLeftFront + _position, normalTop, uvMapping.BottomLeft),
                new VertexPositionNormalTexture(topRightBack + _position, normalTop, uvMapping.TopRight),
                new VertexPositionNormalTexture(topLeftBack + _position, normalTop, uvMapping.TopLeft),
                new VertexPositionNormalTexture(topLeftFront + _position, normalTop, uvMapping.BottomLeft),
                new VertexPositionNormalTexture(topRightFront + _position, normalTop, uvMapping.BottomRight),
                new VertexPositionNormalTexture(topRightBack + _position, normalTop, uvMapping.TopRight),
            });
        }

        public void AddBottomFace()
        {
            var uvMapping = GetUVMapping(_descriptor.TextureBottom);

            _faces.AddRange(new[] 
            {
                new VertexPositionNormalTexture(bottomLeftFront + _position, normalBottom, uvMapping.TopLeft),
                new VertexPositionNormalTexture(bottomLeftBack + _position, normalBottom, uvMapping.BottomLeft),
                new VertexPositionNormalTexture(bottomRightBack + _position, normalBottom, uvMapping.BottomRight),
                new VertexPositionNormalTexture(bottomLeftFront + _position, normalBottom, uvMapping.TopLeft),
                new VertexPositionNormalTexture(bottomRightBack + _position, normalBottom, uvMapping.BottomRight),
                new VertexPositionNormalTexture(bottomRightFront + _position, normalBottom, uvMapping.TopRight),
            });
        }

        public void AddLeftFace()
        {
            var uvMapping = GetUVMapping(_descriptor.TextureLeft);

            _faces.AddRange(new[] 
            {
                new VertexPositionNormalTexture(topLeftFront + _position, normalLeft, uvMapping.TopRight),
                new VertexPositionNormalTexture(bottomLeftBack + _position, normalLeft, uvMapping.BottomLeft),
                new VertexPositionNormalTexture(bottomLeftFront + _position, normalLeft, uvMapping.BottomRight),
                new VertexPositionNormalTexture(topLeftBack + _position, normalLeft, uvMapping.TopLeft),
                new VertexPositionNormalTexture(bottomLeftBack + _position, normalLeft, uvMapping.BottomLeft),
                new VertexPositionNormalTexture(topLeftFront + _position, normalLeft, uvMapping.TopRight),
            });
        }

        public void AddRightFace()
        {
            var uvMapping = GetUVMapping(_descriptor.TextureRight);

            _faces.AddRange(new[] 
            {
                new VertexPositionNormalTexture(topRightFront + _position, normalRight, uvMapping.TopLeft),
                new VertexPositionNormalTexture(bottomRightFront + _position, normalRight, uvMapping.BottomLeft),
                new VertexPositionNormalTexture(bottomRightBack + _position, normalRight, uvMapping.BottomRight),
                new VertexPositionNormalTexture(topRightBack + _position, normalRight, uvMapping.TopRight),
                new VertexPositionNormalTexture(topRightFront + _position, normalRight, uvMapping.TopLeft),
                new VertexPositionNormalTexture(bottomRightBack + _position, normalRight, uvMapping.BottomRight),
            });
        }

        private UVMapping GetUVMapping(BlockFaceTexture texture)
        {
            var offset = (int)texture;
            var step = 1.0f / Enum.GetValues(typeof(BlockFaceTexture)).Length;
            var uStart = offset * step;
            var uEnd = (offset + 1) * step;

            return new UVMapping
            {
                TopLeft = new Vector2(uEnd, 0.0f),
                TopRight = new Vector2(uStart, 0.0f),
                BottomLeft = new Vector2(uEnd, 1.0f),
                BottomRight = new Vector2(uStart, 1.0f),
            };
        }

        public VertexBuffer Build(GraphicsDevice device)
        {
            var vertices = _faces.ToArray();
            var buffer = new VertexBuffer(device, VertexPositionNormalTexture.VertexDeclaration, vertices.Length, BufferUsage.WriteOnly);

            buffer.SetData(vertices);

            return buffer;
        }
    }
}
