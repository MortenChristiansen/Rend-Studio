using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RayTracer.Geometry
{
    public enum RayCollision
    {
        Hit = 1,
        Miss = 0,
        HitFromInsidePrimitive = -1
    }
}
