using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RayTracer.Geometry.Coloring;
using RayTracer.Mathematics;
using RayTracer.Geometry;
using RayTracer.Engine;
using RayTracer.Geometry.Primitives;

namespace RayTracer.Plugins.Shaders
{
    public class ReflectionColorShader : IRecurseShader
    {
        #region IRecurseShader Members

        public void AdjustColor(ref Color colorAcc, Vector intersection, Primitive hitPrimitive, Ray ray, int depth, float rayRefraction, RayCollision collisionResult)
        {
            if (hitPrimitive.Material.Reflection <= 0 || depth >= RenderEngine.Configuration.Depth)
            {
                return;
            }

            Ray reflectionRay = Ray.GetReflectedRay(hitPrimitive, ray.Direction, intersection);
            Color reflectionColor = Color.Black;
            float distance;
            RenderEngine.RenderUnit.RayTrace(reflectionRay, ref reflectionColor, depth + 1, rayRefraction, out distance);
            colorAcc = Color.Add(colorAcc, Color.Multiply(reflectionColor, hitPrimitive.Material.Reflection));
        }

        #endregion

        #region IShader Members

        public string Name
        {
            get { return "Default Reflections"; }
        }

        #endregion
    }
}
