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
		public IntPtr length;
    	public IntPtr limit;
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
    	object data;         	/**< for client use */
    	int intdata;   			/**< for client use */
    	int refcount;       	/**< client should not update this directly */
		
		// constructor
		public ccn_closure(ccn_handler cb, System.Object pdata, int idata)
		{
			p = cb;
			data = pdata;
			intdata = idata;
			refcount = 0;
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
	public static extern int ccn_charbuf_append_charbuf(IntPtr c, IntPtr n);

	[DllImport ("Egal")]
	public static extern int ccn_name_init(IntPtr c);
	
	[DllImport ("Egal")]
	public static extern int ccn_name_from_uri(IntPtr c, string uri);
	
	[DllImport ("Egal")]
	public static extern int ccn_uri_append(IntPtr c, IntPtr ccnb, IntPtr size, int includescheme);
	
	[DllImport ("Egal")]
	public static extern int ccn_create_version(IntPtr h, IntPtr name,
                   int versioning_flags, int secs, int nsecs);
	
	[DllImport ("Egal")]
	public static extern int ccn_name_append_nonce(IntPtr c);


	
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

	
	
	// Delegates, for Callback //
	//==================================//
	public delegate int ccns_callback (IntPtr ccns, IntPtr lhash, IntPtr rhash, IntPtr pname);
	
	public delegate Upcall.ccn_upcall_res ccn_handler (IntPtr selfp, Upcall.ccn_upcall_kind kind, IntPtr info);

	
	
	// Tests //
	[DllImport ("Egal")]
	public static extern int nine();
	
}
