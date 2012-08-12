using UnityEngine;
using System.Collections;

public class Content : MonoBehaviour {

	public enum ccn_content_type {
    	CCN_CONTENT_DATA = 0x0C04C0,
    	CCN_CONTENT_ENCR = 0x10D091,
    	CCN_CONTENT_GONE = 0x18E344,
    	CCN_CONTENT_KEY  = 0x28463F,
    	CCN_CONTENT_LINK = 0x2C834A,
    	CCN_CONTENT_NACK = 0x34008A
	}

}
