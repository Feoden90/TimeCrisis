using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CustomManager
{
	//Base pool manager class. Will be specialized by children.
	public class PoolManager<TKey,TVal>
	{
		//lists that will keep track of object usage;
		private List<TKey> freeObjects;
		private List<TKey> busyObjects;

		//the object container;
		private Dictionary<TKey,TVal> objectList;

		//the getter for the object container (I messed with the setter, so i did this instead);
		public Dictionary<TKey,TVal> getObjectList(){
			return objectList;
		}

		//the public constructor (passing off the function that creates the object TVal);
		public PoolManager(CustomizeObject callback){
			//initialization of containers;
			freeObjects = new List<TKey> ();
			busyObjects = new List<TKey> ();
			objectList = new Dictionary<TKey, TVal> ();
			//assigning the function callback;
			delegateCustomizeObject = callback;
		}

		//public function that instantiates new objects (used 
		public void AllocateNewObjects(int number){
			for (int i = 0; i < number; i++) {
				InstantiateNewObject ();
			}
		}
		
		//delegate for custom object editing;
		public delegate KeyValuePair<TKey,TVal> CustomizeObject();
		public event CustomizeObject delegateCustomizeObject;

		//method that creates and makes usable the new object.
		private void InstantiateNewObject ()
		{
			//Setup the game object as needed;
			KeyValuePair<TKey,TVal> obj = delegateCustomizeObject ();
			//add the game object as child of the manager;
			//tmp.transform.SetParent (this.transform);
			//create an ID for the object, it will be used as access key;
			freeObjects.Add (obj.Key);
			objectList.Add (obj.Key, obj.Value);
		}

		//method that fetches the first free object (creates a new one if none).
		public TVal GetNextFreeObjectInstance ()
		{
			//if no free objects, create a new one;
			if (freeObjects.Count == 0) {
				InstantiateNewObject ();
			}
			//get the first free object and flag it as occupied;
			TKey key = freeObjects [0];
			freeObjects.Remove (key);
			busyObjects.Add (key);
			//return the object instance;
			return objectList [key];
		}

		//method that frees an object instance so that it can be reused.
		public void ReleaseObjectInstance (TKey key)
		{
			//object gets flagged as free again;
			freeObjects.Add (key);
			busyObjects.Remove (key);
		}

	}
}