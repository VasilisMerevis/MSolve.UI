using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSolve.UI
{
    public class TriangleElement
    {
        public IGraphicalNode[] Nodes { get; set; }
        public IGraphicalEdge[] ElementEdges { get; set; }
        
        public TriangleElement(IGraphicalNode node1, IGraphicalNode node2, IGraphicalNode node3)
        {
            Nodes[0] = node1;
            Nodes[1] = node2;
            Nodes[2] = node3;
            ElementEdges[0] = new GraphicalEdgeLinear(node1, node2);
            ElementEdges[1] = new GraphicalEdgeLinear(node2, node3);
            ElementEdges[2] = new GraphicalEdgeLinear(node3, node1);
        }

        public IGraphicalNode CalculateElementCentroid()
        {
            IGraphicalNode centroidNode;
            double x = Nodes[0].XCoordinate + Nodes[1].XCoordinate + Nodes[2].XCoordinate;
            double y = Nodes[0].YCoordinate + Nodes[1].YCoordinate + Nodes[2].YCoordinate;
            double z = Nodes[0].ZCoordinate + Nodes[1].ZCoordinate + Nodes[2].ZCoordinate;
            centroidNode = new GraphicalNode(x, y, z);
            return centroidNode;
        }
    }
}
