using UnityEngine;
using System;
using System.Collections;


public class Control : MonoBehaviour {
	
	public GameObject Car;
	public static int MaxN = -1;
	private static int ObjID = -1;
	
	private Hashtable KnownList;
	bool KnownCar(string name)
	{
		// returns true if a diamond is already in the scene
		// returns false otherwise
		return KnownList.ContainsKey(name);
	}
	
	IEnumerator Start()
	{
		KnownList = new Hashtable();
		
		Car = GameObject.Find ("Car");
		float pos_x = UnityEngine.Random.Range(923.3f, 993.4f);
		float pos_y = 101.1f;
		float pos_z = 1743.6f - pos_x + UnityEngine.Random.Range(0f, 25f);
		Vector3 pos = new Vector3(pos_x, pos_y, pos_z);
		Car.transform.position = pos;
		
		ObjID++;
		// MaxN++;
		
		while (Sync.Initialized == false)
			yield return new WaitForSeconds(0.01f);
		
		if (Sync.Initialized == true)
		{
			System.String name = Sync.prefix + "/" + ObjID + "/" + UnityEngine.Random.Range(-999999f, 999999f);
			System.String content = "" + pos.x + "," + pos.y + "," + pos.z;
			print ("Writing " + name + " to repo: " + content);
			CCN.WriteToRepo(name, content);
			KnownList.Add (name, content);
		}
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
			if(KnownCar(shortname) == false)
			{
				// this diamond is new, unknow
				// we must instantiate it
				print ("New Diamond");
				
				string [] split = content.Split(new Char [] {','});
				int type = Int32.Parse(split[0]);
				Vector3 pos = new Vector3(Int32.Parse(split[1]), Int32.Parse(split[2]), Int32.Parse(split[3]));
				GameObject NewGem;
				NewGem = Instantiate(Car, pos, Quaternion.identity) as GameObject;
				
				ObjID++;
				KnownList.Add(shortname, content);
				
			}
			
			Sync.NewObj = false;
			Sync.NewObjName = "";
			Sync.NewObjContent = "";
			
		}
		
    }
}
