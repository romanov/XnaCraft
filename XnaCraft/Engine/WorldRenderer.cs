using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using XnaCraft.Diagnostics;

namespace XnaCraft.Engine
{
    class WorldRenderer
    {
        private readonly GraphicsDevice _graphicsDevice;
        private readonly DiagnosticsService _diagnosticsService;

        private Effect _effect;
        private Texture2D _textureAtlas;

        public WorldRenderer(Game game, GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
            _diagnosticsService = (DiagnosticsService)game.Services.GetService(typeof(DiagnosticsService));

            _effect = game.Content.Load<Effect>("BlockEffect");

            _textureAtlas = game.Content.Load<Texture2D>("blocks");
        }

        private IEnumerable<Chunk> GetVisibleChunks(World world, Camera camera)
        {
            var ccx = (int)Math.Floor(camera.Position.X / WorldGenerator.CHUNK_SIZE);
            var ccy = (int)Math.Floor(camera.Position.Z / WorldGenerator.CHUNK_SIZE);
            var radius = 5;

            var chunks = new HashSet<Chunk>();

            for (var cx = ccx - radius; cx <= ccx + radius; cx++)
            {
                for (var cy = ccy - radius; cy <= ccy + radius; cy++)
                {
                    var chunk = world.GetChunk(cx, cy);

                    if (chunk != null)
                    {
                        chunks.Add(chunk);
                    }
                }
            }

            _diagnosticsService.SetInfoValue("Chunks", chunks.Count); 

            return chunks;
        }

        public void Render(World world, Camera camera)
        {
            _graphicsDevice.BlendState = BlendState.Opaque;
            _graphicsDevice.DepthStencilState = DepthStencilState.Default;
            //_graphicsDevice.RasterizerState = new RasterizerState { FillMode = FillMode.WireFrame, CullMode = CullMode.None };

            var faces = 0;

            var chunks = GetVisibleChunks(world, camera);

            //_effect.World = Matrix.CreateTranslation(Vector3.Zero);
            //_effect.View = camera.View;
            //_effect.Projection = camera.Projection;
            //_effect.Texture = _textureAtlas;

            _effect.Parameters["World"].SetValue(Matrix.Identity);
            _effect.Parameters["View"].SetValue(camera.View);
            _effect.Parameters["Projection"].SetValue(camera.Projection);
            _effect.Parameters["CameraPosition"].SetValue(camera.Position);
            _effect.Parameters["AmbientColor"].SetValue(Color.White.ToVector4());
            _effect.Parameters["AmbientIntensity"].SetValue(0.8f);
            _effect.Parameters["FogColor"].SetValue(Color.SkyBlue.ToVector4());
            _effect.Parameters["FogNear"].SetValue(90);
            _effect.Parameters["FogFar"].SetValue(140);
            _effect.Parameters["BlockTexture"].SetValue(_textureAtlas);

            _effect.CurrentTechnique.Passes[0].Apply();

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
