﻿using Microsoft.Win32;
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
        public Dictionary<int, IGraphicalNode> nodes = new Dictionary<int, IGraphicalNode>();
        public Dictionary<int, Dictionary<int, int>> elementsConnectivity = new Dictionary<int, Dictionary<int, int>>();

        public GFECMesh()
        {

        }

        public void ReadData()
        {
            OpenFileDialog dialog1 = new OpenFileDialog();
            dialog1.ShowDialog();
            string selectedFilePath = dialog1.FileName;
            string coordinateFilePath = "C:/Users/Public/Documents/coordinateData.dat";
            string conectivityFilePath = "C:/Users/Public/Documents/connectivityData.dat";
            List<string> allLines = new List<string>(File.ReadAllLines(coordinateFilePath));
            List<string> allLines2 = new List<string>(File.ReadAllLines(conectivityFilePath));

            foreach (var line in allLines)
            {
                string separator = "\t";
                string[] fields = line.Split(separator.ToCharArray());

                int nodeID = int.Parse(fields[0]);
                var node = new GraphicalNode(double.Parse(fields[1], CultureInfo.GetCultureInfo("en-US")), double.Parse(fields[2], CultureInfo.GetCultureInfo("en-US")), double.Parse(fields[3], CultureInfo.GetCultureInfo("en-US")));
                nodes.Add(nodeID, node);                
            }

            foreach (var line in allLines2)
            {
                string separator = "\t";
                string[] fields = line.Split(separator.ToCharArray());

                int elementID = int.Parse(fields[0]);
                var elementNodes = new List<int>();

                for (int i = 1; i < fields.Length; i++)
                {
                    elementNodes.Add(int.Parse(fields[i])-1);
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

        public void ExportParaviewXML(string pathToSave)
        {
            XElement messageBody = CreateXMLMessageBody();
            XDocument document = CreateCompleteXML(messageBody);
            document.Save(pathToSave);
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

        private XElement CreateXMLMessageBody()
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
            //XElement[] manufacturer = new XElement[]
            //    {
            //        new XElement ("Name", CompanyName),
            //        new XElement ("Street1", CompanyAddress),
            //        new XElement ("City", City),
            //        new XElement ("PostCode", PostalCode),
            //        new XElement ("CountryCode", CountryCode)
            //    };

            //List<XElement> serials = CreateSerialsElement(SerialNumbers);

            //XElement[] packCreateManufacturerRequest = new XElement[]
            //{
            //    new XElement ("CreationDate", Date),
            //    new XElement ("CodeScheme", CodeScheme),
            //    new XElement ("CodeValue", CodeValue),
            //    new XElement ("BatchId", BatchID),
            //    new XElement ("BatchExpiry", ExpirationDate),
            //    new XElement ("Manufacturer", manufacturer),
            //    new XElement ("SerialIds", serials)
            //};

            //XElement arrayOfPackCreateManufacturerRequest = new XElement("PackCreateManufacturerRequest", packCreateManufacturerRequest);
            //return arrayOfPackCreateManufacturerRequest;
            return piece;
        }
    }
}
