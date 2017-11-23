using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace WindowsFormsApp1 {
    public class Port {

        public Segment destination;
        private Switch father;
        private int portNum;
        public int state = 0;             // 0: inactive  1: sending   2: receiving
        public int gstate = 0;                   // 0: disabled  1: blocking  2: listening
                                          // 3: learning  4: forwarding

        // Cost from this port to root
        public float rootPathCost = 0;
        public long remoteDesignate = -1;

        // Role we play on bridge
        public Boolean designated = false;
        public Boolean root = false;

        // Timer
        public long lastChangeAt = 0;

        public Port(Switch father, int portNum) {
            this.father = father;
            this.portNum = portNum;
        }

        public static int ROOT = 1;
        public static int DES = 2;
        public static int NOTDES = 3;
        public static int ALLDES = 4;
        public static int DISABLED = 0;

        public void setKind(int newKind) {
            if (newKind == DISABLED) {
                designated = false;
                root = false;
                setState(0);
            }
            else if (newKind == NOTDES) {
                designated = false;
                root = false;
                setState(1);            // Blocking
            }
            else if (newKind == DES || newKind == ALLDES) {
                designated = true;
                root = false;
                if (gstate < 2) setState(2);
            }
            else if (newKind == ROOT) {
                designated = false;
                root = true;
                if (gstate < 2) setState(2);
            }
        }

        public void setState(int newState) {
            lastChangeAt = father.clock;
            gstate = newState;
        }
        // Draw method
        public void paint(Graphics page, int x, int y) {
            // Reset area
            //page.setColor(Color.white);
            //page.fillRect(x + 1, y + 1, 8, 8);

            if (state == 0) {
                // Inactive
                //page.setColor(Color.black);
            }
            else if (state == 1) {
                // Sending
                //page.setColor(Color.red);
            }
            else if (state == 2) {
                // Receiving
                //page.setColor(Color.green);
            }

            switch (gstate) {
                case 0:
                    page.DrawEllipse(new Pen(Color.Black),x + 1, y + 1, 8, 8);
                    break;
                case 1:
                    page.DrawLine(new Pen(Color.Black), x + 1, y + 1, x + 1, y + 9);
                    page.DrawLine(new Pen(Color.Black), x + 3, y + 1, x + 3, y + 9);
                    page.DrawLine(new Pen(Color.Black), x + 5, y + 1, x + 5, y + 9);
                    break;
                case 2:
                    page.FillEllipse(new SolidBrush(Color.Black), x + 1, y + 1, 8, 8);
                    break;

                case 3:
                    page.DrawRectangle(new Pen(Color.Black), x + 2, y + 2, 6, 6);
                    break;

                case 4:
                    page.DrawRectangle(new Pen(Color.Black), x + 1, y + 1, 8, 8);
                    break;

            }

            if (destination != null) {
                if (root) {
                    // Thick line
                    page.DrawLine(new Pen(Color.Black), x + 8, y + 3, destination.xPos, y + 3);
                    page.DrawLine(new Pen(Color.Black), x + 8, y + 4, destination.xPos, y + 4);
                    page.DrawLine(new Pen(Color.Black), x + 8, y + 6, destination.xPos, y + 6);
                }
                if (designated) {
                    // Medium line
                    page.DrawLine(new Pen(Color.Black), x + 8, y + 4, destination.xPos, y + 4);
                }
                page.DrawLine(new Pen(Color.Black), x + 8, y + 5, destination.xPos, y + 5);
            }
        }

        /* SEGMENT INTERACTION METHODS */

        // Receive data from the segment
        public void receive(STPPacket frame, int segmentBitRate) {
            switch (gstate) {
                case 0:
                    // Ignore
                    break;
                case 1:
                    if (frame.type == 0 || frame.type == 1) {
                        father.receiveFrame(portNum, frame);
                    }
                    break;
                default:
                    father.receiveFrame(portNum, frame);
                    break;
            }

        }

        // Send data to attached segment
        public Boolean send(STPPacket bpdu) {
            if (destination != null) {
                if (gstate > 0) { // The port must not be disabled
                    destination.arrive(this, bpdu);
                    return true;
                }
            }
            return false;
        }
    }
}
