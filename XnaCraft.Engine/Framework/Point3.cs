﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace XnaCraft.Engine.Framework
{
    public struct Point3
    {
        private static readonly Point3 _zero = new Point3();

        public int X;
        public int Y;
        public int Z;

        public static Point3 Zero
        {
            get
            {
                return _zero;
            }
        }

        public Point3(int x, int y, int z) {
            X = x;
            Y = y;
            Z = z;
        }

        public static bool operator !=(Point3 a, Point3 b)
        {
            return a.X != b.X || a.Y != b.Y || a.Z != b.Z;
        }

        public static bool operator ==(Point3 a, Point3 b)
        {
            return a.Equals(b);
        }

        public static Point3 operator +(Point3 a, Point3 b)
        {
            return new Point3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static Point3 operator -(Point3 a, Point3 b)
        {
            return new Point3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public static Point3 operator +(Point3 a, int value)
        {
            return new Point3(a.X + value, a.Y + value, a.Z + value);
        }

        public static Point3 operator -(Point3 a, int value)
        {
            return new Point3(a.X - value, a.Y - value, a.Z - value);
        }

        public override bool Equals(object obj)
        {
            if (obj is Point)
            {
                return Equals((Point)obj);
            }

            return false;
        }

        public bool Equals(Point3 other)
        {
            return X == other.X && Y == other.Y && Z == other.Z;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;
                hash = hash * 23 + X;
                hash = hash * 23 + Y;
                hash = hash * 23 + Z;
                return hash;
            }
        }

        public override string ToString()
        {
            return String.Format("X = {0}, Y = {1}, Z = {2}", X, Y, Z);
        }
    }
}
