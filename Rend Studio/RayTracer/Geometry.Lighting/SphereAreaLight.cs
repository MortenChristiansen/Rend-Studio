using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RayTracer.Geometry.Primitives;
using RayTracer.Mathematics;

namespace RayTracer.Geometry.Lighting
{
    public class SphereAreaLight : AreaLight
    {
        public SphereAreaLight(float brightness, Sphere shape)
            : base(brightness, shape)
        {

        }

        internal override Vector[] GetSamplingVectors(Vector point)
        {
            return Vector.GetConeScatterVectors(Shape.Position - point, ((Sphere)Shape).Radius, 8);
        }
    }
}
