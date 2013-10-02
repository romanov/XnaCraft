using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace XnaCraft.Engine
{
    public static class GameExtensions
    {
        public static T GetService<T>(this Game game)
        {
            return (T)game.Services.GetService(typeof(T));
        }
    }
}
