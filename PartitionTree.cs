using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stl2Blueprint
{
    class PartitionTree
	{
		//NOTE: This tree is written with the understanding that nothing will be added to it
		//      after creation. Changing it to accept additions may break many things.
		private ITreeNode rootNode;
		private readonly int leafThreshold = 500;
		public PartitionTree(List<Triangle> triangles, BoundingBox BoundingBox)
		{
			if (triangles.Count < leafThreshold)
			{
				rootNode = new Leaf(triangles, BoundingBox);
			}
			else
			{
				rootNode = new Node(triangles, BoundingBox, leafThreshold);
			}
		}

		public bool ContainsPoint(Vector3 p)
		{
			return rootNode.ContainsPoint(p);
		}
		public bool ContainsBox(BoundingBox box)
		{
			return rootNode.ContainsBox(box);
		}

		private static Random rand = new Random();
		protected static float Normal_distribution(float mean, float stdDev, float min, float max) {
			double u1 = 1.0 - rand.NextDouble(); //uniform(0,1] random doubles
			double u2 = 1.0 - rand.NextDouble();
			double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
						 Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
			double randNormal =
						 mean + stdDev * randStdNormal; //random normal(mean,stdDev^2)
			return Convert.ToSingle(Math.Min(Math.Max(min,randNormal),max));
		}
		protected static BoundingBox calculateBoundingBox(List<Triangle> Triangles)
		{
			BoundingBox BoundingBox = new BoundingBox(Triangles[0].BoundingBox);
			foreach (Triangle t in Triangles)
			{
				BoundingBox.Extend(t.BoundingBox);
			}
			return BoundingBox;
		}

		enum Axis { X, Y, Z}

		interface ITreeNode
		{
			BoundingBox BoundingBox { get; }
			bool ContainsPoint(Vector3 p);
			bool ContainsBox(BoundingBox box);
		}

		class Leaf : ITreeNode
		{
			public BoundingBox BoundingBox { get; }
			private readonly List<Triangle> Triangles;

			public Leaf(List<Triangle> Triangles, BoundingBox? BoundingBox)
			{
				this.Triangles = Triangles;
				if (BoundingBox == null)
				{
					this.BoundingBox = PartitionTree.calculateBoundingBox(Triangles);
				}
			}
			public bool ContainsPoint(Vector3 p)
			{
				int count = 0;
				//Vector3 ray = Center - p;
				for (int i = 0; i < Triangles.Count; i++)
				{
					if (Triangles[i].IntersectsPoint(p))
						count++;
				}
				return count % 2 == 1;
			}

			public bool ContainsBox(BoundingBox box)
			{
				foreach (Triangle t in Triangles)
				{
					if (t.IntersectsBox(box))
						return true;
				}
				return false;
			}
		}

		class Node : ITreeNode
		{
			public BoundingBox BoundingBox { get; }
			private ITreeNode positiveNode;
			private ITreeNode negativeNode;
			private Axis SplitAxis;
			private float SplitPosition;

			public Node(List<Triangle> Triangles, BoundingBox? BoundingBox, int leafThreshold)
			{
				if(BoundingBox == null)
				{
					this.BoundingBox = PartitionTree.calculateBoundingBox(Triangles);
				}
				int attempts = 0;
				int maxAttempts = 50;
				List<Triangle> positiveTriangles = new List<Triangle>();
				List<Triangle> negativeTriangles = new List<Triangle>();
				while (attempts < maxAttempts)
				{
					positiveTriangles.Clear();
					negativeTriangles.Clear();
					//TODO: improve splitting algorithm
					if (this.BoundingBox.extents.y > this.BoundingBox.extents.z)
					{
						SplitAxis = Axis.Y;
						SplitPosition = Normal_distribution(this.BoundingBox.center.y,1,
							this.BoundingBox.min.y,this.BoundingBox.max.y);
					}
					else
					{
						SplitAxis = Axis.Z;
						SplitPosition = Normal_distribution(this.BoundingBox.center.z, 1,
							this.BoundingBox.min.z, this.BoundingBox.max.z);
					}

					foreach (Triangle t in Triangles)
					{
						if (OperativeValue(t.BoundingBox.max) <= SplitPosition)
						{
							negativeTriangles.Add(t);
						}
						else if (OperativeValue(t.BoundingBox.min) > SplitPosition)
						{
							positiveTriangles.Add(t);
						}
						else
						{
							positiveTriangles.Add(t);
							negativeTriangles.Add(t);
						}
					}
					if(positiveTriangles.Count>2*negativeTriangles.Count
						|| negativeTriangles.Count > 2 * positiveTriangles.Count)
					{
						attempts++;
					} else
					{
						break;
					}
				}

				if(attempts >= maxAttempts)
				{
					SplitAxis = Axis.Y;
					SplitPosition = this.BoundingBox.min.y;
					negativeTriangles.Clear();
					positiveTriangles = Triangles;
					leafThreshold = Triangles.Count + 1;
				}

				if (positiveTriangles.Count < leafThreshold)
				{
					positiveNode = new Leaf(positiveTriangles, null);
				}
				else
				{
					positiveNode = new Node(positiveTriangles, null, leafThreshold);
				}

				if (negativeTriangles.Count < leafThreshold)
				{
					negativeNode = new Leaf(positiveTriangles, null);
				}
				else
				{
					negativeNode = new Node(positiveTriangles, null, leafThreshold);
				}
			}

			private float OperativeValue(Vector3 p)
			{
				switch (SplitAxis)
				{
					case Axis.X:
						return p.x;
					case Axis.Y:
						return p.y;
					case Axis.Z:
						return p.z;
					default:
						return 0;
				}
			}
			public bool ContainsPoint(Vector3 p)
			{
				if (!BoundingBox.Contains(p))
					return false;
				return (OperativeValue(p) > SplitPosition) ? positiveNode.ContainsPoint(p) : negativeNode.ContainsPoint(p);
			}

			public bool ContainsBox(BoundingBox box)
			{
				if (!BoundingBox.Intersects(box))
					return false;
				if (OperativeValue(box.min) > SplitPosition)
				{
					return positiveNode.ContainsBox(box);
				}
				if (OperativeValue(box.max) <= SplitPosition)
				{
					return positiveNode.ContainsBox(box);
				}
				return positiveNode.ContainsBox(box) || negativeNode.ContainsBox(box);
			}
		}
	}
}
