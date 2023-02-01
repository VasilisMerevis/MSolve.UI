using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace MSolve.UI
{
    public class Mesh
    {
        public IGraphicalNode[] Nodes { get; set; }
        IGraphicalElement[] Elements { get; set; }
        Dictionary<int, List<int>> NodesOwnerElements { get; set; }
        List<double[]> OffsetVectors { get; set; }

        //public static TriangleElement[] SplitQuadInTriangles(QuadElement quadElement)
        //{
        //    var triangleElements = new TriangleElement[2];
        //    var triangleElement1 = new TriangleElement(quadElement.Node1, quadElement.Node2, quadElement.Node4);
        //    var triangleElement2 = new TriangleElement(quadElement.Node3, quadElement.Node2, quadElement.Node4);
        //    triangleElements[0] = triangleElement1;
        //    triangleElements[1] = triangleElement2;
        //    return triangleElements;
        //}

        public void FindOwnerElements()
        {
            for (int i = 0; i < Nodes.Length; i++)
            {
                List<int> elementListThatShareNode = new List<int>();
                int nodeIndex = Nodes[i].GlobalIndex;
                for (int j = 0; j < Elements.Length; j++)
                {
                    for (int k = 0; k < Elements[j].Nodes.Length; k++)
                    {
                        if (Elements[j].Nodes[k].GlobalIndex == nodeIndex)
                        {
                            elementListThatShareNode.Add(k);
                        }
                    }
                }
                NodesOwnerElements.Add(nodeIndex, elementListThatShareNode);
            }
        }

        private double[] CreateNodePositionVector(IGraphicalNode node)
        {
            double[] positionVector = new double[]
            {
                node.XCoordinate,
                node.YCoordinate,
                node.ZCoordinate
            };
            return positionVector;
        }

        private List<double[]> CreateListWithNodesPositionVectors(IGraphicalNode[] nodes)
        {
            List<double[]> listWithPositionVectors = new List<double[]>();
            for (int i = 0; i < nodes.Length; i++)
            {
                listWithPositionVectors.Add(
                    new double[]
                    {
                        nodes[i].XCoordinate,
                        nodes[i].YCoordinate,
                        nodes[i].ZCoordinate
                    });
            }
            return listWithPositionVectors;
        }

        private IGraphicalNode[] GetNodesFromPosistionVectors(List<double[]> positionVectors)
        {
            IGraphicalNode[] nodes = new IGraphicalNode[positionVectors.Count];
            for (int i = 0; i < nodes.Length; i++)
            {
                nodes[i].XCoordinate = positionVectors[i][0];
                nodes[i].YCoordinate = positionVectors[i][1];
                nodes[i].ZCoordinate = positionVectors[i][2];
            }
            return nodes;
        }

        public Mesh CreateNewOffsetMesh(double distance)
        {
            Mesh offsetMesh = new Mesh();
            FindOwnerElements();
            for (int i = 0; i < Elements.Length; i++)
            {
                Elements[i].GetNormalVector();
            }

            List<double[]> unitNormalVectors = new List<double[]>();
            IGraphicalNode[] offsetNodes = new IGraphicalNode[Nodes.Length];
            for (int i = 0; i < Nodes.Length; i++)
            {
                foreach (var item in NodesOwnerElements[i])
                {
                    unitNormalVectors.Add(Elements[item].UnitNormalVector);
                }
                OffsetVectors.Add(MathVector.GetSumOfVectors(unitNormalVectors));
            }

            List<double[]> finalOffsetVectors = new List<double[]>();
            foreach (var offsetVector in OffsetVectors)
            {
                double[] newOffsetVector = MathVector.VectorScalarProduct(offsetVector, distance);
                finalOffsetVectors.Add(newOffsetVector);
            }

            List<double[]> initialPosistionVectors = CreateListWithNodesPositionVectors(Nodes);
            List<double[]> finalPositionVectors = new List<double[]>();
            for (int i = 0; i < finalOffsetVectors.Count; i++)
            {
                finalPositionVectors.Add(MathVector.GetSumOfVectors(
                    new List<double[]>()
                    {
                        finalOffsetVectors[i],
                        initialPosistionVectors[i]
                    }));
            }

            offsetMesh.Nodes = GetNodesFromPosistionVectors(finalPositionVectors);
            return offsetMesh;
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
