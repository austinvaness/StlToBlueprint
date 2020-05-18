using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stl2Blueprint
{
    interface Mesh
    {
        BoundingBox Bounds { get; }
        bool ContainsPoint(Vector3 p);

        bool IntersectsBox(BoundingBox box);
    }
}
