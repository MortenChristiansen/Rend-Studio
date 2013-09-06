////using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using RayTracer.Mathematics;

//namespace RayTracer.IO2
//{
//    public class Object3D
//    {
//        public System.Collections.ArrayList Materials;
//        public System.Collections.ArrayList Meshes;

//        double rotx, roty, zoom;

//        public void Zoom(double n)
//        {
//            this.zoom = n;
//        }
//        public void RotateX(double n)
//        {
//            this.rotx = n;
//        }
//        public void RotateY(double n)
//        {
//            this.roty = n;
//        }

//        public Object3D()
//        {
//            this.Meshes = new System.Collections.ArrayList();
//            this.Materials = new System.Collections.ArrayList();
//        }
//    }

//    /// <summary>
//    /// material thingy
//    /// </summary>
//    public class Material
//    {
//        public string Name;
//        public string TextureFileName;

//        public bool EnvironmentMap;
//        public bool TextureMapping;

//        public double[] Specular = new double[4];
//        public double[] Diffuse = new double[4];
//        public double[] Ambient = new double[4];
//        public double[] Emission = new double[4];
//        public double[] Shininess = new double[1];
//    }

//    /// <summary>
//    /// a object class, contains only object data (no camera, no subobjects)
//    /// </summary>
//    public class Mesh
//    {
//        public System.Collections.ArrayList Vertices;
//        public System.Collections.ArrayList Faces;
//        public System.Collections.ArrayList Normals;
//        public System.Collections.ArrayList Textures;
//        public Material Material;
//        public bool Texture;
//        public double[] Matrix = new double[16];
//        public bool DrawNormals;
//        public int DisplayList;

//        public Mesh()
//        {
//            this.Vertices = new System.Collections.ArrayList();
//            this.Faces = new System.Collections.ArrayList();
//            this.Normals = new System.Collections.ArrayList();
//            this.Material = new Material();
//            this.Texture = false;
//            this.DisplayList = 0;		// no display list yet
//            Textures = new System.Collections.ArrayList();
//            for (int x = 0; x < 4; x++)
//            {
//                this.Matrix[x * 4 + x] = 1;
//            }
//        }


//        /// <sumary>
//        /// calculates normals, expensive! do not use unless no other option
//        /// if used each frame we will kill you on the spot.. bang!
//        /// </sumary>
//        public void CalculateSmoothNormals()
//        {
//            // most likely the face normals are not done yet
//            this.CalculateFaceNormals();

//            this.Normals = new System.Collections.ArrayList();
//            int index = 0;
//            foreach (Vertex vertex in this.Vertices)
//            {
//                index++;
//                Normal normal = new Normal();
//                foreach (Face face in this.Faces)
//                {
//                    if ((face.v1 == index) || (face.v2 == index) || (face.v3 == index))
//                    {
//                        normal.x += face.Normal.x;
//                        normal.y += face.Normal.y;
//                        normal.z += face.Normal.z;
//                    }
//                }
//                normal.Normalize();
//                this.Normals.Add(normal);
//            }
//        }

//        public void CalculateFaceNormals()
//        {
//            // throw away old normal list and make a new one
//            this.Normals = new System.Collections.ArrayList();

//            // for each frame find the normal
//            foreach (Face face in this.Faces)
//            {
//                Normal normal = new Normal();

//                // get 2 vectors of pollie
//                Vertex v1 = new Vertex((double)(((Vertex)this.Vertices[face.v1]).x - ((Vertex)this.Vertices[face.v3]).x),
//                            (double)(((Vertex)this.Vertices[face.v1]).y - ((Vertex)this.Vertices[face.v3]).y),
//                            (double)(((Vertex)this.Vertices[face.v1]).z - ((Vertex)this.Vertices[face.v3]).z));
//                Vertex v2 = new Vertex((double)(((Vertex)this.Vertices[face.v3]).x - ((Vertex)this.Vertices[face.v2]).x),
//                            (double)(((Vertex)this.Vertices[face.v3]).y - ((Vertex)this.Vertices[face.v2]).y),
//                            (double)(((Vertex)this.Vertices[face.v3]).z - ((Vertex)this.Vertices[face.v2]).z));

//                // do cross product between them
//                normal.x = (v1.y * v2.z) - (v1.z * v2.y);
//                normal.y = (v1.z * v2.x) - (v1.x * v2.z);
//                normal.z = (v1.x * v2.y) - (v1.y * v2.x);

//                // normalize it
//                normal.Normalize();

//                // store normal index
//                face.Normal = normal;
//            }
//        }


//        public void Dump()
//        {
//            foreach (Vertex vertex in this.Vertices)
//            {
//                System.Console.WriteLine(vertex.ToString());
//            }

//            foreach (Face face in this.Faces)
//            {
//                System.Console.WriteLine(face.ToString());
//            }
//        }
//    }

//    /// <summary>
//    /// all info about a single face
//    /// </summary>
//    public class Face
//    {
//        public int v1, v2, v3;
//        public int n1, n2, n3;
//        public int t1, t2, t3;
//        public int Material;
//        public Normal Normal;
//        public override string ToString()
//        {
//            return "vertex<" + this.v1 + "," + this.v2 + "," + this.v3 + ">";
//        }
//    }

//    /// <summary>
//    /// Vertex normal, almost thesame as Vertex
//    /// </summary>
//    public class Normal
//    {
//        public double x, y, z;
//        public void Normalize()
//        {
//            double r = System.Math.Sqrt(this.x * this.x + this.y * this.y + this.z * this.z);
//            if (r != 0)
//            {
//                this.x /= r;
//                this.y /= r;
//                this.z /= r;
//            }
//        }

//        public override string ToString()
//        {
//            return "(" + this.x + "," + this.y + "," + this.z + ")";
//        }

//        public static explicit operator Vector(Normal normal)
//        {
//            return new Vector(normal.y, normal.z, normal.x);
//        }
//    }

//    public class TextureCoordinate
//    {
//        public double u, v;
//    }

//    /// <summary>
//    /// vertex struct, contains all info about a point in the object
//    /// </summary>
//    public class Vertex
//    {
//        public double x, y, z;

//        public Vertex()
//        {
//            this.x = 0;
//            this.y = 0;
//            this.z = 0;
//        }

//        public Vertex(double x, double y, double z)
//        {
//            this.x = x;
//            this.y = y;
//            this.z = z;
//        }

//        public override string ToString()
//        {
//            return "(" + this.x + "," + this.y + "," + this.z + ")";
//        }

//        public static explicit operator Vector(Vertex vertex)
//        {
//            return new Vector(vertex.y, vertex.z, vertex.x);
//        }
//    }

//    public class Loader_3ds
//    {
//        /// our temporary private object3d
//        private Object3D object3d;

//        /// the id's of the chunks from the 3ds file
//        private enum chunkId : ushort
//        {
//            /// 3ds fileformat
//            primary = 0x4D4D,
//            /// file version
//            version = 0x0002,

//            objectInfo = 0x3D3D,

//            /// material chunk
//            material = 0xAFFF,
//            /// material name
//            matName = 0xA000,
//            /// rgb for diffusion
//            matDiffuse = 0xA020,
//            /// texture map
//            matMap = 0xA200,
//            /// texture map filename
//            matMapFile = 0xA300,

//            objectId = 0x4000,
//            /// meshdata
//            objectMesh = 0x4100,
//            /// vertices
//            objectVertices = 0x4110,
//            /// faces
//            objectFaces = 0x4120,
//            /// per face material
//            objectMaterial = 0x4130,
//            /// texture map coordinates
//            objectUV = 0x4140,
//            /// the projection matrix
//            objectMatrix = 0x4160,

//            /// camera (???)
//            cameraId = 0x4700,

//            /// keyframes
//            keyFramesId = 0xB000,
//            /// camera
//            keyFrameCamera = 0xB003,
//            /// keyframe start & stop thingies
//            keyFramesStartStop = 0xB008,
//            /// camera positions
//            keyFrameCameraPositions = 0xB020
//        }

//        /// the file reader for the current file
//        private System.IO.BinaryReader file;

//        /// Here is our structure for our 3DS indicies (since .3DS stores 4 unsigned shorts)
//        //public struct tIndices
//        //{
//        //    ushort a, b, c, bVisible;
//        //}

//        /// This holds the chunk info
//        public class ChunkHeader
//        {
//            /// The chunk's ID
//            public ushort id;
//            /// The length of the chunk
//            public uint length;
//            /// The amount of bytes read within that chunk
//            public uint bytesRead = 0;

//            public override string ToString()
//            {
//                return System.String.Format("id={0:x4}, length={1}, read={2}", this.id, this.length, this.bytesRead);
//            }
//        }

//        /// <summary>
//        /// reads an unsigned short (2 bytes)
//        /// </summary>
//        private ushort readUShort(ChunkHeader chunk)
//        {
//            chunk.bytesRead += 2;
//            return this.file.ReadUInt16();
//        }

//        /// <summary>
//        /// reads an unsigned integer (4 bytes)
//        /// </summary>
//        private uint readUInt32(ChunkHeader chunk)
//        {
//            chunk.bytesRead += 4;
//            return this.file.ReadUInt32();
//        }

//        /// <summary>
//        /// reads a zero terminated ASCII string
//        /// </summary>
//        private string readASCIIz(ChunkHeader chunk)
//        {
//            char temp;
//            string returnValue = "";
//            do
//            {
//                temp = this.file.ReadChar();
//                returnValue += temp;
//            } while (temp != 0);
//            chunk.bytesRead += (uint)returnValue.Length;
//            return returnValue;
//        }


//        /// <summary>
//        /// reads a double (big endian)
//        /// </summary>
//        private double readFloat(ChunkHeader chunk)
//        {
//            return System.BitConverter.ToSingle(System.BitConverter.GetBytes(this.readUInt32(chunk)), 0);
//        }

//        /// <summary>
//        /// reads a list of bytes from a file
//        /// </summary>
//        private void readFile(ChunkHeader chunk, byte[] data)
//        {
//            chunk.bytesRead += (uint)data.Length;
//            data = this.file.ReadBytes(data.Length);
//        }

//        /// <summary>
//        /// reads a chunk header (id+length)
//        /// </summary>
//        private void ReadChunk(ChunkHeader chunkHeader)
//        {
//            chunkHeader.id = this.readUShort(chunkHeader);
//            chunkHeader.length = this.readUInt32(chunkHeader);
//            chunkHeader.bytesRead = 6;
//        }

//        /// <summary>
//        /// skips remaining bytes of chunk
//        /// </summary>
//        private void SkipChunk(ChunkHeader chunkHeader)
//        {
//            this.file.BaseStream.Seek(chunkHeader.length - chunkHeader.bytesRead, System.IO.SeekOrigin.Current);
//            chunkHeader.bytesRead += (chunkHeader.length - chunkHeader.bytesRead);
//        }

//        /// <summary>
//        /// skips remaining bytes of chunk
//        /// </summary>
//        public void ProcessNextChunk(ChunkHeader start)
//        {
//            ChunkHeader currentChunk = new ChunkHeader();

//            while (start.length > start.bytesRead)
//            {
//                this.ReadChunk(currentChunk);
//                System.Console.WriteLine(currentChunk);

//                uint version;
//                switch (currentChunk.id)
//                {
//                    case (ushort)chunkId.version:
//                        version = this.readUInt32(currentChunk);
//                        System.Console.WriteLine("version thiongy" + version);
//                        break;

//                    case (ushort)chunkId.objectInfo:
//                        ChunkHeader tempChunk = new ChunkHeader();
//                        this.ReadChunk(tempChunk);
//                        version = this.readUInt32(currentChunk);
//                        System.Console.WriteLine("object info" + version);
//                        currentChunk.bytesRead += tempChunk.bytesRead;
//                        this.ProcessNextChunk(currentChunk);
//                        break;

//                    case (ushort)chunkId.material:
//                        Material material = new Material();
//                        this.object3d.Materials.Add(material);
//                        this.ProcessNextMaterialChunk(currentChunk);
//                        break;

//                    case (ushort)chunkId.objectId:
//                        System.Console.WriteLine("processing object thingie named : " + this.readASCIIz(currentChunk));

//                        System.Collections.ArrayList arrayTemp = this.ProcessNextObjectChunk(currentChunk);
//                        foreach (object o in arrayTemp)
//                        {
//                            this.object3d.Meshes.Add(o);
//                        }
//                        break;

//                    default:
//                        this.SkipChunk(currentChunk);
//                        break;
//                }
//                start.bytesRead += currentChunk.bytesRead;
//            }
//        }

//        /// <summary>
//        /// in an object chunk process all subchunks
//        /// </summary>
//        public System.Collections.ArrayList ProcessNextObjectChunk(ChunkHeader start)
//        {
//            System.Collections.ArrayList meshes = new System.Collections.ArrayList();
//            Mesh mesh = new Mesh();

//            ChunkHeader currentChunk = new ChunkHeader();

//            while (start.length > start.bytesRead)
//            {
//                this.ReadChunk(currentChunk);
//                System.Console.WriteLine(currentChunk);

//                switch (currentChunk.id)
//                {

//                    case (ushort)chunkId.objectMesh:
//                        System.Collections.ArrayList arrayTemp = this.ProcessNextObjectChunk(currentChunk);
//                        foreach (object o in arrayTemp)
//                        {
//                            meshes.Add(o);
//                        }
//                        break;

//                    case (ushort)chunkId.objectFaces:
//                        ushort faces = this.readUShort(currentChunk);
//                        System.Console.WriteLine("aantal faces : " + faces);

//                        while (faces > 0)
//                        {
//                            Face f = new Face();

//                            // set vertices indexes
//                            f.v1 = (int)this.readUShort(currentChunk);
//                            f.v2 = (int)this.readUShort(currentChunk);
//                            f.v3 = (int)this.readUShort(currentChunk);
//                            // set texture indexes
//                            f.t1 = f.v1;
//                            f.t2 = f.v2;
//                            f.t3 = f.v3;
//                            // set normal indexes
//                            f.n1 = f.v1;
//                            f.n2 = f.v2;
//                            f.n3 = f.v3;

//                            // skip hole/fill
//                            this.readUShort(currentChunk);
//                            // add face to list
//                            mesh.Faces.Add(f);
//                            faces--;
//                        }
//                        this.SkipChunk(currentChunk);
//                        break;
//                    case (ushort)chunkId.objectVertices:
//                        ushort vertices = this.readUShort(currentChunk);
//                        System.Console.WriteLine("aantal vertices : " + vertices);

//                        while (vertices > 0)
//                        {
//                            Vertex v = new Vertex();
//                            v.x = this.readFloat(currentChunk);
//                            v.y = this.readFloat(currentChunk);
//                            v.z = this.readFloat(currentChunk);
//                            mesh.Vertices.Add(v);
//                            vertices--;
//                        }
//                        this.SkipChunk(currentChunk);
//                        break;

//                    case (ushort)chunkId.objectUV:
//                        System.Console.WriteLine("1 ");
//                        mesh.Material.TextureMapping = true;
//                        System.Console.WriteLine("1 ");
//                        ushort textures = this.readUShort(currentChunk);
//                        System.Console.WriteLine("1 ");
//                        System.Console.WriteLine("aantal vertices : " + textures);
//                        System.Console.WriteLine("1 ");

//                        while (textures > 0)
//                        {
//                            TextureCoordinate t = new TextureCoordinate();
//                            t.u = this.readFloat(currentChunk);
//                            t.v = this.readFloat(currentChunk);
//                            mesh.Textures.Add(t);
//                            textures--;
//                        }
//                        this.SkipChunk(currentChunk);
//                        break;

//                    case (ushort)chunkId.objectMatrix:

//                        // fill vertical 3 cells then move to the next horizontal one
//                        for (int i = 0; i < 3 * 4; i++)
//                        {
//                            mesh.Matrix[((i << 2) + ((int)(i / 3))) % 12] = this.readFloat(currentChunk);
//                        }
//                        this.SkipChunk(currentChunk);
//                        break;

//                    default:
//                        this.SkipChunk(currentChunk);
//                        break;
//                }
//                start.bytesRead += currentChunk.bytesRead;
//            }
//            //mesh.CalculateSmoothNormals();
//            mesh.CalculateFaceNormals();
//            meshes.Add(mesh);
//            return meshes;
//        }

//        /// <summary>
//        /// in a material chunk process all subchunks
//        /// </summary>
//        public System.Collections.ArrayList ProcessNextMaterialChunk(ChunkHeader start)
//        {
//            Material material = new Material();
//            System.Collections.ArrayList returnValue = new System.Collections.ArrayList();
//            byte[] tempstring;

//            ChunkHeader currentChunk = new ChunkHeader();

//            while (start.length > start.bytesRead)
//            {
//                this.ReadChunk(currentChunk);

//                switch (currentChunk.id)
//                {
//                    case (ushort)chunkId.matName:
//                        tempstring = new byte[currentChunk.length - currentChunk.bytesRead];
//                        this.readFile(currentChunk, tempstring);
//                        material.Name = System.Text.Encoding.ASCII.GetString(tempstring);
//                        break;

//                    case (ushort)chunkId.matDiffuse:
//                        ChunkHeader diffuse = new ChunkHeader();
//                        this.ReadChunk(diffuse);
//                        byte[] colors = new byte[3];
//                        this.readFile(diffuse, colors);
//                        this.SkipChunk(diffuse);
//                        currentChunk.bytesRead += diffuse.bytesRead;
//                        break;

//                    case (ushort)chunkId.matMap:
//                        System.Collections.ArrayList arrayTemp = ProcessNextMaterialChunk(currentChunk);
//                        foreach (object o in arrayTemp)
//                        {
//                            returnValue.Add(o);
//                        }
//                        break;

//                    case (ushort)chunkId.matMapFile:
//                        tempstring = new byte[currentChunk.length - currentChunk.bytesRead];
//                        this.readFile(currentChunk, tempstring);
//                        material.TextureFileName = System.Text.Encoding.ASCII.GetString(tempstring);
//                        break;

//                    default:
//                        this.SkipChunk(currentChunk);
//                        break;
//                }
//                start.bytesRead += currentChunk.bytesRead;
//            }
//            return returnValue;
//        }


//        /// <summary>
//        /// in a material chunk process all subchunks
//        /// </summary>
//        public Object3D Load(string filename)
//        {
//            this.object3d = new Object3D();

//            Material material = new Material();
//            material.Ambient = new double[] { 0.1d, 0.1d, 0.1d, 1.0d };
//            material.Diffuse = new double[] { 0.2d, 0.2d, 0.2d, 1.0d };
//            material.Specular = new double[] { 1.0d, 1.0d, 1.0d, 1.0d };
//            material.Emission = new double[] { 1.0d, 0.5d, 0.1d, 1.0d };
//            material.Shininess = new double[] { 10.0d };
//            this.object3d.Materials.Add(material);

//            ChunkHeader currentChunk = new ChunkHeader();
//            this.file = new System.IO.BinaryReader(System.IO.File.Open(filename, System.IO.FileMode.Open));
//            this.ReadChunk(currentChunk);
//            if (currentChunk.id != (ushort)chunkId.primary)
//            {
//                System.Console.WriteLine("Not a 3ds file, id = " + currentChunk.id);
//                return null;
//            }

//            this.ProcessNextChunk(currentChunk);

//            return this.object3d;
//        }
//    }
//}
