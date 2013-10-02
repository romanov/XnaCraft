using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using XnaCraft.Diagnostics;

namespace XnaCraft.Engine
{
    class BlockPosWorldRenderer : IWorldRenderer
    {
        private readonly GraphicsDevice _graphicsDevice;
        private readonly DiagnosticsService _diagnosticsService;

        private Effect _effect;
        private Texture2D _textureAtlas;

        public BlockPosWorldRenderer(Game game)
        {
            _graphicsDevice = game.GraphicsDevice;
            _diagnosticsService = game.GetService<DiagnosticsService>();

            _effect = game.Content.Load<Effect>("BlockPosEffect");
            _textureAtlas = game.Content.Load<Texture2D>("block_pos");
        }

        public void Render(World world, Camera camera)
        {
            _graphicsDevice.BlendState = BlendState.Opaque;
            _graphicsDevice.DepthStencilState = DepthStencilState.Default;
            //_graphicsDevice.RasterizerState = new RasterizerState { FillMode = FillMode.WireFrame, CullMode = CullMode.None };

            var faces = 0;

            var chunks = world.GetVisibleChunks(camera);

            _effect.Parameters["World"].SetValue(Matrix.Identity);
            _effect.Parameters["View"].SetValue(camera.View);
            _effect.Parameters["Projection"].SetValue(camera.Projection);
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
