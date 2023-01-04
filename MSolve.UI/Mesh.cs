using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace MSolve.UI
{
    public class Mesh
    {
        IGraphicalNode[] Nodes { get; set; }
        IGraphicalElement[] Elements { get; set; }
        public static TriangleElement[] SplitQuadInTriangles(QuadElement quadElement)
        {
            var triangleElements = new TriangleElement[2];
            var triangleElement1 = new TriangleElement(quadElement.Node1, quadElement.Node2, quadElement.Node4);
            var triangleElement2 = new TriangleElement(quadElement.Node3, quadElement.Node2, quadElement.Node4);
            triangleElements[0] = triangleElement1;
            triangleElements[1] = triangleElement2;
            return triangleElements;
        }

        //public static QuadElement[] SplitTriangleInQuads(TriangleElement triangleElement)
        //{
        //    var quadElements = new QuadElement[3];
        //    IGraphicalNode centroidNode = triangleElement.CalculateElementCentroid();
        //    quadElements[0] = new QuadElement(centroidNode, triangleElement.Node1, triangleElement.Node2);
        //    quadElements[1] = new QuadElement(centroidNode, triangleElement.Node2, triangleElement.Node3);
        //    quadElements[2] = new QuadElement(centroidNode, triangleElement.Node3, triangleElement.Node1);
        //    return quadElements;
        //}

        //private static IGraphicalNode[] OffsetTriangle(TriangleElement element)
        //{
        //    IGraphicalNode[] graphicalNodes = new GraphicalNode[3];
            
        //}

        //public static List<IGraphicalNode> CalculateElementMidpoints(IGraphicalElement[] element)
        //{

        //}
    }
}
