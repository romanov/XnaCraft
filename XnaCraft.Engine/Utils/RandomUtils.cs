using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XnaCraft.Engine.Utils
{
    public static class RandomUtils
    {
        private static readonly Random Random = new Random();

        public static int GetRandomInteger() 
        {
            return Random.Next();
        }
    }
}
