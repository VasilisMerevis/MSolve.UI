using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSolve.UI
{
    public class QuadElement
    {
        public IGraphicalNode Node1 { get; set; }
        public IGraphicalNode Node2 { get; set; }
        public IGraphicalNode Node4 { get; set; }
        public IGraphicalNode Node3 { get; set; }
        public QuadElement(IGraphicalNode node1, IGraphicalNode node2, IGraphicalNode node3, IGraphicalNode node4)
        {
            Node1 = node1;
            Node2 = node2;
            Node3 = node3;
            Node4 = node4;
        }
    }
}