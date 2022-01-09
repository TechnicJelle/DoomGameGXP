using System;
using System.Reflection;
using System.Collections.Generic;

namespace GXPEngine.Managers
{
	public class UpdateManager
	{
		private delegate void UpdateDelegate();
		
		private UpdateDelegate _updateDelegates;
		private Dictionary<GameObject, UpdateDelegate> _updateReferences = new Dictionary<GameObject, UpdateDelegate>();
		
		//------------------------------------------------------------------------------------------------------------------------
		//														UpdateManager()
		//------------------------------------------------------------------------------------------------------------------------
		public UpdateManager ()
		{
		}
		
		//------------------------------------------------------------------------------------------------------------------------
		//														Step()
		//------------------------------------------------------------------------------------------------------------------------
		public void Step ()
		{
			if (_updateDelegates != null)
				_updateDelegates ();
		}

		//------------------------------------------------------------------------------------------------------------------------
		//														Add()
		//------------------------------------------------------------------------------------------------------------------------
		public void Add(GameObject gameObject) {
			MethodInfo info = gameObject.GetType().GetMethod("Update", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
			if (info != null) {
				UpdateDelegate onUpdate = (UpdateDelegate)Delegate.CreateDelegate(typeof(UpdateDelegate), gameObject, info, false);
				if (onUpdate != null && !_updateReferences.ContainsKey(gameObject)) {
					_updateReferences[gameObject] = onUpdate;
					_updateDelegates += onUpdate;
				}
			} else {
				validateCase(gameObject);
			}
		}
		
		//------------------------------------------------------------------------------------------------------------------------
		//														validateCase()
		//------------------------------------------------------------------------------------------------------------------------
		private void validateCase(GameObject gameObject) {
			MethodInfo info = gameObject.GetType().GetMethod("Update", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
			if (info != null) {
				throw new Exception("'Update' function was not binded for '" + gameObject + "'. Please check its case. (capital U?)");
			}
		}

		//------------------------------------------------------------------------------------------------------------------------
		//														Contains()
		//------------------------------------------------------------------------------------------------------------------------
		public Boolean Contains (GameObject gameObject)
		{
			return _updateReferences.ContainsKey (gameObject);
		}

		//------------------------------------------------------------------------------------------------------------------------
		//														Remove()
		//------------------------------------------------------------------------------------------------------------------------
		public void Remove(GameObject gameObject) {
			if (_updateReferences.ContainsKey(gameObject)) {
				UpdateDelegate onUpdate = _updateReferences[gameObject];
				if (onUpdate != null) _updateDelegates -= onUpdate;			
				_updateReferences.Remove(gameObject);
			}
		}

		public string GetDiagnostics() {
			string output = "";
			output += "Number of update delegates: " + _updateReferences.Count+'\n';
			return output;
		}
	}
}

