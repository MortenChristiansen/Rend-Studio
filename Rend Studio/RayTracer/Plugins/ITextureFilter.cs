using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RayTracer.Geometry.Coloring;

namespace RayTracer.Plugins
{
    public interface ITextureFilter : IPlugin
    {
        Color GetTexelColor(float x, float y);
        ITextureFilter GetFilterInstance(Color[,] pixels);
    }
}
