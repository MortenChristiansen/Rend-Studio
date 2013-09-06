using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RayTracer.Geometry.Coloring;
using RayTracer.Mathematics;
using RayTracer.Engine;

namespace RayTracer.Geometry.Primitives
{
    public class Plane : Primitive
    {
        private Vector _normal;

        public Plane(Vector position, Vector normal, Material material)
            : base(position, material)
        {
            _normal = normal;
        }

        internal override RayCollision Intersect(Ray ray, ref float distance)
        {
            float d = Position * _normal;
            float dot = Vector.Dot(_normal, ray.Direction);
            if (dot != 0)
            {
                float dist = (d - Vector.Dot(_normal, ray.Origin)) / dot;
                if (dist > 0)
                {
                    if (dist < distance)
                    {
                        distance = dist;
                        if (dot < 0) return RayCollision.Hit;
                        else return RayCollision.HitFromInsidePrimitive;
                    }
                }
            }

            return RayCollision.Miss;
        }

        internal override Vector GetNormal(Vector intersection)
        {
            return _normal;
        }

        internal override BoundingBox GetBoundingBox()
        {
            return new BoundingBox();
        }

        internal override bool Intersect(BoundingBox box)
        {
            return true;
        }

        internal override Color GetColor(Vector intersection)
        {
            //if (Material.Texture == null || RenderEngine.DisableTextures) 
            return Material.Color;
        }
    }
}
