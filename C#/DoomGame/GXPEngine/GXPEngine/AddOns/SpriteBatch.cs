using System;
using System.Collections.Generic;
using GXPEngine.Core;
using GXPEngine.OpenGL;

namespace GXPEngine {
	/// <summary>
	/// A SpriteBatch is a GameObject that can be used to render many sprites efficiently, and change the color, alpha and blend mode of
	/// all of those sprites simultaneously. 
	/// Usage: Add any number of sprites as child to a sprite batch, and then call the Freeze method.
	/// Note that this will destroy the individual sprites, so there won't be colliders for them anymore, and 
	/// the position and rotation of individual sprites cannot be changed anymore.
	/// </summary>
	public class SpriteBatch : GameObject {
		Dictionary<Texture2D, BufferRenderer> renderers;
		protected uint _color = 0xFFFFFF;
		protected float _alpha = 1.0f;

		public BlendMode blendMode = null;
		bool initialized = false;

		Rectangle _bounds;

		/// <summary>
		/// Create a new SpriteBatch game object. 
		/// After adding sprites as child to this game object, call the Freeze method to started batched drawing.
		/// </summary>
		public SpriteBatch() : base(false) {
			renderers = new Dictionary<Texture2D, BufferRenderer>();
		}

		/// <summary>
		/// Call this method after adding sprites as child to this game object, and positioning and rotating them correctly.
		/// This will destroy the individual sprites and their colliders.
		/// Note that the individual color, alpha and blend mode of those sprites is forgotten: 
		/// use the color, alpha and blend mode of the sprite batch instead.
		/// </summary>
		public void Freeze() {
			float boundsMinX = float.MaxValue;
			float boundsMaxX = float.MinValue;
			float boundsMinY = float.MaxValue;
			float boundsMaxY = float.MinValue;

			List<GameObject> children = GetChildren(true); // intentional clone!
			foreach (GameObject child in children) {
				if (child is Sprite) {
					Sprite tile = (Sprite)child;
					tile.parent = null; // To get the proper Extents

					if (!renderers.ContainsKey(tile.texture)) {
						renderers[tile.texture] = new BufferRenderer(tile.texture);
					}

					BufferRenderer rend = renderers[tile.texture];

					Vector2[] bounds = tile.GetExtents();
					float[] uvs = tile.GetUVs(false);
					for (int corner = 0; corner < 4; corner++) {
						rend.AddVert(bounds[corner].x, bounds[corner].y);
						rend.AddUv(uvs[corner * 2], uvs[corner * 2 + 1]);

						if (bounds[corner].x < boundsMinX) boundsMinX = bounds[corner].x;
						if (bounds[corner].x > boundsMaxX) boundsMaxX = bounds[corner].x;
						if (bounds[corner].y < boundsMinY) boundsMinY = bounds[corner].y;
						if (bounds[corner].y > boundsMaxY) boundsMaxY = bounds[corner].y;
					}
					tile.Destroy();
				}
			}
			_bounds = new Rectangle(boundsMinX, boundsMinY, boundsMaxX - boundsMinX, boundsMaxY - boundsMinY);

			// Create buffers:
			foreach (var texture in renderers.Keys) {
				renderers[texture].CreateBuffers(); 
			}

			initialized = true;
		}

		/// <summary>
		/// Gets the four corners of this object as a set of 4 Vector2s.
		/// </summary>
		/// <returns>
		/// The extents.
		/// </returns>
		public Vector2[] GetExtents() {
			Vector2[] ret = new Vector2[4];
			ret[0] = TransformPoint(_bounds.left, _bounds.top);
			ret[1] = TransformPoint(_bounds.right, _bounds.top);
			ret[2] = TransformPoint(_bounds.right, _bounds.bottom);
			ret[3] = TransformPoint(_bounds.left, _bounds.bottom);
			return ret;
		}

		protected override void OnDestroy() {
			foreach (BufferRenderer rend in renderers.Values) {
				rend.Dispose();
			}
			renderers.Clear();
			initialized = false;
		}


		override protected void RenderSelf(GLContext glContext) {
			if (!initialized) return;

			bool test = false;

			Vector2[] bounds = GetExtents();
			float maxX = float.MinValue;
			float maxY = float.MinValue;
			float minX = float.MaxValue;
			float minY = float.MaxValue;
			for (int i = 0; i < 4; i++) {
				if (bounds[i].x > maxX) maxX = bounds[i].x;
				if (bounds[i].x < minX) minX = bounds[i].x;
				if (bounds[i].y > maxY) maxY = bounds[i].y;
				if (bounds[i].y < minY) minY = bounds[i].y;
			}
			test = (maxX < game.RenderRange.left) || (maxY < game.RenderRange.top) || (minX >= game.RenderRange.right) || (minY >= game.RenderRange.bottom);

			if (test == false) {
				if (blendMode != null) blendMode.enable();
				glContext.SetColor((byte)((_color >> 16) & 0xFF),
									(byte)((_color >> 8) & 0xFF),
									(byte)(_color & 0xFF),
									(byte)(_alpha * 0xFF));

				foreach (var rend in renderers.Values) {
					rend.DrawBuffers(glContext);
				}

				glContext.SetColor(255, 255, 255, 255);

				if (blendMode != null) BlendMode.NORMAL.enable();
			} else {
				//Console.WriteLine("Not rendering sprite batch");
			}
		}

		//------------------------------------------------------------------------------------------------------------------------
		//														color
		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the color filter for this sprite.
		/// This can be any value between 0x000000 and 0xFFFFFF.
		/// </summary>
		public uint color {
			get { return _color; }
			set { _color = value & 0xFFFFFF; }
		}

		//------------------------------------------------------------------------------------------------------------------------
		//														color
		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Sets the color of the sprite batch.
		/// </summary>
		/// <param name='r'>
		/// The red component, range 0..1
		/// </param>
		/// <param name='g'>
		/// The green component, range 0..1
		/// </param>
		/// <param name='b'>
		/// The blue component, range 0..1
		/// </param>
		public void SetColor(float r, float g, float b) {
			r = Mathf.Clamp(r, 0, 1);
			g = Mathf.Clamp(g, 0, 1);
			b = Mathf.Clamp(b, 0, 1);
			byte rr = (byte)Math.Floor((r * 255));
			byte rg = (byte)Math.Floor((g * 255));
			byte rb = (byte)Math.Floor((b * 255));
			color = (uint)rb + (uint)(rg << 8) + (uint)(rr << 16);
		}

		//------------------------------------------------------------------------------------------------------------------------
		//														alpha
		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the alpha value of the sprite batch. 
		/// Setting this value allows you to make the sprite batch (semi-)transparent.
		/// The alpha value should be in the range 0...1, where 0 is fully transparent and 1 is fully opaque.
		/// </summary>
		public float alpha {
			get { return _alpha; }
			set { _alpha = value; }
		}
	}

	/// <summary>
	/// A helper class for SpriteBatches, and possibly other complex objects or collections with larger vertex and uv lists.
	/// </summary>
	public class BufferRenderer {
		protected float[] verts;
		protected float[] uvs;
		protected int numberOfVertices; // The number of rendered quads is numberOfVertices/4

		Texture2D _texture;

		List<float> vertList = new List<float>();
		List<float> uvList = new List<float>();

		public BufferRenderer(Texture2D texture) {
			_texture = texture;
		}

		public void AddVert(float x, float y) {
			vertList.Add(x);
			vertList.Add(y);
		}
		public void AddUv(float u, float v) {
			uvList.Add(u);
			uvList.Add(v);
		}

		public void CreateBuffers() {
			verts = vertList.ToArray();
			uvs = uvList.ToArray();
			numberOfVertices = verts.Length / 2;
		}

		public void DrawBuffers(GLContext glContext) {
			_texture.Bind();

			GL.EnableClientState(GL.TEXTURE_COORD_ARRAY);
			GL.EnableClientState(GL.VERTEX_ARRAY);
			GL.TexCoordPointer(2, GL.FLOAT, 0, uvs);
			GL.VertexPointer(2, GL.FLOAT, 0, verts);
			GL.DrawArrays(GL.QUADS, 0, numberOfVertices);
			GL.DisableClientState(GL.VERTEX_ARRAY);
			GL.DisableClientState(GL.TEXTURE_COORD_ARRAY);

			_texture.Unbind();
		}

		public void Dispose() {
			// For this backend: nothing needed
		}
	}
}
