using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace XnaCraft.Engine
{
    public class BlockRenderer
    {
        private readonly GraphicsDevice _device;

        private BlockGeometry _geometry;
        private BasicEffect _effect;
        private Texture2D _textureAtlas;

        public BlockRenderer(GraphicsDevice device, ContentManager contentManager)
        {
            _device = device;

            _geometry = new BlockGeometry(_device);

            _effect = new BasicEffect(_device);
            _effect.TextureEnabled = true;

            _textureAtlas = contentManager.Load<Texture2D>("blocks");
        }

        struct FaceDescriptor
        {
            public int StartVertex;
            public Vector3 Position;
        }

        private Dictionary<Texture2D, List<FaceDescriptor>> _facesToRender = new Dictionary<Texture2D, List<FaceDescriptor>>();

        public void AddFace(int startVertex, Vector3 position, Texture2D texture)
        {
            var list = default(List<FaceDescriptor>);
            var face = new FaceDescriptor { StartVertex = startVertex, Position = position };

            if (_facesToRender.TryGetValue(texture, out list)) {
                list.Add(face);
            } else {
                _facesToRender.Add(texture, new List<FaceDescriptor> { face });
            }
        }

        public void Render(Camera camera)
        {
            _device.SetVertexBuffer(_geometry.Buffer);

            _effect.View = camera.View;
            _effect.Projection = camera.Projection;
            _effect.Texture = _textureAtlas;

            foreach (var faceToRender in _facesToRender)
            {
                foreach (var face in faceToRender.Value)
                {
                    _effect.World = Matrix.CreateTranslation(face.Position);
                    _effect.CurrentTechnique.Passes[0].Apply();
                    _device.DrawPrimitives(PrimitiveType.TriangleStrip, face.StartVertex, 2);
                }
            }

            foreach (var list in _facesToRender.Values)
            {
                list.Clear();
            }
        }
    }
}
