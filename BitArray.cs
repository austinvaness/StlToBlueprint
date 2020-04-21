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
    /// An array of bool values stored using ulong values.
    /// Useful for when memory usage is important.
    /// </summary>
    public class BitArray
    {
        private readonly ulong [] data;
        private readonly int realSize;
        private readonly int [] size;

        public int Count { get; private set; }

        public BitArray(int size)
        {
            size = Math.Max(size, 0);
            this.size = new [] { size, 1, 1 };
            realSize = size;
            if (realSize == 0)
                data = new ulong [0];
            else if (realSize % 64 == 0)
                data = new ulong [realSize / 64];
            else
                data = new ulong [(realSize / 64) + 1];
        }

        public BitArray(int sizeX, int sizeY)
        {
            sizeX = Math.Max(sizeX, 0);
            sizeY = Math.Max(sizeY, 0);
            this.size = new [] { sizeX, sizeY, 1 };
            realSize = sizeX * sizeY;
            if (realSize == 0)
                data = new ulong [0];
            else if (realSize % 64 == 0)
                data = new ulong [realSize / 64];
            else
                data = new ulong [(realSize / 64) + 1];
        }
        public BitArray (int sizeX, int sizeY, int sizeZ)
        {
            sizeX = Math.Max(sizeX, 0);
            sizeY = Math.Max(sizeY, 0);
            sizeZ = Math.Max(sizeZ, 0);
            this.size = new [] { sizeX, sizeY, sizeZ };
            realSize = sizeX * sizeY * sizeZ;
            if (realSize == 0)
                data = new ulong [0];
            else if (realSize % 64 == 0)
                data = new ulong [realSize / 64];
            else
                data = new ulong [(realSize / 64) + 1];
        }

        public int Length(int dim)
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
                if (index < 0 || index >= realSize)
                    throw new IndexOutOfRangeException();
                return ((data [index / 64] >> (index % 64)) & 1) == 1;
            }
            set
            {
                if (index < 0 || index >= realSize)
                    throw new IndexOutOfRangeException();
                int pos = index / 64;
                bool set = this [index];
                if (value)
                {
                    if(!set)
                    {
                        data [pos] = data [pos] | ((ulong)1 << (index % 64));
                        Count++;
                    }
                }
                else if(set)
                {
                    data [pos] = data [pos] & ~((ulong)1 << (index % 64));
                    Count--;
                }
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
