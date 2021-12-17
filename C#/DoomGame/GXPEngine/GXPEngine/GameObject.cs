using System;
using System.Collections.Generic;
using GXPEngine.Core;

namespace GXPEngine
{
	/// <summary>
	/// GameObject is the base class for all display objects. 
	/// </summary>
	public abstract class GameObject : Transformable
	{
		public string name;
		private Collider _collider;
		
		private List<GameObject> _children = new List<GameObject>();
		private GameObject _parent = null;
		
		public bool visible = true;
		private bool destroyed = false;

		//------------------------------------------------------------------------------------------------------------------------
		//														GameObject()
		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="GXPEngine.GameObject"/> class.
		/// Since GameObjects contain a display hierarchy, a GameObject can be used as a container for other objects.
		/// Other objects can be added using child commands as AddChild.
		/// </summary>
		/// <param name="addCollider">
		/// If <c>true</c>, then the virtual function createCollider will be called, which can be overridden to create a collider that 
		/// will be added to the collision manager. 
		/// </param> 
		public GameObject(bool addCollider=false)
		{
			if (addCollider) {
				_collider = createCollider ();
			}
		}

		/// <summary>
		/// Create and return a collider to use for this game object. Null is allowed.
		/// </summary>
		protected virtual Collider createCollider () {
			return null;
		}

		//------------------------------------------------------------------------------------------------------------------------
		//														Index
		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the index of this object in the parent's hierarchy list.
		/// Returns -1 if no parent is defined.
		/// </summary>
		/// <value>
		/// The index.
		/// </value>
		public int Index {
			get { 
				if (parent == null) return -1;
				return parent._children.IndexOf(this);
			}
		}

		//------------------------------------------------------------------------------------------------------------------------
		//														collider
		//------------------------------------------------------------------------------------------------------------------------
		internal Collider collider {
			get { return _collider; }
		}
		
		//------------------------------------------------------------------------------------------------------------------------
		//														game
		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the game that this object belongs to. 
		/// This is a unique instance throughout the runtime of the game.
		/// Use this to access the top of the displaylist hierarchy, and to retreive the width and height of the screen.
		/// </summary>
		public Game game {
			get {
				return Game.main;
			}
		}

		/// <summary>
		/// Get all a list of all objects that currently overlap this one.
		/// Calling this method will test collisions between this object and all other colliders in the scene.
		/// It can be called mid-step and is included for convenience, not performance.
		/// Set includeTriggers to true to include trigger colliders in the list, and 
		/// includeSolid to include solid (=non-trigger) colliders.
		/// </summary>
		public GameObject[] GetCollisions (bool includeTriggers = true, bool includeSolid = true)
		{
			return game.GetGameObjectCollisions(this, includeTriggers, includeSolid);
		}
		
		//------------------------------------------------------------------------------------------------------------------------
		//														Render
		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// This function is called by the renderer. You can override it to change this object's rendering behaviour.
		/// When not inside the GXPEngine package, specify the parameter as GXPEngine.Core.GLContext.
		/// This function was made public to accomodate split screen rendering. Use SetViewPort for that.
 		/// </summary>
		/// <param name='glContext'>
		/// Gl context, will be supplied by internal caller.
		/// </param>
		public virtual void Render(GLContext glContext) {
			if (visible) {
				glContext.PushMatrix(matrix);
				
				RenderSelf (glContext);
				foreach (GameObject child in GetChildren()) {
					child.Render(glContext);
				}
				
				glContext.PopMatrix();
			}
		}
		
		//------------------------------------------------------------------------------------------------------------------------
		//														RenderSelf
		//------------------------------------------------------------------------------------------------------------------------
		protected virtual void RenderSelf(GLContext glContext) {
			//if (visible == false) return;
			//glContext.PushMatrix(matrix);
			//glContext.PopMatrix();
		}



		//------------------------------------------------------------------------------------------------------------------------
		//														parent
		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the parent GameObject.
		/// When the parent moves, this object moves along.
		/// </summary>
		public GameObject parent {
			get { return _parent; }
			set { 
				bool wasActive = InHierarchy ();
				if (_parent != null) {
					_parent.removeChild(this);
					_parent = null;
				}
				_parent = value;
				if (value != null) {
					if (destroyed) {
						throw new Exception ("Destroyed game objects cannot be added to the game!");
					}
					_parent.addChild(this);
				}
				bool isActive = InHierarchy ();
				if (wasActive && !isActive) {
					UnSubscribe ();
				} else if (!wasActive && isActive) {
					Subscribe ();
				}
			}
		}

		//------------------------------------------------------------------------------------------------------------------------
		//														OnDestroy()
		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Subclasses can implement this method to clean up resources once on destruction. 
		/// Will be called by the engine when the game object is destroyed.
		/// </summary>
		protected virtual void OnDestroy ()
		{
			//empty
		}

		//------------------------------------------------------------------------------------------------------------------------
		//														Destroy()
		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Destroy this instance, and removes it from the game. To complete garbage collection, you must nullify all 
		/// your own references to this object.
		/// </summary>
		public virtual void Destroy ()
		{
			destroyed = true;
			// Detach from parent (and thus remove it from the managers):
			if (parent != null) parent = null;

			OnDestroy();

			// Destroy all children:
			while (_children.Count > 0) {
				GameObject child = _children[0];
				if (child != null) child.Destroy();
			}
		}

		//------------------------------------------------------------------------------------------------------------------------
		//														LateDestroy()
		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Destroy this instance, and removes it from the game, *after* finishing the current Update + OnCollision loops.
		/// To complete garbage collection, you must nullify all your own references to this object.
		/// </summary>
		public void LateDestroy() {
			HierarchyManager.Instance.LateDestroy (this);
		}

		//------------------------------------------------------------------------------------------------------------------------
		//														AddChild()
		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Adds the specified GameObject as a child to this one.
		/// </summary>
		/// <param name='child'>
		/// Child object to add.
		/// </param>
		public void AddChild(GameObject child) {
			child.parent = this;	
		}

		//------------------------------------------------------------------------------------------------------------------------
		//														LateAddChild()
		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Adds the specified GameObject as a child to this one, *after* finishing the current Update + OnCollision loops.
		/// </summary>
		/// <param name='child'>
		/// Child object to add.
		/// </param>
		public void LateAddChild(GameObject child) {
			HierarchyManager.Instance.LateAdd (this, child);
		}

		/// <summary>
		/// Removes this GameObject from the hierarchy (=sets the parent to null).
		/// </summary>
		public void Remove() {
			parent = null;
		}

		//------------------------------------------------------------------------------------------------------------------------
		//														LateDestroy()
		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Removes this GameObject from the hierarchy, *after* finishing the current Update + OnCollision loops.
		/// </summary>
		public void LateRemove() {
			HierarchyManager.Instance.LateRemove (this);
		}

		//------------------------------------------------------------------------------------------------------------------------
		//														RemoveChild()
		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Removes the specified child GameObject from this object.
		/// </summary>
		/// <param name='child'>
		/// Child object to remove.
		/// </param>
		public void RemoveChild (GameObject child)
		{
			if (child.parent == this) {
				child.parent = null;
			}
		}
		
		//------------------------------------------------------------------------------------------------------------------------
		//														removeChild()
		//------------------------------------------------------------------------------------------------------------------------
		private void removeChild(GameObject child) {
			_children.Remove(child);

		}
		
		//------------------------------------------------------------------------------------------------------------------------
		//														addChild()
		//------------------------------------------------------------------------------------------------------------------------
		private void addChild(GameObject child) {
			if (child.HasChild(this)) return; //no recursive adding
			_children.Add(child);
			return;
		}
		
		//------------------------------------------------------------------------------------------------------------------------
		//														AddChildAt()
		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Adds the specified GameObject as a child to this object at an specified index. 
		/// This will alter the position of other objects as well.
		/// You can use this to determine the draw order of child objects.
		/// </summary>
		/// <param name='child'>
		/// Child object to add.
		/// </param>
		/// <param name='index'>
		/// Index in the child list where the object should be added.
		/// </param>
		public void AddChildAt(GameObject child, int index) {
			if (child.parent != this) {
				AddChild(child);
			}
			if (index < 0) index = 0;
			if (index >= _children.Count) index = _children.Count - 1;
			_children.Remove(child);
			_children.Insert(index, child);			
		}

		//------------------------------------------------------------------------------------------------------------------------
		//														LateAddChild()
		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Adds the specified GameObject as a child to this one, at the specified index,
		/// *after* finishing the current Update + OnCollision loops.
		/// </summary>
		public void LateAddChildAt(GameObject child, int index) {
			HierarchyManager.Instance.LateAdd (this, child, index);
		}

		//------------------------------------------------------------------------------------------------------------------------
		//														HasChild()
		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Returns 'true' if the specified object is a descendant of this object.
		/// </summary>
		/// <param name='gameObject'>
		/// The GameObject that should be tested.
		/// </param>
		public bool HasChild(GameObject gameObject) {
			// for compatibility reasons, the name of this method is not changed - but it is very confusing!
			GameObject par = gameObject;
			while (par != null) {
				if (par == this) return true;
				par = par.parent;
			}
			return false;
		}

		/// <summary>
		/// Returns whether this game object is currently active, or equivalently, a descendant of Game.
		/// </summary>
		public bool InHierarchy() {
			GameObject current = parent;
			while (current != null) {
				if (current is Game)
					return true;
				current = current.parent;
			}
			return false;
		}
				
		//------------------------------------------------------------------------------------------------------------------------
		//														GetChildren()
		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Returns a list of all children that belong to this object.
		/// The function returns System.Collections.Generic.List<GameObject>.
		/// (If safe=false, then the method is slightly faster, but modifying the list will break the engine!)
		/// </summary>
		public List<GameObject> GetChildren(bool safe=true) {
			if (safe) {
				return new List<GameObject> (_children);
			} else {
				return _children;
			}
		}

		/// <summary>
		/// Returns the number of children of this game object.
		/// </summary>
		/// <returns>The number of children of this game object.</returns>
		public int GetChildCount() {
			return _children.Count;
		}
		
		//------------------------------------------------------------------------------------------------------------------------
		//														SetChildIndex()
		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Inserts the specified object in this object's child list at given location.
		/// This will alter the position of other objects as well.
		/// You can use this to determine the drawing order of child objects.
		/// </summary>
		/// <param name='child'>
		/// Child.
		/// </param>
		/// <param name='index'>
		/// Index.
		/// </param>
		public void SetChildIndex(GameObject child, int index) {
			if (child.parent != this) AddChild(child);
			if (index < 0) index = 0;
			if (index >= _children.Count) index = _children.Count - 1;
			_children.Remove(child);
			_children.Insert(index, child);
		}

		private void Subscribe() {
			game.Add (this);
			foreach (GameObject child in _children) {
				child.Subscribe ();
			}
		}

		private void UnSubscribe() {
			game.Remove (this);
			foreach (GameObject child in _children) {
				child.UnSubscribe ();
			}
		}

		//------------------------------------------------------------------------------------------------------------------------
		//														HitTest()
		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Tests if this object overlaps with the one specified. 
		/// </summary>
		/// <returns>
		/// <c>true</c>, if 'this' overlaps with 'other'.
		/// </returns>
		/// <param name='other'>
		/// The other game object.
		/// </param>
		virtual public bool HitTest(GameObject other) {
			return _collider != null && other._collider != null && _collider.HitTest (other._collider);
		}

		/// <summary>
		/// If changing the x and y coordinates of this GameObject by vx and vy respectively
		///   would cause a collision with the GameObject other, this method returns a 
		///   "time of impact" between 0 and 1,
		///   which is a scalar multiplier for vx and vy, giving the amount of safe movement until collision.
		/// It is zero if the two game objects are already overlapping, and 
		///   moving by vx and vy would cause a worse overlap.
		/// In all other cases, the returned value is bigger than 1.
		/// If a time of impact below 1 is returned, the normal will be the collision normal 
		///   (otherwise it is undefined).
		/// </summary>
		virtual public float TimeOfImpact (GameObject other, float vx, float vy, out Vector2 normal) {
			normal = new Vector2 ();
			if (_collider == null || other._collider == null || parent==null)
				return float.MaxValue;
			// Compute world space velocity:
			//Vector2 p1 = parent.TransformPoint (vx, vy);
			//Vector2 p0 = parent.TransformPoint (0, 0);
			Vector2 worldVelocity=parent.TransformDirection(vx,vy);
			float TOI=_collider.TimeOfImpact (other._collider, 
				//p1.x-p0.x, p1.y-p0.y, 
				worldVelocity.x,worldVelocity.y,
				out normal
			);
			return TOI;
		}

		/// <summary>
		/// Tries to move this object by vx,vy (in parent space, similar to the translate method), 
		/// until it collides with one of the given objects. Objects without a solid (=non-trigger) collider are ignored.
		/// In case of a collision, it returns a Collision object with information such as the normal and time of impact 
		/// (the point and penetration depth fields of the collision object will always be zero).
		/// Otherwise it returns null.
		/// 
		/// As objectsToCheck, pass an array or List of game objects to check against 
		/// (this moving game object will move through all objects that are not in the given array or list).
		/// </summary>
		virtual public Collision MoveUntilCollision(float vx, float vy, IEnumerable<GameObject> objectsToCheck) {
			Collision col = null;
			float minTOI = 1;
			foreach (GameObject other in objectsToCheck) {
				if (other.collider != null && other.collider.isTrigger) continue;
				Vector2 newNormal;
				float newTOI = TimeOfImpact (other, vx, vy, out newNormal);
				if (newTOI < minTOI) {
					col = new Collision (this, other, newNormal, newTOI);
					minTOI = newTOI;
				}
			}
			x += vx * minTOI;
			y += vy * minTOI;
			return col;
		}

		/// <summary>
		/// Tries to move this object by vx,vy (in parent space, similar to the translate method), 
		/// until it collides with another object that has a solid (=non-trigger) collider. 
		/// In case of a collision, it returns a Collision object with information such as the normal and time of impact 
		/// (the point and penetration depth fields of the collision object will always be zero).
		/// Otherwise it returns null.
		/// 
		/// Note: this is a very expensive method since it uses GetCollisions, and 
		/// tunneling is possible since it uses discrete collision detection - use with care.
		/// </summary>
		virtual public Collision MoveUntilCollision(float vx, float vy) {
			x += vx;
			y += vy;
			GameObject[] overlaps = GetCollisions (false,true);
			x -= vx;
			y -= vy;
			return MoveUntilCollision (vx, vy, overlaps);
		}

		//------------------------------------------------------------------------------------------------------------------------
		//														HitTestPoint()
		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Returns <c>true</c> if a 2D point (given in global / screen space) overlaps with this object.
		/// You could use this for instance to check if the mouse (Input.mouseX, Input.mouseY) is over the object.
		/// </summary>
		/// <param name='x'>
		/// The x coordinate to test.
		/// </param>
		/// <param name='y'>
		/// The y coordinate to test.
		/// </param>
		virtual public bool HitTestPoint(float x, float y) {
			return _collider != null && _collider.HitTestPoint(x, y);
		}		
		
		//------------------------------------------------------------------------------------------------------------------------
		//														TransformPoint()
		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Transforms a point from local to global space.
		/// If you insert a point relative to the object, it will return that same point relative to the game.
		/// </summary>
		/// <param name='x'>
		/// The x coordinate to transform.
		/// </param>
		/// <param name='y'>
		/// The y coordinate to transform.
		/// </param>
		public override Vector2 TransformPoint(float x, float y) {
			Vector2 ret = base.TransformPoint (x, y);
			if (parent == null) {
				return ret;
			} else {
				return parent.TransformPoint (ret.x, ret.y);
			}
		}

		/// <summary>
		/// Transforms a direction vector from local to global space.
		/// If you insert a vector relative to the object, it will return that same vector relative to the game.
		/// Note: only scale and rotation information are taken into account, not translation (coordinates).
		/// </summary>
		/// <param name='x'>
		/// The x coordinate to transform.
		/// </param>
		/// <param name='y'>
		/// The y coordinate to transform.
		/// </param>
		public override Vector2 TransformDirection(float x, float y) {
			Vector2 ret = base.TransformDirection (x, y);
			if (parent == null) {
				return ret;
			} else {
				return parent.TransformDirection (ret.x, ret.y);
			}
		}

		//------------------------------------------------------------------------------------------------------------------------
		//												InverseTransformPoint()
		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Transforms the point from global into local space.
		/// If you insert a point relative to the game, it will return that same point relative to this GameObject.
		/// </summary>
		/// <param name='x'>
		/// The x coordinate to transform.
		/// </param>
		/// <param name='y'>
		/// The y coordinate to transform.
		/// </param>
		public override Vector2 InverseTransformPoint(float x, float y) {
			Vector2 ret = base.InverseTransformPoint (x, y);
			if (parent == null) {
				return ret;
			} else {
				return parent.InverseTransformPoint (ret.x, ret.y);
			}
		}

		/// <summary>
		/// Transforms the vector from global into local space.
		/// If you insert a vector relative to the game, it will return that same vector relative to this GameObject.
		/// Note: only scale and rotation information are taken into account, not translation (coordinates).
		/// </summary>
		/// <param name='x'>
		/// The x coordinate to transform.
		/// </param>
		/// <param name='y'>
		/// The y coordinate to transform.
		/// </param>
		public override Vector2 InverseTransformDirection(float x, float y) {
			Vector2 ret = base.InverseTransformDirection (x, y);
			if (parent == null) {
				return ret;
			} else {
				return parent.InverseTransformDirection (ret.x, ret.y);
			}
		}

		/// <summary>
		/// Returns the first object of the given type, found within the descendants of this game object
		/// (including this game object itself).
		/// If there's no descendant of the given type, returns null.
		/// For example, if you have made a Player class, call this method like this: 
		///  game.FindObjectOfType(typeof(Player));
		/// </summary>
		/// <param name="type">The object type you're looking for (must inherit from GameObject)</param>
		/// <returns>A descendant of the given type, if it exists.</returns>
		public GameObject FindObjectOfType(Type type) {
			if (GetType() == type) {
				return this;
			}
			foreach (GameObject child in _children) {
				GameObject result = child.FindObjectOfType(type);
				if (result!=null) {
					return result;
				}
			}
			return null;
		}

		/// <summary>
		/// Returns the first object of the given type, found within the descendants of this game object
		/// (including this game object itself).
		/// If there's no descendant of the given type, returns null.
		/// The given type must inherit from GameObject.
		/// </summary>
		/// <returns>A descendant of the given type, if it exists.</returns>
		public T FindObjectOfType<T>() where T:GameObject {
			if (this is T) {
				return (T)this;
			}
			foreach (GameObject child in _children) {
				T result = child.FindObjectOfType<T>();
				if (result!=null) {
					return result;
				}
			}
			return null;
		}

		/// <summary>
		/// Returns the all objects of the given type, found within the descendants of this game object
		/// (including this game object itself).
		/// For example, if you have made a Player class, call this like this: 
		///  game.FindObjectsOfType(typeof(Player));
		/// </summary>
		/// <param name="type">The object type you're looking for (must inherit from GameObject)</param>
		/// <returns>All descendants of the given type.</returns>
		public GameObject[] FindObjectsOfType(Type type) {
			List<GameObject> results = new List<GameObject>();
			FindObjectsOfType(type, results);
			return results.ToArray();
		}

		private void FindObjectsOfType(Type type, List<GameObject> results) {
			if (GetType() == type) {
				results.Add(this);
			}
			foreach (GameObject child in _children) {
				child.FindObjectsOfType(type, results);
			}
		}

		/// <summary>
		/// Returns the all objects of the given type, found within the descendants of this game object
		/// (including this game object itself).
		/// The type must inherit from GameObject.
		/// </summary>
		/// <returns>All descendants of the given type.</returns>
		public T[] FindObjectsOfType<T>() where T:GameObject {
			List<T> results = new List<T>();
			FindObjectsOfType<T>(results);
			return results.ToArray();
		}

		private void FindObjectsOfType<T>(List<T> results) where T:GameObject {
			if (this is T) {
				results.Add((T)this);
			}
			foreach (GameObject child in _children) {
				child.FindObjectsOfType<T>(results);
			}
		}

		//------------------------------------------------------------------------------------------------------------------------
		//														ToString()
		//------------------------------------------------------------------------------------------------------------------------
		public override string ToString() {
			return "[" + this.GetType().Name + "::" + name + "]";
		}
				
	}
}

