using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace XnaCraft.Engine
{
    public class BlockGeometry
    {
        public VertexBuffer Buffer { get; private set; }

        public const int FrontFaceOffset = 0;
        public const int BackFaceOffset = 4;
        public const int TopFaceOffset = 8;
        public const int BottomFaceOffset = 12;
        public const int LeftFaceOffset = 16;
        public const int RightFaceOffset = 20;

        public BlockGeometry(GraphicsDevice device)
        {
            var vertices = GenerateVertices();

            Buffer = new VertexBuffer(device, VertexPositionNormalTexture.VertexDeclaration, vertices.Length, BufferUsage.WriteOnly);
            Buffer.SetData(vertices);
        }

        private VertexPositionNormalTexture[] GenerateVertices()
        {
            var topLeftFront = new Vector3(-0.5f, 0.5f, -0.5f);
            var topLeftBack = new Vector3(-0.5f, 0.5f, 0.5f);
            var topRightFront = new Vector3(0.5f, 0.5f, -0.5f);
            var topRightBack = new Vector3(0.5f, 0.5f, 0.5f);
            var bottomLeftFront = new Vector3(-0.5f, -0.5f, -0.5f);
            var bottomLeftBack = new Vector3(-0.5f, -0.5f, 0.5f);
            var bottomRightFront = new Vector3(0.5f, -0.5f, -0.5f);
            var bottomRightBack = new Vector3(0.5f, -0.5f, 0.5f);

            var normalFront = new Vector3(0.0f, 0.0f, 1.0f);
            var normalBack = new Vector3(0.0f, 0.0f, -1.0f);
            var normalTop = new Vector3(0.0f, 1.0f, 0.0f);
            var normalBottom = new Vector3(0.0f, -1.0f, 0.0f);
            var normalLeft = new Vector3(-1.0f, 0.0f, 0.0f);
            var normalRight = new Vector3(1.0f, 0.0f, 0.0f);

            var texture = 1;

            var textureTopLeft = new Vector2((texture + 1) * 0.25f, 0.0f);
            var textureTopRight = new Vector2(texture * 0.25f, 0.0f);
            var textureBottomLeft = new Vector2((texture + 1) * 0.25f, 1.0f);
            var textureBottomRight = new Vector2(texture * 0.25f, 1.0f);

            return new VertexPositionNormalTexture[]
            {
                // Front face
                new VertexPositionNormalTexture(topRightFront, normalFront, textureTopRight),
                new VertexPositionNormalTexture(topLeftFront, normalFront, textureTopLeft),
                new VertexPositionNormalTexture(bottomRightFront, normalFront, textureBottomRight),
                new VertexPositionNormalTexture(bottomLeftFront, normalFront, textureBottomLeft),

                // Back face
                new VertexPositionNormalTexture(topLeftBack, normalBack, textureTopRight),
                new VertexPositionNormalTexture(topRightBack, normalBack, textureTopLeft),
                new VertexPositionNormalTexture(bottomLeftBack, normalBack, textureBottomRight),
                new VertexPositionNormalTexture(bottomRightBack, normalBack, textureBottomLeft),

                // Top face
                new VertexPositionNormalTexture(topRightBack, normalTop, textureTopRight),
                new VertexPositionNormalTexture(topLeftBack, normalTop, textureTopLeft),
                new VertexPositionNormalTexture(topRightFront, normalTop, textureBottomRight),
                new VertexPositionNormalTexture(topLeftFront, normalTop, textureBottomLeft),

                // Bottom face
                new VertexPositionNormalTexture(bottomLeftFront, normalBottom, textureTopLeft),
                new VertexPositionNormalTexture(bottomLeftBack, normalBottom, textureBottomLeft),
                new VertexPositionNormalTexture(bottomRightFront, normalBottom, textureTopRight),
                new VertexPositionNormalTexture(bottomRightBack, normalBottom, textureBottomRight),

                // Left face
                new VertexPositionNormalTexture(bottomLeftFront, normalLeft, textureBottomRight),
                new VertexPositionNormalTexture(topLeftFront, normalLeft, textureTopRight),
                new VertexPositionNormalTexture(bottomLeftBack, normalLeft, textureBottomLeft),
                new VertexPositionNormalTexture(topLeftBack, normalLeft, textureTopLeft),

                // Right face
                new VertexPositionNormalTexture(topRightBack, normalRight, textureTopRight),
                new VertexPositionNormalTexture(topRightFront, normalRight, textureTopLeft),
                new VertexPositionNormalTexture(bottomRightBack, normalRight, textureBottomRight),
                new VertexPositionNormalTexture(bottomRightFront, normalRight, textureBottomLeft),
            };
        }
    }
}
