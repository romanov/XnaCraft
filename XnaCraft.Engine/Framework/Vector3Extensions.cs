using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace XnaCraft.Engine.Framework
{
    public static class Vector3Extensions
    {
        public static Point3 ToPoint3(this Vector3 vector)
        {
            return new Point3((int)Math.Round(vector.X), (int)Math.Round(vector.Y), (int)Math.Round(vector.Z));
        }

        public static Vector3 ToVector3(this Point3 point)
        {
            return new Vector3(point.X, point.Y, point.Z);
        }
    }
}
