using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RayTracer.Geometry;
using RayTracer.Geometry.Primitives;
using RayTracer.Mathematics;

namespace RayTracer.Plugins.SpatialStructures
{
    public interface ISpatialStructure : IPlugin
    {
        Primitive GetClosestIntersectionPrimitive(Ray ray, ref float distance, out RayCollision collisionResult);
        void Initialize();
        ISpatialStructure GetStructureInstance(Primitive[] primitives);
        BoundingBox BoundingBox { get; }
        int MinimumPrimitives { get; }
    }
}
