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

        public BoundingBox(Vector3 min, Vector3 max)
        {
            this.min = min;
            this.max = max;
            size = max - min;
            center = (max + min) / 2;
            extents = max - center;
        }
        public BoundingBox(BoundingBox clone)
        {
            this.min = clone.min;
            this.max = clone.max;
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

        //Commented out for not working (maybe)
        //public void Extend(BoundingBox other)
        //{
        //    this.min.x = Math.Min(this.min.x, other.min.x);
        //    this.min.y = Math.Min(this.min.y, other.min.y);
        //    this.min.z = Math.Min(this.min.z, other.min.z);

        //    this.max.x = Math.Max(this.max.x, other.max.x);
        //    this.max.y = Math.Max(this.max.y, other.max.y);
        //    this.max.z = Math.Max(this.max.z, other.max.z);
        //}

        public static BoundingBox Merge(BoundingBox b1, BoundingBox b2)
        {
            return new BoundingBox(new Vector3(
                    Math.Min(b1.min.x, b2.min.x),
                    Math.Min(b1.min.y, b2.min.y),
                    Math.Min(b1.min.z, b2.min.z)
                ), new Vector3(
                    Math.Max(b1.max.x, b2.max.x),
                    Math.Max(b1.max.y, b2.max.y),
                    Math.Max(b1.max.z, b2.max.z)
                ));
        }
    }
}
