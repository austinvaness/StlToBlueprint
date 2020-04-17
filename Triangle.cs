using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stl2Blueprint
{
    public struct Triangle
    {
        public readonly Vector3 vertex1;
        public readonly Vector3 vertex2;
        public readonly Vector3 vertex3;
        public readonly Vector3 normal;
        public readonly Line edge12;
        public readonly Line edge13;
        public readonly Line edge23;

        private readonly Vector3 edge1;
        private readonly Vector3 edge2;
        public Triangle (Vector3 vertex1, Vector3 vertex2, Vector3 vertex3, Vector3 normal)
        {
            this.vertex1 = vertex1;
            this.vertex2 = vertex2;
            this.vertex3 = vertex3;
            edge12 = new Line(vertex1, vertex2);
            edge13 = new Line(vertex1, vertex3);
            edge23 = new Line(vertex2, vertex3);
            this.normal = normal;
            edge1 = vertex2 - vertex1;
            edge2 = vertex3 - vertex1;

        }

        public static bool Parse (string [] lines, ref int start, out Triangle t)
        {
            if (start >= lines.Length)
            {
                t = new Triangle();
                return false;
            }

            string l = lines [start].TrimStart();
            while (!l.StartsWith("facet"))
            {
                start++;
                if (start >= lines.Length)
                {
                    t = new Triangle();
                    return false;
                }
                l = lines [start].TrimStart();

            }

            Vector3 n = Vector3.Parse(l.Split(new [] { ' ' }, StringSplitOptions.RemoveEmptyEntries), 2);
            l = lines [++start].TrimStart();
            
            while(!l.StartsWith("vertex"))
                l = lines [++start].TrimStart();

            Vector3 v1 = Vector3.Parse(l.Split(new [] { ' ' }, StringSplitOptions.RemoveEmptyEntries), 1);
            l = lines [++start].TrimStart();
            Vector3 v2 = Vector3.Parse(l.Split(new [] { ' ' }, StringSplitOptions.RemoveEmptyEntries), 1);
            l = lines [++start].TrimStart();
            Vector3 v3 = Vector3.Parse(l.Split(new [] { ' ' }, StringSplitOptions.RemoveEmptyEntries), 1);
            l = lines [++start].TrimStart();

            while(!l.StartsWith("endfacet"))
                l = lines [++start].TrimStart();

            t = new Triangle(v1, v2, v3, n);
            return true;
        }

        public static bool Read(StreamReader sr, out Triangle t)
        {
            if (sr.EndOfStream)
            {
                t = new Triangle();
                return false;
            }

            string l = sr.ReadLine().TrimStart();
            while (!l.StartsWith("facet"))
            {
                if (sr.EndOfStream)
                {
                    t = new Triangle();
                    return false;
                }

                l = sr.ReadLine().TrimStart();
            }


            Vector3 n = Vector3.Parse(l.Split(new [] { ' ' }, StringSplitOptions.RemoveEmptyEntries), 2);

            l = sr.ReadLine().TrimStart();
            while (!l.StartsWith("vertex"))
                l = sr.ReadLine().TrimStart();

            Vector3 v1 = Vector3.Parse(l.Split(new [] { ' ' }, StringSplitOptions.RemoveEmptyEntries), 1);
            l = sr.ReadLine();
            Vector3 v2 = Vector3.Parse(l.Split(new [] { ' ' }, StringSplitOptions.RemoveEmptyEntries), 1);
            l = sr.ReadLine();
            Vector3 v3 = Vector3.Parse(l.Split(new [] { ' ' }, StringSplitOptions.RemoveEmptyEntries), 1);

            l = sr.ReadLine().TrimStart();
            while (!l.StartsWith("endfacet"))
                l = sr.ReadLine().TrimStart();

            t = new Triangle(v1, v2, v3, n);
            return true;
        }

        public static Triangle Read(BinaryReader br)
        {
            Vector3 n = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
            Vector3 v1 = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
            Vector3 v2 = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
            Vector3 v3 = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
            br.ReadUInt16();
            return new Triangle(v1, v2, v3, n);
        }

        //https://en.wikipedia.org/wiki/M%C3%B6ller%E2%80%93Trumbore_intersection_algorithm#Java_Implementation
        const float EPSILON = 0.0000001f;
        public bool IntersectsRay (Vector3 rayOrigin, Vector3 rayVector)
        {
            Vector3 h = rayVector.Cross(edge2);
            float a = edge1.Dot(h);
            if (a > -EPSILON && a < EPSILON)
                return false;    // This ray is parallel to this triangle.
            float f = 1.0f / a;
            Vector3 s = rayOrigin - vertex1;
            float u = f * (s.Dot(h));
            if (u < 0.0 || u > 1.0)
                return false;
            Vector3 q = s.Cross(edge1);
            float v = f * rayVector.Dot(q);
            if (v < 0.0 || u + v > 1.0)
                return false;
            // At this stage we can compute t to find out where the intersection point is on the line.
            double t = f * edge2.Dot(q);
            return t > EPSILON;
        }
    }
}
