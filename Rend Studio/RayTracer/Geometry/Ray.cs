using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RayTracer.Mathematics;
using RayTracer.Geometry;
using RayTracer.Engine;
using RayTracer.Geometry.Primitives;

namespace RayTracer.Geometry
{
    public struct Ray
    {
        public readonly Vector Origin;
        public readonly Vector Direction;

        public Ray(Vector origin, Vector direction)
        {
            //normalize direction
            Origin = origin;
            Direction = direction.Normal;
        }

        public static Ray GetReflectedRay(Primitive reflectedPrimitive, Vector originalRayDirection, Vector intersection)
        {
            Vector reflectedPrimitiveNormal = reflectedPrimitive.GetNormal(intersection);
            Vector reflectedDirection = (originalRayDirection - 2f * Vector.Dot(originalRayDirection, reflectedPrimitiveNormal) * reflectedPrimitiveNormal).Normal;
            return new Ray(intersection + reflectedDirection * Vector.Epsilon, reflectedDirection);
        }
    }
}
