using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RayTracer.Mathematics
{
    public struct Coordinate
    {
        public readonly float X;
        public readonly float Y;

        public Coordinate(float x, float y)
            : this()
        {
            X = x;
            Y = y;
        }
    }
}
