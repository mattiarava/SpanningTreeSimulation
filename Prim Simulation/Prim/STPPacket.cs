using System;

public class BPDUPacket
{
	public BPDUPacket()
	{
        this.MaxAge    = TimeSpan.FromMilliseconds(20000); //Default 20 seconds for BPDU
        this.HelloTime = TimeSpan.FromMilliseconds(2000); //Default 2 seconds in 802.1D
	}
    public int RootBridgeId   { get; set; }
    public int RootPathCost   { get; set; }
    public int SenderBridgeId { get; set; }
    public TimeSpan MaxAge    { get; set; }
    public TimeSpan HelloTime { get; set; }
    public int ForwardDelay   { get; set; }
}
