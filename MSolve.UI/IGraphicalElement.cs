using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSolve.UI
{
    public interface IGraphicalElement
    {
        int NumberOfNodes { get; }
        IGraphicalNode[] Nodes { get; set; }
        IGraphicalNode CalculateElementCentroid();
    }
}
