using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RayTracer.Geometry.Primitives;
using RayTracer.Mathematics;

namespace RayTracer.Geometry
{
    public struct BoundingBox
    {
        public readonly Vector Position;
        public readonly Vector Size;

        public BoundingBox(Vector position, Vector size) : this()
        {
            Position = position;
            Size = size;
        }

        public bool Contains(Vector point)
        {
            return point.X >= Position.X && point.X <= Position.X + Size.X &&
                   point.Y >= Position.Y && point.Y <= Position.Y + Size.Y &&
                   point.Z >= Position.Z && point.Z <= Position.Z + Size.Z;
        }

        public Vector[] GetCorners()
        {
            return new Vector[]{
                    Position,
                    Position + new Vector(Size.X, 0, 0),
                    Position + new Vector(0, Size.Y, 0),
                    Position + new Vector(0, 0, Size.Z),
                    Position + Size,
                    Position + new Vector(Size.X, Size.Y, 0),
                    Position + new Vector(0, Size.Y, Size.Z),
                    Position + new Vector(Size.X, 0, Size.Z)
                };
        }

        public float CalculateSurfaceArea()
        {
            return 2 * (Size.X * Size.Y + Size.Y * Size.Z + Size.Z * Size.X);
        }

        public RayCollision Intersect(Ray ray, ref float distance)
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
                float rc = 1f / d.X;
                distances[0] = (v1.X - o.X) * rc;
                distances[3] = (v2.X - o.X) * rc;
            }
            if (d.Y != 0)
            {
                float rc = 1f / d.Y;
                distances[1] = (v1.Y - o.Y) * rc;
                distances[4] = (v2.Y - o.Y) * rc;
            }
            if (d.Z != 0)
            {
                float rc = 1f / d.Z;
                distances[2] = (v1.Z - o.Z) * rc;
                distances[5] = (v2.Z - o.Z) * rc;
            }
            for (int i = 0; i < 6; i++) if (distances[i] > 0)
                {
                    intersections[i] = o + distances[i] * d;
                    if ((intersections[i].X > (v1.X - Vector.Epsilon)) && (intersections[i].X < (v2.X + Vector.Epsilon)) &&
                        (intersections[i].Y > (v1.Y - Vector.Epsilon)) && (intersections[i].Y < (v2.Y + Vector.Epsilon)) &&
                        (intersections[i].Z > (v1.Z - Vector.Epsilon)) && (intersections[i].Z < (v2.Z + Vector.Epsilon)))
                    {
                        if (distances[i] < distance)
                        {
                            distance = distances[i];
                            retval = RayCollision.Hit;
                        }
                    }
                }

            return retval;
        }

        public float IntersectVolume(BoundingBox boundingBox)
        {
            float volume = 0;
            for (byte axis = 0; axis < 3; axis++)
            {
                float len1 = Position[axis] - boundingBox.Position[axis];
                float len2 = boundingBox.Position[axis] + boundingBox.Size[axis] - (Position[axis] + Size[axis]);
                float len3 = boundingBox.Size[axis] - (len1 < 0 ? 0 : len1) - (len2 < 0 ? 0 : len2);
                if (len3 <= 0) return 0;
                if (volume == 0) volume = len3;
                else volume *= len3;
            }
            return volume;
        }

        public static BoundingBox CalculateBoundingBox(List<Primitive> primitives)
        {
            Vector bbPos = Vector.Max;
            Vector bbSize = Vector.Min;

            foreach (Primitive primitive in primitives)
            {
                bbPos = Vector.Minimize(bbPos, primitive.GetBoundingBox().Position);
                bbSize = Vector.Maximize(bbSize, primitive.GetBoundingBox().Position + primitive.GetBoundingBox().Size);
            }

            bbSize -= bbPos;
            return new BoundingBox(bbPos, bbSize);
        }

        public Vector GetCenter()
        {
            return Position + Size * 0.5f;
        }

        public float GetLongestEdge()
        {
            return (float)Math.Max(Size.X, Math.Max(Size.Y, Size.Z));
        }
    }
}
