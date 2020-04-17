using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Stl2Blueprint
{
    public struct Vector3
    {
        public float x, y, z;

        public Vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public float Dot(Vector3 other)
        {
            return x * other.x + y * other.y + z * other.z;
        }

        public Vector3 Cross(Vector3 other)
        {
            Vector3 result = new Vector3
            {
                x = y * other.z - z * other.y,
                y = z * other.x - x * other.z,
                z = x * other.y - y * other.x
            };
            return result;
        }

        public static Vector3 Normalize (Vector3 v)
        {
            return v / v.Length();
        }

        public float Length()
        {
            return (float)Math.Sqrt(x * x + y * y + z * z);
        }

        public static Vector3 operator + (Vector3 v, float n)
        {
            Vector3 result = new Vector3();
            result.x = v.x + n;
            result.y = v.y + n;
            result.z = v.z + n;
            return result;
        }

        public static Vector3 operator - (Vector3 v, float n)
        {
            Vector3 result = new Vector3();
            result.x = v.x - n;
            result.y = v.y - n;
            result.z = v.z - n;
            return result;
        }

        public static Vector3 operator * (Vector3 v, float n)
        {
            Vector3 result = new Vector3();
            result.x = v.x * n;
            result.y = v.y * n;
            result.z = v.z * n;
            return result;
        }

        public static Vector3 operator / (Vector3 v, float n)
        {
            Vector3 result = new Vector3();
            result.x = v.x / n;
            result.y = v.y / n;
            result.z = v.z / n;
            return result;
        }

        public static Vector3 operator - (Vector3 v1, Vector3 v2)
        {
            Vector3 result = new Vector3();
            result.x = v1.x - v2.x;
            result.y = v1.y - v2.y;
            result.z = v1.z - v2.z;
            return result;
        }

        public static Vector3 operator + (Vector3 v1, Vector3 v2)
        {
            Vector3 result = new Vector3();
            result.x = v1.x + v2.x;
            result.y = v1.y + v2.y;
            result.z = v1.z + v2.z;
            return result;
        }

        public static bool operator ==(Vector3 v1, Vector3 v2)
        {
            return v1.x == v2.x && v1.y == v2.y && v1.z == v2.z;
        }

        public static bool operator !=(Vector3 v1, Vector3 v2)
        {
            return v1.x != v2.x || v1.y != v2.y || v1.z != v2.z;
        }

        public static Vector3 Parse(string[] values, int start)
        {
            return new Vector3(float.Parse(values [start]), float.Parse(values [start + 1]), float.Parse(values [start + 2]));
        }

        public override bool Equals (object obj)
        {
            return obj is Vector3 vector &&
                   x == vector.x &&
                   y == vector.y &&
                   z == vector.z;
        }

        public override int GetHashCode ()
        {
            int hashCode = 373119288;
            hashCode = hashCode * -1521134295 + x.GetHashCode();
            hashCode = hashCode * -1521134295 + y.GetHashCode();
            hashCode = hashCode * -1521134295 + z.GetHashCode();
            return hashCode;
        }

        public static Vector3 Round(Vector3 v)
        {
            return new Vector3((float)Math.Round(v.x), (float)Math.Round(v.y), (float)Math.Round(v.z));
        }

        public static Vector3 Floor(Vector3 v)
        {
            return new Vector3((int)v.x, (int)v.y, (int)v.z);
        }

        public static float ScalerProjection(Vector3 v, Vector3 guide)
        {
            return v.Dot(guide);
        }

        public float Min()
        {
            return Math.Min(Math.Min(x, y), z);
        }

        public override string ToString ()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(x).Append('x');
            sb.Append(y).Append('x');
            sb.Append(z);
            return sb.ToString();
        }
    }
}
