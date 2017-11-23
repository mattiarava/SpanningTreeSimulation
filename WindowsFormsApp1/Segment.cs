using System.Collections.Generic;
using System.Drawing;

namespace WindowsFormsApp1 {
    /* The Segment class represents a LAN segment -- the part of a network
 * wherein everything that is sent by one station is received by all
 * others.
 *
 * This class enqueues send operations instead of simulating collisions.
 */

    public class Segment {

        public int bps;                        // Bit-rate of the segment
        public int xPos = 0;                   // Display position
        public int segNum;                     // Segment number

        private List<Port> attachedPorts = new List<Port>();         // Ports to transmit to
        private FrameQueue waitingFrames = new FrameQueue(); // Frames to transmit


        // Constructor: determines the speed of the segment and its position on map
        public Segment(int bps, int xPos, FrameQueue afq, int segNum) {
            if (afq != null) waitingFrames = afq;
            this.bps = bps;
            this.xPos = xPos;
            this.segNum = segNum;
        }

        // Call when a new port joins the segment
        public void AttachPort(Port port) {
            if (!attachedPorts.Contains(port)) {
                attachedPorts.Add(port);
            }
        }

        // Call when a port is removed from the segment
        public void DetachPort(Port port) {
            if (attachedPorts.Contains(port)) {
                attachedPorts.Remove(port);
            }
        }

        /* PORT INTERACTION METHODS */

        // Call when a frame is emitted on the segment
        public void arrive(Port sender, STPPacket bpdu) {
            waitingFrames.enqueue(new FrameInfo(this, sender, bpdu));
        }

        // Call to make a frame depart to every attached port on the segment
        public void depart(FrameInfo i) {
            Port p;
            if (i == null) {
                return;
            }
            foreach (var itm in attachedPorts) {
                if (itm != i.sender) {
                    itm.receive(i.bpdu, bps);
                }
            }
        }

            // Call to make the segment conduct a frame
            public void broadcast(Port origin) {
            FrameInfo i = waitingFrames.dequeue();
            Port p;
            if (i == null) {
                return;
            }
            foreach (var itm in attachedPorts) {
                if (itm != i.sender) {
                    itm.receive(i.bpdu, bps);
                }
            }
        }

        /* DISPLAY METHOD */
        public void paint(Graphics page, int h) {
            if (waitingFrames.empty()) {
                // Inactive
                //page.setColor(Color.black);
                foreach(var itm in attachedPorts) {
                    itm.state = 0;
                }
            }
            else {
                // Frames waiting
                FrameInfo i = waitingFrames.Peek();
                if (i.domain == this) {
                    //page.setColor(Color.green);
                    foreach (var p in attachedPorts) {
                        if (p != i.sender) {
                            p.state = 1;
                        }
                        else {
                            p.state = 2;
                        }
                    }
                }
                else {
                    foreach (var p in attachedPorts) {
                        p.state = 0;
                    }
                    if (waitingFrames.match_domain(this)) {
                        //page.setColor(Color.blue);
                    }
                    else {
                        //page.setColor(Color.black);
                    }
                }
            }
            page.DrawLine(new Pen(Color.Black), xPos, 3, xPos, h - 10);
            page.DrawLine(new Pen(Color.Black), xPos + 1, 3, xPos + 1, h - 10);

            //page.setColor(Color.black);
            page.DrawString( "S" + (segNum + 1),new Font(FontFamily.GenericSerif,1,FontStyle.Regular), new SolidBrush(Color.Black) , xPos + 2, h - 15);
            page.DrawString("(sp " + bps + ")", new Font(FontFamily.GenericSerif, 1, FontStyle.Regular), new SolidBrush(Color.Black), xPos + 2, h - 5);
        }
    }
}
