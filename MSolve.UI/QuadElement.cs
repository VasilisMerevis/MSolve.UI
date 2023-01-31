using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSolve.UI
{
    public class QuadElement :IGraphicalElement
    {
        public IGraphicalNode[] Nodes { get; set; }
        public int NumberOfNodes { get; }
        //public IGraphicalNode Node2 { get; set; }
        //public IGraphicalNode Node4 { get; set; }
        //public IGraphicalNode Node3 { get; set; }
        public QuadElement(IGraphicalNode node1, IGraphicalNode node2, IGraphicalNode node3, IGraphicalNode node4)
        {
            Nodes[0] = node1;
            Nodes[1] = node2;
            Nodes[2] = node3;
            Nodes[3] = node4;

            NumberOfNodes = 4;
        }

        public IGraphicalNode CalculateElementCentroid()
        {
            throw new NotImplementedException();
        }
    }
}