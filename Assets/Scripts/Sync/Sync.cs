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
	public static int TIMEOUT = 10;
	
	Thread oThread;
	
	public GameObject Car;
	public static string me = "";
	public static Hashtable Others;
	
	public static int counter_for_run = 0;
	
	static bool KnownCar(string name)
	{
		return Others.ContainsKey(name);
	}
	
	void Start()
	{
		// prepare
		Others = new Hashtable();
		
		// start
		h = GetHandle();
		
		int res = WriteSlice(h, prefix, topo);
		print("WriteSlice returned: " + res);
		
		WatchOverRepo(h, prefix, topo);
		
		CarToRepo(h);
		
    	RegisterInterestFilter(h, me + "/state");
    
		oThread = new Thread(new ThreadStart(run));
      	oThread.Start();
	}
	
	IntPtr GetHandle()
	{
		// this is a C# expansion of Egal.GetHandle()
		IntPtr ccn = Egal.ccn_create();
		if (Egal.ccn_connect(ccn, "") == -1) 
        	print("could not connect to ccnd.\n");
		else
			print ("a handle is connected to ccnd.");
		return ccn;
	}
	
	int WriteSlice(IntPtr h, System.String p, System.String t)
	{
		// this is a C# expansion of Egal.WriteSlice
		int res;
		IntPtr prefix = Egal.ccn_charbuf_create();
		IntPtr topo = Egal.ccn_charbuf_create();
		
		Egal.ccn_name_init(prefix);
    	Egal.ccn_name_init(topo);
		
		res = Egal.ccn_name_from_uri(prefix, p);
		if(res<0)
		{
			print ("Prefix not right");
			return res;
		}
		
		res = Egal.ccn_name_from_uri(topo, t);
		if(res<0)
		{
			print ("Topo not right");
			return res;
		}
		
		int timeout = TIMEOUT;
    	if (timeout < -1) 
    	{
   	    	print("Timeout cannot be less than -1");
        	return -1;
    	}
    	timeout *= 1000;
		
		IntPtr slice = Egal.ccns_slice_create();
		Egal.ccns_slice_set_topo_prefix(slice, topo, prefix);
		
		res = Egal.ccns_write_slice(h, slice, prefix);
    
    	Egal.ccns_slice_destroy(ref slice); // after this, slice == 0
		
		return res;
	}
	
	static String NameTrim(String playername)
	{
		// remove version and whatever after it from names
		String ShortPlayerName = "";
		int index = playername.IndexOf('%');
		if(index <= 0)
			ShortPlayerName = playername;
		else
			ShortPlayerName = playername.Remove (index-1);
		
		return ShortPlayerName;
	}
	
	static int WatchCallback(IntPtr ccns, IntPtr lhash, IntPtr rhash, IntPtr pname)
	{
		print ("WatchCallback...");
		
		IntPtr uri = Egal.ccn_charbuf_create();
		
		Egal.ccn_charbuf Name = (Egal.ccn_charbuf)Marshal.PtrToStructure(pname, typeof(Egal.ccn_charbuf));
		Egal.ccn_uri_append(uri, Name.buf, Name.length, 1);
		
		IntPtr temp = Egal.ccn_charbuf_as_string(uri);
		String PlayerName = Marshal.PtrToStringAnsi(temp);
		
		
		String ShortPlayerName = NameTrim(PlayerName);
		
		if(KnownCar(ShortPlayerName) == false && ShortPlayerName != me)
		{
			print ("New Player Joined.");
		}
		else
		{
			print ("Known Player.");
		}
		
		
		Egal.ccn_charbuf_destroy(ref uri);
		return 0;
	}
	
	int WatchOverRepo(IntPtr h, string p, string t)
	{
		// this is a C# expansion of Egal.WatchOverRepo
		int res;
		IntPtr prefix = Egal.ccn_charbuf_create();
		IntPtr topo = Egal.ccn_charbuf_create();
		
		Egal.ccn_name_init(prefix);
    	Egal.ccn_name_init(topo);
		
		res = Egal.ccn_name_from_uri(prefix, p);
		if(res<0)
		{
			print ("Prefix not right");
			return res;
		}
		
		res = Egal.ccn_name_from_uri(topo, t);
		if(res<0)
		{
			print ("Topo not right");
			return res;
		}
		
		int timeout = TIMEOUT;
    	if (timeout < -1) 
    	{
   	    	print("Timeout cannot be less than -1");
        	return -1;
    	}
    	timeout *= 1000;
		
		IntPtr slice = Egal.ccns_slice_create();
    	Egal.ccns_slice_set_topo_prefix(slice, topo, prefix);
    
    	IntPtr ccns = Egal.ccns_open(h, slice, WatchCallback, IntPtr.Zero, IntPtr.Zero);
    
    	// ccns_close(&ccns, NULL, NULL);
    
    	Egal.ccns_slice_destroy(ref slice);
    
    	return res;
		
	}
	
	public struct NormalStruct
	{
    	public IntPtr nm;
    	IntPtr cb;
    	public IntPtr ccn; 	// handle
    	public int vSize;
    	public string value; /* not so sure */
		
		// constructor
		public NormalStruct(IntPtr name, IntPtr contentbuffer, IntPtr handle,
			 int valuesize, string content)
		{
			this.nm = name;
			this.cb = contentbuffer;
			this.ccn = handle;
			this.vSize = valuesize;
			this.value = content;
		}
	}
	
	int PutContent(IntPtr h, NormalStruct Data)
	{
		int res = 0;
		
		Egal.ccn_charbuf Nm = new Egal.ccn_charbuf();
		Nm = (Egal.ccn_charbuf)Marshal.PtrToStructure(Data.nm, typeof(Egal.ccn_charbuf));
		
		IntPtr uri = Egal.ccn_charbuf_create();
		Egal.ccn_uri_append(uri, Nm.buf, Nm.length, 0);
		
		IntPtr name = Egal.SyncCopyName(Data.nm);
		IntPtr cb = Egal.ccn_charbuf_create();
		IntPtr cob = Egal.ccn_charbuf_create();
		
		Egal.ccn_charbuf_append(cb, Data.value, Data.vSize);
		Egal.ccn_charbuf Cb = new Egal.ccn_charbuf();
		Cb = (Egal.ccn_charbuf)Marshal.PtrToStructure(cb, typeof(Egal.ccn_charbuf));
		
		Egal.ccn_signing_params Sp = new Egal.ccn_signing_params(CCN.CCN_API_VERSION);
		Sp.sp_flags |= SP.signingparameters.CCN_SP_FINAL_BLOCK;
		IntPtr pSp = Marshal.AllocHGlobal(Marshal.SizeOf(Sp));
		Marshal.StructureToPtr(Sp, pSp, true);
		
		/*
		// test zone
		Egal.ccn_signing_params Test = new Egal.ccn_signing_params(CCN.CCN_API_VERSION);
		IntPtr pTest = Marshal.AllocHGlobal(Marshal.SizeOf(Test));
		Marshal.StructureToPtr(Test, pTest, true);
		IntPtr a = IntPtr.Zero;
		IntPtr b = IntPtr.Zero;
		IntPtr c = IntPtr.Zero;
		res = Egal.ccn_chk_signing_params(h, pSp, pTest, ref a, ref b, ref c);
		print ("check result: " + res);
		// test zone end
		*/
		
		// Data.ccn, Cb.buf, Data.vSize is correct
		res |= Egal.ccn_sign_content(h, cob, name, pSp, Cb.buf, Data.vSize);
		if(res<0) print ("sign content error.");
		
		Egal.ccn_charbuf Cob = new Egal.ccn_charbuf();
		Cob = (Egal.ccn_charbuf)Marshal.PtrToStructure(cob, typeof(Egal.ccn_charbuf));
		res |= Egal.ccn_put(Data.ccn, Cob.buf, Cob.length);
		if (res<0) print ("ccn_put error.");
		
		// cleanup
		Egal.ccn_charbuf_destroy(ref uri);
		Egal.ccn_charbuf_destroy(ref name);
		Egal.ccn_charbuf_destroy(ref cb);
		Egal.ccn_charbuf_destroy(ref cob);
		Marshal.FreeHGlobal(pSp);
		
		return res;
	}
	
	Upcall.ccn_upcall_res WriteCallback(IntPtr selfp, Upcall.ccn_upcall_kind kind, IntPtr info)
	{
		print("WriteCallback... " + kind);
		Upcall.ccn_upcall_res ret = Upcall.ccn_upcall_res.CCN_UPCALL_RESULT_OK;
		
		Egal.ccn_upcall_info Info = new Egal.ccn_upcall_info();
			Info = (Egal.ccn_upcall_info)Marshal.PtrToStructure(info, typeof(Egal.ccn_upcall_info));
			IntPtr h = Info.h;
		
			Egal.ccn_closure Selfp = new Egal.ccn_closure();
			Selfp = (Egal.ccn_closure)Marshal.PtrToStructure(selfp, typeof(Egal.ccn_closure));
			NormalStruct Data = new NormalStruct();
			Data = (NormalStruct) Marshal.PtrToStructure(Selfp.data, typeof(NormalStruct));
		
		switch(kind)
		{
		case Upcall.ccn_upcall_kind.CCN_UPCALL_FINAL:
           	// Marshal.FreeHGlobal(selfp); // this again, will make Unity crash
			break;
			
		case Upcall.ccn_upcall_kind.CCN_UPCALL_INTEREST:
			
			
			PutContent(h, Data); // publish content
			ret = Upcall.ccn_upcall_res.CCN_UPCALL_RESULT_INTEREST_CONSUMED;
			break;
			
		case Upcall.ccn_upcall_kind.CCN_UPCALL_CONTENT:
			// repo first returns a content...
			// this is info of the repo...
			break;
			
		default:
			ret = Upcall.ccn_upcall_res.CCN_UPCALL_RESULT_ERR;
			break;
		}
		
		// print ("ref count: " + Selfp.refcount);
		// print ("WriteCallback returnning..." + ret);
		return ret;
	}
	
	void WriteToRepo(IntPtr h, System.String name, System.String content)
	{
		print ("Writing " + name + " to repo: " + content);
		
		int res;
		
		IntPtr cb = Egal.ccn_charbuf_create();
		IntPtr nm = Egal.ccn_charbuf_create();
		IntPtr cmd = Egal.ccn_charbuf_create();
		
		Egal.ccn_name_from_uri(nm, name);
		Egal.ccn_create_version(h, nm, VersioningFlags.CCN_V_NOW, 0, 0);
		
		NormalStruct Data = new NormalStruct(nm, cb, h, content.Length, content);
		IntPtr pData = Marshal.AllocHGlobal(Marshal.SizeOf(Data));
		Marshal.StructureToPtr(Data, pData, true);
		
		IntPtr template = Egal.SyncGenInterest(IntPtr.Zero, 1, 4, -1, -1, IntPtr.Zero);
		
		Egal.ccn_closure Action = new Egal.ccn_closure(WriteCallback, pData, 0);
		IntPtr pnt = Marshal.AllocHGlobal(Marshal.SizeOf(Action));
		Marshal.StructureToPtr(Action, pnt, true);

		res = Egal.ccn_set_interest_filter(h, nm, pnt); // listen: interest
		
		res = Egal.ccn_charbuf_append_charbuf(cmd, nm);	
		res = Egal.ccn_name_from_uri(cmd, "%C1.R.sw");
		Egal.ccn_name_append_nonce(cmd);
		
		counter_for_run++;
		res = Egal.ccn_set_run_timeout(h, 0);
		Egal.ccn_express_interest(h, cmd, pnt, template); // express interest
		counter_for_run--;
		 
		return;
	}
	
	void CarToRepo(IntPtr h)
	{
		Car = GameObject.Find ("Car");
		float pos_x = UnityEngine.Random.Range(923.3f, 993.4f);
		float pos_y = 101.1f;
		float pos_z = 1743.6f - pos_x + UnityEngine.Random.Range(0f, 20f);
		Vector3 pos = new Vector3(pos_x, pos_y, pos_z);
		Car.transform.position = pos;
		
		System.String name = Sync.prefix + "/0/" + UnityEngine.Random.Range(-999999, 999999);
		System.String content = "" + pos.x + "," + pos.y + "," + pos.z;
			
		WriteToRepo(h, name, content+','+Car.GetInstanceID());
		
		Car.name = "" + Car.GetInstanceID();
		
		// Others.Add (name, content);
		me = name;
	}
	
	Upcall.ccn_upcall_res PublishState(IntPtr selfp,
                                        Upcall.ccn_upcall_kind kind,
                                        IntPtr info)
	{
		print ("Publish State...");
		return Upcall.ccn_upcall_res.CCN_UPCALL_RESULT_OK;
		
	}
	
	int RegisterInterestFilter(IntPtr h, string name)
	{
		int res = 0;
    	IntPtr nm = Egal.ccn_charbuf_create();
    	res = Egal.ccn_name_from_uri(nm, name);
    	if (res < 0)  return res;
 
    	NormalStruct State = new NormalStruct(nm, IntPtr.Zero, h, 0, "");
		IntPtr pState = Marshal.AllocHGlobal(Marshal.SizeOf(State));
		Marshal.StructureToPtr(State, pState, true);
		
    	Egal.ccn_closure Action = new Egal.ccn_closure(PublishState, pState, 0);
    	IntPtr pAction = Marshal.AllocHGlobal(Marshal.SizeOf(Action));
		Marshal.StructureToPtr(Action, pAction, true);
		
    	res = Egal.ccn_set_interest_filter(h, nm, pAction);
		return res;
	}
	
	public void run()
	{
		Thread t = Thread.CurrentThread;
		// print (t.IsAlive);
		while(t.IsAlive == true)
		{
			while(counter_for_run>0)
				;
			Egal.ccn_run(h, -1);
			
		}
		
	}
	
	/*
	void Update()
	{
		// read from repo for New Players	
		Egal.bufnode BufNode;
		BufNode.name = "";
		BufNode.content = "";
		BufNode.next = IntPtr.Zero;
		
		IntPtr temp = Egal.ReadFromBuffer();
		if(temp != IntPtr.Zero)
		{
			BufNode = (Egal.bufnode)Marshal.PtrToStructure(temp, typeof(Egal.bufnode));
			//print(BufNode.name);
			//print(BufNode.content);
			NewObjName = BufNode.name;
			NewObjContent = BufNode.content;
			Marshal.FreeCoTaskMem(temp);
			
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
		
		
		// write to C state buffer
		string state = "" + Car.transform.position.x + ", " 
			+ Car.transform.position.y + ", " + Car.transform.position.z;
		Egal.WriteToStateBuffer(state, 128);
		
		// Ask for state of other players
		foreach(DictionaryEntry d in Others)
		{
			Egal.AskForState(hh, d.Key.ToString()+"/state", 1000);
		}
		Egal.ccn_run(hh, 10);
	}
	*/
	
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
		Egal.ccn_set_run_timeout(h, 0);
		oThread.Abort();
		oThread.Join();
	}
}
