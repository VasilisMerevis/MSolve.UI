using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace MSolve.UI
{
    public class AnsysMesh
    {
        public IGraphicalNode[] Nodes { get; set; }
        public IGraphicalElement[] Elements { get; set; }
        string nodestxtName = @"/AnsysNodesList.txt";
        string elementstxtName = @"/AnsysElementsList.txt";
        public void ImportMesh(string folderPath)
        {
            ImportNodes(folderPath);
            ImportConnectivity(folderPath);
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
					}
                    k++;
                }
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
