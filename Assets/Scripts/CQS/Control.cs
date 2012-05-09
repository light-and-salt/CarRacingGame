using UnityEngine;
using System;
using System.Collections;


public class Control : MonoBehaviour {
	
	public GameObject[] GemArray = new GameObject[4];
	public static int MaxN = -1;
	private static int ObjID = -1;
	
	private Hashtable KnownList;
	bool KnownDiamond(string name)
	{
		// returns true if a diamond is already in the scene
		// returns false otherwise
		return KnownList.ContainsKey(name);
	}
	
	void Start()
	{
		GemArray[0] = GameObject.Find ("purple diamond");
		GemArray[1] = GameObject.Find ("gray diamond");
		GemArray[2] = GameObject.Find ("lavendar diamond");
		GemArray[3] = GameObject.Find ("turquoise diamond");
		GemArray[0].active = false;
		GemArray[1].active = false;
		GemArray[2].active = false;
		GemArray[3].active = false;
		
		KnownList = new Hashtable();
	}
	
	void Update() {
		if(Sync.NewObj == true)
		{
			// read from Sync for the new object
			string shortname = "";
			
			int index = Sync.NewObjName.IndexOf('%');
			
			if(index == 0)
			{
				shortname = Sync.NewObjName;
			}
			else
			{
				shortname = Sync.NewObjName.Remove (index-1);
			}
			string content = Sync.NewObjContent;
			print ("Control: Got Object From Sync -- " + shortname + ", " + content);
			if(KnownDiamond(shortname) == false)
			{
				// this diamond is new, unknow
				// we must instantiate it
				print ("New Diamond");
				
				string [] split = content.Split(new Char [] {','});
				int type = Int32.Parse(split[0]);
				Vector3 pos = new Vector3(Int32.Parse(split[1]), Int32.Parse(split[2]), Int32.Parse(split[3]));
				GameObject NewGem;
				NewGem = Instantiate(GemArray[type], pos, Quaternion.identity) as GameObject;
				
				ObjID++;
				KnownList.Add(shortname, content);
				
			}
			
			
			Sync.NewObj = false;
			Sync.NewObjName = "";
			Sync.NewObjContent = "";
			
		}
		
        if (Input.GetKeyUp (KeyCode.Space) && Sync.Initialized == true)
		{
			Vector3 pos = new Vector3(UnityEngine.Random.Range(30, 50), 10, UnityEngine.Random.Range(3, 18));
			int type = UnityEngine.Random.Range(0, 4);
			GameObject NewGem;
			NewGem = Instantiate(GemArray[type], pos, Quaternion.identity) as GameObject;
			
			ObjID++;
			// MaxN++;
			System.String name = Sync.prefix + "/" + ObjID + "/" + UnityEngine.Random.Range(-999999, 999999);
			System.String content = "" + type + "," + pos.x + "," + pos.y + "," + pos.z;
			print ("Writing " + name + " to repo: " + content);
			CCN.WriteToRepo(name, content);
			KnownList.Add (name, content);
		}   
    }
}
