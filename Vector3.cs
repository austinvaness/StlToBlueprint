using System;
using System.Globalization;
using System.Text;

namespace Stl2Blueprint
{
    public struct Vector3
    {
        public float x, y, z;

        public Vector3(float xyz)
        {
            x = xyz;
            y = xyz;
            z = xyz;
        }
        public Vector3(Vector3I intVector)
        {
            x = intVector.x;
            y = intVector.y;
            z = intVector.z;
        }

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
            return new Vector3(
                y * other.z - z * other.y,
                z * other.x - x * other.z,
                x * other.y - y * other.x
            );
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
            return new Vector3(v.x + n, v.y + n, v.z + n);
        }

        public static Vector3 operator - (Vector3 v, float n)
        {
            return new Vector3(v.x - n, v.y - n, v.z - n);
        }

        public static Vector3 operator * (Vector3 v, float n)
        {
            return new Vector3(v.x * n, v.y * n, v.z * n);
        }
        public static Vector3 operator * (Vector3 v, Vector3 v2)
        {
            return new Vector3(v.x * v2.x, v.y * v2.y, v.z * v2.z);
        }

        public static Vector3 operator - (Vector3 v)
        {
            return new Vector3(-v.x, v.y, v.z);
        }

        public static Vector3 operator / (Vector3 v, float n)
        {
            return new Vector3(v.x / n, v.y / n, v.z / n);
        }

        public static Vector3 operator / (Vector3 v, Vector3 v2)
        {
            return new Vector3(v.x / v2.x, v.y / v2.y, v.z / v2.z);
        }

        public static Vector3 operator - (Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z);
        }

        public static Vector3 operator + (Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);
        }

        public static bool operator ==(Vector3 v1, Vector3 v2)
        {
            return v1.x == v2.x && v1.y == v2.y && v1.z == v2.z;
        }

        public static bool operator !=(Vector3 v1, Vector3 v2)
        {
            return v1.x != v2.x || v1.y != v2.y || v1.z != v2.z;
        }

        public static bool TryParse(string[] values, int start, out Vector3 v)
        {
            v = new Vector3();
            if(start + 2 >= values.Length)
                return false;
            if (!TryParse(values [start], out float x) || !TryParse(values [start + 1], out float y) || !TryParse(values [start + 2], out float z))
                return false;
            v.x = x;
            v.y = y;
            v.z = z;
            return true;
        }

        private static bool TryParse(string s, out float n)
        {
            return float.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out n);
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

        public static Vector3I Round(Vector3 v)
        {
            return new Vector3I((int)Math.Round(v.x), (int)Math.Round(v.y), (int)Math.Round(v.z));
        }

        public static Vector3I Floor(Vector3 v)
        {
            return new Vector3I((int)v.x, (int)v.y, (int)v.z);
        }

        public static float ScalerProjection(Vector3 v, Vector3 guide)
        {
            return v.Dot(guide);
        }

        public Vector3 Abs()
        {
            return new Vector3(Math.Abs(x), Math.Abs(y), Math.Abs(z));
        }

        public float Min()
        {
            return Math.Min(Math.Min(x, y), z);
        }

        public static Vector3 Min(params Vector3[] vectors)
        {
            Vector3 min = new Vector3(float.MaxValue);
            foreach(Vector3 v in vectors)
            {
                if (v.x < min.x)
                    min.x = v.x;
                if (v.y < min.y)
                    min.y = v.y;
                if (v.z < min.z)
                    min.z = v.z;
            }
            return min;
        }

        public static Vector3 Max(params Vector3[] vectors)
        {
            Vector3 max = new Vector3(float.MinValue);
            foreach (Vector3 v in vectors)
            {
                if (v.x > max.x)
                    max.x = v.x;
                if (v.y > max.y)
                    max.y = v.y;
                if (v.z > max.z)
                    max.z = v.z;
            }
            return max;
        }

        public float Max()
        {
            return Math.Max(Math.Max(x, y), z);
        }

        public override string ToString ()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(x).Append('x');
            sb.Append(y).Append('x');
            sb.Append(z);
            return sb.ToString();
        }

        public static Vector3 Zero = new Vector3();
        public static Vector3 AxisX = new Vector3(1, 0, 0);
        public static Vector3 AxisY = new Vector3(0, 1, 0);
        public static Vector3 AxisZ = new Vector3(0, 0, 1);

    }
}
