using System.Collections.Generic;

namespace Stl2Blueprint
{
    public struct Line
    {
        public readonly Vector3 Start;
        public readonly Vector3 End;
        public readonly float Len;
        public readonly Vector3 Dir;

        public Line (Vector3 start, Vector3 end)
        {
            Start = start;
            End = end;
            Vector3 diff = end - start;
            Len = diff.Length();
            if (Len == 0)
                Dir = new Vector3();
            else
                Dir = diff / Len;

        }

        public static bool operator == (Line v1, Line v2)
        {
            return (v1.Start == v2.Start && v1.End == v2.End) ||
                (v1.End == v2.Start && v1.Start == v2.End);
        }

        public static bool operator != (Line v1, Line v2)
        {
            return (v1.Start != v2.Start || v1.End != v2.End) &&
                (v1.End != v2.Start || v1.Start != v2.End);
        }

        public override bool Equals (object obj)
        {
            return obj is Line edge && ((Start == edge.Start && End == edge.End) || (End == edge.Start && Start == edge.End));
        }

        public override int GetHashCode ()
        {
            return -864309161 + Start.GetHashCode() + End.GetHashCode();
        }
    }
}
