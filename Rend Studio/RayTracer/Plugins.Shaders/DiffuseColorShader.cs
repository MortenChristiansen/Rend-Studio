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
    public class DiffuseColorShader : ILightShader
    {
        #region ILightShader Members
        public void AdjustColor(ref Color color, Primitive hitPrimitive, PositionedLight lightPrimitive, Vector intersection, Vector originalRayDirection, float shade)
        {
            if (hitPrimitive.Material.Diffuse <= 0.0f)
                return;

            Color hitPrimitiveColor = hitPrimitive.GetColor(intersection);
            Vector lightDirection = (lightPrimitive.Position - intersection).Normal;
            Vector hitPrimitiveNormal = hitPrimitive.GetNormal(intersection);
            Color appliedColor = lightPrimitive.Material.Color.Filter(hitPrimitiveColor);
            float angle = Vector.Dot(hitPrimitiveNormal, lightDirection);
            if (angle <= 0) return;

            float coefficient = angle * hitPrimitive.Material.Diffuse * shade * lightPrimitive.GetLightCoefficient(intersection);
            color += appliedColor * coefficient;
        }

        #endregion

        #region IShader Members

        public string Name
        {
            get { return "Lambert Shading"; }
        }

        #endregion
    }
}
