using System;
using System.Collections.Generic;
using System.Deployment.Application;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Stl2Blueprint
{
    public class Mesh
    {
        public Triangle[] Triangles { get; }
        public Line [] Edges { get; }
        public Vector3[] Verticies { get; }
        public BoundingBox Bounds { get; }

        public Mesh(IEnumerable<Triangle> triangles, IEnumerable<Line> edges, IEnumerable<Vector3> verticies,
            Vector3 min, Vector3 max)
        {
            Triangles = triangles.ToArray();
            Edges = edges.ToArray();
            Verticies = verticies.ToArray();
            Bounds = new BoundingBox(min, max);
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
            int errors = 0;
            int total = 0;
            if (header.StartsWith("solid"))
            {
                br.Close();
                StreamReader sr = new StreamReader(File.OpenRead(fileName), ascii);
                while (Triangle.ReadFindAscii(sr, out string triangleLine))
                {
                    if (Triangle.ReadAscii(sr, triangleLine, out Triangle t))
                    {
                        min = Vector3.Min(min, t.aabb.min);
                        max = Vector3.Max(max, t.aabb.max);
                        triangles.Add(t);
                        edges.Add(t.edge12);
                        edges.Add(t.edge31);
                        edges.Add(t.edge23);
                        verticies.Add(t.vertex1);
                        verticies.Add(t.vertex2);
                        verticies.Add(t.vertex3);
                    }
                    else
                    {
                        errors++;
                    }
                    total++;
                }
                sr.Close();
            }
            else
            {
                uint count = br.ReadUInt32();
                total = (int)count;
                for (uint i = 0; i < count; i++)
                {
                    if(Triangle.ReadBinary(br, out Triangle t))
                    {
                        min = Vector3.Min(min, t.aabb.min);
                        max = Vector3.Max(max, t.aabb.max);
                        triangles.Add(t);
                        edges.Add(t.edge12);
                        edges.Add(t.edge31);
                        edges.Add(t.edge23);
                        verticies.Add(t.vertex1);
                        verticies.Add(t.vertex2);
                        verticies.Add(t.vertex3);
                    }
                    else
                    {
                        errors++;
                    }
                }
                br.Close();
            }
            if (errors / (float)total > GlobalConstants.meshErrorThreshold)
                MessageBox.Show(errors + " invalid triangles were ignored in the stl file.");

            return new Mesh(triangles, edges, verticies, min, max);
        }

        public bool ContainsPoint (Vector3 p)
        {
            int count = 0;
            for(int i = 0; i < Triangles.Length; i++)
            {
                if (Triangles[i].IntersectsPoint(p))
                    count++;
            }
            return count % 2 == 1;
        }

        public bool IntersectsBox(BoundingBox box)
        {
            foreach(Triangle t in Triangles)
            {
                if (t.IntersectsBox(box))
                    return true;
            }
            return false;
        }
    }
}
