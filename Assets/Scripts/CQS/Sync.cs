using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

public class Sync : MonoBehaviour {
	
	public static IntPtr h;
	
	public static bool NewObj = false;
	public static string NewObjName = "";
	public static string NewObjContent = "";
	
	public static bool Initialized = false;
	
	public static System.String prefix = "ccnx:/ndn/ucla.edu/apps/cqs/game0/scene0";
	private static System.String topo = "ccnx:/ndn/broadcast/cqs/game0/scene0";
	
	Thread oThread;
	
	void Start()
	{
		// create a handle
		h = CCN.GetHandle();
		
		// Write a Slice to repo
		int res = CCN.WriteSlice(h, prefix, topo);
		print("WriteSlice returned: " + res);
		if(res != 0)
		{
			Initialized = false;
			Debug.Log ("WriteSlice Failed. -- CQS");
			return;
		}

		// Watch Over the repo
		CCN.WatchOverRepo(h, prefix, topo);
		
		// Start a new thread for ccn_run()
		// Otherwise Unity blocks
		oThread = new Thread(new ThreadStart(run));
      	oThread.Start();
		
		Initialized = true;	 
	}
	
	public void run()
	{
		CCN.ccn_run(h, -1);
	}
	
	
	IEnumerator WatchOverRepo()
	{
		CCN.WatchOverRepo(h, prefix, topo);
		while(true)
		{
			yield return new WaitForSeconds(0);
        	CCN.ccn_run(h, 13);
		}
	}
	
	void Update()
	{
		if(Initialized == true)
		{	
			
			CCN.bufnode BufNode;
			BufNode.name = "";
			BufNode.content = "";
			BufNode.next = IntPtr.Zero;
			
			IntPtr temp = CCN.ReadFromBuffer();
			if(temp != IntPtr.Zero)
			{
				BufNode = (CCN.bufnode)Marshal.PtrToStructure(temp, typeof(CCN.bufnode));
				print(BufNode.name);
				print(BufNode.content);
				NewObjName = BufNode.name;
				NewObjContent = BufNode.content;
				Sync.NewObj = true;
				Marshal.FreeCoTaskMem(temp);
			}
		}
	}
	
	void OnApplicationQuit() 
	{
		print ("quitting...");
		print ("killing thread...");
		CCN.ccn_set_run_timeout(h, 0);
		oThread.Abort();
		oThread.Join();
	}
}
