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
        private readonly Vector3 _topLeftFront = new Vector3(-0.5f, 0.5f, -0.5f);
        private readonly Vector3 _topLeftBack = new Vector3(-0.5f, 0.5f, 0.5f);
        private readonly Vector3 _topRightFront = new Vector3(0.5f, 0.5f, -0.5f);
        private readonly Vector3 _topRightBack = new Vector3(0.5f, 0.5f, 0.5f);
        private readonly Vector3 _bottomLeftFront = new Vector3(-0.5f, -0.5f, -0.5f);
        private readonly Vector3 _bottomLeftBack = new Vector3(-0.5f, -0.5f, 0.5f);
        private readonly Vector3 _bottomRightFront = new Vector3(0.5f, -0.5f, -0.5f);
        private readonly Vector3 _bottomRightBack = new Vector3(0.5f, -0.5f, 0.5f);

        private readonly List<VertexPositionTexture> _faces = new List<VertexPositionTexture>();

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
                new VertexPositionTexture(_topLeftFront + _position, uvMapping.TopLeft),
                new VertexPositionTexture(_bottomLeftFront + _position, uvMapping.BottomLeft),
                new VertexPositionTexture(_topRightFront + _position, uvMapping.TopRight),
                new VertexPositionTexture(_bottomLeftFront + _position, uvMapping.BottomLeft),
                new VertexPositionTexture(_bottomRightFront + _position, uvMapping.BottomRight),
                new VertexPositionTexture(_topRightFront + _position, uvMapping.TopRight),
            });
        }

        public void AddBackFace()
        {
            var uvMapping = GetUVMapping(_descriptor.TextureBack);

            _faces.AddRange(new[] 
            {
                new VertexPositionTexture(_topLeftBack + _position, uvMapping.TopRight),
                new VertexPositionTexture(_topRightBack + _position, uvMapping.TopLeft),
                new VertexPositionTexture(_bottomLeftBack + _position, uvMapping.BottomRight),
                new VertexPositionTexture(_bottomLeftBack + _position, uvMapping.BottomRight),
                new VertexPositionTexture(_topRightBack + _position, uvMapping.TopLeft),
                new VertexPositionTexture(_bottomRightBack + _position, uvMapping.BottomLeft),
            });
        }

        public void AddTopFace()
        {
            var uvMapping = GetUVMapping(_descriptor.TextureTop);

            _faces.AddRange(new[] 
            {
                new VertexPositionTexture(_topLeftFront + _position, uvMapping.BottomLeft),
                new VertexPositionTexture(_topRightBack + _position, uvMapping.TopRight),
                new VertexPositionTexture(_topLeftBack + _position, uvMapping.TopLeft),
                new VertexPositionTexture(_topLeftFront + _position, uvMapping.BottomLeft),
                new VertexPositionTexture(_topRightFront + _position, uvMapping.BottomRight),
                new VertexPositionTexture(_topRightBack + _position, uvMapping.TopRight),
            });
        }

        public void AddBottomFace()
        {
            var uvMapping = GetUVMapping(_descriptor.TextureBottom);

            _faces.AddRange(new[] 
            {
                new VertexPositionTexture(_bottomLeftFront + _position, uvMapping.TopLeft),
                new VertexPositionTexture(_bottomLeftBack + _position, uvMapping.BottomLeft),
                new VertexPositionTexture(_bottomRightBack + _position, uvMapping.BottomRight),
                new VertexPositionTexture(_bottomLeftFront + _position, uvMapping.TopLeft),
                new VertexPositionTexture(_bottomRightBack + _position, uvMapping.BottomRight),
                new VertexPositionTexture(_bottomRightFront + _position, uvMapping.TopRight),
            });
        }

        public void AddLeftFace()
        {
            var uvMapping = GetUVMapping(_descriptor.TextureLeft);

            _faces.AddRange(new[] 
            {
                new VertexPositionTexture(_topLeftFront + _position, uvMapping.TopRight),
                new VertexPositionTexture(_bottomLeftBack + _position, uvMapping.BottomLeft),
                new VertexPositionTexture(_bottomLeftFront + _position, uvMapping.BottomRight),
                new VertexPositionTexture(_topLeftBack + _position, uvMapping.TopLeft),
                new VertexPositionTexture(_bottomLeftBack + _position, uvMapping.BottomLeft),
                new VertexPositionTexture(_topLeftFront + _position, uvMapping.TopRight),
            });
        }

        public void AddRightFace()
        {
            var uvMapping = GetUVMapping(_descriptor.TextureRight);

            _faces.AddRange(new[] 
            {
                new VertexPositionTexture(_topRightFront + _position, uvMapping.TopLeft),
                new VertexPositionTexture(_bottomRightFront + _position, uvMapping.BottomLeft),
                new VertexPositionTexture(_bottomRightBack + _position, uvMapping.BottomRight),
                new VertexPositionTexture(_topRightBack + _position, uvMapping.TopRight),
                new VertexPositionTexture(_topRightFront + _position, uvMapping.TopLeft),
                new VertexPositionTexture(_bottomRightBack + _position, uvMapping.BottomRight),
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
            var buffer = new VertexBuffer(device, VertexPositionTexture.VertexDeclaration, vertices.Length, BufferUsage.WriteOnly);

            buffer.SetData(vertices);

            return buffer;
        }

        [Serializable]
        struct VertexPositionTexture : IVertexType
        {
            private Vector3 _position;
            private Vector2 _textureCoordinate;

            public static readonly VertexElement[] VertexElements = new []
            { 
                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                new VertexElement(sizeof(float) * 3, VertexElementFormat.Vector2,  VertexElementUsage.TextureCoordinate, 0),
            };

            public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration(VertexElements);

            VertexDeclaration IVertexType.VertexDeclaration { get { return VertexDeclaration; } }

            public VertexPositionTexture(Vector3 position, Vector2 textureCoordinate)
            {
                _position = position;
                _textureCoordinate = textureCoordinate;
            }

            public Vector3 Position { get { return _position; } set { _position = value; } }
            public Vector2 TextureCoordinate { get { return _textureCoordinate; } set { _textureCoordinate = value; } }
        }
    }
}
