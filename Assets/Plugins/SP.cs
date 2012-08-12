using UnityEngine;
using System.Collections;

public class SP : MonoBehaviour {

	public enum signingparameters
	{
		CCN_SP_TEMPL_TIMESTAMP      = 0x0001,
        CCN_SP_TEMPL_FINAL_BLOCK_ID = 0x0002,
        CCN_SP_TEMPL_FRESHNESS      = 0x0004,
        CCN_SP_TEMPL_KEY_LOCATOR    = 0x0008,
        CCN_SP_FINAL_BLOCK          = 0x0010,
        CCN_SP_OMIT_KEY_LOCATOR     = 0x0020,
	}
}
