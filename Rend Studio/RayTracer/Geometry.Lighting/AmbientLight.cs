using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RayTracer.Geometry.Coloring;
using RayTracer.Mathematics;

namespace RayTracer.Geometry.Lighting
{
    public class AmbientLight
    {
        public Color Color { get; private set; }
        public float Brightness { get; private set; }

        public AmbientLight(float brightness, Color color)
        {
            Brightness = brightness;
            Color = color;
        }
    }
}
