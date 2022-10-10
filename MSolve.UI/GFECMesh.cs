using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                var node = new GraphicalNode(double.Parse(fields[1], CultureInfo.InvariantCulture), double.Parse(fields[2], CultureInfo.InvariantCulture), double.Parse(fields[3], CultureInfo.InvariantCulture));
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
                    elementNodes.Add(int.Parse(fields[i]));
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
    }
}
