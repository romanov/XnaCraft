using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XnaCraft.Engine
{
    class BlockPosChunkVertexBuilder : IChunkVertexBuilder
    {
        Vector3 topLeftFront = new Vector3(-0.5f, 0.5f, -0.5f);
        Vector3 topLeftBack = new Vector3(-0.5f, 0.5f, 0.5f);
        Vector3 topRightFront = new Vector3(0.5f, 0.5f, -0.5f);
        Vector3 topRightBack = new Vector3(0.5f, 0.5f, 0.5f);
        Vector3 bottomLeftFront = new Vector3(-0.5f, -0.5f, -0.5f);
        Vector3 bottomLeftBack = new Vector3(-0.5f, -0.5f, 0.5f);
        Vector3 bottomRightFront = new Vector3(0.5f, -0.5f, -0.5f);
        Vector3 bottomRightBack = new Vector3(0.5f, -0.5f, 0.5f);

        private List<VertexPositionTexture9> _faces = new List<VertexPositionTexture9>();

        private Vector3 _position;
        private BlockDescriptor _descriptor;

        public VertexBuffer Build(GraphicsDevice device)
        {
            var vertices = _faces.ToArray();
            var buffer = new VertexBuffer(device, VertexPositionTexture9.VertexDeclaration, vertices.Length, BufferUsage.WriteOnly);
            
            buffer.SetData(vertices);

            return buffer;
        }

        public void BeginBlock(Vector3 position, BlockDescriptor descriptor)
        {
            _position = position;
            _descriptor = descriptor;
        }

        public void AddFrontFace()
        {
            var uvMapping = GetUVMapping(_position);

            _faces.AddRange(new[]
            {
                new VertexPositionTexture9(topLeftFront + _position, uvMapping.Select(x => x.TopLeft).ToArray()),
                new VertexPositionTexture9(bottomLeftFront + _position, uvMapping.Select(x => x.BottomLeft).ToArray()),
                new VertexPositionTexture9(topRightFront + _position, uvMapping.Select(x => x.TopRight).ToArray()),
                new VertexPositionTexture9(bottomLeftFront + _position, uvMapping.Select(x => x.BottomLeft).ToArray()),
                new VertexPositionTexture9(bottomRightFront + _position, uvMapping.Select(x => x.BottomRight).ToArray()),
                new VertexPositionTexture9(topRightFront + _position, uvMapping.Select(x => x.TopRight).ToArray()),
            });
        }

        public void AddBackFace()
        {
            var uvMapping = GetUVMapping(_position);

            _faces.AddRange(new[]
            {
                new VertexPositionTexture9(topLeftBack + _position, uvMapping.Select(x => x.TopRight).ToArray()),
                new VertexPositionTexture9(topRightBack + _position, uvMapping.Select(x => x.TopLeft).ToArray()),
                new VertexPositionTexture9(bottomLeftBack + _position, uvMapping.Select(x => x.BottomRight).ToArray()),
                new VertexPositionTexture9(bottomLeftBack + _position, uvMapping.Select(x => x.BottomRight).ToArray()),
                new VertexPositionTexture9(topRightBack + _position, uvMapping.Select(x => x.TopLeft).ToArray()),
                new VertexPositionTexture9(bottomRightBack + _position, uvMapping.Select(x => x.BottomLeft).ToArray()),
            });
        }

        public void AddTopFace()
        {
            var uvMapping = GetUVMapping(_position);

            _faces.AddRange(new[]
            {
                new VertexPositionTexture9(topLeftFront + _position, uvMapping.Select(x => x.BottomLeft).ToArray()),
                new VertexPositionTexture9(topRightBack + _position, uvMapping.Select(x => x.TopRight).ToArray()),
                new VertexPositionTexture9(topLeftBack + _position, uvMapping.Select(x => x.TopLeft).ToArray()),
                new VertexPositionTexture9(topLeftFront + _position, uvMapping.Select(x => x.BottomLeft).ToArray()),
                new VertexPositionTexture9(topRightFront + _position, uvMapping.Select(x => x.BottomRight).ToArray()),
                new VertexPositionTexture9(topRightBack + _position, uvMapping.Select(x => x.TopRight).ToArray()),
            });
        }

        public void AddBottomFace()
        {
            var uvMapping = GetUVMapping(_position);

            _faces.AddRange(new[] 
            {
                new VertexPositionTexture9(bottomLeftFront + _position, uvMapping.Select(x => x.TopLeft).ToArray()),
                new VertexPositionTexture9(bottomLeftBack + _position, uvMapping.Select(x => x.BottomLeft).ToArray()),
                new VertexPositionTexture9(bottomRightBack + _position, uvMapping.Select(x => x.BottomRight).ToArray()),
                new VertexPositionTexture9(bottomLeftFront + _position, uvMapping.Select(x => x.TopLeft).ToArray()),
                new VertexPositionTexture9(bottomRightBack + _position, uvMapping.Select(x => x.BottomRight).ToArray()),
                new VertexPositionTexture9(bottomRightFront + _position, uvMapping.Select(x => x.TopRight).ToArray()),
            });
        }

        public void AddLeftFace()
        {
            var uvMapping = GetUVMapping(_position);

            _faces.AddRange(new[] 
            {
                new VertexPositionTexture9(topLeftFront + _position, uvMapping.Select(x => x.TopRight).ToArray()),
                new VertexPositionTexture9(bottomLeftBack + _position, uvMapping.Select(x => x.BottomLeft).ToArray()),
                new VertexPositionTexture9(bottomLeftFront + _position, uvMapping.Select(x => x.BottomRight).ToArray()),
                new VertexPositionTexture9(topLeftBack + _position, uvMapping.Select(x => x.TopLeft).ToArray()),
                new VertexPositionTexture9(bottomLeftBack + _position, uvMapping.Select(x => x.BottomLeft).ToArray()),
                new VertexPositionTexture9(topLeftFront + _position, uvMapping.Select(x => x.TopRight).ToArray()),
            });
        }

        public void AddRightFace()
        {
            var uvMapping = GetUVMapping(_position);

            _faces.AddRange(new[]
            {
                new VertexPositionTexture9(topRightFront + _position, uvMapping.Select(x => x.TopLeft).ToArray()),
                new VertexPositionTexture9(bottomRightFront + _position, uvMapping.Select(x => x.BottomLeft).ToArray()),
                new VertexPositionTexture9(bottomRightBack + _position, uvMapping.Select(x => x.BottomRight).ToArray()),
                new VertexPositionTexture9(topRightBack + _position, uvMapping.Select(x => x.TopRight).ToArray()),
                new VertexPositionTexture9(topRightFront + _position, uvMapping.Select(x => x.TopLeft).ToArray()),
                new VertexPositionTexture9(bottomRightBack + _position, uvMapping.Select(x => x.BottomRight).ToArray()),
            });
        }

        private UVMapping[] GetUVMapping(Vector3 position)
        {
            var xString = position.X.ToString("000");
            var yString = position.Y.ToString("000");
            var zString = position.Z.ToString("000");

            var text = xString.Substring(Math.Max(0, xString.Length - 4), 3)
                + yString.Substring(Math.Max(0, yString.Length - 4), 3)
                + zString.Substring(Math.Max(0, zString.Length - 4), 3);

            var result = new UVMapping[9];

            for (var i = 0; i < 9; i++)
            {
                var xOffset = text[i] - '0';
                var yOffset = i;

                var xStep = 1.0f / 10;
                var xStart = xOffset * xStep;
                var xEnd = (xOffset + 1) * xStep;

                var yStep = 1.0f / 9;
                var yStart = yOffset * yStep;
                var yEnd = (yOffset + 1) * yStep;

                var uvMapping = new UVMapping
                {
                    TopLeft = new Vector2(xEnd, yStart),
                    TopRight = new Vector2(xStart, yStart),
                    BottomLeft = new Vector2(xEnd, yEnd),
                    BottomRight = new Vector2(xStart, yEnd),
                };

                result[i] = uvMapping;
            }

            return result;
        }

        [Serializable]
        public struct VertexPositionTexture9 : IVertexType
        {
            Vector3 _position;
            Vector2 _textureCoordinate0;
            Vector2 _textureCoordinate1;
            Vector2 _textureCoordinate2;
            Vector2 _textureCoordinate3;
            Vector2 _textureCoordinate4;
            Vector2 _textureCoordinate5;
            Vector2 _textureCoordinate6;
            Vector2 _textureCoordinate7;
            Vector2 _textureCoordinate8;

            public static readonly VertexElement[] VertexElements = new VertexElement[]
            { 
        
                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                new VertexElement(sizeof(float)*3,VertexElementFormat.Vector2,  VertexElementUsage.TextureCoordinate, 0),
                new VertexElement(sizeof(float)*5,VertexElementFormat.Vector2,  VertexElementUsage.TextureCoordinate, 1),               
                new VertexElement(sizeof(float)*7,VertexElementFormat.Vector2,  VertexElementUsage.TextureCoordinate, 2),               
                new VertexElement(sizeof(float)*9,VertexElementFormat.Vector2,  VertexElementUsage.TextureCoordinate, 3),               
                new VertexElement(sizeof(float)*11,VertexElementFormat.Vector2,  VertexElementUsage.TextureCoordinate, 4),               
                new VertexElement(sizeof(float)*13,VertexElementFormat.Vector2,  VertexElementUsage.TextureCoordinate, 5),               
                new VertexElement(sizeof(float)*15,VertexElementFormat.Vector2,  VertexElementUsage.TextureCoordinate, 6),               
                new VertexElement(sizeof(float)*17,VertexElementFormat.Vector2,  VertexElementUsage.TextureCoordinate, 7),               
                new VertexElement(sizeof(float)*19,VertexElementFormat.Vector2,  VertexElementUsage.TextureCoordinate, 8),               
             };

            public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration(VertexElements);
            VertexDeclaration IVertexType.VertexDeclaration { get { return VertexDeclaration; } }

            public VertexPositionTexture9(Vector3 position, Vector2[] textureCoordinates)
            {
                _position = position;
                _textureCoordinate0 = textureCoordinates[0];
                _textureCoordinate1 = textureCoordinates[1];
                _textureCoordinate2 = textureCoordinates[2];
                _textureCoordinate3 = textureCoordinates[3];
                _textureCoordinate4 = textureCoordinates[4];
                _textureCoordinate5 = textureCoordinates[5];
                _textureCoordinate6 = textureCoordinates[6];
                _textureCoordinate7 = textureCoordinates[7];
                _textureCoordinate8 = textureCoordinates[8];
            }

            public Vector3 Position { get { return _position; } set { _position = value; } }
            public Vector2 TextureCoordinate0 { get { return _textureCoordinate0; } set { _textureCoordinate0 = value; } }
            public Vector2 TextureCoordinate1 { get { return _textureCoordinate1; } set { _textureCoordinate1 = value; } }
            public Vector2 TextureCoordinate2 { get { return _textureCoordinate2; } set { _textureCoordinate2 = value; } }
            public Vector2 TextureCoordinate3 { get { return _textureCoordinate3; } set { _textureCoordinate3 = value; } }
            public Vector2 TextureCoordinate4 { get { return _textureCoordinate4; } set { _textureCoordinate4 = value; } }
            public Vector2 TextureCoordinate5 { get { return _textureCoordinate5; } set { _textureCoordinate5 = value; } }
            public Vector2 TextureCoordinate6 { get { return _textureCoordinate6; } set { _textureCoordinate6 = value; } }
            public Vector2 TextureCoordinate7 { get { return _textureCoordinate7; } set { _textureCoordinate7 = value; } }
            public Vector2 TextureCoordinate8 { get { return _textureCoordinate8; } set { _textureCoordinate8 = value; } }
        }
    }
}
