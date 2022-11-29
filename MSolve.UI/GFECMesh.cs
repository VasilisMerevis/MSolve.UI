using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MSolve.UI
{
    public class GFECMesh
    {
        public Dictionary<int, IGraphicalNode> initialNodes = new Dictionary<int, IGraphicalNode>();
        public Dictionary<int, Dictionary<int, int>> elementsConnectivity = new Dictionary<int, Dictionary<int, int>>();
        //public Dictionary<int, IGraphicalNode> nodalDisplacements = new Dictionary<int, IGraphicalNode>();
        private string gfecFileName;
        private int gfecTotalFiles;
        private string pathExport;
        private string filenameExport;
        List<Dictionary<int, IGraphicalNode>> allTimeStepsDisp = new List<Dictionary<int, IGraphicalNode>>();

        public GFECMesh()
        {
            gfecFileName = "paraviewDataStep3";
            gfecTotalFiles = 60;
            pathExport = "C:/Users/Public/Documents/TEST/";
            filenameExport = "paraviewDataStep";
        }

        public void ReadData()
        {
            //OpenFileDialog dialog1 = new OpenFileDialog();
            //dialog1.ShowDialog();
            //string selectedFilePath = dialog1.FileName;

            ReadCoordinateData();
            ReadConnectivityData();
            for (int i = 0; i < gfecTotalFiles; i++)
            {
                allTimeStepsDisp.Add(ReadDynamicResults(i));
            }
        }

        private void ReadCoordinateData()
        {
            string coordinateFilePath = "C:/Users/Public/Documents/Initial/MSolve_Results_Regale/coordinateData.dat";
            List<string> alllinesCoordinates = new List<string>(File.ReadAllLines(coordinateFilePath));
            foreach (var line in alllinesCoordinates)
            {
                string separator = "\t";
                string[] fields = line.Split(separator.ToCharArray());

                int nodeID = int.Parse(fields[0]);
                var node = new GraphicalNode(double.Parse(fields[1], System.Globalization.NumberStyles.Float, CultureInfo.GetCultureInfo("en-US")), double.Parse(fields[2], System.Globalization.NumberStyles.Float, CultureInfo.GetCultureInfo("en-US")), double.Parse(fields[3], System.Globalization.NumberStyles.Float, CultureInfo.GetCultureInfo("en-US")));
                initialNodes.Add(nodeID, node);
            }
        }

        private void ReadConnectivityData()
        {
            string conectivityFilePath = "C:/Users/Public/Documents/Initial/MSolve_Results_Regale/connectivityData.dat";
            List<string> alllinesConnectivity = new List<string>(File.ReadAllLines(conectivityFilePath));
            foreach (var line in alllinesConnectivity)
            {
                string separator = "\t";
                string[] fields = line.Split(separator.ToCharArray());

                int elementID = int.Parse(fields[0]);
                var elementNodes = new List<int>();

                for (int i = 1; i < fields.Length; i++)
                {
                    elementNodes.Add(int.Parse(fields[i]) - 1);
                }

                elementsConnectivity[elementID] = new Dictionary<int, int>()
                {
                    { 1, elementNodes[0] },
                    { 2, elementNodes[1] },
                    { 3, elementNodes[2] },
                    { 4, elementNodes[3] },
                    { 5, elementNodes[4] },
                    { 6, elementNodes[5] },
                    { 7, elementNodes[6] },
                    { 8, elementNodes[7] }
                };
            }
        }

        private Dictionary<int, IGraphicalNode> ReadDynamicResults(int timestep)
        {
            Dictionary<int, IGraphicalNode> nodalDisplacements = new Dictionary<int, IGraphicalNode>();
            string dynamicResults1 = "C:/Users/Public/Documents/Initial/MSolve_Results_Regale/Regale Full Solution Vectors/RegaleSolution"+timestep.ToString()+".txt";
            List<string> alllinesDynamicResults = new List<string>(File.ReadAllLines(dynamicResults1));
            int k = 1;
            for (int i = 0; i <= alllinesDynamicResults.Count - 3; i += 3)
            {
                //double kati = Double.Parse(allLines3[i], System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture);
                var singleNodeDisplacement = new GraphicalNode(Double.Parse(alllinesDynamicResults[i], System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture), Double.Parse(alllinesDynamicResults[i + 1], System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture), Double.Parse(alllinesDynamicResults[i + 2], System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture));
                nodalDisplacements.Add(k, singleNodeDisplacement);
                k = k + 1;

            }
            return nodalDisplacements;
        }

        
        private void UpdateNodalPositions(Dictionary<int, IGraphicalNode> nodes, int timestep)
        {
            //Dictionary<int, IGraphicalNode> nodalDisplacements = allTimeStepsDisp[timestep];
            for (int i = 1; i <= nodes.Count; i++)
            {
                allTimeStepsDisp[timestep][i].XCoordinate = allTimeStepsDisp[timestep][i].XCoordinate + nodes[i].XCoordinate;
                allTimeStepsDisp[timestep][i].YCoordinate = allTimeStepsDisp[timestep][i].YCoordinate + nodes[i].YCoordinate;
                allTimeStepsDisp[timestep][i].ZCoordinate = allTimeStepsDisp[timestep][i].ZCoordinate + nodes[i].ZCoordinate;
            }
        }
        public void ExportParaviewXML(string pathToSave)
        {
            XElement messageBody = CreateXMLMessageBody(initialNodes);
            XDocument document = CreateCompleteXML(messageBody);
            document.Save(pathExport + filenameExport+"-1");

            for (int i = 0; i < gfecTotalFiles; i++)
            {
                UpdateNodalPositions(initialNodes, i);
                XElement messageBodyStep = CreateXMLMessageBody(allTimeStepsDisp[i]);
                XDocument documentStep = CreateCompleteXML(messageBodyStep);
                documentStep.Save("C:/Users/Public/Documents/TEST/"+filenameExport+i.ToString()+".vtu");
            }
            
            CreatePVDFile();
        }

        private XDocument CreateCompleteXML(XElement unstructuredGrid)
        {
            XDocument document = new XDocument(
                new XDeclaration("1.0", "UTF-8", "yes"),
                new XElement(
                    "VTKFile",
                    new XAttribute("type", "UnstructuredGrid"),
                    new XAttribute("version", "0.1"),
                    new XAttribute("byte_order", "LittleEndian"),
                    new XAttribute("compressor", "vtkZLibDataCompressor"),
                    new XElement("UnstructuredGrid", unstructuredGrid)));
            return document;
        }

        private XElement CreateXMLMessageBody(Dictionary<int, IGraphicalNode> nodes)
        {
            List<string> stringOfNodes = new List<string>();
            foreach (var node in nodes)
            {
                stringOfNodes.Add(node.Value.XCoordinate.ToString(new CultureInfo("en-US")) + " " + node.Value.YCoordinate.ToString(new CultureInfo("en-US")) + " " + node.Value.ZCoordinate.ToString(new CultureInfo("en-US")) + "\n");
            }

            List<string> testString = new List<string>();
            for (int i = 0; i < 8; i++)
            {
                testString.Add(stringOfNodes[i]);
            }

            
            XElement pointsDataArray = new XElement(
                "DataArray",
                new XAttribute("type", "Float32"),
                new XAttribute("NumberOfComponents", 3),
                new XAttribute("format", "ascii"),
                stringOfNodes
                );

            List<string> connectivity = new List<string>();
            foreach (var element in elementsConnectivity)
            {
                connectivity.Add(
                    element.Value[1].ToString() + " " +
                    element.Value[2].ToString() + " " +
                    element.Value[3].ToString() + " " +
                    element.Value[4].ToString() + " " +
                    element.Value[5].ToString() + " " +
                    element.Value[6].ToString() + " " +
                    element.Value[7].ToString() + " " +
                    element.Value[8].ToString() + " " +
                    "\n");
            }

            int k = 0;
            List<string> offsets = new List<string>();
            for (int i = 0; i < connectivity.Count; i++)
            {
                k = k + 8;
                offsets.Add(k.ToString()+"\n");
            }

            k = 12;
            List<string> types = new List<string>();
            for (int i = 0; i < connectivity.Count; i++)
            {
                types.Add(k.ToString()+"\n");
            }

            XElement[] cells = new XElement[]
            {
                new XElement(
                    "DataArray",
                    new XAttribute("type", "Int32"),
                    new XAttribute("Name", "connectivity"),
                    new XAttribute("format", "ascii"),
                    connectivity
                ),
                new XElement(
                    "DataArray",
                    new XAttribute("type", "Int32"),
                    new XAttribute("Name", "offsets"),
                    new XAttribute("format", "ascii"),
                    offsets
                ),
                new XElement(
                    "DataArray",
                    new XAttribute("type", "UInt8"),
                    new XAttribute("Name", "types"),
                    new XAttribute("format", "ascii"),
                    types
                ),
            };


            XElement piece = new XElement(
                "Piece",
                new XAttribute("NumberOfPoints", nodes.Count),
                new XAttribute("NumberOfCells", elementsConnectivity.Count),
                new XElement("CellData"),
                new XElement("PointData"),
                new XElement("Points", pointsDataArray),
                new XElement("Cells", cells)
                                );
            
            return piece;
        }

        private void CreatePVDFile()
        {
            List<XElement> timesteps = new List<XElement>();
            for (int i = 0; i < gfecTotalFiles; i++)
            {
                timesteps.Add(
                    new XElement(
                        "DataSet",
                        new XAttribute("timestep", i.ToString()),
                        new XAttribute("group", ""),
                        new XAttribute("part", ""),
                        new XAttribute("file", filenameExport + i.ToString() + ".vtu")
                    )
                    );
            }

            XDocument pvdFile = new XDocument(
                new XDeclaration("1.0","",""),
                new XElement("VTKFile",
                new XAttribute("type", "Collection"),
                new XAttribute("version", "0.1"),
                new XAttribute("byte_order", "LittleEndian"),
                new XAttribute("compressor", "vtkZLibDataCompressor"),
                new XElement("Collection", timesteps)
                ));

            pvdFile.Save("C:/Users/Public/Documents/TEST/timeBasedUnstructured.pvd");
        }
    }
}
