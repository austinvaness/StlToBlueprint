using System;
using System.Collections.Generic;
using System.Deployment.Application;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stl2Blueprint
{
    public class Mesh
    {
        public Triangle[] Triangles { get; }
        public Line [] Edges { get; }
        public Vector3[] Verticies { get; }
        public Vector3 Center { get; }
        public Vector3 Size { get; }
        public Vector3 Min { get; }
        public Vector3 Max { get; }

        public Mesh(IEnumerable<Triangle> triangles, IEnumerable<Line> edges, IEnumerable<Vector3> verticies,
            Vector3 center, Vector3 size, Vector3 min, Vector3 max)
        {
            Triangles = triangles.ToArray();
            Edges = edges.ToArray();
            Verticies = verticies.ToArray();
            Center = center;
            Size = size;
            Min = min;
            Max = max;
        }

        public static Mesh ParseStl(string fileName)
        {
            List<Triangle> triangles = new List<Triangle>();
            HashSet<Line> edges = new HashSet<Line>();
            HashSet<Vector3> verticies = new HashSet<Vector3>();
            Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);

            BinaryReader br = new BinaryReader(File.OpenRead(fileName));
            
            ASCIIEncoding ascii = new ASCIIEncoding();
            string header = ascii.GetString(br.ReadBytes(80));
            if(header.StartsWith("solid"))
            {
                br.Close();
                StreamReader sr = new StreamReader(File.OpenRead(fileName), ascii);
                while(Triangle.Read(sr, out Triangle t))
                {
                    MinMax(ref min, ref max, ref t);
                    triangles.Add(t);
                    edges.Add(t.edge12);
                    edges.Add(t.edge13);
                    edges.Add(t.edge23);
                    verticies.Add(t.vertex1);
                    verticies.Add(t.vertex2);
                    verticies.Add(t.vertex3);
                }
                sr.Close();
            }
            else
            {
                uint count = br.ReadUInt32();
                for (uint i = 0; i < count; i++)
                {
                    Triangle t = Triangle.Read(br);
                    MinMax(ref min, ref max, ref t);
                    triangles.Add(t);
                    edges.Add(t.edge12);
                    edges.Add(t.edge13);
                    edges.Add(t.edge23);
                    verticies.Add(t.vertex1);
                    verticies.Add(t.vertex2);
                    verticies.Add(t.vertex3);
                }
                br.Close();
            }

            Vector3 size = max - min;
            Vector3 center = (size / 2) + min;
            return new Mesh(triangles, edges, verticies, center, size, min, max);
        }

        private static void MinMax(ref Vector3 min, ref Vector3 max, ref Triangle t)
        {
            if (t.vertex1.x > max.x)
                max.x = t.vertex1.x;
            else if (t.vertex1.x < min.x)
                min.x = t.vertex1.x;
            if (t.vertex1.y > max.y)
                max.y = t.vertex1.y;
            else if (t.vertex1.y < min.y)
                min.y = t.vertex1.y;
            if (t.vertex1.z > max.z)
                max.z = t.vertex1.z;
            else if (t.vertex1.z < min.z)
                min.z = t.vertex1.z;

            if (t.vertex2.x > max.x)
                max.x = t.vertex2.x;
            else if (t.vertex2.x < min.x)
                min.x = t.vertex2.x;
            if (t.vertex2.y > max.y)
                max.y = t.vertex2.y;
            else if (t.vertex2.y < min.y)
                min.y = t.vertex2.y;
            if (t.vertex2.z > max.z)
                max.z = t.vertex2.z;
            else if (t.vertex2.z < min.z)
                min.z = t.vertex2.z;

            if (t.vertex3.x > max.x)
                max.x = t.vertex3.x;
            else if (t.vertex3.x < min.x)
                min.x = t.vertex3.x;
            if (t.vertex3.y > max.y)
                max.y = t.vertex3.y;
            else if (t.vertex3.y < min.y)
                min.y = t.vertex3.y;
            if (t.vertex3.z > max.z)
                max.z = t.vertex3.z;
            else if (t.vertex3.z < min.z)
                min.z = t.vertex3.z;
        }

        public bool ContainsPoint (Vector3 p)
        {
            Vector3 dir = Vector3.Normalize(p - Center);
            int count = 0;
            for(int i = 0; i < Triangles.Length; i++)
            {
                if (Triangles[i].IntersectsRay(p, dir))
                    count++;
            }
            return count % 2 == 1;
        }
    }
}
