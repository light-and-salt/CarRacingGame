using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

public class Sync : MonoBehaviour {
	
	public static IntPtr h;
	public static IntPtr hh;
	
	public static bool NewObj = false;
	public static string NewObjName = "";
	public static string NewObjContent = "";
	
	public static bool Initialized = false;
	
	public static System.String prefix = "ccnx:/ndn/ucla.edu/apps/cqs/car/scene0";
	private static System.String topo = "ccnx:/ndn/broadcast/cqs/car/scene0";
	
	Thread oThread;
	
	public GameObject Car;
	public static string me = "";
	public static Hashtable Others;
	
	
	bool KnownCar(string name)
	{
		// returns true if a diamond is already in the scene
		// returns false otherwise
		return Others.ContainsKey(name);
	}
	
	void Start()
	{
		// prepare
		Others = new Hashtable();
		
		// start
		h = CCN.GetHandle();
		hh = CCN.GetHandle();
		
		int res = CCN.WriteSlice(h, prefix, topo);
		print("WriteSlice returned: " + res);

		CCN.WatchOverRepo(h, prefix, topo);
		
		CarToRepo();
		
    	CCN.RegisterInterestFilter(h, me + "/state");
    
		oThread = new Thread(new ThreadStart(run));
      	oThread.Start();
		
		
	}
	
	void CarToRepo()
	{
		
		Car = GameObject.Find ("Car");
		float pos_x = UnityEngine.Random.Range(923.3f, 993.4f);
		float pos_y = 101.1f;
		float pos_z = 1743.6f - pos_x + UnityEngine.Random.Range(0f, 20f);
		Vector3 pos = new Vector3(pos_x, pos_y, pos_z);
		Car.transform.position = pos;


		
			System.String name = Sync.prefix + "/0/" + UnityEngine.Random.Range(-999999, 999999);
			System.String content = "" + pos.x + "," + pos.y + "," + pos.z;
			print ("Writing " + name + " to repo: " + content);
			
			CCN.WriteToRepo(name, content+','+Car.GetInstanceID());
		
		Car.name = "" + Car.GetInstanceID();
		
		// Others.Add (name, content);
		me = name;
	}
	
	public void run()
	{
		CCN.ccn_run(h, -1);
	}
	
	void Update()
	{
		// read from repo for New Players	
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
		
		
		// write to C state buffer
		string state = "" + Car.transform.position.x + ", " 
			+ Car.transform.position.y + ", " + Car.transform.position.z;
		CCN.WriteToStateBuffer(state, 128);
		
		
		
		if(NewObj == true)
		{
			// read from Sync for the new object
			string shortname = "";
			int index = Sync.NewObjName.IndexOf('%');
			if(index <= 0)
				shortname = Sync.NewObjName;
			else
				shortname = Sync.NewObjName.Remove (index-1);
			
			string content = Sync.NewObjContent;
			
			if(shortname.EndsWith("/state"))
				ApplyNewState(shortname, content);
			else
				WelcomeNewPlayer(shortname, content);
		}
		
		
		// Ask for state of other players
		foreach(DictionaryEntry d in Others)
		{
			CCN.AskForState(hh, d.Key.ToString()+"/state", 1000);
		}
		CCN.ccn_run(hh, 10);
	}
	
	void ApplyNewState(string shortname, string content)
	{
		print("New State: " + shortname + ", " + content);
		int index = shortname.IndexOf("/state");
		shortname = shortname.Remove(index);
		
		print (shortname);
		print(Others[shortname]);
		string hashvalue = Others[shortname].ToString();
		string [] split = hashvalue.Split(new Char [] {','});
		string id = split[4];
		
		print(id);
		GameObject Car = GameObject.Find(id);
		
		split = content.Split(new Char [] {','});
		print(Car);
		Car.transform.position = new Vector3(Single.Parse(split[0]), Single.Parse(split[1]), Single.Parse(split[2]));
		
		NewObj = false;
	}
	
	void WelcomeNewPlayer(string shortname, string content)
	{
		if(KnownCar(shortname) == false && shortname != me)
			{
				
				print ("New Player Joined. " + shortname + ", " + content);

				string [] split = content.Split(new Char [] {','});
				Vector3 pos = new Vector3(Single.Parse(split[0]), Single.Parse(split[1]), Single.Parse(split[2]));
				GameObject NewCar;
				NewCar = Instantiate(Car, pos, Car.transform.rotation) as GameObject;
				NewCar.name = "" + NewCar.GetInstanceID();
				Others.Add(shortname, content+','+NewCar.GetInstanceID());

			}
			else
				print("Known Player. " + shortname + ", " + content);

			Sync.NewObj = false;
			Sync.NewObjName = "";
			Sync.NewObjContent = "";
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
