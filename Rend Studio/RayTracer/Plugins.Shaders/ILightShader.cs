using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RayTracer.Geometry.Coloring;
using RayTracer.Geometry;
using RayTracer.Geometry.Lighting;
using RayTracer.Mathematics;
using RayTracer.Engine;
using RayTracer.Geometry.Primitives;

namespace RayTracer.Plugins.Shaders
{
    public interface ILightShader : IPlugin
    {
        void AdjustColor(ref Color color, Primitive hitPrimitive, PositionedLight lightPrimitive, Vector intersection, Vector originalRayDirection, float shade);
    }
}
