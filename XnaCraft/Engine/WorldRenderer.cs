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
        private readonly BlockRenderer _blockRenderer;
        private readonly DiagnosticsService _diagnosticsService;

        public WorldRenderer(Game game, GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
            _blockRenderer = new BlockRenderer(_graphicsDevice);
            _diagnosticsService = (DiagnosticsService)game.Services.GetService(typeof(DiagnosticsService));
        }

        struct Ray2
        {
            public Vector2 P1;
            public Vector2 P2;

            public Ray2(Vector2 p1, Vector2 p2)
            {
                P1 = p1;
                P2 = p2;
            }
        }

        private IEnumerable<World.Chunk> GetVisibleChunks(World world, Camera camera)
        {
            var ccx = (int)Math.Floor(camera.Position.X / WorldGenerator.CHUNK_SIZE);
            var ccy = (int)Math.Floor(camera.Position.Z / WorldGenerator.CHUNK_SIZE);
            var radius = 3;

            var chunks = new HashSet<World.Chunk>();

            for (var cx = ccx - radius; cx <= ccx + radius; cx++)
            {
                for (var cy = ccy - radius; cy <= ccy + radius; cy++)
                {
                    var chunk = world.GetChunk(cx, cy);

                    chunks.Add(chunk);
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

            foreach (var chunk in chunks)
            {
                for (var x = 0; x < WorldGenerator.CHUNK_SIZE; x++)
                {
                    for (var y = 0; y < WorldGenerator.CHUNK_SIZE; y++)
                    {
                        for (var z = 0; z < WorldGenerator.CHUNK_SIZE; z++)
                        {
                            var descriptor = chunk.Blocks[x, y, z];

                            if (descriptor != null)
                            {
                                var position = new Vector3(chunk.X * WorldGenerator.CHUNK_SIZE + x, y, chunk.Y * WorldGenerator.CHUNK_SIZE + z);

                                var top = y < WorldGenerator.CHUNK_SIZE - 1 ? chunk.Blocks[x, y + 1, z] : null;
                                var bottom = y > 0 ? chunk.Blocks[x, y - 1, z] : null;
                                var front = z > 0 ? chunk.Blocks[x, y, z - 1] : null;
                                var back = z < WorldGenerator.CHUNK_SIZE - 1 ? chunk.Blocks[x, y, z + 1] : null;
                                var left = x > 0 ? chunk.Blocks[x - 1, y, z] : null;
                                var right = x < WorldGenerator.CHUNK_SIZE - 1 ? chunk.Blocks[x + 1, y, z] : null;

                                if (top == null)
                                {
                                    _blockRenderer.AddFace(BlockGeometry.TopFaceOffset, position, descriptor.TextureTop);
                                    faces++;
                                }
                                if (bottom == null && y != 0)
                                {
                                    _blockRenderer.AddFace(BlockGeometry.BottomFaceOffset, position, descriptor.TextureBottom);
                                    faces++;
                                }
                                if (front == null)
                                {
                                    var draw = false;

                                    if (z == 0)
                                    {
                                        var neighbourChunk = world.GetChunk(chunk.X, chunk.Y - 1);

                                        if (neighbourChunk.Blocks != null)
                                        {
                                            draw = neighbourChunk.Blocks[x, y, WorldGenerator.CHUNK_SIZE - 1] == null;
                                        }
                                    }
                                    else
                                    {
                                        draw = true;
                                    }

                                    if (draw)
                                    {
                                        _blockRenderer.AddFace(BlockGeometry.FrontFaceOffset, position, descriptor.TextureSide);
                                        faces++;
                                    }
                                }
                                if (back == null)
                                {
                                    var draw = false;

                                    if (z == WorldGenerator.CHUNK_SIZE - 1)
                                    {
                                        var neighbourChunk = world.GetChunk(chunk.X, chunk.Y + 1);

                                        if (neighbourChunk.Blocks != null)
                                        {
                                            draw = neighbourChunk.Blocks[x, y, 0] == null;
                                        }
                                    }
                                    else
                                    {
                                        draw = true;
                                    }

                                    if (draw)
                                    {
                                        _blockRenderer.AddFace(BlockGeometry.BackFaceOffset, position, descriptor.TextureSide);
                                        faces++;
                                    }
                                }
                                if (left == null)
                                {
                                    var draw = false;

                                    if (x == 0)
                                    {
                                        var neighbourChunk = world.GetChunk(chunk.X - 1, chunk.Y);

                                        if (neighbourChunk.Blocks != null)
                                        {
                                            draw = neighbourChunk.Blocks[WorldGenerator.CHUNK_SIZE - 1, y, z] == null;
                                        }
                                    }
                                    else
                                    {
                                        draw = true;
                                    }

                                    if (draw)
                                    {
                                        _blockRenderer.AddFace(BlockGeometry.LeftFaceOffset, position, descriptor.TextureSide);
                                        faces++;
                                    }
                                }
                                if (right == null)
                                {
                                    var draw = false;

                                    if (x == WorldGenerator.CHUNK_SIZE - 1)
                                    {
                                        var neighbourChunk = world.GetChunk(chunk.X + 1, chunk.Y);

                                        if (neighbourChunk.Blocks != null)
                                        {
                                            draw = neighbourChunk.Blocks[0, y, z] == null;
                                        }
                                    }
                                    else
                                    {
                                        draw = true;
                                    }

                                    if (draw)
                                    {
                                        _blockRenderer.AddFace(BlockGeometry.RightFaceOffset, position, descriptor.TextureSide);
                                        faces++;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            _diagnosticsService.SetInfoValue("Faces", faces);
            _blockRenderer.Render(camera);
        }
    }
}
