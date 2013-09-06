using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RayTracer.Mathematics;
using RayTracer.Geometry.Primitives;
using RayTracer.Geometry.Coloring;

namespace RayTracer.Geometry.Models
{
    public class ModelBuilder
    {
        private Material _defaultMaterial;

        public Face[] Faces { get; set; }
        public int[] VectorIndices { get; set; }
        public Face[] FaceNormals { get; set; }
        public int[] FaceMaterials { get; set; }
        public Material[] Materials { get; set; }

        public ModelBuilder(Material defaultMaterial)
        {
            _defaultMaterial = defaultMaterial;
        }

        public void PopulateNormals()
        {
            for (int i = 0; i < FaceNormals.Length; i++)
            {
                Faces[i].NormalA = FaceNormals[i].A;
                Faces[i].NormalB = FaceNormals[i].B;
                Faces[i].NormalC = FaceNormals[i].C;
            }
        }

        public void PopulateTextureCoords(Coordinate[] textureCoords)
        {
            for (int i = 0; i < Faces.Length; i++)
            {
                Faces[i].TextureCoordinateA = textureCoords[VectorIndices[i * 3]];
                Faces[i].TextureCoordinateB = textureCoords[VectorIndices[i * 3 + 1]];
                Faces[i].TextureCoordinateC = textureCoords[VectorIndices[i * 3 + 2]];
            }
        }

        public void PopulateMaterials()
        {
            for (int i = 0; i < Faces.Length; i++)
            {
                if (Materials.Length == 0) Faces[i].Material = _defaultMaterial.Clone();
                else Faces[i].Material = Materials[FaceMaterials[i]];
            }
        }
    }
}
