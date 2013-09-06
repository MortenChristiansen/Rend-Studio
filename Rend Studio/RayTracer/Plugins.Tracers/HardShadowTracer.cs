using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RayTracer.Geometry.Lighting;
using RayTracer.Mathematics;
using RayTracer.Geometry;
using RayTracer.Engine;
using RayTracer.Geometry.Primitives;

namespace RayTracer.Plugins.Tracers
{
    public class HardShadowTracer : IShadowTracer
    {
        #region IShadowShader Members

        public float CalculateShade(Vector intersection, PositionedLight lightPrimitive, int depth)
        {
            Vector lightDirection = lightPrimitive.Position - intersection;
            float lightDistance = lightDirection.Length;
            lightDirection = lightDirection.Normal;
            Ray ray = new Ray(intersection + lightDirection * Vector.Epsilon, lightDirection);

            RayCollision collisionresult = RayCollision.Miss;
            Primitive hitPrimitive = RenderEngine.RenderUnit.FindNearest(ref lightDistance, ray, ref collisionresult);
            if (!(hitPrimitive is PositionedLight) && collisionresult != RayCollision.Miss)
            {
                return hitPrimitive.Material.Transparency;
            }
            else
            {
                return 1.0f;
            }
        }

        #endregion

        #region IShader Members

        public string Name
        {
            get { return "Hard Shadows"; }
        }

        #endregion
    }
}
