using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RayTracer.Geometry.Coloring;
using RayTracer.Mathematics;
using System.Windows.Media;
using RayTracer.Engine;
using Color = RayTracer.Geometry.Coloring.Color;

namespace RayTracer.Geometry.Primitives
{
    public class Sphere : Primitive
    {
        public float Radius { get; private set; }
        public float RadiusSq { get; private set; }

        private BoundingBox _boundingBox;

        public Sphere(Vector centerPosition, float radius, Material material)
            : this(centerPosition, radius, material, null)
        {
            
        }

        public Sphere(Vector centerPosition, float radius, Material material, Texture texture)
            : base(centerPosition, material)
        {
            Radius = radius;
            RadiusSq = radius * radius;

            Vector halfSize = new Vector(Radius, Radius, Radius);
            _boundingBox = new BoundingBox(Position - halfSize, halfSize * 2);
            Material.Texture = texture;
        }

        internal override RayCollision Intersect(Ray ray, ref float distance)
        {
            Vector v = ray.Origin - Position;
            float b = -Vector.Dot(v, ray.Direction);
            float det = (b * b) - Vector.Dot(v, v) + (Radius * Radius);
            RayCollision retval = RayCollision.Miss;
            if (det > 0)
            {
                det = (float)Math.Sqrt(det);
                float i1 = b - det;
                float i2 = b + det;
                if (i2 > 0)
                {
                    if (i1 < 0)
                    {
                        if (i2 < distance)
                        {
                            distance = i2;
                            retval = RayCollision.HitFromInsidePrimitive;
                        }
                    }
                    else
                    {
                        if (i1 < distance)
                        {
                            distance = i1;
                            retval = RayCollision.Hit;
                        }
                    }
                }
            }

            return retval;
        }

        internal override Vector GetNormal(Vector intersection)
        {
            if (Material.BumpMap == null || RenderEngine.Configuration.DisableTextures) 
                return (intersection - Position).Normal;

            Coordinate baseCoord = GetTextureCoordinates(intersection);
            Color normalColor = Material.BumpMap.GetTexelColor(baseCoord.X, baseCoord.Y);
            float xAngle = (normalColor.R / 255) - 0.5f;
            //float yAngle = (normalColor.R / 255) - 0.5f;
            //float zAngle = (normalColor.R / 255) - 0.5f;
            Vector baseNormal = (intersection - Position).Normal;
            return baseNormal.Rotate(1, xAngle);
            //return ((intersection - Position).Normal + new Vector(normalColor.R - 128, normalColor.G - 128, normalColor.B - 128) * (1 / 128)).Normal;



            //Vector baseNormal = (intersection - Position).Normal;
            //Coordinate baseCoord = GetTextureCoordinates(intersection);
            //float baseOffsetHeight = Material.BumpMap.GetTexelColor(baseCoord.X, baseCoord.Y).GetAverageValue() / 255f;

            //var angleOffsets = new float[2];
            //float xOffset = 1f / (float)Material.BumpMap.Width;
            //float yOffset = 1f / (float)Material.BumpMap.Height;
            //angleOffsets[0] = xOffset / Radius;
            //angleOffsets[1] = yOffset / Radius;

            //Vector sum = new Vector();
            //for (int i = 0; i < 4; i++)
            //{
            //    float xDirection = i % 2 == 0 ? 1 : -1;
            //    float yDirection = i / 2 == 0 ? 1 : -1;
            //    Vector vector = baseNormal.Rotate(Vector.NormalVector(Vector.XUnit, baseNormal), xDirection * angleOffsets[0]);
            //    vector = vector.Rotate(Vector.NormalVector(Vector.YUnit, baseNormal), yDirection * angleOffsets[1]);
            //    Coordinate coordinate = GetTextureCoordinates(Position + vector);
            //    float offsetHeight = Material.BumpMap.GetTexelColor(coordinate.X, coordinate.Y).GetAverageValue() / 255f;
            //    sum += (vector - baseNormal).Normal * (baseOffsetHeight - offsetHeight) * Material.BumpFactor + baseNormal;
            //}

            //var g = sum.Normal;

            //return g;
        }

        internal override BoundingBox GetBoundingBox()
        {
            return _boundingBox;
        }

        internal override bool Intersect(BoundingBox box)
        {
            float dmin = 0;
            Vector spherePos = Position;
            Vector boxPos = box.Position;
            Vector boxSize = box.Size;
            for (byte i = 0; i < 3; i++)
            {
                if (spherePos[i] < boxPos[i])
                {
                    dmin = dmin + (spherePos[i] - boxPos[i]) * (spherePos[i] - boxPos[i]);
                }
                else if (spherePos[i] > (boxPos[i] + boxSize[i]))
                {
                    dmin = dmin + (spherePos[i] - (boxPos[i] + boxSize[i])) * (spherePos[i] - (boxPos[i] + boxSize[i]));
                }
            }
            return (dmin <= RadiusSq);
        }

        internal override Color GetColor(Vector intersection)
        {
            if (Texture == null || RenderEngine.Configuration.DisableTextures) return Material.Color;

            Coordinate coord = GetTextureCoordinates(intersection);

            return Texture.GetTexelColor(coord.X, coord.Y);
        }

        private Coordinate GetTextureCoordinates(Vector intersection)
        {
            Vector vn = new Vector(0, 1, 0);
            Vector ve = new Vector(1, 0, 0);
            Vector vc = Vector.Cross(vn, ve);

            Vector vp = (intersection - Position) * (1.0f / Radius);

            float phi = (float)Math.Acos(-Vector.Dot(vp, vn));
            float v = phi * (1.0f / (float)Math.PI);
            float u;

            float val = Vector.Dot(ve, vp) / (float)Math.Sin(phi);
            val = val > 1 ? 1 : val;
            val = val < -1 ? -1 : val;
            float theta = ((float)Math.Acos(val)) * (0.5f / (float)Math.PI);
            if (Vector.Dot(vc, vp) >= 0)
            {
                u = (1 - theta);
            }
            else
            {
                u = theta;
            }

            if (float.IsNaN(u) || float.IsNaN(v))
            {
                Console.WriteLine(string.Format("Error calculating texture coordinates on sphere: {0}, {1}", u, v));
                Console.WriteLine(string.Format("Sphere position: {0}, {1}, {2}", Position.X, Position.Y, Position.Z));
                Console.WriteLine(string.Format("Sphere intersection: {0}, {1}, {2}", intersection.X, intersection.Y, intersection.Z));
                Console.WriteLine(string.Format("Sphere radius = {0}, intersection deviance = {1}", Radius, Math.Abs(Radius - (intersection - Position).Length)));
                //throw new ArgumentException("The intersection was not valid");
                return new Coordinate(0, 0);
            }

            return new Coordinate(u, v);
        }
    }
}
