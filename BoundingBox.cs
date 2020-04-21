using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stl2Blueprint
{
    public struct BoundingBox
    {
        public Vector3 min;
        public Vector3 max;
        public Vector3 center;
        public Vector3 extents;
        public Vector3 size;

        public BoundingBox (Vector3 min, Vector3 max)
        {
            this.min = min;
            this.max = max;
            size = max - min;
            center = (max + min) / 2;
            extents = max - center;
        }

        public bool Intersects(BoundingBox other)
        {
            return other.min.x <= max.x && other.max.x >= min.x &&
                other.min.y <= max.y && other.max.y >= min.y &&
                other.min.z <= max.z && other.max.z >= min.z;
        }

        public bool Contains(Vector3 pos)
        {
            return pos.x >= min.x && pos.x <= max.x &&
                pos.y >= min.y && pos.y <= max.y &&
                pos.z >= min.z && pos.z <= max.z;
        }
    }
}
