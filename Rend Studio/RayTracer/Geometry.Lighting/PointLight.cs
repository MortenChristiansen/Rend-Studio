using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using RayTracer.Geometry.Coloring;
using Color = RayTracer.Geometry.Coloring.Color;
using RayTracer.Mathematics;
using RayTracer.Geometry.Primitives;

namespace RayTracer.Geometry.Lighting
{
    public class PointLight : PositionedLight
    {
        public Vector LightPoint { get; private set; }

        public PointLight(float brightness, Vector lightPoint, Color color)
            : this(brightness, lightPoint, color, 5)
        {
        }

        public PointLight(float brightness, Vector lightPoint, Color color, float radius)
            : base(brightness, new Sphere(lightPoint, radius, new Material(0.0f, 1.0f, 0.0f, 1.0f, 1f, color)))
        {
            LightPoint = lightPoint;
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
    }
}
