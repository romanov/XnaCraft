using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace XnaCraft.Engine
{
    public static class Utils
    {
        private static readonly Random _random = new Random();

        public static int GetRandomInteger() 
        {
            return _random.Next();
        }
    }
}
