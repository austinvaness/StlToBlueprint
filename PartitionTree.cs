using System;
using System.Collections.Generic;
using System.Drawing;
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
		Axis favouredAxis;
		public PartitionTree(List<Triangle> triangles, Axis favouredAxis)
		{
			Axis axis1;
			Axis axis2;
			switch (favouredAxis)
			{
				case Axis.X:
					axis1 = Axis.Y;
					axis2 = Axis.Z;
					break;
				case Axis.Y:
					axis1 = Axis.X;
					axis2 = Axis.Z;
					break;
				default:
					axis1 = Axis.X;
					axis2 = Axis.Y;
					break;
			}

			this.favouredAxis = favouredAxis;
			rootNode = createNode(triangles, leafThreshold, PartitionTree.calculateBoundingBox(triangles), favouredAxis, axis1, axis2);
		}

		public bool ContainsPoint(Vector3 p)
		{
			return rootNode.ContainsPoint(p);
		}
		public Color getColor(Vector3 p)
		{
			return rootNode.getColor(p);
		}
		public bool ContainsBox(BoundingBox box)
		{
			return rootNode.ContainsBox(box);
		}

		private static Random rand = new Random();
		protected static float Normal_distribution(float mean, float stdDev, float min, float max)
		{
			double u1 = 1.0 - rand.NextDouble(); //uniform(0,1] random doubles
			double u2 = 1.0 - rand.NextDouble();
			double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
						 Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
			double randNormal =
						 mean + stdDev * randStdNormal; //random normal(mean,stdDev^2)
			return Convert.ToSingle(Math.Min(Math.Max(min, randNormal), max));
		}

		protected static BoundingBox calculateBoundingBox(List<Triangle> Triangles)
		{
			BoundingBox BoundingBox = new BoundingBox(Triangles[0].BoundingBox);
			foreach (Triangle t in Triangles)
			{
				BoundingBox = BoundingBox.Merge(BoundingBox,t.BoundingBox);
			}
			return BoundingBox;
		}

		public enum Axis { X, Y, Z}

		interface ITreeNode
		{
			Color getColor(Vector3 p);
			BoundingBox BoundingBox { get; }
			bool ContainsPoint(Vector3 p);
			bool ContainsBox(BoundingBox box);
		}

		class Leaf : ITreeNode
		{
			public BoundingBox BoundingBox { get; }
			private readonly List<Triangle> Triangles;
			public Color color { get; }
			Axis favouredAxis;

			public Leaf(List<Triangle> Triangles, Axis favouredAxis)
			{
				this.favouredAxis = favouredAxis;
				color = Color.FromArgb(rand.Next(0, 255), rand.Next(0, 255), rand.Next(0, 255));
				this.Triangles = Triangles;
				this.BoundingBox = PartitionTree.calculateBoundingBox(Triangles);
			}
			public bool ContainsPoint(Vector3 p)
			{
				int count = 0;
				Vector3 ray;
				switch (favouredAxis)
				{
					case Axis.X:
						ray = Vector3.AxisX;
						break;
					case Axis.Y:
						ray = Vector3.AxisY;
						break;
					default:
						ray = Vector3.AxisZ;
						break;
				}
				for (int i = 0; i < Triangles.Count; i++)
				{
					if (Triangles[i].IntersectsRay(p, -ray))
						count++;
				}
				return count % 2 == 1;
			}

			public Color getColor(Vector3 p)
			{
				return color;
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

		struct SplitAttempt
		{
			public Axis axis;
			public float position;
			public float score;
		}

		static float OperativeValue(Vector3 p, Axis SplitAxis)
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

		static ITreeNode createNode(List<Triangle> Triangles, int leafThreshold, BoundingBox BoundingBox, Axis favouredAxis, Axis axis1, Axis axis2)
		{
			int attemptIndex = 0;
			int maxAttempts = 50;//Per axis
			float distributionFactor = 0.5f;
			SplitAttempt[] splitAttempts = new SplitAttempt[maxAttempts * 2];
			List<Triangle> positiveTriangles = new List<Triangle>();
			List<Triangle> negativeTriangles = new List<Triangle>();
			SplitAttempt splitAttempt = new SplitAttempt();
			for (int i = 0; i < 2; i++)
			{
				splitAttempt.score = 0;
				if (i == 0)
				{
					splitAttempt.axis = axis1;
				}
				else
				{
					splitAttempt.axis = axis2;
				}
				while (attemptIndex < maxAttempts)
				{
					positiveTriangles.Clear();
					negativeTriangles.Clear();

					splitAttempt.position = Normal_distribution(
						OperativeValue(BoundingBox.center, splitAttempt.axis),
						OperativeValue(BoundingBox.extents, splitAttempt.axis) * distributionFactor,
						OperativeValue(BoundingBox.min, splitAttempt.axis),
						OperativeValue(BoundingBox.max, splitAttempt.axis));

					int pCount = 0;
					int nCount = 0;
					splitAttempt.score = 0;
					foreach (Triangle t in Triangles)
					{
						if (OperativeValue(t.BoundingBox.min, splitAttempt.axis) > splitAttempt.position)
						{
							positiveTriangles.Add(t);
							splitAttempt.score++;
						}
						else if (OperativeValue(t.BoundingBox.max, splitAttempt.axis) <= splitAttempt.position)
						{
							negativeTriangles.Add(t);
							splitAttempt.score++;
						}
						else
						{
							splitAttempt.score--;
							positiveTriangles.Add(t);
							negativeTriangles.Add(t);
						}
					}
					splitAttempt.score -= Math.Max(positiveTriangles.Count, negativeTriangles.Count);
					positiveTriangles.Clear();
					negativeTriangles.Clear();
					splitAttempts[i * maxAttempts + attemptIndex] = splitAttempt;
					attemptIndex++;
				}
			}

			float bestScore = float.MinValue;
			int indexOfBestScore = -1;
			for (int a = 0; a < splitAttempts.Length; a++)
			{
				if (bestScore < splitAttempts[a].score)
				{
					bestScore = splitAttempts[a].score;
					indexOfBestScore = a;
				}
			}



			if (indexOfBestScore == -1)
			{
				splitAttempt.axis = axis1;
				splitAttempt.position = OperativeValue(BoundingBox.min, axis1);
				negativeTriangles.Clear();
				positiveTriangles = Triangles;
			}
			else
			{

				splitAttempt = splitAttempts[indexOfBestScore];
				positiveTriangles.Clear();
				negativeTriangles.Clear();
				foreach (Triangle t in Triangles)
				{
					if (OperativeValue(t.BoundingBox.min, splitAttempt.axis) > splitAttempt.position)
					{
						positiveTriangles.Add(t);
					}
					else if (OperativeValue(t.BoundingBox.max, splitAttempt.axis) <= splitAttempt.position)
					{
						negativeTriangles.Add(t);
					}
					else
					{
						positiveTriangles.Add(t);
						negativeTriangles.Add(t);
					}
				}
			}

			Console.WriteLine("T:" + Triangles.Count + " pt:" + positiveTriangles.Count + " nt:" + negativeTriangles.Count);

			ITreeNode positiveNode;
			ITreeNode negativeNode;
			//Catch for not finding a good solution to splitting
			if (negativeTriangles.Count == 0 || positiveTriangles.Count == 0)
			{
				return new Leaf(Triangles, favouredAxis);
			}
			else
			{
				if (positiveTriangles.Count < leafThreshold || positiveTriangles.Count == Triangles.Count)
				{
					positiveNode = new Leaf(positiveTriangles, favouredAxis);
				}
				else
				{
					BoundingBox pBoundingBox = new BoundingBox(BoundingBox);
					switch(splitAttempt.axis)
					{
						case Axis.X:
							pBoundingBox.min.x = splitAttempt.position;
							break;
						case Axis.Y:
							pBoundingBox.min.y = splitAttempt.position;
							break;
						case Axis.Z:
							pBoundingBox.min.z = splitAttempt.position;
							break;
					}
					positiveNode = createNode(positiveTriangles, leafThreshold, pBoundingBox, favouredAxis, axis1, axis2);
				}

				if (negativeTriangles.Count < leafThreshold || negativeTriangles.Count == Triangles.Count)
				{
					negativeNode = new Leaf(negativeTriangles, favouredAxis);
				}
				else
				{
					BoundingBox nBoundingBox = new BoundingBox(BoundingBox);
					switch (splitAttempt.axis)
					{
						case Axis.X:
							nBoundingBox.max.x = splitAttempt.position;
							break;
						case Axis.Y:
							nBoundingBox.max.y = splitAttempt.position;
							break;
						case Axis.Z:
							nBoundingBox.max.z = splitAttempt.position;
							break;
					}
					negativeNode = createNode(negativeTriangles, leafThreshold, nBoundingBox, favouredAxis, axis1, axis2);
				}
				return new Node(positiveNode, negativeNode, splitAttempt.axis, splitAttempt.position);
			}
		}

		class Node : ITreeNode
		{
			public BoundingBox BoundingBox { get; }
			private ITreeNode positiveNode;
			private ITreeNode negativeNode;
			private Axis SplitAxis;
			private float SplitPosition;

			public Node(ITreeNode positiveNode, ITreeNode negativeNode, Axis SplitAxis, float SplitPosition)
			{
				BoundingBox = BoundingBox.Merge(positiveNode.BoundingBox, negativeNode.BoundingBox);
				this.positiveNode = positiveNode;
				this.negativeNode = negativeNode;
				this.SplitAxis = SplitAxis;
				this.SplitPosition = SplitPosition;
			}

			public bool ContainsPoint(Vector3 p)
			{
				//if (!BoundingBox.Contains(p))
				//	return false;
				return (OperativeValue(p,SplitAxis) > SplitPosition) ? positiveNode.ContainsPoint(p) : negativeNode.ContainsPoint(p);
			}
			public Color getColor(Vector3 p)
			{
				return (OperativeValue(p, SplitAxis) > SplitPosition) ? positiveNode.getColor(p) : negativeNode.getColor(p);
			}

			public bool ContainsBox(BoundingBox box)
			{
				//if (!BoundingBox.Intersects(box))
				//	return false;
				if (OperativeValue(box.min, SplitAxis) > SplitPosition)
				{
					return positiveNode.ContainsBox(box);
				}
				if (OperativeValue(box.max, SplitAxis) <= SplitPosition)
				{
					return negativeNode.ContainsBox(box);
				}
				return positiveNode.ContainsBox(box) || negativeNode.ContainsBox(box);
			}
		}
	}
}
