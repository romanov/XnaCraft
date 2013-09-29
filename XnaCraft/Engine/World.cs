﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;

namespace XnaCraft.Engine
{
    class World
    {
        private readonly Dictionary<Point, Chunk> _chunks = new Dictionary<Point, Chunk>();

        public World()
        {

        }

        public Chunk GetChunk(int x, int y)
        {
            lock (_chunks)
            {
                var chunk = default(Chunk);

                if (_chunks.TryGetValue(new Point(x, y), out chunk))
                {
                    return chunk;
                }
                else
                {
                    return null;
                }
            }
        }

        public Dictionary<Point, Chunk> GetAdjacentChunks(Chunk chunk)
        {
            var chunks = new[] 
            { 
                GetChunk(chunk.X, chunk.Y - 1),
                GetChunk(chunk.X, chunk.Y + 1),
                GetChunk(chunk.X - 1, chunk.Y),
                GetChunk(chunk.X + 1, chunk.Y),
            };

            return chunks.Where(c => c != null).ToDictionary(c => new Point(c.X, c.Y));
        }

        public void AddChunk(int x, int y, Chunk chunk)
        {
            lock (_chunks)
            {
                _chunks.Add(new Point(x, y), chunk);
            }
        }

        public bool HasChunk(int x, int y)
        {
            lock (_chunks)
            {
                return _chunks.ContainsKey(new Point(x, y));
            }
        }
    }
}
