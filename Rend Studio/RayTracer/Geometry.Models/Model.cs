using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RayTracer.Geometry.Primitives;
using RayTracer.Geometry.Coloring;
using RayTracer.Mathematics;
using RayTracer.Plugins.SpatialStructures;

namespace RayTracer.Geometry.Models
{
    public class Model
    {
        private Face[] _faces;
        private ISpatialStructure _structure;

        internal Model(Face[] faces)
        {
            _faces = faces;
        }

        public List<Primitive> GetTriangles(float scale, Vector offset)
        {
            return GetTriangles(new Vector(scale), offset);
        }

        public List<Primitive> GetTriangles(float scale, Vector offset, Material material)
        {
            return GetTriangles(new Vector(scale), offset, material);
        }

        public List<Primitive> GetTriangles(Vector scale, Vector offset)
        {
            return GetTriangles(scale, offset, null);
        }

        public List<Primitive> GetTriangles(Vector scale, Vector offset, Material material)
        {
            List<Primitive> triangles = new List<Primitive>();

            foreach (var face in _faces)
            {
                Triangle triangle = face.ToTriangle(scale, offset);
                if (material != null) triangle.Material = material;
                triangles.Add(triangle);
            }

            return triangles;
        }

        public Primitive[] GetTriangles()
        {
            Primitive[] triangles = new Primitive[_faces.Length];

            for (int i = 0; i < triangles.Length; i++ )
            {
                triangles[i] = _faces[i].ToTriangle();
            }

            return triangles;
        }

        public Mesh GetMesh(float scale, Vector translation)
        {
            if (_structure == null)
            {
                var kdtree = new KdTree();
                _structure = kdtree.GetStructureInstance(GetTriangles());
            }
            return new Mesh(_structure, scale, translation);
        }
    }
}

//private kd tree (this must not be a local copy but a reference to the original)
//offset vector
//scale value
//transformation matrix
//inherit from primitive
//Whenever an intersection must be made, we translate the ray to the local coordinate view of the kd tree by applying scales and transformations