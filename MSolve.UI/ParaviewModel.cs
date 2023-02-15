using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace MSolve.UI
{
    public class ParaviewModel
    {
        private IGraphicalNode[] Nodes { get; set; }
        private IGraphicalElement[] Elements { get; set; }
        private List<string> nodalDisplacements;

        public ParaviewModel(Mesh mesh, List<string> dispVector)
        {
            Nodes = mesh.Nodes;
            Elements = mesh.Elements;
            nodalDisplacements = dispVector;
        }

           
        public void ExportParaviewXML(string pathToSave, string name)
        {
            XElement messageBody = CreateXMLMessageBody(Nodes);
            XDocument document = CreateCompleteXML(messageBody);
            document.Save(pathToSave + name);
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
            for (int i=0; i<nodes.Length; i++)
            {
                stringOfNodes.Add(nodes[i].XCoordinate.ToString(new CultureInfo("en-US")) + " " + nodes[i].YCoordinate.ToString(new CultureInfo("en-US")) + " " + nodes[i].ZCoordinate.ToString(new CultureInfo("en-US")) + "\n");
            }

            //List<string> testString = new List<string>();
            //for (int i = 0; i < 8; i++)
            //{
            //    testString.Add(stringOfNodes[i]);
            //}


            XElement pointsDataArray = new XElement(
                "DataArray",
                new XAttribute("type", "Float32"),
                new XAttribute("NumberOfComponents", 3),
                new XAttribute("format", "ascii"),
                stringOfNodes
                );

            List<string> connectivity = new List<string>();
            for (int i=0; i<Elements.Length; i++)
            {
                string connectivityForElement="";
                for (int j = 0; j < Elements[i].Nodes.Length; j++)
                {
                    connectivityForElement = connectivityForElement + " " + Elements[i].Nodes[j].GlobalIndex.ToString();
                }
                connectivityForElement = connectivityForElement + "\n";
                connectivity.Add(connectivityForElement);   
            }
        
            int k = 0;

            
            List<string> offsets = new List<string>();
            for (int i = 0; i < connectivity.Count; i++)
            {
                k = k + 8;
                offsets.Add(k.ToString() + "\n");
            }

            k = 12;
            List<string> types = new List<string>();
            for (int i = 0; i < connectivity.Count; i++)
            {
                types.Add(k.ToString() + "\n");
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
                new XAttribute("NumberOfPoints", nodes.Length),
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
    }
}
