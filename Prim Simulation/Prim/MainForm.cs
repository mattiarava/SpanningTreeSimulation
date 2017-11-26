using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Prim
{
    public partial class MainForm : Form
    {
        private const int PAINTED = 0, UNPAINTED = 1;
        private int index, n, x, y, x1, x2, y1, y2;
        private Color[] color;
        private List<Node> graph;
        private PriorityQueue queue;

        private int nodeListLength;

        public MainForm()
        {
            InitializeComponent();

        }

        private void CreateGraph()
        {
            this.graph = new List<Node>();
            String[] nomi = new String[] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l" };
            int indx = 0;
            for (indx = 0; indx < this.nodeListLength; indx++)
            {
                graph.Add(new Node(indx, Node.INFINITY));
            }
            graph[0].isRoot = true;
            indx = 0;
            Random ran = new Random();
            foreach (var node in graph)
            {
                Shuffler shuffler = new Shuffler();
                int nPorts = 1 + ((ran.Next(10000)) % (this.nodeListLength - 1));
                int adj;
                List<int> list = new List<int>();
                for (int v = 0; v < this.nodeListLength; v++)
                {
                    list.Add(v);
                    //ran = new Random(ran.Next(10000));
                    //adj = ran.Next(1000)%(this.nodeListLength - 2);
                    //while ((adj == indx))
                    //{
                    //    adj = ran.Next(10000)%(this.nodeListLength - 2);
                    //}
                    //node.Adjacency.Add(graph[adj]);
                    //node.Weight.Add(ran.Next(12));
                }
                shuffler.Shuffle(list, ran);
                for (int i = 0; i < nPorts; i++)
                {
                    if (list[i] == indx)
                    {
                        if (!graph[list[nPorts]].Adjacency.ContainsKey(node))
                        {
                            node.Adjacency.Add(graph[list[nPorts]], 1 + (ran.Next(100)) % 9);
                        }
                    }
                    else
                    {
                        if (!graph[list[i]].Adjacency.ContainsKey(node))
                        {
                            node.Adjacency.Add(graph[list[i]], 1 + ran.Next(100) % 9);
                        }
                    }
                }
                indx++;
            }
            n = graph.Count;
            color = new Color[n];

            for (int m = 0; m < n; m++)
                color[m] = Color.Blue;
        }

        //void MST()
        //{
        //    bool[] output = new bool[n];
        //    int[] pi = new int[n];
        //    List<Node> copy = new List<Node>();

        //    for (int i = 0; i < graph.Count; i++)
        //        copy.Add(graph[i]);

        //    queue = new PriorityQueue(copy);
        //    queue.buildHeap();

        //    for (int i = 0; i < queue.NodeList.Count; i++)
        //    {
        //        Node node = queue.extractMin();

        //        output[node.Id] = true;
        //        for (int j = 0; j < node.Adjacency.Count; j++)
        //        {
        //            Node next = node.Adjacency[j];
        //            int weight = node.Weight[j];

        //            if (!output[next.Id] && weight < next.Key)
        //            {
        //                pi[next.Id] = node.Id;
        //                node.Adjacency[j].Key = weight;
        //            }
        //        }
        //    }

        //    pi[0] = -1;

        //    for (int i = 0; i < n; i++)
        //    {
        //        u[i] = i;
        //        v[i] = pi[i];
        //    }

        //    // reorder the edges in the minimum spanning tree

        //    for (int i = 0; i < n - 1; i++)
        //    {
        //        for (int j = i + 1; j < n; j++)
        //        {
        //            Node nodeI = graph[u[i]];
        //            Node nodeJ = graph[u[j]];

        //            if (v[i] >= v[j] && nodeI.Key > nodeJ.Key)
        //            {
        //                int t = u[i];

        //                u[i] = u[j];
        //                u[j] = t;
        //                t = v[i];
        //                v[i] = v[j];
        //                v[j] = t;
        //            }
        //        }
        //    }
        //}

        private List<Node> MST()
        {
            List<Node> spanningTree = new List<Node>();
            Node next = new Node(graph.Where(n => n.isRoot).First());
            next.Adjacency = new Dictionary<Node, int>();
            spanningTree.Add(next);
            while (spanningTree.Count < this.nodeListLength)
            {
                int parentId = -1;
                int minWeight = 10;
                KeyValuePair<Node, int> minAdj = new KeyValuePair<Node, int>();
                foreach (var node in spanningTree)
                {
                    foreach (var adj in graph.Where(n => n.Id == node.Id).First().Adjacency)
                    {
                        if (adj.Value < minWeight && !spanningTree.Select(n => n.Id).ToList().Contains(adj.Key.Id))
                        {
                            minWeight = adj.Value;
                            minAdj = adj;
                            parentId = node.Id;
                        }
                    }
                }
                spanningTree.Where(n => n.Id == parentId).First().Adjacency.Add(minAdj.Key, minAdj.Value);
                next = new Node(minAdj.Key);
                next.Adjacency = new Dictionary<Node, int>();
                spanningTree.Add(next);
            }
            return spanningTree;
        }

        private void calculateXY(int id)
        {
            int Width = panel1.Width;
            int Height = panel1.Height;

            x = Width / 2 + (int)(Width * Math.Cos(2 * id * Math.PI / n) / 4.0);
            y = Height / 2 + (int)(Width * Math.Sin(2 * id * Math.PI / n) / 4.0);
        }

        private void draw(Graphics g)
        {
            List<Node> spanningTree = MST();
            this.drawGraph(spanningTree, g);
        }

        private void drawGraph(List<Node> list, Graphics g)
        {
            int Width = panel1.Width;
            int Height = panel1.Height;
            Font font = new Font("Courier New", 12f, FontStyle.Bold);
            //List<Node> nodeList = queue.NodeList;
            Pen pen = new Pen(Color.Black);
            SolidBrush textBrush = new SolidBrush(Color.White);
            foreach (var node in list)
            {
                int i = 0;
                foreach (var adj in node.Adjacency)
                {
                    calculateXY(node.Id);
                    x1 = x + (Width / 2) / n / 2;
                    y1 = y + (Width / 2) / n / 2;
                    calculateXY(adj.Key.Id);
                    x2 = x + (Width / 2) / n / 2;
                    y2 = y + (Width / 2) / n / 2;
                    g.DrawLine(pen, x1, y1, x2, y2);
                    int spostamentoX = 0, spostamentoY = 0;
                    if (x1 == x2)
                        spostamentoY = 55;
                    else if (y1 == y2)
                        spostamentoX = -65;
                    else if ((x1 + x2) / 2 == 607 && (y1 + y2) / 2 == 402)
                    {
                        if ((x1 > x2 && y1 > y2) || (x1 < x2 && y1 < y2))
                        {
                            spostamentoX = -55;
                            spostamentoY = -55;
                        }
                        else
                        {
                            spostamentoX = -55;
                            spostamentoY = 55;
                        }
                    }
                    g.DrawString(adj.Value.ToString(), font, new SolidBrush(Color.BlueViolet), (x1 + spostamentoX + x2) / 2, (y1 + spostamentoY + y2) / 2);
                    i++;
                }

                SolidBrush brush = new SolidBrush(Color.Blue);

                string str = node.Id.ToString();

                calculateXY(node.Id);
                g.FillEllipse(brush, x, y, (Width / 2) / n, (Width / 2) / n);
                g.DrawString(str, font,
                    textBrush, (float)(x + (Width / 2) / n / 2) - 12f,
                    (float)(y + (Width / 2) / n / 2) - 12f);
            }
        }

        private void panel1_Paintgraph(object sender, PaintEventArgs pea)
        {
            drawInitial(pea.Graphics);
        }

        private void panel1_Painttree(object sender, PaintEventArgs pea)
        {
            draw(pea.Graphics);
        }

        private void drawInitial(Graphics g)
        {
            this.drawGraph(graph,g);
                //if (v[i] != -1)
                //{
                //    c = new char[1];
                //    c[0] = (char)('a' + v[i]);
                //    str = new string(c);
                //    calculateXY(v[i]);
                //    g.FillEllipse(brush, x, y, (Width / 2) / n, (Width / 2) / n);
                //    g.DrawString(str, font,
                //        textBrush, (float)(x + (Width / 2) / n / 2) - 12f,
                //        (float)(y + (Width / 2) / n / 2) - 12f);
                //}
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CreateGraph();
            index = this.nodeListLength - 1;
            panel1.Paint += new PaintEventHandler(panel1_Paintgraph);
            panel1.Invalidate();
            //if (index < n)
            //    panel1.Invalidate();

            //else
            //    index = -1;
        }

        private void button2_Click(object sender, EventArgs pea)
        {
            index = this.nodeListLength - 1;
            panel1.Paint -= new PaintEventHandler(panel1_Paintgraph);
            panel1.Paint += new PaintEventHandler(panel1_Painttree);
            panel1.Invalidate();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != null)
            {
                Int32.TryParse((string)comboBox1.SelectedItem, out this.nodeListLength);
            }
        }
    }
}