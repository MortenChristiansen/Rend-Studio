using RayTracer.Geometry.Coloring;
using RayTracer.Mathematics;
using RayTracer.Geometry.Primitives;

namespace RayTracer.Geometry.Models
{
    public class Face
    {
        public readonly Vector A;
        public readonly Vector B;
        public readonly Vector C;
        public Vector NormalA { get; set; }
        public Vector NormalB { get; set; }
        public Vector NormalC { get; set; }
        public Coordinate TextureCoordinateA { get; set; }
        public Coordinate TextureCoordinateB { get; set; }
        public Coordinate TextureCoordinateC { get; set; }
        public Material Material { get; set; }

        public Face(Vector a, Vector b, Vector c)
        {
            A = a;
            B = b;
            C = c;
            NormalA = new Vector();
            NormalB = new Vector();
            NormalC = new Vector();

            Material = new Material();
        }

        public Face(Vector a, Vector b, Vector c, Vector aNormal, Vector bNormal, Vector cNormal)
            : this(a, b, c)
        {
            NormalA = aNormal;
            NormalB = bNormal;
            NormalC = cNormal;
        }

        public Triangle ToTriangle(float scale, Vector offset)
        {
            return ToTriangle(new Vector(scale), offset);
        }

        public Triangle ToTriangle(Vector scale, Vector offset)
        {
            var triangle = new Triangle(A.Scale(scale) + offset, B.Scale(scale) + offset, C.Scale(scale) + offset, NormalA, NormalB, NormalC);
            triangle.Material = Material;
            if (Material.Texture != null) triangle.SetTexture(Material.Texture, TextureCoordinateA, TextureCoordinateB, TextureCoordinateC);
            return triangle;
        }

        public Triangle ToTriangle()
        {
            return ToTriangle(1, new Vector());
        }
    }
}
