using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSolve.UI
{
    public class GraphicalNode : IGraphicalNode
    {
        public double XCoordinate { get; set; }
        public double YCoordinate { get; set; }
        public double ZCoordinate { get; set; }

        public GraphicalNode(double xCoordinate, double yCoordinate, double zCoordinate)
        {
            XCoordinate = xCoordinate;
            YCoordinate = yCoordinate;
            ZCoordinate = zCoordinate;
        }
    }
}
