using UnityEngine;
using System.Collections;

public class VersioningFlags : MonoBehaviour {

	/* Not all of these flags make sense with all of the operations */
	public const int CCN_V_REPLACE = 1 ; /**< if last component is version, replace it */
	public const int CCN_V_LOW     = 2; /**< look for early version */
	public const int CCN_V_HIGH    = 4;  /**< look for newer version */
	public const int CCN_V_EST     = 8; /**< look for extreme */
	public const int CCN_V_LOWEST  = (2|8);
	public const int CCN_V_HIGHEST = (4|8);
	public const int CCN_V_NEXT    = (4|1);
	public const int CCN_V_PREV    = (2|1);
	public const int CCN_V_NOW     = 16; /**< use current time */
	public const int CCN_V_NESTOK  = 32; /**< version within version is ok */ 
}
