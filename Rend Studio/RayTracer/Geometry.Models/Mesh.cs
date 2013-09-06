using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RayTracer.Geometry.Coloring;
using RayTracer.Geometry.Primitives;
using RayTracer.Mathematics;
using RayTracer.Plugins.SpatialStructures;

namespace RayTracer.Geometry.Models
{
    public class Mesh : Primitive
    {
        private ISpatialStructure _structure;
        private Matrix4 _transformation;

        public Mesh(ISpatialStructure structure, float scale, Vector translation)
            : base(new Vector(), null)
        {
            _structure = structure;
            _transformation = Matrix4.ScalingMatrix(1 / scale) * Matrix4.TranslationMatrix(translation.Invert()) ;
        }

        internal override RayCollision Intersect(Ray ray, ref float distance)
        {
            float tempDistance = float.MaxValue;
            var correctedRay = new Ray(_transformation.Transform(ray.Origin), ray.Direction);
            RayCollision collisionResult;
            Primitive collisionPrimitive = _structure.GetClosestIntersectionPrimitive(correctedRay, ref tempDistance, out collisionResult);
            if (collisionResult != RayCollision.Miss) distance = tempDistance;

            return collisionResult;
        }

        internal override Vector GetNormal(Vector intersection)
        {
            throw new NotImplementedException();
        }

        internal override BoundingBox GetBoundingBox()
        {
            return _structure.BoundingBox;
        }

        internal override bool Intersect(BoundingBox box)
        {
            throw new NotImplementedException();
        }

        internal override Color GetColor(Vector intersection)
        {
            throw new NotImplementedException();
        }

        private Vector TransformIntersection(Vector intersection)
        {
            return _transformation.Transform(intersection);
        }
    }
}
//private kd tree (this must not be a local copy but a reference to the original)
//offset vector
//scale value
//transformation matrix
//inherit from primitive
//Whenever an intersection must be made, we translate the ray to the local coordinate view of the kd tree by applying scales and transformations
//Primitive has material - mesh can have multiple materials