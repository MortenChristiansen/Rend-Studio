using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RayTracer.Mathematics;

namespace RayTracer.Geometry
{
    class RayPackage
    {
        public Vector Origin { get; private set; }
        public Vector[] Directions { get; private set; }
        public int Count { get; private set; }

        public RayPackage(Vector origin, Vector[] directions)
        {
            Origin = origin;
            Directions = directions;
            Count = Directions.Count();
        }
    }
}
