using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace MSolve.UI
{
    public interface IGraphicalElement
    {
        int NumberOfNodes { get; }
        IGraphicalNode[] Nodes { get; set; }

        double[] UnitNormalVector { get; }
        IGraphicalNode CalculateElementCentroid();
        void GetNormalVector();
    }
}
