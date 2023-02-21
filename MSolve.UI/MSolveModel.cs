using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MSolve.UI
{
    public class MSolveModel
    {
        private IGraphicalNode[] Nodes { get; set; }
        private IGraphicalElement[] Elements { get; set; }

        public MSolveModel(IGraphicalNode[] nodes, IGraphicalElement[] elements)
        {
            Nodes = nodes;
            Elements = elements;
        }

        public void ExportToTXT(string path, string filename)
        {
            ExportNodesToTXT(path, filename);
            ExportConnectivityToTXT(path, filename);
        }

        private void ExportNodesToTXT(string path, string filename)
        {
            string[] strings = new string[Nodes.Length];
            for (int i = 0; i < Nodes.Length; i++)
            {
                strings[i] = Nodes[i].XCoordinate.ToString(CultureInfo.InvariantCulture) + "\t" + Nodes[i].YCoordinate.ToString(CultureInfo.InvariantCulture) + "\t" + Nodes[i].ZCoordinate.ToString(CultureInfo.InvariantCulture);
            }
            File.WriteAllLines(path+filename+"nodes.txt", strings);
        }

        private void ExportConnectivityToTXT(string path, string filename)
        {
            string[] strings = new string[Elements.Length];
            for (int i = 0; i < Elements.Length; i++)
            {
                strings[i] = "";
                for (int j = 0; j < Elements[i].Nodes.Length; j++)
                {
                    strings[i] = strings[i] + Elements[i].Nodes[j].GlobalIndex.ToString() + "\t";
                }
            }
            File.WriteAllLines (path+filename+"connectivity.txt", strings);
        }
    }
}
