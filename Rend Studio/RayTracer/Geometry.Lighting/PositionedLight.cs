using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using RayTracer.Mathematics;
using RayTracer.Engine;
using RayTracer.Geometry.Primitives;

namespace RayTracer.Geometry.Lighting
{
    public abstract class PositionedLight : Primitive
    {
        protected Primitive Shape { get; private set; }

        public float Brightness { get; private set; }

        protected PositionedLight(float brightness, Primitive shape)
            : base(shape.Position, shape.Material)
        {
            Brightness = brightness;
            Shape = shape;
        }

        internal override RayCollision Intersect(Ray ray, ref float distance)
        {
            Shape.Position = Position;
            return Shape.Intersect(ray, ref distance);
        }

        internal override Vector GetNormal(Vector intersection)
        {
            Shape.Position = Position;
            return Shape.GetNormal(intersection);
        }

        public float GetLightCoefficient(Vector intersection)
        {
            float len = 1 + ((Position - intersection).Length / RenderEngine.Scene.WorldScale);
            return Brightness / (len * len);
        }
    }
}
