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
    	public String buf;
	}
	
	[StructLayout (LayoutKind.Sequential)]
	public struct bufnode 
	{
		public string name;
    	public string content;
    	public IntPtr next;
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
	[DllImport ("Egal")]
	//[return: MarshalAs(UnmanagedType.LPStruct)]
	public static extern IntPtr ccn_charbuf_create();
	
	[DllImport ("Egal")]
	public static extern IntPtr ccn_create();
	
	[DllImport ("Egal")]
	public static extern int ccn_connect(IntPtr h, string name);

	[DllImport ("Egal")]
	public static extern int ccn_run(IntPtr h, int timeout);
	
	[DllImport ("Egal")]
	public static extern int ccn_set_run_timeout(IntPtr h, int timeout);
	
	[DllImport ("Egal")]
	public static extern int ccn_name_init(IntPtr c);
	
	[DllImport ("Egal")]
	public static extern int ccn_name_from_uri(IntPtr c, string uri);
	
	[DllImport ("Egal")]
	public static extern IntPtr ccns_slice_create();
	
	[DllImport ("Egal")]
	public static extern int ccns_slice_set_topo_prefix(IntPtr s, IntPtr t, IntPtr p);
	
	[DllImport ("Egal")]
	public static extern int ccns_write_slice(IntPtr h, IntPtr slice, IntPtr name);
	
	[DllImport ("Egal")]
	public static extern void ccns_slice_destroy(ref IntPtr sp);
	

}
