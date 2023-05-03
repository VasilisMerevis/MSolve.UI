using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSolve.UI
{
    public class HexaElement : IGraphicalElement
    {
        public IGraphicalNode[] Nodes { get; set; }
        public int NumberOfNodes { get; }
        public int ID { get; set; }

        public double[] UnitNormalVector { get; set; }

        //public IGraphicalNode Node2 { get; set; }
        //public IGraphicalNode Node4 { get; set; }
        //public IGraphicalNode Node3 { get; set; }
        public HexaElement(IGraphicalNode node1, IGraphicalNode node2, IGraphicalNode node3, IGraphicalNode node4,
            IGraphicalNode node5, IGraphicalNode node6, IGraphicalNode node7, IGraphicalNode node8)
        {
            Nodes = new IGraphicalNode[8];
            Nodes[0] = node1;
            Nodes[1] = node2;
            Nodes[2] = node3;
            Nodes[3] = node4;
            Nodes[4] = node5;
            Nodes[5] = node6;
            Nodes[6] = node7;
            Nodes[7] = node8;

            NumberOfNodes = 8;
        }

        public HexaElement(int id, IGraphicalNode node1, IGraphicalNode node2, IGraphicalNode node3, IGraphicalNode node4,
            IGraphicalNode node5, IGraphicalNode node6, IGraphicalNode node7, IGraphicalNode node8)
        {
            Nodes = new IGraphicalNode[8];
            Nodes[0] = node1;
            Nodes[1] = node2;
            Nodes[2] = node3;
            Nodes[3] = node4;
            Nodes[4] = node5;
            Nodes[5] = node6;
            Nodes[6] = node7;
            Nodes[7] = node8;

            NumberOfNodes = 8;
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