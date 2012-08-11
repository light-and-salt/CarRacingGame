using UnityEngine;
using System.Collections;

public class Upcall : MonoBehaviour {

	/**
 	* Upcalls return one of these values.
 	*/
	public enum ccn_upcall_res {
   		CCN_UPCALL_RESULT_ERR = -1, 		/**< upcall detected an error */
    	CCN_UPCALL_RESULT_OK = 0,   		/**< normal upcall return */
    	CCN_UPCALL_RESULT_REEXPRESS = 1, 	/**< reexpress the same interest again */
    	CCN_UPCALL_RESULT_INTEREST_CONSUMED = 2,/**< upcall claims to consume interest */
    	CCN_UPCALL_RESULT_VERIFY = 3, 		/**< force an unverified result to be verified */
    	CCN_UPCALL_RESULT_FETCHKEY = 4 		/**< request fetching of an unfetched key */
	}
	
	
	/**
 	* This tells what kind of event the upcall is handling.
 	*
 	* The KEYMISSING and RAW codes are used only if deferred verification has been
 	* requested.
 	*/
	public enum ccn_upcall_kind {
    	CCN_UPCALL_FINAL,             /**< handler is about to be deregistered */
    	CCN_UPCALL_INTEREST,          /**< incoming interest */
    	CCN_UPCALL_CONSUMED_INTEREST, /**< incoming interest, someone has answered */
    	CCN_UPCALL_CONTENT,           /**< incoming verified content */
    	CCN_UPCALL_INTEREST_TIMED_OUT,/**< interest timed out */
    	CCN_UPCALL_CONTENT_UNVERIFIED,/**< content that has not been verified */
    	CCN_UPCALL_CONTENT_BAD,       /**< verification failed */
   		CCN_UPCALL_CONTENT_KEYMISSING,/**< key has not been fetched */
    	CCN_UPCALL_CONTENT_RAW        /**< verification has not been attempted */
	}


}
