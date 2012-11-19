using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;

public class Marker : MonoBehaviour {

	public enum ccn_marker:long{
    CCN_MARKER_NONE = -1,
    CCN_MARKER_SEQNUM  = 0x00, /**< consecutive block sequence numbers */
    CCN_MARKER_CONTROL = 0xC1, /**< commands, etc. */ 
    CCN_MARKER_OSEQNUM = 0xF8, /**< deprecated */
    CCN_MARKER_BLKID   = 0xFB, /**< nonconsecutive block ids */
    CCN_MARKER_VERSION = 0xFD  /**< timestamp-based versioning */
	}

}
