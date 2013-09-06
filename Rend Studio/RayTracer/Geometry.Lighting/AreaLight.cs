using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using RayTracer.Geometry.Primitives;
using RayTracer.Mathematics;
using Color = RayTracer.Geometry.Coloring.Color;

namespace RayTracer.Geometry.Lighting
{
    public abstract class AreaLight : PositionedLight
    {
        protected AreaLight(float brightness, Primitive shape)
            : base(brightness, shape)
        {

        }

        internal override BoundingBox GetBoundingBox()
        {
            Shape.Position = Position;
            return Shape.GetBoundingBox();
        }

        internal override bool Intersect(BoundingBox box)
        {
            Shape.Position = Position;
            return Shape.Intersect(box);
        }

        internal override Color GetColor(Vector intersection)
        {
            Shape.Position = Position;
            return Shape.GetColor(intersection);
        }

        internal abstract Vector[] GetSamplingVectors(Vector point);
    }
}
