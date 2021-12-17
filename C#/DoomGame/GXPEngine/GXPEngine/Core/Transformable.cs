using System;
using GXPEngine.Core;

namespace GXPEngine
{
	/// <summary>
	/// The Transformable class contains all positional data of GameObjects.
	/// </summary>
	public class Transformable
	{

		protected float[] _matrix = new float[16] { 
			1.0f, 0.0f, 0.0f, 0.0f,
			0.0f, 1.0f, 0.0f, 0.0f,
			0.0f, 0.0f, 1.0f, 0.0f,
			0.0f, 0.0f, 0.0f, 1.0f };
			
		protected float _rotation = 0.0f;
		protected float _scaleX = 1.0f;
		protected float _scaleY = 1.0f;
		
		//------------------------------------------------------------------------------------------------------------------------
		//														Transform()
		//------------------------------------------------------------------------------------------------------------------------
		public Transformable () {
		}

		//------------------------------------------------------------------------------------------------------------------------
		//														GetMatrix()
		//------------------------------------------------------------------------------------------------------------------------		
		/// <summary>
		/// Returns the gameobject's 4x4 matrix.
		/// </summary>
		/// <value>
		/// The matrix.
		/// </value>
		public float[] matrix {
			get {
				float[] matrix = (float[])_matrix.Clone();
				matrix[0] *= _scaleX;
				matrix[1] *= _scaleX;
				matrix[4] *= _scaleY;
				matrix[5] *= _scaleY;
				return matrix;
			}
		}
		
		//------------------------------------------------------------------------------------------------------------------------
		//														x
		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the x position.
		/// </summary>
		/// <value>
		/// The x.
		/// </value>
		public float x {
			get { return _matrix[12]; }
			set { _matrix[12] = value; }
		}
		
		//------------------------------------------------------------------------------------------------------------------------
		//														y
		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the y position.
		/// </summary>
		/// <value>
		/// The y.
		/// </value>
		public float y {
			get { return _matrix[13]; }
			set { _matrix[13] = value; }
		}
		
		//------------------------------------------------------------------------------------------------------------------------
		//														SetXY
		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Sets the X and Y position.
		/// </summary>
		/// <param name='x'>
		/// The x coordinate.
		/// </param>
		/// <param name='y'>
		/// The y coordinate.
		/// </param>
		public void SetXY(float x, float y) {
			_matrix[12] = x;
			_matrix[13] = y;
		}

		//------------------------------------------------------------------------------------------------------------------------
		//													InverseTransformPoint()
		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Transforms the point from the game's global space to this object's local space.
		/// </summary>
		/// <returns>
		/// The point.
		/// </returns>
		/// <param name='x'>
		/// The x coordinate.
		/// </param>
		/// <param name='y'>
		/// The y coordinate.
		/// </param>
		public virtual Vector2 InverseTransformPoint (float x, float y)
		{
			Vector2 ret = new Vector2 ();
			x -= _matrix [12];
			y -= _matrix [13];
			if (_scaleX != 0) ret.x = ((x * _matrix[0] + y * _matrix[1]) / _scaleX); else ret.x = 0;
			if (_scaleY != 0) ret.y = ((x * _matrix[4] + y * _matrix[5]) / _scaleY); else ret.y = 0;
			return ret;
		}

		/// <summary>
		/// Transforms the direction vector (x,y) from the game's global space to this object's local space.
		/// This means that rotation and scaling is applied, but translation is not.
		/// </summary>
		public virtual Vector2 InverseTransformDirection (float x, float y)
		{
			Vector2 ret = new Vector2 ();
			if (_scaleX != 0) ret.x = ((x * _matrix[0] + y * _matrix[1]) / _scaleX); else ret.x = 0;
			if (_scaleY != 0) ret.y = ((x * _matrix[4] + y * _matrix[5]) / _scaleY); else ret.y = 0;
			return ret;
		}

		//------------------------------------------------------------------------------------------------------------------------
		//														DistanceTo()
		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Returns the distance to another Transformable
		/// </summary>
		public float DistanceTo (Transformable other)
		{
			float dx = other.x - x;
			float dy = other.y - y;
			return Mathf.Sqrt(dx * dx + dy * dy);
		}

				
		//------------------------------------------------------------------------------------------------------------------------
		//														TransformPoint()
		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Transforms the point from this object's local space to the game's global space.
		/// </summary>
		/// <returns>
		/// The point.
		/// </returns>
		/// <param name='x'>
		/// The x coordinate.
		/// </param>
		/// <param name='y'>
		/// The y coordinate.
		/// </param>
		public virtual Vector2 TransformPoint(float x, float y) {
			Vector2 ret = new Vector2();
			ret.x = (_matrix[0] * x * _scaleX + _matrix[4] * y * _scaleY + _matrix[12]);
			ret.y = (_matrix[1] * x * _scaleX + _matrix[5] * y * _scaleY + _matrix[13]);
			return ret;
		}

		/// <summary>
		/// Transforms a direction vector (x,y) from this object's local space to the game's global space. 
		/// This means that rotation and scaling is applied, but translation is not.
		/// </summary>
		public virtual Vector2 TransformDirection(float x, float y) {
			Vector2 ret = new Vector2();
			ret.x = (_matrix[0] * x * _scaleX + _matrix[4] * y * _scaleY);
			ret.y = (_matrix[1] * x * _scaleX + _matrix[5] * y * _scaleY);
			return ret;
		}

		//------------------------------------------------------------------------------------------------------------------------
		//														Rotation
		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the object's rotation in degrees.
		/// </summary>
		/// <value>
		/// The rotation.
		/// </value>
		public float rotation {
			get { return _rotation; }
			set {
				_rotation = value;
				float r = _rotation * Mathf.PI / 180.0f;
				float cs = Mathf.Cos (r);
				float sn = Mathf.Sin (r);
				_matrix[0] = cs;
				_matrix[1] = sn;
				_matrix[4] = -sn;
				_matrix[5] = cs;
			}
		}
		
		//------------------------------------------------------------------------------------------------------------------------
		//														Turn()
		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Turn the specified object with a certain angle in degrees.
		/// </summary>
		/// <param name='angle'>
		/// Angle.
		/// </param>
		public void Turn (float angle) {
			rotation = _rotation + angle;
		}
		
		//------------------------------------------------------------------------------------------------------------------------
		//														Move()
		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Move the object, based on its current rotation.
		/// </summary>
		/// <param name='stepX'>
		/// Step x.
		/// </param>
		/// <param name='stepY'>
		/// Step y.
		/// </param>
		public void Move (float stepX, float stepY) {
			float r = _rotation * Mathf.PI / 180.0f;
			float cs = Mathf.Cos (r);
			float sn = Mathf.Sin (r);
			_matrix[12] = (_matrix[12] + cs * stepX - sn * stepY);
			_matrix[13] = (_matrix[13] + sn * stepX + cs * stepY);
		}
		
		//------------------------------------------------------------------------------------------------------------------------
		//														Translate()
		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Move the object, in world space. (Object rotation is ignored)
		/// </summary>
		/// <param name='stepX'>
		/// Step x.
		/// </param>
		/// <param name='stepY'>
		/// Step y.
		/// </param>
		public void Translate (float stepX, float stepY) {
			_matrix[12] += stepX;
			_matrix[13] += stepY;
		}
		
		//------------------------------------------------------------------------------------------------------------------------
		//														SetScaleXY()
		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Sets the object's scaling
		/// </summary>
		/// <param name='scaleX'>
		/// Scale x.
		/// </param>
		/// <param name='scaleY'>
		/// Scale y.
		/// </param>
		public void SetScaleXY(float scaleX, float scaleY) {
			_scaleX = scaleX;
			_scaleY = scaleY;
		}

		//------------------------------------------------------------------------------------------------------------------------
		//														SetScale()
		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Sets the object's scaling
		/// </summary>
		/// <param name='scale'>
		/// Scale x and y.
		/// </param>
		public void SetScaleXY(float scale) {
			_scaleX = scale;
			_scaleY = scale;
		}

		//------------------------------------------------------------------------------------------------------------------------
		//														scaleX
		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Sets the object's x-axis scale
		/// </summary>
		/// <value>
		/// The scale x.
		/// </value>
		public float scaleX {
			get { return _scaleX; }
			set { _scaleX = value; }
		}
		
		//------------------------------------------------------------------------------------------------------------------------
		//														scaleY
		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Sets the object's y-axis scale
		/// </summary>
		/// <value>
		/// The scale y.
		/// </value>
		public float scaleY {
			get { return _scaleY; }
			set { _scaleY = value; }
		}

		//------------------------------------------------------------------------------------------------------------------------
		//														scale
		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Sets the object's x-axis and y-axis scale
		/// Note: This getter/setter is included for convenience only
		/// Reading this value will return scaleX, not scaleY!!
		/// </summary>
		/// <value>
		/// The scale.
		/// </value>
		public float scale {
			get { 
				return _scaleX; 
			}
			set { 
				_scaleX = value;
				_scaleY = value; 
			}
		}

		/// <summary>
		/// Returns the inverse matrix transformation, if it exists.
		/// (Use this e.g. for cameras used by sub windows)
		/// </summary>
		public Transformable Inverse() {
			Transformable inv=new Transformable();
			if (scaleX == 0 || scaleY == 0)
				throw new Exception ("Cannot invert a transform with scale 0");
			float cs = _matrix [0];
			float sn = _matrix [1];
			inv._matrix [0] = cs / scaleX;
			inv._matrix [1] = -sn / scaleY;
			inv._matrix [4] = sn / scaleX;
			inv._matrix [5] = cs / scaleY;
			inv.x = (-x * cs - y * sn) / scaleX;
			inv.y = (x * sn - y * cs) / scaleY;
			return inv;
		}
	}
}


