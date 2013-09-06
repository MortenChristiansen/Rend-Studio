using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RayTracer.Geometry.Coloring;
using RayTracer.Mathematics;
using RayTracer.Geometry;
using RayTracer.Geometry.Primitives;

namespace RayTracer.Plugins.Shaders
{
    public interface IRecurseShader : IPlugin
    {
        void AdjustColor(ref Color colorAcc, Vector intersection, Primitive hitPrimitive, Ray ray, int depth, float rayRefraction, RayCollision collisionResult);
    }
}
