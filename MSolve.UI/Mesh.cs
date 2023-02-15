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
        public IGraphicalElement[] Elements { get; set; }
        List<List<int>> NodesOwnerElements { get; set; }
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
        public Mesh()
        {
            NodesOwnerElements = new List<List<int>>();
            OffsetVectors = new List<double[]>();
        }

        public Mesh(IGraphicalNode[] nodes, IGraphicalElement[] elements)
        {
            Nodes = nodes;
            Elements = elements;
        }
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
                            elementListThatShareNode.Add(j);
                        }
                    }
                }
                NodesOwnerElements.Add(elementListThatShareNode);
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

            for (int i = 0; i < positionVectors.Count; i++)
            {
                GraphicalNode node = new GraphicalNode(positionVectors[i][0], positionVectors[i][1], positionVectors[i][2]);
                //nodes[i].XCoordinate = positionVectors[i][0];
                //nodes[i].YCoordinate = positionVectors[i][1];
                //nodes[i].ZCoordinate = positionVectors[i][2];
                nodes[i] = node;
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

            
            IGraphicalNode[] offsetNodes = new IGraphicalNode[Nodes.Length];
            for (int i = 0; i < Nodes.Length; i++)
            {
                List<double[]> unitNormalVectors = new List<double[]>();
                foreach (var item in NodesOwnerElements[i])
                {
                    unitNormalVectors.Add(Elements[item].UnitNormalVector);
                }
                OffsetVectors.Add(MathVector.GetSumOfVectors(unitNormalVectors));
            }

            List<double[]> finalOffsetVectors = new List<double[]>();
            foreach (var offsetVector in OffsetVectors)
            {
                double normOffsetVector = MathVector.VectorNorm2(offsetVector);
                double[] newOffsetVector = MathVector.VectorScalarProduct(offsetVector, distance/normOffsetVector);
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

        public void CreateMergedMesh(Mesh mesh1, Mesh mesh2)
        {
            IGraphicalNode[] nodesForMergedMesh = CreateNodesOfMergedMeshes(mesh1.Nodes, mesh2.Nodes);
            //FixIndexingInOffsetMesh(mesh1.Nodes.Length, mesh2.Elements);
            IGraphicalElement[] elementsForMergedMesh = CreateElementsOutOfMergedMeshes(mesh1.Elements, nodesForMergedMesh);
            Nodes = nodesForMergedMesh;
            Elements = elementsForMergedMesh;
        }

        private IGraphicalNode[] CreateNodesOfMergedMeshes(IGraphicalNode[] nodes1, IGraphicalNode[] nodes2)
        {
            IGraphicalNode[] newMeshNodes = new IGraphicalNode[nodes1.Length * 2];
            if (nodes1.Length == nodes2.Length)
            {
                
                for (int i = 0; i < nodes1.Length; i++)
                {
                    newMeshNodes[i] = nodes1[i];
                    nodes2[i].GlobalIndex = i+nodes1.Length;
                    newMeshNodes[nodes1.Length+i] = nodes2[i];
                }
            }
            else
            {
                throw new Exception("Not equal nodes vectors.");
            }
            return newMeshNodes;
        }

        private IGraphicalElement[] CreateElementsOutOfMergedMeshes(IGraphicalElement[] elements1, IGraphicalNode[] newNodesList)
        {
            IGraphicalElement[] newElements = new IGraphicalElement[elements1.Length];
            //for (int i = 0; i < elements1.Length; i++)
            //{
            //    newElements[i] = new HexaElement(
            //        elements1[i].Nodes[0],
            //        elements1[i].Nodes[1],
            //        elements1[i].Nodes[2],
            //        elements1[i].Nodes[3],
            //        elements2[i].Nodes[0],
            //        elements2[i].Nodes[1],
            //        elements2[i].Nodes[2],
            //        elements2[i].Nodes[3]);
            //}

            int lengthOfIntialMesh = newNodesList.Length / 2;
            for (int i = 0; i < elements1.Length; i++)
            {
                int id1 = elements1[i].Nodes[0].GlobalIndex;
                int id2 = elements1[i].Nodes[1].GlobalIndex;
                int id3 = elements1[i].Nodes[2].GlobalIndex;
                int id4 = elements1[i].Nodes[3].GlobalIndex;

                newElements[i] = new HexaElement(
                    newNodesList[id1],
                    newNodesList[id2],
                    newNodesList[id3],
                    newNodesList[id4],
                    newNodesList[id1 + lengthOfIntialMesh],
                    newNodesList[id2 + lengthOfIntialMesh],
                    newNodesList[id3 + lengthOfIntialMesh],
                    newNodesList[id4 + lengthOfIntialMesh]);
            }

            return newElements;
        }

        private IGraphicalElement[] FixIndexingInOffsetMesh(int initialMeshLength, IGraphicalElement[] offsetElements)
        {
            for (int i = 0; i < offsetElements.Length; i++)
            {
                for (int j = 0; j < offsetElements[i].Nodes.Length; j++)
                {
                    offsetElements[i].Nodes[j].GlobalIndex = offsetElements[i].Nodes[j].GlobalIndex + initialMeshLength;
                }
            }
            return offsetElements;
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
