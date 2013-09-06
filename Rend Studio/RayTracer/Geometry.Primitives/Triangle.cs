using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RayTracer.Mathematics;
using RayTracer.Geometry.Coloring;
using RayTracer.Geometry.Models;
using RayTracer.Engine;

namespace RayTracer.Geometry.Primitives
{
    public class Triangle : Primitive
    {
        private readonly BoundingBox _boundingBox;
        private readonly bool _singleNormal = false;
        private Coordinate _textureA;
        private Coordinate _textureB;
        private Coordinate _textureC;

        public readonly Vector A;
        public readonly Vector B;
        public readonly Vector C;

        public readonly Vector NormalA;
        public readonly Vector NormalB;
        public readonly Vector NormalC;

        public Triangle(Vector a, Vector b, Vector c)
            : this(a, b, c, new Vector())
        {

        }

        public Triangle(Vector a, Vector b, Vector c, Vector normal)
            : this(a, b, c, normal, normal, normal)
        {

        }

        public Triangle(Vector a, Vector b, Vector c, Vector normalA, Vector normalB, Vector normalC)
            : base(a)
        {
            A = a;
            B = b;
            C = c;
            NormalA = normalA.Normal;
            NormalB = normalB.Normal;
            NormalC = normalC.Normal;

            _u = new float[RenderEngine.Configuration.Threads];
            _v = new float[RenderEngine.Configuration.Threads];

            _boundingBox = ConstructBoundingBox();
            PerformPrecalculation();
            if ((NormalA == NormalB && NormalB == NormalC) || NormalA.IsNan() || NormalB.IsNan() || NormalC.IsNan()) _singleNormal = true;
        }

        public void SetTexture(Texture texture, Coordinate textureA, Coordinate textureB, Coordinate textureC)
        {
            Material.Texture = texture;
            _textureA = textureA;
            _textureB = textureB;
            _textureC = textureC;
        }

        //Intersect pre-calculation variables
        private Vector _normal;
        private Vector _a;
        //Line 1
        private float _normalYAxis;
        private float _normalZAxis;
        private float _normalD;
        //Line 2
        private float _bNormalY;
        private float _bNormalZ;
        private float _cNormalY;
        private float _cNormalZ;

        private byte _k;

        private float[] _u;
        private float[] _v;

        internal override RayCollision Intersect(Ray ray, ref float distance)
        {
            int threadId = Math.Max(RenderEngine.ExecutingThreadId, 0);

            byte _yAxis = Vector.NextAxis[_k];
            byte _zAxis = Vector.PreviousAxis[_k];
            Vector o = ray.Origin;
            Vector d = ray.Direction;
            float lnd = 1.0f / (d[_k] + _normalYAxis * d[_yAxis] + _normalZAxis * d[_zAxis]);
            float distToPlane = (_normalD - o[_k] - _normalYAxis * o[_yAxis] - _normalZAxis * o[_zAxis]) * lnd;
            if (!(distance > distToPlane && distToPlane > 0)) return RayCollision.Miss;
            float distanceY = o[_yAxis] + distToPlane * d[_yAxis] - _a[_yAxis];
            float distanceZ = o[_zAxis] + distToPlane * d[_zAxis] - _a[_zAxis];
            float β = distanceZ * _bNormalY + distanceY * _bNormalZ;
            _u[threadId] = (float)β;
            if (β < 0) return RayCollision.Miss;
            float γ = distanceY * _cNormalY + distanceZ * _cNormalZ;
            _v[threadId] = (float)γ;
            if (γ < 0 || (β + γ) > 1) return RayCollision.Miss;
            distance = distToPlane;
            return Vector.Dot(d, _normal) > 0 ? RayCollision.HitFromInsidePrimitive : RayCollision.Hit;
        }

        internal override Vector GetNormal(Vector intersection)
        {
            if (_singleNormal)
                return _normal;

            //Normal interpolation:

            Vector ab = B - A;
            Vector aInB = Vector.Project(intersection - A, ab);
            float aOverB = aInB.Length / ab.Length;

            Vector ca = A - C;
            Vector cInA = Vector.Project(intersection - C, ca);
            float cOverA = cInA.Length / ca.Length;

            Vector bc = C - B;
            Vector bInC = Vector.Project(intersection - B, bc);
            Vector cInB = bc - bInC;
            float bOverC = cInB.Length / bc.Length;

            Vector lerp1 = Vector.Lerp(NormalA, NormalB, aOverB);
            Vector lerp2 = Vector.Lerp(NormalC, NormalA, cOverA);
            Vector lerp3 = Vector.Lerp(NormalC, NormalB, bOverC);
            return (lerp1 + lerp2 + lerp3).Normal;
        }

        internal override BoundingBox GetBoundingBox()
        {
            return _boundingBox;
        }

        internal override bool Intersect(BoundingBox box)
        {
            return box.IntersectVolume(_boundingBox) > 0.0f;
        }

        internal override Color GetColor(Vector intersection)
        {
            if (Texture == null || RenderEngine.Configuration.DisableTextures) return Material.Color;

            int threadId = Math.Max(RenderEngine.ExecutingThreadId, 0);

            float U1 = _textureA.X, V1 = _textureA.Y;
            float U2 = _textureB.X, V2 = _textureB.Y;
            float U3 = _textureC.X, V3 = _textureC.Y;
            float u = U1 + _u[threadId] * (U2 - U1) + _v[threadId] * (U3 - U1);
            float v = V1 + _u[threadId] * (V2 - V1) + _v[threadId] * (V3 - V1);
            return Texture.GetTexelColor(u, 1 - v);
        }

        private BoundingBox ConstructBoundingBox()
        {
            Vector bbPosition = Vector.Minimize(A, Vector.Minimize(B, C));
            Vector bbSize = Vector.Maximize(A, Vector.Maximize(B, C)) - bbPosition;

            for (byte axis = 0; axis < 3; axis++)
            {
                if (bbSize[axis] <= 0.0001f)
                {
                    bbSize = Vector.AddValue(bbSize, axis, 0.2f);
                    bbPosition = Vector.AddValue(bbPosition, axis, -0.1f);
                }
            }
            return new BoundingBox(bbPosition, bbSize);
        }

        private void PerformPrecalculation()
        {
            Vector RightVertex = C - A;
            Vector LeftVertex = B - A;
            _normal = -Vector.NormalVector(RightVertex, LeftVertex);
            _a = Position;
            if (Math.Abs(_normal.X) > Math.Abs(_normal.Y))
            {
                if (Math.Abs(_normal.X) > Math.Abs(_normal.Z)) _k = 0; else _k = 2;
            }
            else
            {
                if (Math.Abs(_normal.Y) > Math.Abs(_normal.Z)) _k = 1; else _k = 2;
            }
            byte yAxis = Vector.NextAxis[_k];
            byte zAxis = Vector.PreviousAxis[_k];

            float krec = 1.0f / _normal[_k];
            _normalYAxis = _normal[yAxis] * krec;
            _normalZAxis = _normal[zAxis] * krec;
            _normalD = (_normal * Position) * krec;

            float reci = 1.0f / (RightVertex[yAxis] * LeftVertex[zAxis] - RightVertex[zAxis] * LeftVertex[yAxis]);
            _bNormalY = RightVertex[yAxis] * reci;
            _bNormalZ = -RightVertex[zAxis] * reci;
            _cNormalY = LeftVertex[zAxis] * reci;
            _cNormalZ = -LeftVertex[yAxis] * reci;
        }
    }
}