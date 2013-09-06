using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RayTracer.Mathematics;
using RayTracer.Geometry.Coloring;
using System.Windows.Media;
using Color = RayTracer.Geometry.Coloring.Color;
using RayTracer.Engine;

namespace RayTracer.Geometry.Primitives
{
    public class Box : Primitive
    {
        public Vector Size{ get; private set; }

        private const float EPSILON = 0.000001f;
        private BoundingBox _boundingBox;

        public Box(Vector position, Vector size, Material material)
            : this(position, size, material, null)
        {
            
        }

        public Box(Vector position, Vector size, Material material, Texture texture)
            : base(position, material)
        {
            Size = size;
            _boundingBox = new BoundingBox(Position, Size);
            Material.Texture = texture;
        }

        internal override RayCollision Intersect(Ray ray, ref float distance)
        {
            RayCollision retval = RayCollision.Miss;

            float[] distances = new float[] { -1, -1, -1, -1, -1, -1 };
            Vector[] intersections = new Vector[6];
            Vector d = ray.Direction;
            Vector o = ray.Origin;
            Vector v1 = Position;
            Vector v2 = Position + Size;
            if (d.X != 0)
            {
                float rc = 1.0f / d.X;
                distances[0] = (v1.X - o.X) * rc;
                distances[3] = (v2.X - o.X) * rc;
            }
            if (d.Y != 0)
            {
                float rc = 1.0f / d.Y;
                distances[1] = (v1.Y - o.Y) * rc;
                distances[4] = (v2.Y - o.Y) * rc;
            }
            if (d.Z != 0)
            {
                float rc = 1.0f / d.Z;
                distances[2] = (v1.Z - o.Z) * rc;
                distances[5] = (v2.Z - o.Z) * rc;
            }
            for (int i = 0; i < 6; i++) if (distances[i] > 0)
            {
                intersections[i] = o + distances[i] * d;
                if ((intersections[i].X > (v1.X - EPSILON)) && (intersections[i].X < (v2.X + EPSILON)) &&
                    (intersections[i].Y > (v1.Y - EPSILON)) && (intersections[i].Y < (v2.Y + EPSILON)) &&
                    (intersections[i].Z > (v1.Z - EPSILON)) && (intersections[i].Z < (v2.Z + EPSILON)))
                {
                    if (distances[i] < distance)
                    {
                        distance = distances[i];
                        if (_boundingBox.Contains(ray.Origin)) retval = RayCollision.HitFromInsidePrimitive;
                        else retval = RayCollision.Hit;
                    }
                }
            }

            return retval;
        }

        internal override Vector GetNormal(Vector intersection)
        {
            float[] distances = new float[6];
            distances[0] = Size.X - Position.X;
            distances[1] = Size.X + intersection.X - Position.X;
            distances[2] = Size.Y - Position.Y;
            distances[3] = Size.Y + intersection.Y - Position.Y;
            distances[4] = Size.Z - Position.Z;
            distances[5] = Size.Z + intersection.Z - Position.Z;
            int best = 0;
            float bdist = distances[0];
            for (int i = 0; i < 6; i++) if (distances[i] < bdist)
            {
                bdist = distances[i];
                best = i;
            }
            if (best == 0) 
                return new Vector(-1, 0, 0);
            else if (best == 1) 
                return new Vector(1, 0, 0);
            else if (best == 2) 
                return new Vector(0, -1, 0);
            else if (best == 3) 
                return new Vector(0, 1, 0);
            else if (best == 4) 
                return new Vector(0, 0, -1);
            else 
                return new Vector(0, 0, 1);
        }

        internal override BoundingBox GetBoundingBox()
        {
            return _boundingBox;
        }

        internal override bool Intersect(BoundingBox box)
        {
            return box.IntersectVolume(_boundingBox) > 0.0d;
        }

        internal override Color GetColor(Vector intersection)
        {
            if (Texture == null || RenderEngine.Configuration.DisableTextures) return Material.Color;

            for (byte axis = 0; axis < 3; axis++)
            {
                bool rightToLeft = false;
                byte next = Vector.NextAxis[axis];
                byte prev = Vector.PreviousAxis[axis];
                Vector relativePos;

                if (intersection[axis].AlmostEqual(Position[axis]))
                {
                    relativePos = intersection - Position;
                }
                else if (intersection[axis].AlmostEqual((Position + Size)[axis]))
                {
                    relativePos = Vector.AddValue(intersection - intersection, 0, Size.X);
                    rightToLeft = true;
                }
                else continue;

                float x = (relativePos[next] / Texture.Width) * Texture.Width / Size[next];
                float y = (relativePos[prev] / Texture.Height) * Texture.Height / Size[prev];
                if (rightToLeft) x = 1 - x;

                return Texture.GetTexelColor(x, y);
            }
            throw new Exception("Intersection did not intersect!");
            //return Material.Color;
        }
    }
}
