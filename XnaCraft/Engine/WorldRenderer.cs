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

        public void Render(World world, Camera camera)
        {
            _graphicsDevice.BlendState = BlendState.Opaque;
            _graphicsDevice.DepthStencilState = DepthStencilState.Default;
            //_graphicsDevice.RasterizerState = new RasterizerState { FillMode = FillMode.WireFrame, CullMode = CullMode.None };

            var chunks = new World.Chunk[3, 3];
            chunks[0, 0] = world.GetChunk(0, 0);
            chunks[0, 1] = world.GetChunk(0, 1);
            chunks[0, 2] = world.GetChunk(0, 2);
            chunks[1, 0] = world.GetChunk(1, 0);
            chunks[1, 1] = world.GetChunk(1, 1);
            chunks[1, 2] = world.GetChunk(1, 2);
            chunks[2, 0] = world.GetChunk(2, 0);
            chunks[2, 1] = world.GetChunk(2, 1);
            chunks[2, 2] = world.GetChunk(2, 2);

            var chunksWidth = chunks.GetLength(0);
            var chunksHeight = chunks.GetLength(1);

            var faces = 0;

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
                                        if (chunk.Y > 0)
                                        {
                                            var neighbourChunk = chunks[chunk.X, chunk.Y - 1];

                                            if (neighbourChunk.Blocks != null)
                                            {
                                                draw = neighbourChunk.Blocks[x, y, WorldGenerator.CHUNK_SIZE - 1] == null;
                                            }
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
                                        if (chunk.Y < chunksHeight - 1)
                                        {
                                            var neighbourChunk = chunks[chunk.X, chunk.Y + 1];

                                            if (neighbourChunk.Blocks != null)
                                            {
                                                draw = neighbourChunk.Blocks[x, y, 0] == null;
                                            }
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
                                        if (chunk.X > 0)
                                        {
                                            var neighbourChunk = chunks[chunk.X - 1, chunk.Y];

                                            if (neighbourChunk.Blocks != null)
                                            {
                                                draw = neighbourChunk.Blocks[WorldGenerator.CHUNK_SIZE - 1, y, z] == null;
                                            }
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
                                        if (chunk.X < chunksWidth - 1)
                                        {
                                            var neighbourChunk = chunks[chunk.X + 1, chunk.Y];

                                            if (neighbourChunk.Blocks != null)
                                            {
                                                draw = neighbourChunk.Blocks[0, y, z] == null;
                                            }
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
