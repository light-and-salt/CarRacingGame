using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;

public class Egal: MonoBehaviour {
	
	
	// Structs //
	//==================================//
	[StructLayout (LayoutKind.Sequential)]
	public struct ccn_charbuf 
	{
		public int length;
    	public int limit;
    	public IntPtr buf;
	}
	
	[StructLayout (LayoutKind.Sequential)]
	public struct bufnode 
	{
		public string name;
    	public string content;
    	public IntPtr next;
	}
	
	/**
 	* Handle for upcalls that allow clients receive notifications of
 	* incoming interests and content.
 	*
 	* The client is responsible for managing this piece of memory and the
 	* data therein. The refcount should be initially zero, and is used by the
 	* library to keep to track of multiple registrations of the same closure.
 	* When the count drops back to 0, the closure will be called with
 	* kind = CCN_UPCALL_FINAL so that it has an opportunity to clean up.
 	*/
	[StructLayout (LayoutKind.Sequential)]
	public struct ccn_closure {
		ccn_handler p;      	/**< client-supplied handler */
		public IntPtr data;     /**< for client use */
		int intdata;   			/**< for client use */
		private int refcount;       	/**< client should not update this directly */
		
		// constructor
		public ccn_closure(ccn_handler cb, IntPtr pdata, int idata)
		{
			p = cb;
			data = pdata;
			intdata = idata;
			refcount = 0;
		}
	}
	
	/**
 	* Additional information provided in the upcall.
 	*
 	* The client is responsible for managing this piece of memory and the
 	* data therein. The refcount should be initially zero, and is used by the
 	* library to keep to track of multiple registrations of the same closure.
 	* When the count drops back to 0, the closure will be called with
 	* kind = CCN_UPCALL_FINAL so that it has an opportunity to clean up.
 	*/
	[StructLayout (LayoutKind.Sequential)]
	public struct ccn_upcall_info {
    	public IntPtr h;              /**< The ccn library handle */
    	/* Interest (incoming or matched) */
    	IntPtr interest_ccnb;
    	IntPtr pi;
    	IntPtr interest_comps;
    	int matched_comps;
    	/* Incoming content for CCN_UPCALL_CONTENT* - otherwise NULL */
    	IntPtr content_ccnb;
    	IntPtr pco;
    	IntPtr content_comps;
	}

	/**
 	* Parameters for creating signed content objects.
 	*
 	* A pointer to one of these may be passed to ccn_sign_content() for
 	* cases where the default signing behavior does not suffice.
 	* For the default (sign with the user's default key pair), pass NULL
 	* for the pointer.
 	*
 	* The recommended way to us this is to create a local variable:
 	*
 	*   struct ccn_signing_params myparams = CCN_SIGNING_PARAMS_INIT;
	*
 	* and then fill in the desired fields.  This way if additional parameters
 	* are added, it won't be necessary to go back and modify exiting clients.
 	* 
 	* The template_ccnb may contain a ccnb-encoded SignedInfo to supply
 	* selected fields from under the direction of sp_flags.
 	* It is permitted to omit unneeded fields from the template, even if the
 	* schema says they are manditory.
 	*
 	* If the pubid is all zero, the user's default key pair is used for
 	* signing.  Otherwise the corresponding private key must have already
 	* been supplied to the handle using ccn_load_private_key() or equivalent.
 	*
 	* The default signing key is obtained from ~/.ccnx/.ccnx_keystore unless
 	* the CCNX_DIR is used to override the directory location.
 	*/
 
	[StructLayout (LayoutKind.Sequential)]
	public struct ccn_signing_params {
    	int api_version;
    	public SP.signingparameters sp_flags;
    	IntPtr template_ccnb;
    	[MarshalAs (UnmanagedType.ByValTStr, SizeConst=32)]
		public string pubid;
    	public Content.ccn_content_type co_type;
    	int freshness;
    	// XXX where should digest_algorithm fit in?
		
		// constructor
		public ccn_signing_params(int api)
		{
			api_version = api;
			sp_flags = 0;
			template_ccnb = IntPtr.Zero;
			pubid = "";
			co_type = Content.ccn_content_type.CCN_CONTENT_DATA;
			freshness = -1;
		}
	}

	
	// Aggregated Functions//
	//==================================//	
	[DllImport ("Egal")]
	public static extern int WriteSlice(IntPtr h, System.String prefix, System.String topo);
	// returns 0 for success
	
	[DllImport ("Egal")]
	public static extern IntPtr ReadFromRepo(System.String name);
			
	[DllImport ("Egal")]
	public static extern void WriteToRepo( System.String name, System.String content);
	
	[DllImport ("Egal")]
	public static extern IntPtr ReadFromBuffer();
	[DllImport ("Egal")]
	public static extern void PutToBuffer(string name, string content);
	[DllImport ("Egal")]
	public static extern void testbuffer(int time);
	
	// from C#, mode = 'r', name = content = null
	[DllImport ("Egal")]
	public static extern IntPtr Buffer(char mode, string name, string content);
	
	[DllImport ("Egal")]
	public static extern IntPtr GetHandle();
	
	[DllImport ("Egal")]
	public static extern int WatchOverRepo(IntPtr h, string p, string t);
	
	[DllImport ("Egal")]
	public static extern void RegisterInterestFilter(IntPtr ccn, string name);
	
	[DllImport ("Egal")]
	public static extern void AskForState(IntPtr ccn, string name, int timeout);
	
	[DllImport ("Egal")]
	public static extern int WriteToStateBuffer(string state, int statelens);
	
	
	// CCN-level Functions//
	//==================================//
	
	// ccnd, handle
	[DllImport ("Egal")]
	public static extern IntPtr ccn_create();
	
	[DllImport ("Egal")]
	public static extern int ccn_connect(IntPtr h, string name);

	[DllImport ("Egal")]
	public static extern int ccn_run(IntPtr h, int timeout);
	
	[DllImport ("Egal")]
	public static extern int ccn_set_run_timeout(IntPtr h, int timeout);
	
	// charbuf, name
	[DllImport ("Egal")]
	public static extern IntPtr ccn_charbuf_create();
	
	[DllImport ("Egal")]
	public static extern void ccn_charbuf_destroy(ref IntPtr cbp);
	
	[DllImport ("Egal")]
	public static extern IntPtr ccn_charbuf_as_string(IntPtr c);
	
	[DllImport ("Egal")]
	public static extern int ccn_charbuf_append(IntPtr c, string p, int n);
	
	[DllImport ("Egal")]
	public static extern int ccn_charbuf_append_charbuf(IntPtr c, IntPtr n);

	[DllImport ("Egal")]
	public static extern int ccn_name_init(IntPtr c);
	
	[DllImport ("Egal")]
	public static extern int ccn_name_from_uri(IntPtr c, string uri);
	
	[DllImport ("Egal")]
	public static extern int ccn_uri_append(IntPtr c, IntPtr ccnb, int size, int includescheme);
	
	[DllImport ("Egal")]
	public static extern int ccn_create_version(IntPtr h, IntPtr name,
                   int versioning_flags, int secs, int nsecs);
	
	[DllImport ("Egal")]
	public static extern int ccn_name_append_nonce(IntPtr c);
	
	[DllImport ("Egal")]
	public static extern IntPtr SyncCopyName(IntPtr name);
	
	[DllImport ("Egal")]
	public static extern int ccn_name_append_numeric(IntPtr c, Marker.ccn_marker marker, int value);


	// slice, ccns
	[DllImport ("Egal")]
	public static extern IntPtr ccns_slice_create();
	
	[DllImport ("Egal")]
	public static extern int ccns_slice_set_topo_prefix(IntPtr s, IntPtr t, IntPtr p);
	
	[DllImport ("Egal")]
	public static extern int ccns_write_slice(IntPtr h, IntPtr slice, IntPtr name);
	
	[DllImport ("Egal")]
	public static extern void ccns_slice_destroy(ref IntPtr sp);
	
	[DllImport ("Egal")]
	public static extern IntPtr ccns_open(IntPtr h, IntPtr slice,
          ccns_callback callback, IntPtr rhash, IntPtr pname);
	
	
	// interest 
	[DllImport ("Egal")]
	public static extern IntPtr SyncGenInterest(IntPtr name, int scope, int lifetime, 
		int maxSuffix, int childPref, IntPtr excl);
	
	[DllImport ("Egal")]
	public static extern int ccn_set_interest_filter(IntPtr h, IntPtr namebuf, 
		IntPtr p_ccn_closure);
	
	[DllImport ("Egal")]
	public static extern int ccn_express_interest(IntPtr h, IntPtr namebuf,
		IntPtr p_ccn_closure, IntPtr interest_template);
	
	// signning
	[DllImport ("Egal")]
	public static extern int ccn_sign_content(IntPtr h, IntPtr resultbuf, IntPtr name_prefix, 
		IntPtr param, IntPtr data, int size);
	
	[DllImport ("Egal")]
	public static extern int ccn_chk_signing_params(IntPtr h,
                       IntPtr param,
                       IntPtr result,
                       ref IntPtr ptimestamp,
                       ref IntPtr pfinalblockid,
                       ref IntPtr pkeylocator);
			
	// actions
	[DllImport ("Egal")]
	public static extern int ccn_put(IntPtr h, IntPtr p, int length); // returns -1 for error


	
	
	// Delegates, for Callback //
	//==================================//
	public delegate int ccns_callback (IntPtr ccns, IntPtr lhash, IntPtr rhash, IntPtr pname);
	
	public delegate Upcall.ccn_upcall_res ccn_handler (IntPtr selfp, Upcall.ccn_upcall_kind kind, IntPtr info);

	
	
	// Tests //
	[DllImport ("Egal")]
	public static extern int nine();
	
}
