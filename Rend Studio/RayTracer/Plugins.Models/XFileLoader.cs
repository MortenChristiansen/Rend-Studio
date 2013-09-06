using System;
using System.Globalization;
using System.IO;
using System.Windows.Media;
using RayTracer.Geometry.Coloring;
using RayTracer.Geometry.Models;
using RayTracer.Mathematics;
using Color = RayTracer.Geometry.Coloring.Color;

namespace RayTracer.Plugins.Models
{
    public class XFileLoader : IModelLoader
    {
        public Model ParseFile(string filePath)
        {
            FileInfo file = new FileInfo(filePath);
            var mesh = new ModelBuilder(new Material(0, 0, 0, 1, 0, Color.Magenta));

            using (StreamReader reader = file.OpenText())
            {
                var parser = new StreamParser(reader, '"', ';', ' ', '\t', ',');

                if (!FindFileIndex("Mesh ", reader)) 
                    return null;
                int[] vectorIndices;
                mesh.Faces = ParseFaces(reader, parser, out vectorIndices);
                mesh.VectorIndices = vectorIndices;

                string nextFileIndex = FindNextFileIndex(reader);

                if (nextFileIndex == "MeshNormals")
                {
                    mesh.FaceNormals = ParseFaces(reader, parser);
                    mesh.PopulateNormals();
                    nextFileIndex = FindNextFileIndex(reader);
                }
                

                if (nextFileIndex == "MeshTextureCoords")
                {
                    mesh.PopulateTextureCoords(parser.ParseCoordinates());
                    nextFileIndex = FindNextFileIndex(reader);
                }

                if (nextFileIndex == "VertexDuplicationIndices")
                {
                    //Find out what they mean
                    nextFileIndex = FindNextFileIndex(reader);
                }

                if (nextFileIndex == "MeshMaterialList")
                {
                    int materialCount = parser.ParseInteger();
                    mesh.FaceMaterials = parser.ParseIntegers(parser.ParseInteger());
                    mesh.Materials = new Material[materialCount];
                    for (int i = 0; i < materialCount; i++)
                    {
                        FindFileIndex("Material", reader);
                        Color color = parser.ParseColor();
                        float number = parser.ParseFloat(); //What is this??
                        Vector a = parser.ParseVector(); //What is this??
                        Vector b = parser.ParseVector(); //What is this??
                        Material material = new Material(0, 0.5f, 0, 0, 0, color);

                        if (FindFileIndexBeforeEndOfScope("TextureFileName", reader))
                        {
                            try
                            {
                                string textureName = parser.ParseString();
                                material.Texture = new Texture(textureName);
                            }
                            catch (ArgumentException)
                            {
                                //Texture was not found
                            }
                        }

                        mesh.Materials[i] = material;
                    }
                    mesh.PopulateMaterials();
                }

                return new Model(mesh.Faces);
            }
        }

        private static bool FindFileIndexBeforeEndOfScope(string id, StreamReader reader)
        {
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine().Trim(' ', '\t');
                if (line == "}") return false;
                if (line.ToLower().StartsWith(id.ToLower()))
                {
                    if (!line.EndsWith("{")) reader.ReadLine(); //Skip line
                    return true;
                }
            }
            return false;
        }

        private static bool FindFileIndex(string id, StreamReader reader)
        {
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine().TrimStart(new char[] { ' ', '\t' });
                if (line.ToLower().StartsWith(id.ToLower()))
                {
                    if (!line.EndsWith("{")) reader.ReadLine(); //Skip line
                    return true;
                }
            }
            return false;
        }

        private static string FindNextFileIndex(StreamReader reader)
        {
            string previous = string.Empty;
            while (!reader.EndOfStream)
            {
                string current = reader.ReadLine().Trim(' ', '\t');
                if (current == "{")
                {
                    if (previous == string.Empty) throw new Exception("Previous file index was nothing!");
                    return previous;
                }
                else if (current.EndsWith("{"))
                {
                    return current.TrimEnd(' ', '{');
                }
                previous = current.Split(new char[]{' '})[0];
            }
            return string.Empty;
        }

        private static Face[] ParseFaces(StreamReader reader, StreamParser parser, out int[] vectorIndices)
        {
            Vector[] vectors = parser.ParseVectors(parser.ParseInteger());
            int count = parser.ParseInteger();
            vectorIndices = new int[count * 3];
            Face[] faces = new Face[count];
            for (int i = 0; i < count; i++)
            {
                string[] values = reader.ReadLine().Trim().Split(new char[] { ';' });
                if (values[0] != "3") throw new Exception("Cannot handle other faces than triangles");
                string[] ids = values[1].Split(new char[] { ',' });
                vectorIndices[i * 3] = Int32.Parse(ids[0]);
                vectorIndices[i * 3 + 1] = Int32.Parse(ids[1]);
                vectorIndices[i * 3 + 2] = Int32.Parse(ids[2]);
                
                faces[i] = new Face(
                    vectors[vectorIndices[i * 3]],
                    vectors[vectorIndices[i * 3 + 1]],
                    vectors[vectorIndices[i * 3 + 2]]);
            }

            return faces;
        }

        private static Face[] ParseFaces(StreamReader reader, StreamParser parser)
        {
            int[] nill;
            return ParseFaces(reader, parser, out nill);
        }

        #region IPlugin Members

        public string Name
        {
            get { return "X Model Loader"; }
        }

        #endregion
    }
}
