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
    public class SpecularColorShader : ILightShader
    {
        private const float SpecularPower = 20;

        #region IShader Members

        public string Name
        {
            get { return "Phong"; }
        }

        #endregion

        #region ILightShader Members

        public void AdjustColor(ref Color color, Primitive hitPrimitive, PositionedLight lightPrimitive, Vector intersection, Vector originalRayDirection, float shade)
        {
            if (hitPrimitive.Material.Specular <= 0.0f) return;
            
            Vector lightDirection = (lightPrimitive.Position - intersection).Normal;
            Vector hitPrimitiveNormal = hitPrimitive.GetNormal(intersection);
            Vector reflectedDirection = lightDirection - 2 * Vector.Dot(lightDirection, hitPrimitiveNormal) * hitPrimitiveNormal;
            Vector viewRay = intersection - RenderEngine.Scene.Camera.Position;
            float phongTerm = Math.Max(Vector.Dot(reflectedDirection.Normal, viewRay.Normal), 0);
            float coefficient = hitPrimitive.Material.Specular * shade * lightPrimitive.GetLightCoefficient(intersection);
            phongTerm = (float)Math.Pow(phongTerm, SpecularPower) * coefficient;
            
            color = Color.Add(color, Color.Multiply(lightPrimitive.Material.Color, phongTerm));
        }

        #endregion
    }
}
