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
    public class GlossyReflectionShader : IRecurseShader
    {
        private ReflectionColorShader _reflector;

        public GlossyReflectionShader()
        {
            _reflector = new ReflectionColorShader();
        }

        #region IRecurseShader Members

        public void AdjustColor(ref Color colorAcc, Vector intersection, Primitive hitPrimitive, Ray ray, int depth, float rayRefraction, RayCollision collisionResult)
        {
            if (hitPrimitive.Material.Reflection <= 0 || depth >= RenderEngine.Configuration.Depth)
            {
                return;
            }

            if (depth > 2)
            {
                _reflector.AdjustColor(ref colorAcc, intersection, hitPrimitive, ray, depth, rayRefraction, collisionResult);
                return;
            }
            int rayFactor = depth == 1 ? 2 : 1;

            Ray reflectionRay = Ray.GetReflectedRay(hitPrimitive, ray.Direction, intersection);
            int r = 0;
            int g = 0;
            int b = 0;
            foreach (Vector vector in Vector.GetConeScatterVectors(reflectionRay.Direction, 0.1f, rayFactor))
            {
                Color reflectionColor = Color.Black;
                Ray offsetRay = new Ray(intersection + vector * Vector.Epsilon, vector);
                float distance;
                RenderEngine.RenderUnit.RayTrace(offsetRay, ref reflectionColor, depth + 1, rayRefraction, out distance);

                r += reflectionColor.R;
                g += reflectionColor.G;
                b += reflectionColor.B;
            }
            colorAcc = Color.Add(colorAcc, Color.Multiply(new Color((byte)(r * 0.125), (byte)(g * 0.125), (byte)(b * 0.125)), hitPrimitive.Material.Reflection));
        }

        #endregion

        #region IShader Members

        public string Name
        {
            get { return "Glossy Reflection"; }
        }

        #endregion
    }
}
