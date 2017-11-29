using System;
using System.Collections.Generic;
using System.Linq;

namespace Prim
{
    class Node
    {
        private Node node;
        public Node() { }

        public Node(Node node)
        {
            this.node = node;
        }

        public Node(int id)
        {
            this.Id = id;
        }

        public int Id { get; set; }
        public int RootId { get; set; }
        public int RootPathWeight { get; set; }
        public int BridgeToRootId { get; set; }
        public List<Node> neighbours { get; set; }
        public List<int> Weight { get; set; }

        public void sendBPDU()
        {
            foreach(var itm in neighbours)
            {
                itm.receiveBPDU(new BPDUPacket
                {
                    RootBridgeId = RootId,
                    RootPathCost=this.RootPathWeight,
                    SenderBridgeId=this.Id
                });
            }
            return;
        }

        public void receiveBPDU(BPDUPacket packet)
        {
            int bridgeIndex = neighbours.Select(n => n.Id).ToList().IndexOf(packet.SenderBridgeId);
            if (packet.RootPathCost + Weight[bridgeIndex] < this.RootPathWeight)
            {
                this.RootPathWeight = packet.RootPathCost + Weight[bridgeIndex];
                this.BridgeToRootId = packet.SenderBridgeId;
                sendBPDU();
            }
            return;
        }
    }
}