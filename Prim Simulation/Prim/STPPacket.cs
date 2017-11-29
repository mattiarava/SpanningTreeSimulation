using System;

public class BPDUPacket
{
	public BPDUPacket()
	{
        this.MaxAge    = TimeSpan.FromMilliseconds(20000); //Default 20 seconds for BPDU
        this.HelloTime = TimeSpan.FromMilliseconds(2000); //Default 2 seconds in 802.1D
	}
    public int RootBridgeId   { get; set; }
    public int RootPathCost { get; set; }
    public int SenderBridgeId { get; set; }
    public TimeSpan MaxAge    { get; set; }
    public TimeSpan HelloTime { get; set; }
    public int ForwardDelay   { get; set; }
}

///* Queue ADT (Abstract Data Type) for a DataFrame
// */

//public class FrameQueue {
//    FrameQueueItem first, last;

//    public Boolean empty() {
//        return first == null;
//    }

//    public void enqueue(FrameInfo item) {
//        FrameQueueItem new_item = new FrameQueueItem(item);
//        if (first != null) {
//            last.set_next(new_item);
//            last = new_item;
//        }
//        else {
//            last = new_item;
//            first = last;
//        }
//    }

//    public FrameInfo dequeue() {
//        if (first != null) {
//            FrameInfo result = first.get_item();
//            first = first.get_next();
//            return result;
//        }
//        else {
//            return null;
//        }
//    }

//    // Useful for draw methods
//    public FrameInfo Peek() {
//        if (first != null) {
//            FrameInfo result = first.get_item();
//            return result;
//        }
//        else {
//            return null;
//        }
//    }

//    public bool match_domain(Segment s) {
//        FrameQueueItem cursor = first;
//        while (cursor != null) {
//            if (cursor.get_item().domain == s) {
//                return true;
//            }
//            else {
//                cursor = cursor.get_next();
//            }
//        }
//        return false;
//    }

//}

///* Information on a DataFrame while traveling on a Segment.
// */

//public class FrameInfo {
//    public Port sender;  // What port sent the DataFrame
//    public STPPacket bpdu;   // The actual data
//    public Segment domain;   // Segment on which the frame is transmitted

//    public FrameInfo(Segment domain, Port sender, STPPacket bpdu) {
//        this.sender = sender;
//        this.bpdu = bpdu;
//        this.domain = domain;
//    }
//}

///* Element on a FrameQueue... part of the Queue ADT */
//public class FrameQueueItem {
//    private Port sender;
//    private FrameInfo item;
//    private FrameQueueItem next;

//    public FrameQueueItem(FrameInfo qitem) {
//        item = qitem;
//        next = null;
//    }

//    public FrameInfo get_item() {
//        return item;
//    }

//    public FrameQueueItem get_next() {
//        return next;
//    }

//    public void set_next(FrameQueueItem qitem) {
//        next = qitem;
//    }
//}
