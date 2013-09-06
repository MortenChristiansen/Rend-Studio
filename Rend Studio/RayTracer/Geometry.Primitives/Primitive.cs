using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RayTracer.Mathematics;
using RayTracer.Geometry.Coloring;
using System.Windows.Media;
using Color = RayTracer.Geometry.Coloring.Color;

namespace RayTracer.Geometry.Primitives
{
    public abstract class Primitive
    {
        public Vector Position;
        public Material Material { get; set; }
        public Texture Texture { get { return Material == null ? null : Material.Texture; } }

        protected Primitive(Vector position)
            : this(position, null)
        {

        }

        protected Primitive(Vector position, Material material)
        {
            Position = position;
            Material = material;
        }

        internal abstract RayCollision Intersect(Ray ray, ref float distance);

        internal abstract Vector GetNormal(Vector intersection);

        internal abstract BoundingBox GetBoundingBox();

        internal abstract bool Intersect(BoundingBox box);

        internal abstract Color GetColor(Vector intersection);
    }
}
