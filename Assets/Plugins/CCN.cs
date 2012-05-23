using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;

public class CCN : MonoBehaviour {
	
	
	// ccn/charbuf.h
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
	
	[DllImport ("CCNxPlugin")]
	//[return: MarshalAs(UnmanagedType.LPStruct)]
	public static extern IntPtr ccn_charbuf_create();
	
	
	[DllImport ("CCNxPlugin")]
	public static extern int WriteSlice(IntPtr h, System.String prefix, System.String topo);
	// returns 0 for success
	
	[DllImport ("CCNxPlugin")]
	public static extern IntPtr ReadFromRepo(System.String name);
		
		
	[DllImport ("CCNxPlugin")]
	public static extern void WriteToRepo( System.String name, System.String content);
	
	[DllImport ("CCNxPlugin")]
	public static extern IntPtr ReadFromBuffer();
	[DllImport ("CCNxPlugin")]
	public static extern void PutToBuffer(string name, string content);
	[DllImport ("CCNxPlugin")]
	public static extern void testbuffer(int time);
	
	// from C#, mode = 'r', name = content = null
	[DllImport ("CCNxPlugin")]
	public static extern IntPtr Buffer(char mode, string name, string content);
	
	[DllImport ("CCNxPlugin")]
	public static extern IntPtr GetHandle();
	
	[DllImport ("CCNxPlugin")]
	public static extern int WatchOverRepo(IntPtr h, string p, string t);
	
	[DllImport ("CCNxPlugin")]
	public static extern int ccn_run(IntPtr h, int timeout);
	
	[DllImport ("CCNxPlugin")]
	public static extern int ccn_set_run_timeout(IntPtr h, int timeout);

	[DllImport ("CCNxPlugin")]
	public static extern void RegisterInterestFilter(IntPtr ccn, string name);
	
	[DllImport ("CCNxPlugin")]
	public static extern void AskForState(IntPtr ccn, string name, int timeout);
	
	[DllImport ("CCNxPlugin")]
	public static extern int WriteToStateBuffer(string state, int statelens);
}
