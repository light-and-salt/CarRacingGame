using UnityEngine;
using System;
using System.Collections;


public class Control : MonoBehaviour {
	
	public GameObject Car;
	public static Hashtable KnownList;
	
	bool KnownCar(string name)
	{
		// returns true if a diamond is already in the scene
		// returns false otherwise
		return KnownList.ContainsKey(name);
	}
	
	void Start()
	{
		Car = GameObject.Find ("Car");
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
			if(KnownCar(shortname) == false)
			{
				// this diamond is new, unknow
				// we must instantiate it
				print ("New Player Joined.");

				string [] split = content.Split(new Char [] {','});
				Vector3 pos = new Vector3(Single.Parse(split[0]), Single.Parse(split[1]), Single.Parse(split[2]));
				GameObject NewGem;
				NewGem = Instantiate(Car, pos, Car.transform.rotation) as GameObject;

				
				KnownList.Add(shortname, content);

			}

			Sync.NewObj = false;
			Sync.NewObjName = "";
			Sync.NewObjContent = "";

		}

    }
	
}
