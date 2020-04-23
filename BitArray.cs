using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Stl2Blueprint
{
    /// <summary>
    /// An array of bool values stored using bits.
    /// Useful for when memory usage is important.
    /// </summary>
    public class BitArray
    {
        private readonly System.Collections.BitArray data;
        private readonly int [] size;

        public int Count { get; private set; }

        public BitArray(int size)
        {
            size = Math.Max(size, 0);
            this.size = new [] { size, 1, 1 };
            data = new System.Collections.BitArray(size);
        }

        public BitArray(int sizeX, int sizeY)
        {
            sizeX = Math.Max(sizeX, 0);
            sizeY = Math.Max(sizeY, 0);
            this.size = new [] { sizeX, sizeY, 1 };
            data = new System.Collections.BitArray(sizeX * sizeY);
        }
        public BitArray (int sizeX, int sizeY, int sizeZ)
        {
            sizeX = Math.Max(sizeX, 0);
            sizeY = Math.Max(sizeY, 0);
            sizeZ = Math.Max(sizeZ, 0);
            this.size = new [] { sizeX, sizeY, sizeZ };
            data = new System.Collections.BitArray(sizeX * sizeY * sizeZ);
        }

        public int Length(int dim = 0)
        {
            return size [dim];
        }

        public int GetIndex(int x, int y)
        {
            return x + y * size [0];
        }

        public int GetIndex(int x, int y, int z)
        {
            return x + size [0] * (y + size [1] * z);
        }

        public bool this [int index]
        {
            get
            {
                return data [index];
            }
            set
            {
                data [index] = value;
            }
        }

        public bool this [int x, int y]
        {
            get
            {
                return this [GetIndex(x, y)];
            }
            set
            {
                this [GetIndex(x, y)] = value;
            }
        }

        public bool this [int x, int y, int z]
        {
            get
            {
                return this [GetIndex(x, y, z)];
            }
            set
            {
                this [GetIndex(x, y, z)] = value;
            }
        }
    }
}
