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

        public Node(int id)
        {
            this.Id = id;
            Adjacency = new Dictionary<int, int>();
        }

        public int Id { get; set; }
        public Dictionary<int,int> Adjacency { get; set; }
        public Boolean isRoot { get; set; }
    }

    class Edge
    {
        public Node Neighbour { get; set; }
        public int Weigth { get; set; }
    }
}