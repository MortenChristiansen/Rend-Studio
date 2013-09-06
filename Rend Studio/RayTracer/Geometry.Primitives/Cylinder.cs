//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using RayTracer.Mathematics;
//using RayTracer.Geometry.Coloring;
//using Color = RayTracer.Geometry.Coloring.Color;

//namespace RayTracer.Geometry
//{
//    public class Cylinder : Primitive
//    {
//        public float Height { get; private set; }
//        public float Radius { get; private set; }
//        public Vector TopNormal { get; private set; }

//        public Cylinder(Vector centerPosition, float height, float radius, Vector topNormal, Material material)
//            : base(centerPosition, material)
//        {
//            Height = height;
//            Radius = radius;
//            //TopNormal = Vector.Normalize(topNormal);
//            TopNormal = new Vector(0, 1, 0);
//        }

//        internal override RayCollision Intersect(Ray ray, ref float distance)
//        {
//            RayCollision retval = RayCollision.Miss;

//            Vector p0 = ray.Origin;
//            Vector v = ray.Direction;

//            float a = v.X * v.X + v.Z * v.Z;
//            float b = 2 * (p0.X * v.X + p0.Z * v.Z);
//            float c = p0.X * p0.X + p0.Z * p0.Z;
//            float det = b * b - 4 * a * c;
//            if (det > 0)
//            {
//                float i1 = Radius * (-b - det) / 2 * a;
//                float i2 = (-b + det) / 2 * a;

//                if (i1 < 0)
//                {
//                    if (i2 < distance)
//                    {
//                        distance = i2;
//                        retval = RayCollision.HitFromInsidePrimitive;
//                    }
//                }
//                else
//                {
//                    if (i1 < distance)
//                    {
//                        distance = i1;
//                        retval = RayCollision.Hit;
//                    }
//                }
//            }
            

//            /*
//            Vector a = CenterPosition + TopNormal * (Height / 2);
//            Vector b = CenterPosition - TopNormal * (Height / 2);
//            Vector ab = b - a;
//            Vector ao = ray.Origin - a;
//            Vector aoXab = Vector.Cross(ao, ab); //X
//            Vector vXab = Vector.Cross(ray.Direction, ab); //Y
//            double ab2 = Vector.Dot(ab, ab);
//            double fa = Vector.Dot(vXab, vXab);
//            double fb = 2 * Vector.Dot(vXab, aoXab);
//            double fc = Vector.Dot(aoXab, aoXab) - (Radius * Radius * ab2);

//            double det = fb * fb - 4 * fa * fc;
//            if (det == 0)
//            {
//                retval = RayHit.Hit;
//            }
//            if (det > 0)
//            {
//                double i1 = (-fb - det) / 2 * fa;
//                double i2 = (-fb + det) / 2 * fa;

//                if (i1 < 0)
//                {
//                    if (i2 < distance)
//                    {
//                        distance = i2;
//                        retval = RayHit.HitFromInsidePrimitive;
//                    }
//                }
//                else
//                {
//                    if (i1 < distance)
//                    {
//                        distance = i1;
//                        retval = RayHit.Hit;
//                    }
//                }
//            }
//            */
//            return retval;
//        }

//        internal override Vector GetNormal(Vector intersection)
//        {
//            //Check for intersection with top plane
//            float top = Vector.Dot(TopNormal, (intersection - TopNormal * (Height / 2)));
//            if (top == 0)
//            {
//                return TopNormal;
//            }
//            //Check for intersection with bottom plane
//            float bottom = Vector.Dot(-TopNormal, (intersection + TopNormal * (Height / 2)));
//            if (bottom == 0)
//            {
//                return -TopNormal;
//            }
//            //Check for intersection with curved plane
//            Vector un = intersection - (Position + TopNormal);
//            //Project un onto top plane to get normal
//            Vector n = un - (Vector.Dot(un, TopNormal)) * TopNormal;
//            return n.Normal;
//        }

//        internal override BoundingBox GetBoundingBox()
//        {
//            throw new NotImplementedException();
//        }

//        internal override bool Intersect(BoundingBox box)
//        {
//            throw new NotImplementedException();
//        }

//        internal override Color GetColor(Vector intersection)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
