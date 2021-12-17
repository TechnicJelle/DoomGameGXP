using System;
using System.Collections;

using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

using GXPEngine.OpenGL;

namespace GXPEngine.Core
{
	public class Texture2D
	{
		private static Hashtable LoadCache = new Hashtable();
		private static Texture2D lastBound = null;
		
		const int UNDEFINED_GLTEXTURE 	= 0;
		
		private Bitmap _bitmap;
		private int[] _glTexture;
		private string _filename = "";
		private int count = 0;
		private bool stayInCache = false;

		//------------------------------------------------------------------------------------------------------------------------
		//														Texture2D()
		//------------------------------------------------------------------------------------------------------------------------
		public Texture2D (int width, int height) {
			if (width == 0) if (height == 0) return;
			SetBitmap (new Bitmap(width, height));
		}
		public Texture2D (string filename) {
			Load (filename);
		}
		public Texture2D (Bitmap bitmap) {
			SetBitmap (bitmap);
		}

		//------------------------------------------------------------------------------------------------------------------------
		//														GetInstance()
		//------------------------------------------------------------------------------------------------------------------------
		public static Texture2D GetInstance (string filename, bool keepInCache=false) {
			Texture2D tex2d = LoadCache[filename] as Texture2D;
			if (tex2d == null) {
				tex2d = new Texture2D(filename);
				LoadCache[filename] = tex2d;
			}
			tex2d.stayInCache |= keepInCache; // setting it once to true keeps it in cache
			tex2d.count ++;
			return tex2d;
		}


		//------------------------------------------------------------------------------------------------------------------------
		//														RemoveInstance()
		//------------------------------------------------------------------------------------------------------------------------
		public static void RemoveInstance (string filename)
		{
			if (LoadCache.ContainsKey (filename)) {
				Texture2D tex2D = LoadCache[filename] as Texture2D;
				tex2D.count --;
				if (tex2D.count == 0 && !tex2D.stayInCache) LoadCache.Remove (filename);
			}
		}

		public void Dispose () {
			if (_filename != "") {
				Texture2D.RemoveInstance (_filename);
			}
		}

		//------------------------------------------------------------------------------------------------------------------------
		//														bitmap
		//------------------------------------------------------------------------------------------------------------------------
		public Bitmap bitmap {
			get { return _bitmap; }
		}
		
		//------------------------------------------------------------------------------------------------------------------------
		//														filename
		//------------------------------------------------------------------------------------------------------------------------
		public string filename {
			get { return _filename; }
		}
		
		//------------------------------------------------------------------------------------------------------------------------
		//														width
		//------------------------------------------------------------------------------------------------------------------------
		public int width {
			get { return _bitmap.Width; }
		}

		//------------------------------------------------------------------------------------------------------------------------
		//														height
		//------------------------------------------------------------------------------------------------------------------------
		public int height {
			get { return _bitmap.Height; }
		}

		//------------------------------------------------------------------------------------------------------------------------
		//														Bind()
		//------------------------------------------------------------------------------------------------------------------------
		public void Bind() {
			if (lastBound == this) return;
			lastBound = this;
			GL.BindTexture(GL.TEXTURE_2D, _glTexture[0]);
		}

		//------------------------------------------------------------------------------------------------------------------------
		//														Unbind()
		//------------------------------------------------------------------------------------------------------------------------
		public void Unbind() {
			//GL.BindTexture (GL.TEXTURE_2D, 0);
		}

		//------------------------------------------------------------------------------------------------------------------------
		//														Load()
		//------------------------------------------------------------------------------------------------------------------------
		private void Load(string filename) {
			_filename = filename;
			Bitmap bitmap;
			try {
				bitmap = new Bitmap(filename);
			} catch {
				throw new Exception("Image " + filename + " cannot be found.");
			}
			SetBitmap(bitmap);
		}

		//------------------------------------------------------------------------------------------------------------------------
		//														SetBitmap()
		//------------------------------------------------------------------------------------------------------------------------
		private void SetBitmap(Bitmap bitmap) {
			_bitmap = bitmap;
			CreateGLTexture ();
		}

		//------------------------------------------------------------------------------------------------------------------------
		//														CreateGLTexture()
		//------------------------------------------------------------------------------------------------------------------------
		private void CreateGLTexture ()
		{
			if (_glTexture != null)
				if (_glTexture.Length > 0)
					if (_glTexture[0] != UNDEFINED_GLTEXTURE)
						destroyGLTexture ();
				
			_glTexture = new int[1];
			if (_bitmap == null)
				_bitmap = new Bitmap (64, 64);

			GL.GenTextures (1, _glTexture);
			
			GL.BindTexture (GL.TEXTURE_2D, _glTexture[0]);
			if (Game.main.PixelArt) {
				GL.TexParameteri (GL.TEXTURE_2D, GL.TEXTURE_MIN_FILTER, GL.NEAREST);
				GL.TexParameteri (GL.TEXTURE_2D, GL.TEXTURE_MAG_FILTER, GL.NEAREST);
			} else {
				GL.TexParameteri (GL.TEXTURE_2D, GL.TEXTURE_MIN_FILTER, GL.LINEAR);
				GL.TexParameteri (GL.TEXTURE_2D, GL.TEXTURE_MAG_FILTER, GL.LINEAR);
			}
			GL.TexParameteri (GL.TEXTURE_2D, GL.TEXTURE_WRAP_S, GL.GL_CLAMP_TO_EDGE_EXT);
			GL.TexParameteri (GL.TEXTURE_2D, GL.TEXTURE_WRAP_T, GL.GL_CLAMP_TO_EDGE_EXT);	
			
			UpdateGLTexture();
			GL.BindTexture (GL.TEXTURE_2D, 0);
			lastBound = null;
		}
		
		//------------------------------------------------------------------------------------------------------------------------
		//														UpdateGLTexture()
		//------------------------------------------------------------------------------------------------------------------------
		public void UpdateGLTexture() {
			BitmapData data = _bitmap.LockBits (new System.Drawing.Rectangle (0, 0, _bitmap.Width, _bitmap.Height),
			                                     ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			               			
			GL.BindTexture (GL.TEXTURE_2D, _glTexture[0]);
			GL.TexImage2D(GL.TEXTURE_2D, 0, GL.RGBA, _bitmap.Width, _bitmap.Height, 0,
			              GL.BGRA, GL.UNSIGNED_BYTE, data.Scan0);
			              
			_bitmap.UnlockBits(data);
			lastBound = null;
		}

		//------------------------------------------------------------------------------------------------------------------------
		//														destroyGLTexture()
		//------------------------------------------------------------------------------------------------------------------------
		private void destroyGLTexture() {
			GL.DeleteTextures(1, _glTexture);
			_glTexture[0] = UNDEFINED_GLTEXTURE;
		}

		//------------------------------------------------------------------------------------------------------------------------
		//														Clone()
		//------------------------------------------------------------------------------------------------------------------------
		public Texture2D Clone (bool deepCopy=false) {
			Bitmap bitmap;
			if (deepCopy) {
				bitmap = _bitmap.Clone () as Bitmap;
			} else {
				bitmap = _bitmap;
			}
			Texture2D newTexture = new Texture2D(0, 0);
			newTexture.SetBitmap(bitmap);
			return newTexture;
		}

		public bool wrap {
			set { 
				GL.BindTexture (GL.TEXTURE_2D, _glTexture[0]);
				GL.TexParameteri (GL.TEXTURE_2D, GL.TEXTURE_WRAP_S, value?GL.GL_REPEAT:GL.GL_CLAMP_TO_EDGE_EXT);
				GL.TexParameteri (GL.TEXTURE_2D, GL.TEXTURE_WRAP_T, value?GL.GL_REPEAT:GL.GL_CLAMP_TO_EDGE_EXT);	
				GL.BindTexture (GL.TEXTURE_2D, 0);
				lastBound = null;
			}
		}

		public static string GetDiagnostics() {
			string output = "";
			output += "Number of textures in cache: " + LoadCache.Keys.Count+'\n';
			return output;
		}
	}
}

