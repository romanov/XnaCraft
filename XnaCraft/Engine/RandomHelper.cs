using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XnaCraft.Engine
{
    public static class RandomHelper
    {
        private static readonly Random _random = new Random();

        public static int GetRandomInteger() 
        {
            return _random.Next();
        }
    }
}
