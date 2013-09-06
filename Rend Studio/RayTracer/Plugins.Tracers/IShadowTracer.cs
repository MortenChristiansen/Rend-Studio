using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RayTracer.Mathematics;
using RayTracer.Geometry.Lighting;

namespace RayTracer.Plugins.Tracers
{
    public interface IShadowTracer : IPlugin
    {
        float CalculateShade(Vector intersection, PositionedLight lightPrimitive, int depth);
    }
}
