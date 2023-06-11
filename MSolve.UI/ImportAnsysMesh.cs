using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace MSolve.UI
{
    public class AnsysMesh
    {
        public IGraphicalNode[] Nodes { get; set; }
        public IGraphicalElement[] Elements { get; set; }
        string nodestxtName = @"/AnsysNodesList.txt";
        string elementstxtName = @"/AnsysElementsList.txt";
		List<IGraphicalNode[]> allTimeStepsDisp = new List<IGraphicalNode[]>();
		public double ScaleFactor { get; set; }
		List<string> nodalDisplacements;
		private string gfecFileName;
		//private int gfecTotalFiles;
		private string pathExport;
		private string filenameExport;

		public AnsysMesh()
		{
			gfecFileName = "paraviewDataStep3";
			pathExport = "C:/Users/Public/Documents/TEST/";
			filenameExport = "paraviewDataStep";
			ScaleFactor = 0.0;
			nodalDisplacements = new List<string>();
		}
		public void ImportMesh(string folderPath)
        {
            ImportNodes(folderPath);
            ImportConnectivity(folderPath);
            ReadAllDynamicResults();

		}
        private void ImportNodes(string folderPath)
        {
            try
            {
                StreamReader stream = new StreamReader(folderPath+nodestxtName);
                string file = stream.ReadToEnd();
                List<string> lines = new List<string>(file.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries));
                lines.RemoveAt(0);

                Nodes = new IGraphicalNode[lines.Count];
                int i = 0;
                foreach (var line in lines)
                {
                    // in case of first line
                    string[] fields = line.Split(new string[] { "\t" }, StringSplitOptions.None);
                    int nodeIndex = int.Parse(fields[0]);
                    Nodes[i] = new GraphicalNode(nodeIndex-1, double.Parse(fields[1]), double.Parse(fields[2]), double.Parse(fields[3]));
                    i++;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void ImportConnectivity(string folderPath)
        {
            try
            {
                StreamReader stream = new StreamReader(folderPath + elementstxtName);
                string file = stream.ReadToEnd();
                List<string> lines = new List<string>(file.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries));
                lines.RemoveAt(0);

                Elements = new IGraphicalElement[lines.Count];
                int k = 0;
                foreach (var line in lines)
                {
                    // in case of first line
                    string[] fields = line.Split(new string[] { "\t" }, StringSplitOptions.None);
                    int elementIndex = int.Parse(fields[0]);
                    List<int> nodes = new List<int>();
                    switch (fields[1].ToString())
                    {
                        case "Quad4":
                            int node1 = int.Parse(fields[2]) - 1;
                            int node2 = int.Parse(fields[3]) - 1;
                            int node3 = int.Parse(fields[4]) - 1;
                            int node4 = int.Parse(fields[5]) - 1;
                            Elements[k] = new QuadElement(elementIndex, Nodes[node1], Nodes[node2], Nodes[node3], Nodes[node4]);
                            break;
						case "Quad8":
                            for (int i = 2; i < fields.Length-1; i++)
                            {
                                nodes.Add(int.Parse(fields[i]) - 1);
                            }
							Elements[k] = new HexaElement(elementIndex, Nodes[nodes[0]], Nodes[nodes[1]], Nodes[nodes[2]], Nodes[nodes[3]],
                                Nodes[nodes[4]], Nodes[nodes[5]], Nodes[nodes[6]], Nodes[nodes[7]]);
							break;
						case "Tri6":
							for (int i = 2; i < fields.Length-1; i++)
							{

								nodes.Add(int.Parse(fields[i]) - 1);
							}
							Elements[k] = new WedgeElement(elementIndex, Nodes[nodes[0]], Nodes[nodes[1]], Nodes[nodes[2]], Nodes[nodes[3]],
								Nodes[nodes[4]], Nodes[nodes[5]]);
							break;
						case "Hex20":
							for (int i = 2; i < fields.Length - 1; i++)
							{
								nodes.Add(int.Parse(fields[i]) - 1);
							}
							Elements[k] = new Hexa20Element(elementIndex, Nodes[nodes[0]], Nodes[nodes[1]], Nodes[nodes[2]], Nodes[nodes[3]],
								Nodes[nodes[4]], Nodes[nodes[5]], Nodes[nodes[6]], Nodes[nodes[7]],
								Nodes[nodes[8]], Nodes[nodes[9]], Nodes[nodes[10]], Nodes[nodes[11]],
								Nodes[nodes[12]], Nodes[nodes[13]], Nodes[nodes[14]], Nodes[nodes[15]],
								Nodes[nodes[16]], Nodes[nodes[17]], Nodes[nodes[18]], Nodes[nodes[19]]);
							break;
					}
                    k++;
                }
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

		private IGraphicalNode[] ReadDynamicResults(string dynamicResults)
		{
			
			//string dynamicResults1 = "C:/Users/Public/Documents/Initial/MSolve_Results_Regale/Regale Full Solution Vectors/RegaleSolution"+timestep.ToString()+".txt";
			List<string> alllinesDynamicResults = new List<string>(File.ReadAllLines(dynamicResults));
			alllinesDynamicResults.RemoveAt(0);
			IGraphicalNode[] nodalDisplacements = new IGraphicalNode[alllinesDynamicResults.Count];
			int k = 1;
			for (int i = 0; i < alllinesDynamicResults.Count; i ++)
			{
				string[] fields = alllinesDynamicResults[i].Split(new string[] { "\t" }, StringSplitOptions.None);
				//double kati = Double.Parse(allLines3[i], System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture);
				var singleNodeDisplacement = new GraphicalNode(
                    int.Parse(fields[0]),
                    Double.Parse(fields[1], System.Globalization.NumberStyles.Float, CultureInfo.CurrentCulture),
                    Double.Parse(fields[2], System.Globalization.NumberStyles.Float, CultureInfo.CurrentCulture),
                    Double.Parse(fields[3], System.Globalization.NumberStyles.Float, CultureInfo.CurrentCulture));
				nodalDisplacements[i] = singleNodeDisplacement;
				k = k + 1;

			}
			return nodalDisplacements;
		}

		private void ReadAllDynamicResults()
		{
			OpenFileDialog fileDialog = new OpenFileDialog();
			fileDialog.Multiselect = true;
			fileDialog.ShowDialog();
			List<string> files = new List<string>(fileDialog.FileNames);
			var folderPath = System.IO.Path.GetDirectoryName(files[0]);

			foreach (var file in files)
			{
				allTimeStepsDisp.Add(ReadDynamicResults(file));
			}
			//for (int i = 0; i < gfecTotalFiles; i++)
			//{
			//	allTimeStepsDisp.Add(ReadDynamicResults(i));
			//}


		}

		private void UpdateNodalPositions(IGraphicalNode[] nodes, int timestep)
		{
			//Dictionary<int, IGraphicalNode> nodalDisplacements = allTimeStepsDisp[timestep];
			for (int i = 0; i < nodes.Length; i++)
			{
				allTimeStepsDisp[timestep][i].XCoordinate = allTimeStepsDisp[timestep][i].XCoordinate + nodes[i].XCoordinate;
				allTimeStepsDisp[timestep][i].YCoordinate = allTimeStepsDisp[timestep][i].YCoordinate + nodes[i].YCoordinate;
				allTimeStepsDisp[timestep][i].ZCoordinate = allTimeStepsDisp[timestep][i].ZCoordinate + nodes[i].ZCoordinate;

				nodalDisplacements.Add(allTimeStepsDisp[timestep][i].XCoordinate.ToString(new CultureInfo("en-US")) + " " + allTimeStepsDisp[timestep][i].YCoordinate.ToString(new CultureInfo("en-US")) + " " + allTimeStepsDisp[timestep][i].ZCoordinate.ToString(new CultureInfo("en-US")) + "\n");
			}
		}
		private void UpdateNodalPositions(IGraphicalNode[] nodes, int timestep, double scaleFactor)
		{
			//Dictionary<int, IGraphicalNode> nodalDisplacements = allTimeStepsDisp[timestep];
			for (int i = 0; i < nodes.Length; i++)
			{
				allTimeStepsDisp[timestep][i].XCoordinate = scaleFactor * allTimeStepsDisp[timestep][i].XCoordinate + nodes[i].XCoordinate;
				allTimeStepsDisp[timestep][i].YCoordinate = scaleFactor * allTimeStepsDisp[timestep][i].YCoordinate + nodes[i].YCoordinate;
				allTimeStepsDisp[timestep][i].ZCoordinate = scaleFactor * allTimeStepsDisp[timestep][i].ZCoordinate + nodes[i].ZCoordinate;
			}
		}

		public void ExportParaviewXML(string pathToSave)
		{
			XElement messageBody = CreateXMLMessageBody(Nodes);
			XDocument document = CreateCompleteXML(messageBody);
			document.Save(pathExport + filenameExport + "-1");

			for (int i = 0; i < allTimeStepsDisp.Count; i++)
			{
				if (ScaleFactor != 0)
				{
					UpdateNodalPositions(Nodes, i, ScaleFactor);
				}
				else
				{
					UpdateNodalPositions(Nodes, i);
				}

				XElement messageBodyStep = CreateXMLMessageBody(allTimeStepsDisp[i]);
				XDocument documentStep = CreateCompleteXML(messageBodyStep);
				documentStep.Save("C:/Users/Public/Documents/AnsysVideo/" + filenameExport + i.ToString() + ".vtu");
				nodalDisplacements.Clear();
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

		private XElement CreateXMLMessageBody(IGraphicalNode[] nodes)
		{
			List<string> stringOfNodes = new List<string>();
			foreach (var node in nodes)
			{
				stringOfNodes.Add(node.XCoordinate.ToString(new CultureInfo("en-US")) + " " + node.YCoordinate.ToString(new CultureInfo("en-US")) + " " + node.ZCoordinate.ToString(new CultureInfo("en-US")) + "\n");
			}

			XElement pointsDataArray = new XElement(
				"DataArray",
				new XAttribute("type", "Float32"),
				new XAttribute("NumberOfComponents", 3),
				new XAttribute("format", "ascii"),
				stringOfNodes
				);

			List<string> connectivity = new List<string>();
			List<string> offsets = new List<string>();
			List<string> types = new List<string>();
			int k = 0;
			foreach (var element in Elements)
			{
				string lineForConnectivity = "";
				foreach (var node in element.Nodes)
				{
					lineForConnectivity = lineForConnectivity + " " + node.GlobalIndex.ToString();
				}
				lineForConnectivity = lineForConnectivity + "\n";
				connectivity.Add(lineForConnectivity);
				k = k + element.NumberOfNodes;
				offsets.Add(k.ToString() + "\n");

				switch(element.NumberOfNodes)
				{
					case 6:
						types.Add(13.ToString() + "\n");
						break;
					case 8:
						types.Add(12.ToString() + "\n");
						break;
					case 20:
						types.Add(25.ToString() + "\n");
						break;

				}
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
				new XAttribute("NumberOfPoints", Nodes.Length),
				new XAttribute("NumberOfCells", Elements.Length),
				new XElement("CellData"),
				new XElement("PointData",
					new XElement("DataArray",
						new XAttribute("type", "Float32"),
						new XAttribute("NumberOfComponents", "3"),
						new XAttribute("Name", "Displacements"),
						new XAttribute("format", "ascii"),
						nodalDisplacements)),
				new XElement("Points", pointsDataArray),
				new XElement("Cells", cells)
								);

			return piece;
		}

		private void CreatePVDFile()
		{
			List<XElement> timesteps = new List<XElement>();
			for (int i = 0; i < allTimeStepsDisp.Count; i++)
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
				new XDeclaration("1.0", "", ""),
				new XElement("VTKFile",
				new XAttribute("type", "Collection"),
				new XAttribute("version", "0.1"),
				new XAttribute("byte_order", "LittleEndian"),
				new XAttribute("compressor", "vtkZLibDataCompressor"),
				new XElement("Collection", timesteps)
				));

			pvdFile.Save("C:/Users/Public/Documents/AnsysVideo/timeBasedUnstructured.pvd");
		}
	}
}
