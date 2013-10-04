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

        private readonly List<VertexPositionTextureOcclusion> _faces = new List<VertexPositionTextureOcclusion>();

        private Vector3 _position;
        private BlockDescriptor _descriptor;
        private int[, ,] _neighbours;

        public void BeginBlock(Vector3 position, BlockDescriptor descriptor)
        {
            _position = position;
            _descriptor = descriptor;
        }

        public void AddFrontFace(int[, ,] neighbours)
        {
            _neighbours = neighbours;
            var uvMapping = GetUVMapping(_descriptor.TextureFront);

            _faces.AddRange(new[] 
            {
                new VertexPositionTextureOcclusion(_topLeftFront + _position, uvMapping.TopLeft, GetOcclusion(Dominant.Z, XDir.Left, YDir.Top, ZDir.Front)),
                new VertexPositionTextureOcclusion(_bottomLeftFront + _position, uvMapping.BottomLeft, GetOcclusion(Dominant.Z, XDir.Left, YDir.Bottom, ZDir.Front)),
                new VertexPositionTextureOcclusion(_topRightFront + _position, uvMapping.TopRight, GetOcclusion(Dominant.Z, XDir.Right, YDir.Top, ZDir.Front)),
                new VertexPositionTextureOcclusion(_bottomLeftFront + _position, uvMapping.BottomLeft, GetOcclusion(Dominant.Z, XDir.Left, YDir.Bottom, ZDir.Front)),
                new VertexPositionTextureOcclusion(_bottomRightFront + _position, uvMapping.BottomRight, GetOcclusion(Dominant.Z, XDir.Right, YDir.Bottom, ZDir.Front)),
                new VertexPositionTextureOcclusion(_topRightFront + _position, uvMapping.TopRight, GetOcclusion(Dominant.Z, XDir.Right, YDir.Top, ZDir.Front)),
            });
        }

        public void AddBackFace(int[, ,] neighbours)
        {
            _neighbours = neighbours;
            var uvMapping = GetUVMapping(_descriptor.TextureBack);

            _faces.AddRange(new[] 
            {
                new VertexPositionTextureOcclusion(_topLeftBack + _position, uvMapping.TopRight, GetOcclusion(Dominant.Z, XDir.Left, YDir.Top, ZDir.Back)),
                new VertexPositionTextureOcclusion(_topRightBack + _position, uvMapping.TopLeft, GetOcclusion(Dominant.Z, XDir.Right, YDir.Top, ZDir.Back)),
                new VertexPositionTextureOcclusion(_bottomLeftBack + _position, uvMapping.BottomRight, GetOcclusion(Dominant.Z, XDir.Left, YDir.Bottom, ZDir.Back)),
                new VertexPositionTextureOcclusion(_bottomLeftBack + _position, uvMapping.BottomRight, GetOcclusion(Dominant.Z, XDir.Left, YDir.Bottom, ZDir.Back)),
                new VertexPositionTextureOcclusion(_topRightBack + _position, uvMapping.TopLeft, GetOcclusion(Dominant.Z, XDir.Right, YDir.Top, ZDir.Back)),
                new VertexPositionTextureOcclusion(_bottomRightBack + _position, uvMapping.BottomLeft, GetOcclusion(Dominant.Z, XDir.Right, YDir.Bottom, ZDir.Back)),
            });
        }

        public void AddTopFace(int[, ,] neighbours)
        {
            _neighbours = neighbours;
            var uvMapping = GetUVMapping(_descriptor.TextureTop);

            _faces.AddRange(new[] 
            {
                new VertexPositionTextureOcclusion(_topLeftFront + _position, uvMapping.BottomLeft, GetOcclusion(Dominant.Y, XDir.Left, YDir.Top, ZDir.Front)),
                new VertexPositionTextureOcclusion(_topRightBack + _position, uvMapping.TopRight, GetOcclusion(Dominant.Y, XDir.Right, YDir.Top, ZDir.Back)),
                new VertexPositionTextureOcclusion(_topLeftBack + _position, uvMapping.TopLeft, GetOcclusion(Dominant.Y, XDir.Left, YDir.Top, ZDir.Back)),
                new VertexPositionTextureOcclusion(_topLeftFront + _position, uvMapping.BottomLeft, GetOcclusion(Dominant.Y, XDir.Left, YDir.Top, ZDir.Front)),
                new VertexPositionTextureOcclusion(_topRightFront + _position, uvMapping.BottomRight, GetOcclusion(Dominant.Y, XDir.Right, YDir.Top, ZDir.Front)),
                new VertexPositionTextureOcclusion(_topRightBack + _position, uvMapping.TopRight, GetOcclusion(Dominant.Y, XDir.Right, YDir.Top, ZDir.Back)),
            });
        }

        public void AddBottomFace(int[, ,] neighbours)
        {
            _neighbours = neighbours;
            var uvMapping = GetUVMapping(_descriptor.TextureBottom);

            _faces.AddRange(new[] 
            {
                new VertexPositionTextureOcclusion(_bottomLeftFront + _position, uvMapping.TopLeft, GetOcclusion(Dominant.Y, XDir.Left, YDir.Bottom, ZDir.Front)),
                new VertexPositionTextureOcclusion(_bottomLeftBack + _position, uvMapping.BottomLeft, GetOcclusion(Dominant.Y, XDir.Left, YDir.Bottom, ZDir.Back)),
                new VertexPositionTextureOcclusion(_bottomRightBack + _position, uvMapping.BottomRight, GetOcclusion(Dominant.Y, XDir.Right, YDir.Bottom, ZDir.Back)),
                new VertexPositionTextureOcclusion(_bottomLeftFront + _position, uvMapping.TopLeft, GetOcclusion(Dominant.Y, XDir.Left, YDir.Bottom, ZDir.Front)),
                new VertexPositionTextureOcclusion(_bottomRightBack + _position, uvMapping.BottomRight, GetOcclusion(Dominant.Y, XDir.Right, YDir.Bottom, ZDir.Back)),
                new VertexPositionTextureOcclusion(_bottomRightFront + _position, uvMapping.TopRight, GetOcclusion(Dominant.Y, XDir.Right, YDir.Bottom, ZDir.Front)),
            });
        }

        public void AddLeftFace(int[, ,] neighbours)
        {
            _neighbours = neighbours;
            var uvMapping = GetUVMapping(_descriptor.TextureLeft);

            _faces.AddRange(new[] 
            {
                new VertexPositionTextureOcclusion(_topLeftFront + _position, uvMapping.TopRight, GetOcclusion(Dominant.X, XDir.Left, YDir.Top, ZDir.Front)),
                new VertexPositionTextureOcclusion(_bottomLeftBack + _position, uvMapping.BottomLeft, GetOcclusion(Dominant.X, XDir.Left, YDir.Bottom, ZDir.Back)),
                new VertexPositionTextureOcclusion(_bottomLeftFront + _position, uvMapping.BottomRight, GetOcclusion(Dominant.X, XDir.Left, YDir.Bottom, ZDir.Front)),
                new VertexPositionTextureOcclusion(_topLeftBack + _position, uvMapping.TopLeft, GetOcclusion(Dominant.X, XDir.Left, YDir.Top, ZDir.Back)),
                new VertexPositionTextureOcclusion(_bottomLeftBack + _position, uvMapping.BottomLeft, GetOcclusion(Dominant.X, XDir.Left, YDir.Bottom, ZDir.Back)),
                new VertexPositionTextureOcclusion(_topLeftFront + _position, uvMapping.TopRight, GetOcclusion(Dominant.X, XDir.Left, YDir.Top, ZDir.Front)),
            });
        }

        public void AddRightFace(int[, ,] neighbours)
        {
            _neighbours = neighbours;
            var uvMapping = GetUVMapping(_descriptor.TextureRight);

            _faces.AddRange(new[] 
            {
                new VertexPositionTextureOcclusion(_topRightFront + _position, uvMapping.TopLeft, GetOcclusion(Dominant.X, XDir.Right, YDir.Top, ZDir.Front)),
                new VertexPositionTextureOcclusion(_bottomRightFront + _position, uvMapping.BottomLeft, GetOcclusion(Dominant.X, XDir.Right, YDir.Bottom, ZDir.Front)),
                new VertexPositionTextureOcclusion(_bottomRightBack + _position, uvMapping.BottomRight, GetOcclusion(Dominant.X, XDir.Right, YDir.Bottom, ZDir.Back)),
                new VertexPositionTextureOcclusion(_topRightBack + _position, uvMapping.TopRight, GetOcclusion(Dominant.X, XDir.Right, YDir.Top, ZDir.Back)),
                new VertexPositionTextureOcclusion(_topRightFront + _position, uvMapping.TopLeft, GetOcclusion(Dominant.X, XDir.Right, YDir.Top, ZDir.Front)),
                new VertexPositionTextureOcclusion(_bottomRightBack + _position, uvMapping.BottomRight, GetOcclusion(Dominant.X, XDir.Right, YDir.Bottom, ZDir.Back)),
            });
        }

        private int GetOcclusion(Dominant dominant, XDir xDir, YDir yDir, ZDir zDir) 
        {
            return CalculateOcclusion(GetOcclusionValues( GetOcclusionMapping(dominant, xDir, yDir, zDir)));
        }


        private Point3 GetOcclusionValues(Point3[] mapping)
        {
            var values = new Point3();

            values.X = _neighbours[1 + mapping[0].X, 1 + mapping[0].Y, 1 + mapping[0].Z];
            values.Y = _neighbours[1 + mapping[1].X, 1 + mapping[1].Y, 1 + mapping[1].Z];
            values.Z = _neighbours[1 + mapping[2].X, 1 + mapping[2].Y, 1 + mapping[2].Z];

            return values;
        }

        private int CalculateOcclusion(Point3 mapping)
        {
            var side1 = mapping.X;
            var side2 = mapping.Y;
            var corner = mapping.Z;

            if (side1 == 1 && side2 == 1)
            {
                return 0;
            }

            return 3 - (side1 + side2 + corner);
        }

        private Point3[] GetOcclusionMapping(Dominant dominant, XDir xDir, YDir yDir, ZDir zDir)
        {
            if (dominant == Dominant.X)
            {
                return new[] 
                {
                    new Point3((int)xDir, 0, (int)zDir),
                    new Point3((int)xDir, (int)yDir, 0),
                    new Point3((int)xDir, (int)yDir, (int)zDir),
                };
            }
            else if (dominant == Dominant.Y)
            {
                return new[] 
                {
                    new Point3(0, (int)yDir, (int)zDir),
                    new Point3((int)xDir, (int)yDir, 0),
                    new Point3((int)xDir, (int)yDir, (int)zDir),
                };
            }
            else if (dominant == Dominant.Z)
            {
                return new[] 
                {
                    new Point3(0, (int)yDir, (int)zDir),
                    new Point3((int)xDir, 0, (int)zDir),
                    new Point3((int)xDir, (int)yDir, (int)zDir),
                };
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        enum Dominant
        {
            X, Y, Z
        }

        enum XDir
        {
            Left = -1,
            Right = 1,
        }

        enum YDir
        {
            Top = 1,
            Bottom = -1,
        }

        enum ZDir
        {
            Front = -1,
            Back = 1,
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
            var buffer = new VertexBuffer(device, VertexPositionTextureOcclusion.VertexDeclaration, vertices.Length, BufferUsage.WriteOnly);

            buffer.SetData(vertices);

            return buffer;
        }

        [Serializable]
        struct VertexPositionTextureOcclusion : IVertexType
        {
            private Vector3 _position;
            private Vector2 _textureCoordinate;
            private float _occlusion;

            public static readonly VertexElement[] VertexElements = new[]
            { 
                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                new VertexElement(sizeof(float) * 3, VertexElementFormat.Vector2,  VertexElementUsage.TextureCoordinate, 0),
                new VertexElement(sizeof(float) * 5, VertexElementFormat.Single,  VertexElementUsage.Color, 0),
            };

            public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration(VertexElements);

            VertexDeclaration IVertexType.VertexDeclaration { get { return VertexDeclaration; } }

            public VertexPositionTextureOcclusion(Vector3 position, Vector2 textureCoordinate, float occlusion)
            {
                _position = position;
                _textureCoordinate = textureCoordinate;
                _occlusion = occlusion;
            }

            public Vector3 Position { get { return _position; } set { _position = value; } }
            public Vector2 TextureCoordinate { get { return _textureCoordinate; } set { _textureCoordinate = value; } }
            public float Occlusion { get { return _occlusion; } set { _occlusion = value; } }
        }
    }
}
