using System;
using System.Collections.Generic;

namespace Prim
{
    class Node
    {
        public const int INFINITY = int.MaxValue;
        private Node node;

        public Node() { }

        public Node(Node node)
        {
            this.node = node;
        }

        public Node(int id, int key)
        {
            this.Id = id;
            this.Key = key;
            Adjacency = new Dictionary<Node, int>();
            Pi = new List<Node>();
        }

        public int Id { get; set; }
        public int Key { get; set; }
        public Dictionary<Node,int> Adjacency { get; set; }
        public List<Node> Pi { get; set; }
        public Boolean isRoot { get; set; }
        public void addNodeToPi(Node node)
        {
            Pi.Add(node);
        }

        public void clearPi()
        {
            Pi = new List<Node>();
        }
    }

    class Edge
    {
        public Node Neighbour { get; set; }
        public int Weigth { get; set; }
    }
}