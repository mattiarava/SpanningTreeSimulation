using System;
using System.Collections.Generic;

namespace Prim
{
    class Node
    {
        public const int INFINITY = int.MaxValue;

        public Node(int id, int key)
        {
            this.Id = id;
            this.Key = key;
            Adjacency = new List<Node>();
            Pi = new List<Node>();
            Weight = new List<int>();
        }

        public int Id { get; set; }
        public int Key { get; set; }
        public List<Node> Adjacency { get; set; }
        public List<Node> Pi { get; set; }
        public List<int> Weight { get; set; }
        public void addNodeToPi(Node node)
        {
            Pi.Add(node);
        }

        public void clearPi()
        {
            Pi = new List<Node>();
        }
    }
}