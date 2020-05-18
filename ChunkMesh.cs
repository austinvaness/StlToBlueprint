using System;
using System.Collections.Generic;

namespace Stl2Blueprint
{
    class ChunkMesh : Mesh
    {
        public struct Chunk
        {
            public Vector3I position;
            public BoundingBox boundingBox;
            public List<Triangle> triangles;

            override public string ToString ()
            {
                return string.Format("P{0},{1},{2}, T:{3}, Bmin:{4}, Bmax:{5}",
                    position.x, position.y, position.z,
                    (triangles == null ? "null" : triangles.Count.ToString()),
                    boundingBox.min, boundingBox.max);
            }

            public int IntersectionCount(Vector3 p)
            {
                if (triangles == null)
                {
                    return 0;
                }
                int count = 0;
                for (int i = 0; i < triangles.Count; i++)
                {
                    if (triangles[i].IntersectsRay(p, Vector3.AxisX))
                        count++;
                }
                return count;
            }

            public bool IntersectsBox(BoundingBox box)
            {
                if (triangles == null)
                {
                    return false;
                }
                foreach (Triangle t in triangles)
                {
                    if (t.IntersectsBox(box))
                        return true;
                }
                return false;
            }
        }

        public Vector3I Size { get; }
        public Vector3 Offset { get; }
        public Vector3 ChunkSize { get; }
        public Chunk[] Chunks { get; }
        public BoundingBox Bounds { get; }

        public ChunkMesh(StandardMesh mesh, Vector3I size, BoundingBox boundingBox)
        {
            Triangle[] Triangles = mesh.Triangles;
            Bounds = mesh.Bounds;
            Offset = boundingBox.min;
            this.Size = size;
            ChunkSize = boundingBox.size / size;
            Chunks = new Chunk[size.x * size.y * size.z];
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    for (int z = 0; z < size.z; z++)
                    {
                        Vector3I pos = new Vector3I(x, y, z);
                        int i = GetIndex(pos);
                        Chunks[i].position = pos;
                        Chunks[i].boundingBox = new BoundingBox(
                            (pos * ChunkSize) + Offset,
                            (pos * ChunkSize) + Offset + ChunkSize
                        );
                    }
                }
            }
            for (int t = 0; t < Triangles.Length; t++)
            {
                Triangle triangle = Triangles[t];
                Vector3I min = BoundingChunk(Vector3.Min(triangle.vertex1, triangle.vertex2, triangle.vertex3));
                Vector3I max = BoundingChunk(Vector3.Max(triangle.vertex1, triangle.vertex2, triangle.vertex3));
                for (int x = min.x; x <= Math.Min(max.x, size.x - 1); x++)
                {
                    for (int y = min.y; y <= Math.Min(max.y, size.y - 1); y++)
                    {
                        for (int z = min.z; z <= Math.Min(max.z, size.z - 1); z++)
                        {
                            Chunk chunk = Chunks[GetIndex(x, y, z)];
                            if (triangle.IntersectsBox(chunk.boundingBox))
                            {
                                if (chunk.triangles == null)
                                {
                                    chunk.triangles = new List<Triangle>();
                                }
                                chunk.triangles.Add(triangle);
                                Chunks[GetIndex(x, y, z)] = chunk;
                            }
                        }
                    }
                }
            }
        }

        public bool ContainsPoint(Vector3 p)
        {
            int count;
            Vector3I chunkPos = BoundingChunk(p);
            Chunk chunk = Chunks[GetIndex(0, chunkPos.y, chunkPos.z)];
            count = chunk.IntersectionCount(p);
            return count % 2 == 1;
        }

        public bool IntersectsBox(BoundingBox box)
        {
            Vector3I min = BoundingChunk(box.min);
            Vector3I max = BoundingChunk(box.max);
            for (int x = min.x; x <= Math.Min(max.x, Size.x - 1); x++)
            {
                for (int y = min.y; y <= Math.Min(max.y, Size.y - 1); y++)
                {
                    for (int z = min.z; z <= Math.Min(max.z, Size.z - 1); z++)
                    {
                        Chunk chunk = Chunks[GetIndex(x, y, z)];
                        if (chunk.IntersectsBox(box))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private int GetIndex(Vector3I position)
        {
            return GetIndex(position.x, position.y, position.z);
        }
        private int GetIndex(int x, int y, int z)
        {
            return x + y * Size.x + z * Size.x * Size.y;
        }
        public Vector3I BoundingChunk(Vector3 position)
        {
            return new Vector3I(
                (int)Math.Floor((position.x - Offset.x) / ChunkSize.x),
                (int)Math.Floor((position.y - Offset.y) / ChunkSize.y),
                (int)Math.Floor((position.z - Offset.z) / ChunkSize.z)
            );
        }
    }
}
