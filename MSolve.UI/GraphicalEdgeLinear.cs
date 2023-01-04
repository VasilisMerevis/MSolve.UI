using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSolve.UI
{
    public class GraphicalEdgeLinear : IGraphicalEdge
    {
        public IGraphicalNode[] Points { get; set; }
        public GraphicalEdgeLinear(IGraphicalNode point1, IGraphicalNode point2)
        {
            Points[0] = point1;
            Points[1] = point2;
        }
        public IGraphicalNode CaclulateEdgeMidpoint()
        {
            double x = (Points[0].XCoordinate + Points[1].XCoordinate) / 2.0;
            double y = (Points[0].YCoordinate + Points[1].YCoordinate) / 2.0;
            double z = (Points[0].ZCoordinate + Points[1].ZCoordinate) / 2.0;
            IGraphicalNode midpoint = new GraphicalNode(x, y, z);
            return midpoint;
        }
    }
}
