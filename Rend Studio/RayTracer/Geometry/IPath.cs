using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RayTracer.Mathematics;

namespace RayTracer.Geometry
{
    public interface IPath
    {
        void AddPoint(Vector point);
        Vector GetValue(float distance);
    }
}
