using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Stl2Blueprint
{
    public struct Vector3I
    {
        public int x, y, z;

        public Vector3I (int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static Vector3I operator + (Vector3I v, int n)
        {
            return new Vector3I(v.x + n, v.y + n, v.z + n);
        }
        public static Vector3 operator + (Vector3I v, float n)
        {
            return new Vector3(v.x + n, v.y + n, v.z + n);
        }

        public static Vector3I operator - (Vector3I v, int n)
        {
            return new Vector3I(v.x - n, v.y - n, v.z - n);
        }
        public static Vector3 operator - (Vector3I v, float n)
        {
            return new Vector3(v.x - n, v.y - n, v.z - n);
        }

        public static Vector3I operator * (Vector3I v, int n)
        {
            return new Vector3I(v.x * n, v.y * n, v.z * n);
        }
        public static Vector3 operator * (Vector3I v, float n)
        {
            return new Vector3(v.x * n, v.y * n, v.z * n);
        }

        public static Vector3I operator / (Vector3I v, int n)
        {
            return new Vector3I(v.x / n, v.y / n, v.z / n);
        }
        public static Vector3 operator / (Vector3I v, float n)
        {
            return new Vector3(v.x / n, v.y / n, v.z / n);
        }
    }
}
