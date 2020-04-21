using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stl2Blueprint
{
    public struct Matrix
    {
        public Matrix (float [,] data)
        {
            this.data = data;
        }

        private float [,] data;

        public float this[int x, int y]
        {
            get
            {
                if (data == null)
                    data = new float [3, 4];
                return data [x, y];
            }
            set
            {
                if (data == null)
                    data = new float [3, 4];
                data [x, y] = value;
            }
        }

        public Vector3 Multiply(Vector3 v, float w)
        {
            return new Vector3(
                data[0, 0] * v.x + data[0, 1] * v.y + data[0, 2] * v.z + data[0, 3] * w,
                data[1, 0] * v.x + data[1, 1] * v.y + data[1, 2] * v.z + data[1, 3] * w,
                data[2, 0] * v.x + data[2, 1] * v.y + data[2, 2] * v.z + data[2, 3] * w
            );
        }
    }
}
