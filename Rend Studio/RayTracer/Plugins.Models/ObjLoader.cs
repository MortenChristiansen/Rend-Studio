using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RayTracer.Geometry.Models;
using System.IO;
using RayTracer.Mathematics;
using System.Globalization;

namespace RayTracer.Plugins.Models
{
    public class ObjLoader : IModelLoader
    {
        #region IModelLoader Members

        public Model ParseFile(string filePath)
        {
            bool bobj = true;
            if (filePath.IndexOf(".obj") > 0)
                bobj = true;

            string filebody;

            try
            {
                FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                StreamReader reader = new StreamReader(file);
                filebody = reader.ReadToEnd();
                file.Close();
            }
            catch
            {
                Console.WriteLine("File not found:" + filePath);
                return null;
            }

            string[] lines = filebody.Split('\n');

            //Vector ta = new Vector(1, 0, 0);
            //textureArray.Add(ta);
            //Vector tb = new Vector(0, 0, 0);
            //textureArray.Add(tb);
            //Vector tc = new Vector(1, 1, 0);
            //textureArray.Add(tc);
            //Vector td = new Vector(0, 1, 0);
            //textureArray.Add(td);

            var faces = new List<Face>();
            var vertexArray = new List<Vector>();
            var normalArray = new List<Vector>();

            foreach (string line in lines)
            {
                if (line.Length < 1) continue;
                string auxline = line.Replace("  ", " ");
                auxline = auxline.Replace("\r", "");
                auxline = auxline.Replace("\n", "");
                auxline = auxline.TrimEnd();
                if (auxline.Length == 0) continue;
                if (auxline.IndexOf("mtllib") >= 0)
                {
                    auxline = auxline.Replace("mtllib", "");
                }
                else if ((auxline[0] == 'v') && (auxline[1] == ' ')) //we do not read the vertex normals, they are calculated below
                {
                    auxline = auxline.Replace("v ", "");
                    string[] rtPoints = auxline.Split(' ');
                    Vector pt = new Vector
                    (
                        float.Parse(rtPoints[0], NumberStyles.Float, CultureInfo.GetCultureInfo("en-US").NumberFormat),
                        float.Parse(rtPoints[1], NumberStyles.Float, CultureInfo.GetCultureInfo("en-US").NumberFormat),
                        float.Parse(rtPoints[2], NumberStyles.Float, CultureInfo.GetCultureInfo("en-US").NumberFormat)
                    );
                    vertexArray.Add(pt);
                    Vector nrm = new Vector(0, 0, 0);
                    normalArray.Add(nrm);
                }
                else if (auxline[0] == 'f')
                {
                    auxline = auxline.Replace("f ", "");
                    string[] auxsub = auxline.Split(' ');


                    if (auxsub.Length == 3)
                    {
                        string[] vx = auxsub[0].Split('/');
                        string[] vy = auxsub[1].Split('/');
                        string[] vz = auxsub[2].Split('/');


                        int va = int.Parse(vx[0]);// - 1;
                        int vb = int.Parse(vy[0]);// - 1;
                        int vc = int.Parse(vz[0]);// - 1;

                        if (bobj)
                        {
                            va -= 1;
                            vb -= 1;
                            vc -= 1;
                        }

                        Vector A = vertexArray[va];
                        Vector B = vertexArray[vb];
                        Vector C = vertexArray[vc];

                        //int txa = 0, txb = 1, txc = 2;
                        /*if (idx < 0)
                        {
                            txa = 3; txb = 2; txc = 1;
                        }*/

                        int na = va, nb = vb, nc = vc;
                        if (vx.Length == 3)
                        {
                            na = int.Parse(vx[2]);// -1;
                            nb = int.Parse(vy[2]);// -1;
                            nc = int.Parse(vz[2]);// -1;
                            Vector nA = normalArray[na];
                            if (bobj)
                            {
                                na -= 1;
                                nb -= 1;
                                nc -= 1;
                            }
                            Vector nB = normalArray[nb];
                            Vector nC = normalArray[nc];
                            faces.Add(new Face(A, B, C, nA, nB, nC));
                        }
                        else
                        {
                            //3,1,2
                            // todo is triangle va vb vc
                            if (va != vb && vb != vc && va != vc)
                            {
                                faces.Add(new Face(A, B, C));
                                //Triangle tri = new Triangle(this, va, vb, vc, txa, txb, txc, na, nb, nc, mat, txfilename, interpolate);
                                //tri.Init();
                                //AddTriangle(tri);
                                //// update normals
                                //// vertex normals are initially 0,0,0
                                //// each reached vertex makes normal to sum its triangle normal


                                //normalAdd(na, tri.tnormal);
                                //normalAdd(nb, tri.tnormal);
                                //normalAdd(nc, tri.tnormal);
                            }
                        }
                        //idx *= -1;
                    }
                }
            }
            return new Model(faces.ToArray());
        }

        #endregion

        #region IPlugin Members

        public string Name
        {
            get { return "Obj File Loader"; }
        }

        #endregion
    }
}
