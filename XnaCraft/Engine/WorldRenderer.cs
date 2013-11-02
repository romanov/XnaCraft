using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using XnaCraft.Engine.Diagnostics;
using Microsoft.Xna.Framework.Content;

namespace XnaCraft.Engine
{
    class WorldRenderer : IWorldRenderer
    {
        private readonly GraphicsDevice _graphicsDevice;
        private readonly DiagnosticsService _diagnosticsService;

        private Effect _effect;
        private Texture2D _textureAtlas;

        public WorldRenderer(GraphicsDevice graphicsDevice, ContentManager contentManager, DiagnosticsService diagnosticsService)
        {
            _graphicsDevice = graphicsDevice;
            _diagnosticsService = diagnosticsService;

            _effect = contentManager.Load<Effect>("BlockEffect");
            _textureAtlas = contentManager.Load<Texture2D>("blocks");
        }

        public void Render(World world, Camera camera)
        {
            _graphicsDevice.BlendState = BlendState.Opaque;
            _graphicsDevice.DepthStencilState = DepthStencilState.Default;
            //_graphicsDevice.RasterizerState = new RasterizerState { FillMode = FillMode.WireFrame, CullMode = CullMode.None };

            _effect.Parameters["World"].SetValue(Matrix.Identity);
            _effect.Parameters["View"].SetValue(camera.View);
            _effect.Parameters["Projection"].SetValue(camera.Projection);
            _effect.Parameters["CameraPosition"].SetValue(camera.Position);
            _effect.Parameters["AmbientColor"].SetValue(Color.White.ToVector4());
            _effect.Parameters["AmbientIntensity"].SetValue(0.8f);
            _effect.Parameters["FogColor"].SetValue(Color.Gray.ToVector4());
            _effect.Parameters["FogNear"].SetValue(150);
            _effect.Parameters["FogFar"].SetValue(200);
            _effect.Parameters["BlockTexture"].SetValue(_textureAtlas);

            _effect.CurrentTechnique.Passes[0].Apply();

            var chunks = world.GetVisibleChunks(camera);
            var faces = 0;

            _diagnosticsService.SetInfoValue("Chunks", chunks.Count());

            foreach (var chunk in chunks)
            {
                faces += chunk.Buffer.VertexCount / 3;
                _graphicsDevice.SetVertexBuffer(chunk.Buffer);
                _graphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, chunk.Buffer.VertexCount / 3);
            }

            _diagnosticsService.SetInfoValue("Faces", faces);
        }
    }
}
