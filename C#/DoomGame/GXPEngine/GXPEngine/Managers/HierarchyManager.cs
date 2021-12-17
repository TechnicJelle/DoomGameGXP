using GXPEngine;
using System.Collections.Generic;
using System;

namespace GXPEngine {
	/// <summary>
	/// If you are getting strange bugs because you are calling Destroy during the Update loop, 
	/// you can use this class to do this more cleanly: when using 
	/// HierarchyManager.Instance.LateDestroy,
	/// all these hierarchy changes will be made after the update loop is finished.
	/// You can also use HierarchyManager.Instance.LateAdd to add a game object after the update loop is finished.
	/// Similarly, you can use HierarchyManager.Instance.LateCall to postpone a certain method call until 
	/// after the update loop.
	/// </summary>
	public class HierarchyManager {

		struct GameObjectPair {
			public GameObject parent;
			public GameObject child;
			public int index;
			public GameObjectPair(GameObject pParent, GameObject pChild, int pIndex = -1) {
				parent = pParent;
				child = pChild;
				index = pIndex;
			}
		}

		public static HierarchyManager Instance {
			get {
				if (instance == null) {
					instance = new HierarchyManager ();
				}				
				return instance;
			}
		}
		private static HierarchyManager instance; 

		private List<GameObjectPair> toAdd;
		private List<GameObject> toDestroy;
		private List<GameObject> toRemove;
		private List<Action> toCall;

		// Private constructor: don't construct these yourself - get the one HierarchyManager using HierarchyManager.Instance
		HierarchyManager() {
			Game.main.OnAfterStep += UpdateHierarchy;
			toAdd = new List<GameObjectPair> ();
			toDestroy = new List<GameObject> ();
			toRemove = new List<GameObject> ();
			toCall = new List<Action> ();
		}

		public void LateAdd(GameObject parent, GameObject child, int index=-1) {
			toAdd.Add (new GameObjectPair(parent,child,index));
		}

		public void LateDestroy(GameObject obj) {
			toDestroy.Add (obj);
		}

		public void LateRemove(GameObject obj) {
			toRemove.Add (obj);
		}

		public bool IsOnDestroyList(GameObject obj) {
			return toDestroy.Contains (obj);
		}

		public void LateCall(Action meth) {
			toCall.Add (meth);
		}

		public void UpdateHierarchy() {
			foreach (GameObjectPair pair in toAdd) {
				if (pair.index >= 0) {
					pair.parent.AddChildAt (pair.child, pair.index);
				} else {
					pair.parent.AddChild (pair.child);
				}
			}
			toAdd.Clear ();

			foreach (GameObject obj in toDestroy) {
				obj.Destroy ();
			}
			toDestroy.Clear ();

			foreach (GameObject obj in toRemove) {
				obj.Remove ();
			}
			toRemove.Clear ();

			// This type of loop supports calling LateCall from within the loop:
			for (int i = 0; i < toCall.Count; i++) {
				toCall [i] ();
			}
			toCall.Clear ();
		}
	}
}
