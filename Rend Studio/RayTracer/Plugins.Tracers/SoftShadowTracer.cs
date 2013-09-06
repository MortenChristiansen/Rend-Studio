using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RayTracer.Mathematics;
using RayTracer.Geometry.Lighting;
using RayTracer.Geometry;
using RayTracer.Engine;
using RayTracer.Geometry.Primitives;

namespace RayTracer.Plugins.Tracers
{
    public class SoftShadowTracer : IShadowTracer
    {
        private HardShadowTracer _hardShadowShader;

        public SoftShadowTracer()
        {
            _hardShadowShader = new HardShadowTracer();
        }

        private static float GetShadowFromDirection(Vector intersection, Vector direction)
        {
            float distance = float.MaxValue;
            RayCollision collisionResult = RayCollision.Miss;
            Primitive collisionPrimitive = RenderEngine.RenderUnit.FindNearest(ref distance, new Ray(intersection + direction * Vector.Epsilon, direction), ref collisionResult);
            if (!(collisionPrimitive is PositionedLight) && collisionResult != RayCollision.Miss)
            {
                return collisionPrimitive.Material.Transparency;
            }
            else
            {
                return 1.0f;
            }
        }

        #region IShadowShader Members

        public float CalculateShade(Vector intersection, PositionedLight lightPrimitive, int depth)
        {
            if (lightPrimitive is PointLight)
            {
                return _hardShadowShader.CalculateShade(intersection, lightPrimitive, depth);
            }
            else if (lightPrimitive is AreaLight)
            {
                ValueAggregator ag = new ValueAggregator();
                Vector[] vectors = ((AreaLight)lightPrimitive).GetSamplingVectors(intersection);
                foreach (Vector direction in vectors)
                {
                    ag.AddValue(GetShadowFromDirection(intersection, direction));
                }
                return ag.AverageFloats();
            }
            return 1.0f;
        }

        #endregion

        #region IShader Members

        public string Name
        {
            get { return "Soft Shadows"; }
        }

        #endregion
    }
}
