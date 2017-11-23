using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace WindowsFormsApp1
{
    public class Switch
    {
        // Internal clock (this could possibly be external, but this simplifies
        // the simulation).
        public long clock = 0;

        // Attributes for indentification
        public long macAddress { get; set; }    // Bridge MAC address
        public int numBridge { get; set; }      // Bridge number
        public String label { get; set; }      // Friendly name

        // Number of ports
        public int portsQty { get; set; }

        // Attributes for topology
        public Boolean root { get; set; }
        public long rootAddress { get; set; }
        public int rootPort { get; set; }

        // Attributes for simulation
        public int processingDelay { get; set; } // Imaginary delay between send and react
        public int sendDelay { get; set; }       // Imaginary delay between react and reset

        public Hashtable MACtoPorts { get; set; }
        public Port[] ports { get; set; }

        // Attributes for drawing
        public int xPos { get; set; }            // Drawable position
        public int yPos { get; set; }

        /* ********** METHODS ********** */

        public void paint(Graphics page)
        {
            page.DrawRectangle(new Pen(root ? Color.Red : Color.Black), new Rectangle(xPos, yPos, 30, portsQty * 10 + 1));
            //page.setColor(Color.black);
            for (int i = 0; i < portsQty; i++)
            {
                // Each port has a 10x10 area
                ports[i].paint(page, xPos + 20, yPos + i * 10);
            }
            //page.setColor(Color.black);
            page.DrawString(label, new Font(FontFamily.GenericSansSerif, 1), new SolidBrush(Color.Black), xPos, yPos + portsQty * 10 + 1 + 10);

        }

        /* INIT METHODS */
        private void setupPorts(int portsQty)
        {
            int i;
            ports = new Port[portsQty];
            for (i = 0; i < portsQty; i++)
            {
                ports[i] = new Port(this, i);
            }
        }

        public Switch(long macAddress, int numBridge, int portsQty, int xPos, int yPos)
        {
            this.root = true;
            this.rootAddress = rootAddress;
            this.macAddress = macAddress;
            this.numBridge = numBridge;
            this.xPos = xPos;
            this.yPos = yPos;
            this.setupPorts(portsQty);
            this.portsQty = (portsQty > 0 ? portsQty : 0);
        }

        public Switch(long macAddress, int numBridge, int portsQty, String label,
               int xPos, int yPos/*, TextArea log*/)
        {
            this.root = true;
            this.rootAddress = macAddress;
            this.macAddress = macAddress;
            this.numBridge = numBridge;
            this.xPos = xPos;
            this.yPos = yPos;
            this.label = label;
            //this.log = log;
            this.setupPorts(portsQty);
            this.portsQty = portsQty;
        }

        /* PROPERTIES ACCESS METHODS */
        public long getMacAddress()
        {
            return macAddress;
        }

        public int getPortsQty()
        {
            return portsQty;
        }

        public String getLabel()
        {
            return label;
        }

        /* UI METHODS */
        public String getName()
        {
            return "Bridge " + label + " (" + macAddress + ")";
        }

        private void explain(String explanation)
        {
            //if (log != null) {
            //    log.append(getName() + " " + explanation + "\n");
            //}
        }

        /* SEGMENT INTERACTION METHODS */

        // Attach a port of the bridge to a segment
        public Boolean attachPort(int num, Segment segment)
        {
            if (num < portsQty && ports[num].destination == null)
            {
                segment.AttachPort(ports[num]);
                ports[num].destination = segment;
                return true;
            }

            return false;
        }

        // Detach a port of the bridge from a segment
        public Boolean detachPort(int num, Segment segment)
        {
            if (num < portsQty && ports[num].destination == segment)
            {
                segment.DetachPort(ports[num]);
                ports[num].destination = null;
                return true;
            }

            return false;
        }

        public void receiveFrame(int num, STPPacket frame)
        {
            int i;
            // Configuration bpdu (we do not simulate other frames)
            if (frame.type == 0)
            {
                if (root)
                {
                    // Determine if we are still the root bridge
                    if (frame.macRoot < macAddress)
                    {
                        // We are no longer the root!
                        root = false;
                        explain("Lost root status - detected better root: mac " + frame.macRoot + " attainable on port " + (num + 1));
                        explain("Broadcasting new root information on all other ports...");

                        // Set the root port information
                        rootAddress = frame.macRoot;
                        rootPort = num;

                        // Forward a configuration BPDU on all ports (except receiver)
                        // Reset root data
                        for (i = 0; i < portsQty; i++)
                        {
                            if (i != num)
                            {
                                ports[i].setKind(Port.DES);
                                ports[i].rootPathCost = 0;
                                ports[i].remoteDesignate = -1;

                                STPPacket bpdu = new STPPacket();
                                bpdu.type = 0;                    // Configuration
                                bpdu.macRoot = frame.macRoot;
                                bpdu.macSender = macAddress;
                                bpdu.PortId = i;
                                bpdu.RootPathCost = frame.RootPathCost
                                                       + 1 / ports[num].destination.bps;
                                ports[i].send(bpdu);
                            }
                            else
                            {
                                ports[i].setKind(Port.ROOT);
                                ports[i].remoteDesignate = frame.macSender;
                                ports[i].rootPathCost = frame.RootPathCost
                                                        + 1 / ports[num].destination.bps;
                            }
                        }
                    }
                    else if (frame.macRoot == macAddress)
                    {
                        if (frame.macSender == macAddress
                            && frame.macSender == frame.macRoot)
                        {
                            if (frame.PortId < num)
                            {
                                explain("Disabling port " + (num + 1) + ": connected to same segment as port " + (frame.PortId + 1));
                                ports[num].setKind(Port.DISABLED);
                            }
                        }
                    }
                    else
                    {
                        // We are still the root
                        // In that case, squelch the frame
                        explain("Squelching root announcement from bridge mac " + frame.macSender + " who thinks mac " + frame.macRoot + " is the root (we know root to be mac " + rootAddress + ").");
                    }
                }
                else
                {
                    // We were not the root
                    if (frame.macRoot < rootAddress)
                    {
                        // There is a new root!
                        explain("Updating root info: root now mac " + frame.macRoot + " attainable on port " + (num + 1) + "at cost " + (frame.RootPathCost + 1 / ports[num].destination.bps) + " (root was mac " + rootAddress + ").");
                        rootAddress = frame.macRoot;
                        rootPort = num;

                        // Reset all costs on other ports;
                        for (i = 0; i < portsQty; i++)
                        {
                            if (i != num)
                            {
                                ports[i].setKind(Port.DES);
                                ports[i].rootPathCost = 0;
                                ports[i].remoteDesignate = -1;
                            }
                            else
                            {
                                ports[i].setKind(Port.ROOT);
                                ports[i].rootPathCost = frame.RootPathCost
                                                      + 1 / ports[num].destination.bps;
                                ports[i].remoteDesignate = frame.macSender;
                            }
                        }

                        // Forward a configuration BPDU on all ports (except receiver)
                        for (i = 0; i < portsQty; i++)
                        {
                            if (i != num)
                            {
                                STPPacket bpdu = new STPPacket();
                                bpdu.type = 0;                    // Configuration
                                bpdu.macRoot = frame.macRoot;
                                bpdu.macSender = macAddress;
                                bpdu.PortId = i;
                                bpdu.RootPathCost = frame.RootPathCost
                                                       + 1 / ports[num].destination.bps;
                                ports[i].send(bpdu);
                            }
                        }
                    }
                    else if (frame.macRoot == rootAddress)
                    {
                        // This case only happens if there is a loop on our network.
                        if (frame.RootPathCost + 1 / ports[num].destination.bps
                            < ports[rootPort].rootPathCost
                            || (frame.RootPathCost + 1 / ports[num].destination.bps
                                == ports[rootPort].rootPathCost
                                && ports[rootPort].remoteDesignate > frame.macSender))
                        {
                            explain("Found better route to root mac " + rootAddress + ":");
                            explain(" - Old route on port " + (rootPort + 1) + " had cost "
                                    + ports[rootPort].rootPathCost + " thru mac " + ports[rootPort].remoteDesignate);
                            explain(" - New route on port " + (num + 1) + " has cost "
                                    + (frame.RootPathCost + 1 / ports[num].destination.bps) + " thru mac " + frame.macSender);


                            // Found a better route to the root bridge
                            ports[num].rootPathCost = frame.RootPathCost
                                                      + 1 / ports[num].destination.bps;
                            ports[num].remoteDesignate = frame.macSender;

                            if (num == rootPort)
                            {
                                // The better route is thru the same port as before
                                // Update the info
                                explain(" - The old and new root ports are the same");

                            }
                            else
                            {
                                // The better route is thru another port
                                // Update the info
                                explain(" - The old and new root ports are different");
                            }

                            rootPort = num;

                            for (i = 0; i < portsQty; i++)
                            {
                                if (i == num)
                                {
                                    explain(" -> Port " + (i + 1) + " is now the root port ");
                                    ports[i].setKind(Port.ROOT);
                                }
                                else
                                {
                                    if (ports[i].rootPathCost == 0)
                                    {
                                        // We are alone for this port
                                        explain(" -> Port " + (i + 1) + " is designated: there is no other known route to the root for this segment");
                                        ports[i].remoteDesignate = -1;
                                        ports[i].setKind(Port.DES);
                                    }
                                    else if (ports[i].rootPathCost - 1 / ports[i].destination.bps
                                        > frame.RootPathCost + 1 / ports[num].destination.bps)
                                    {
                                        // We are designated for this port
                                        explain(" -> Port " + (i + 1) + " is designated: this bridge has the best known route to the root for this segment (cost " + ports[i].rootPathCost + ")");
                                        ports[i].setKind(Port.DES);
                                        ports[i].remoteDesignate = -1;
                                    }
                                    else if (ports[i].rootPathCost - 1 / ports[i].destination.bps
                                        == frame.RootPathCost + 1 / ports[num].destination.bps)
                                    {
                                        if (ports[i].remoteDesignate == -1
                                            || ports[i].remoteDesignate > macAddress)
                                        {
                                            // We are designated for this port
                                            explain(" -> Port " + (i + 1) + " is designated: this bridge has one of the best known route to the root for this segment and a lower mac than the other best known routes");
                                            ports[i].setKind(Port.DES);
                                            ports[i].remoteDesignate = -1;
                                        }
                                        else
                                        {
                                            // Tie: we lose and are no longer the designate
                                            explain(" -> Port " + (i + 1) + " is not designated this bridge has one of the best known route to the root for this segment but a higher mac than the other best known routes");
                                            ports[i].setKind(Port.NOTDES);
                                        }
                                    }
                                    else
                                    {
                                        explain(" -> Port " + (i + 1) + " is not designated this bridge has a worse route for this segment than the current known best route thru mac " + ports[i].remoteDesignate);
                                        ports[i].setKind(Port.NOTDES);
                                    }
                                }
                            }

                            // Update children
                            for (i = 0; i < portsQty; i++)
                            {
                                if (i != num)
                                {
                                    STPPacket bpdu = new STPPacket();
                                    bpdu.type = 0; // Configuration
                                    bpdu.macRoot = rootAddress;
                                    bpdu.macSender = macAddress;
                                    bpdu.PortId = i;
                                    bpdu.RootPathCost = ports[rootPort].rootPathCost;
                                    ports[i].send(bpdu);
                                }
                            }

                        }
                        else
                        {
                            // The bpdu relayed to us has a higher cost than our current
                            // root path, but it may come from one of our neighbors and
                            // indicate it is in a better position than we are to
                            // service a particular segment.
                            if (ports[rootPort].rootPathCost > frame.RootPathCost
                                || (ports[rootPort].rootPathCost == frame.RootPathCost
                                   && macAddress > frame.macSender))
                            {
                                if (ports[num].remoteDesignate == -1 ||
                                    ports[num].remoteDesignate > frame.macSender)
                                {
                                    ports[num].remoteDesignate = frame.macSender;
                                    explain("Received indication from mac " + frame.macSender + " that it is a better designate than our previously known best route for segments atteched to port " + (num + 1));
                                }

                                if (rootPort != num)
                                {
                                    explain("Received indication from mac " + frame.macSender + " that it has a better route than ours for segment attached to port " + (num + 1) + "; port no longer designated.");
                                    ports[num].setKind(Port.NOTDES);
                                }
                            }
                            else
                            {
                                explain("Received a frame from mac " + frame.macSender + " for current root (mac " + rootAddress + ") with a path cost higher than ours. Not doing anything...");
                                if (ports[rootPort].rootPathCost
                                    - 1 / ports[rootPort].destination.bps
                                    < frame.RootPathCost)
                                {
                                    explain("- Our cost: " + ports[rootPort].rootPathCost + "; their cost: " + frame.RootPathCost);
                                }
                                else
                                {
                                    explain("- Our mac: " + macAddress + "; their mac: " + frame.macSender);
                                }
                            }
                        }



                    }
                    else
                    {
                        // This is from a bridge that thinks it is the root
                        // Squelch the frame
                        explain("Squelching root announcement from bridge mac " + frame.macSender + " who thinks mac " + frame.macRoot + " is the root (we know root to be mac " + rootAddress + ").");
                    }
                }
            }
        }
    }
}
