using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSolve.UI
{
	public class Hexa20Element : IGraphicalElement
	{
		public IGraphicalNode[] Nodes { get; set; }
		public int NumberOfNodes { get; }
		public int ID { get; set; }

		public double[] UnitNormalVector { get; set; }

		//public IGraphicalNode Node2 { get; set; }
		//public IGraphicalNode Node4 { get; set; }
		//public IGraphicalNode Node3 { get; set; }
		public Hexa20Element(IGraphicalNode node1, IGraphicalNode node2, IGraphicalNode node3, IGraphicalNode node4,
			IGraphicalNode node5, IGraphicalNode node6, IGraphicalNode node7, IGraphicalNode node8,
			IGraphicalNode node9, IGraphicalNode node10, IGraphicalNode node11, IGraphicalNode node12,
			IGraphicalNode node13, IGraphicalNode node14, IGraphicalNode node15, IGraphicalNode node16,
			IGraphicalNode node17, IGraphicalNode node18, IGraphicalNode node19, IGraphicalNode node20)
		{
			Nodes = new IGraphicalNode[20];
			Nodes[0] = node1;
			Nodes[1] = node2;
			Nodes[2] = node3;
			Nodes[3] = node4;
			Nodes[4] = node5;
			Nodes[5] = node6;
			Nodes[6] = node7;
			Nodes[7] = node8;
			Nodes[8] = node9;
			Nodes[9] = node10;
			Nodes[10] = node11;
			Nodes[11] = node12;
			Nodes[12] = node13;
			Nodes[13] = node14;
			Nodes[14] = node15;
			Nodes[15] = node16;
			Nodes[16] = node17;
			Nodes[17] = node18;
			Nodes[18] = node19;
			Nodes[19] = node20;

			NumberOfNodes = 20;
		}

		public Hexa20Element(int id, IGraphicalNode node1, IGraphicalNode node2, IGraphicalNode node3, IGraphicalNode node4,
			IGraphicalNode node5, IGraphicalNode node6, IGraphicalNode node7, IGraphicalNode node8,
			IGraphicalNode node9, IGraphicalNode node10, IGraphicalNode node11, IGraphicalNode node12,
			IGraphicalNode node13, IGraphicalNode node14, IGraphicalNode node15, IGraphicalNode node16,
			IGraphicalNode node17, IGraphicalNode node18, IGraphicalNode node19, IGraphicalNode node20)
		{
			Nodes = new IGraphicalNode[20];
			Nodes[0] = node1;
			Nodes[1] = node2;
			Nodes[2] = node3;
			Nodes[3] = node4;
			Nodes[4] = node5;
			Nodes[5] = node6;
			Nodes[6] = node7;
			Nodes[7] = node8;
			Nodes[8] = node9;
			Nodes[9] = node10;
			Nodes[10] = node11;
			Nodes[11] = node12;
			Nodes[12] = node13;
			Nodes[13] = node14;
			Nodes[14] = node15;
			Nodes[15] = node16;
			Nodes[16] = node17;
			Nodes[17] = node18;
			Nodes[18] = node19;
			Nodes[19] = node20;

			NumberOfNodes = 20;
			ID = id;
		}


		public IGraphicalNode CalculateElementCentroid()
		{
			throw new NotImplementedException();
		}

		public void GetNormalVector()
		{
			throw new NotImplementedException();
		}
	}
}