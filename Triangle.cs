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
        public readonly Line edge31;
        public readonly Line edge13;
        public readonly Line edge23;
        public BoundingBox BoundingBox { get; }
        public bool Invalid;
        
        private readonly float planeOffset;


        public Triangle (Vector3 vertex1, Vector3 vertex2, Vector3 vertex3, Vector3 normal)
        {
            this.vertex1 = vertex1;
            this.vertex2 = vertex2;
            this.vertex3 = vertex3;
            edge12 = new Line(vertex1, vertex2);
            edge31 = new Line(vertex3, vertex1);
            edge13 = new Line(vertex1, vertex3);
            edge23 = new Line(vertex2, vertex3);
            BoundingBox = new BoundingBox(Vector3.Min(vertex1, vertex2, vertex3), Vector3.Max(vertex1, vertex2, vertex3));

            if (normal == Vector3.Zero)
                normal = edge12.Dir.Cross(edge13.Dir);
            Invalid = normal == Vector3.Zero;
            this.normal = Vector3.Normalize(normal);
            planeOffset = normal.Dot(vertex1);
        }
        
        public static bool ReadFindAscii(StreamReader sr, out string firstLine)
        {
            firstLine = "";
            if (sr.EndOfStream)
                return false;

            string l = sr.ReadLine().TrimStart();
            while (!l.StartsWith("facet"))
            {
                if (sr.EndOfStream)
                    return false;

                l = sr.ReadLine().TrimStart();
            }
            firstLine = l;
            return true;
        }

        public static bool ReadAscii(StreamReader sr, string firstLine, out Triangle t)
        {
            t = new Triangle();
            string l = firstLine;

            if (!Vector3.TryParse(l.Split(new [] { ' ' }, StringSplitOptions.RemoveEmptyEntries), 2, out Vector3 n))
                return false;

            l = sr.ReadLine().TrimStart();
            while (!l.StartsWith("vertex"))
                l = sr.ReadLine().TrimStart();

            if (!Vector3.TryParse(l.Split(new [] { ' ' }, StringSplitOptions.RemoveEmptyEntries), 1, out Vector3 v1))
                return false;
            l = sr.ReadLine();
            if (!Vector3.TryParse(l.Split(new [] { ' ' }, StringSplitOptions.RemoveEmptyEntries), 1, out Vector3 v2))
                return false;
            l = sr.ReadLine();
            if (!Vector3.TryParse(l.Split(new [] { ' ' }, StringSplitOptions.RemoveEmptyEntries), 1, out Vector3 v3))
                return false;

            l = sr.ReadLine().TrimStart();
            while (!l.StartsWith("endfacet"))
                l = sr.ReadLine().TrimStart();

            t = new Triangle(v1, v2, v3, n);
            return !t.Invalid;
        }

        public static bool ReadBinary(BinaryReader br, out Triangle t)
        {
            Vector3 n = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
            Vector3 v1 = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
            Vector3 v2 = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
            Vector3 v3 = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
            br.ReadUInt16();
            t = new Triangle(v1, v2, v3, n);
            return !t.Invalid;
        }

        // TODO? http://jcgt.org/published/0005/03/03/paper.pdf
        // https://en.wikipedia.org/wiki/M%C3%B6ller%E2%80%93Trumbore_intersection_algorithm#Java_Implementation
        const float EPSILON = 0.0000001f;
        public bool IntersectsRay (Vector3 rayOrigin, Vector3 rayVector)
        {
            Vector3 h = rayVector.Cross(edge13.Vector);
            float a = edge12.Vector.Dot(h);
            if (a > -EPSILON && a < EPSILON)
                return false;    // This ray is parallel to this triangle.
            float f = 1.0f / a;
            Vector3 s = rayOrigin - vertex1;
            float u = f * (s.Dot(h));
            if (u < 0.0 || u > 1.0)
                return false;
            Vector3 q = s.Cross(edge12.Vector);
            float v = f * rayVector.Dot(q);
            if (v < 0.0 || u + v > 1.0)
                return false;
            // At this stage we can compute t to find out where the intersection point is on the line.
            double t = f * edge13.Vector.Dot(q);
            return t > EPSILON;
        }
        
        /// <summary>
        /// Runs a ray intersection check with the +x axis as direction.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool IntersectsPointX(Vector3 point)
        {
            Vector3 edge1 = edge12.Vector;
            Vector3 edge2 = edge13.Vector;
            Vector3 h = new Vector3(0, -edge2.z, edge2.y);
            float a = edge1.y * h.y + edge1.z * h.z;
            if (a > -EPSILON && a < EPSILON)
                return false;    // This ray is parallel to this triangle.
            float f = 1f / a;
            Vector3 s = point - vertex1;
            float u = f * (s.y * h.y + s.z * h.z);
            if (u < 0.0 || u > 1.0)
                return false;
            Vector3 q = s.Cross(edge1);
            float v = f * q.x;
            if (v < 0.0 || u + v > 1.0)
                return false;
            // At this stage we can compute t to find out where the intersection point is on the line.
            double t = f * edge2.Dot(q);
            return t > EPSILON;
        }

        // https://fileadmin.cs.lth.se/cs/Personal/Tomas_Akenine-Moller/pubs/tribox.pdf
        public bool IntersectsBox(BoundingBox box)
        {
            if (!BoundingBox.Intersects(box))
                return false;

            // Plane - AABB intersection
            // https://gdbooks.gitbooks.io/3dcollisions/content/Chapter2/static_aabb_plane.html
            float r = box.extents.x * Math.Abs(normal.x) + box.extents.y * Math.Abs(normal.y) + box.extents.z * Math.Abs(normal.z);
            float dist = normal.Dot(box.center) - planeOffset;
            if (Math.Abs(dist) > r)
                return false;

            Vector3 [] f = new [] { edge12.Vector, edge23.Vector, edge31.Vector };
            Vector3 v0 = vertex1 - box.center;
            Vector3 v1 = vertex2 - box.center;
            Vector3 v2 = vertex3 - box.center;
            for(int j = 0; j < 3; j++)
            {
                Vector3 a = new Vector3(0, -f [j].z, f [j].y);
                float p0 = a.y * v0.y + a.z * v0.z;
                float p1 = a.y * v1.y + a.z * v1.z;
                float p2 = a.y * v2.y + a.z * v2.z;
                float radius = box.extents.y * Math.Abs(a.y) + box.extents.z * Math.Abs(a.z);
                if (Math.Min(Math.Min(p0, p1), p2) > radius || Math.Max(Math.Max(p0, p1), p2) < -radius)
                    return false;
            }
            for (int j = 0; j < 3; j++)
            {
                Vector3 a = new Vector3(f[j].z, 0, -f[j].x);
                float p0 = a.x * v0.x + a.z * v0.z;
                float p1 = a.x * v1.x + a.z * v1.z;
                float p2 = a.x * v2.x + a.z * v2.z;
                float radius = box.extents.x * Math.Abs(a.x) + box.extents.z * Math.Abs(a.z);
                if (Math.Min(Math.Min(p0, p1), p2) > radius || Math.Max(Math.Max(p0, p1), p2) < -radius)
                    return false;
            }
            for (int j = 0; j < 3; j++)
            {
                Vector3 a = new Vector3(-f[j].y, f[j].x, 0);
                float p0 = a.x * v0.x + a.y * v0.y;
                float p1 = a.x * v1.x + a.y * v1.y;
                float p2 = a.x * v2.x + a.y * v2.y;
                float radius = box.extents.x * Math.Abs(a.x) + box.extents.y * Math.Abs(a.y);
                if (Math.Min(Math.Min(p0, p1), p2) > radius || Math.Max(Math.Max(p0, p1), p2) < -radius)
                    return false;
            }
            return true;
        }
    }
}
