using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RayTracer.Geometry.Coloring;
using RayTracer.Mathematics;
using RayTracer.Geometry;

namespace RayTracer.Plugins
{
    public interface ISuperSampler : IPlugin
    {
        Color[] RenderRays(Ray[] rays);
    }
}
