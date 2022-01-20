using System;
using GXPEngine.Core;

namespace GXPEngine.MyGame
{
	public class Tile
	{
		public MyGame.TileType Type { get; }
		public bool Visible = false;
		public float LastCalculatedDistanceToPlayer = float.MaxValue;
		public int Col { get; }
		public int Row { get; }

		private const uint COLOR = 0xFFFFFF;
		private const float ALPHA = 1.0f;
		private Texture2D _texture;
		private float[] _uvs;
		private readonly BlendMode _blendMode = null;

		public Sprite TempSprite { get; } //Sprite for debugging purposes for on the minimap, while the 3D textures are still being implemented

		public Tile(MyGame.TileType t, int col, int row, string filename=null)
		{
			Type = t;
			Col = col;
			Row = row;
			if (Type == MyGame.TileType.Empty)
			{
				if (filename != null)
					throw new Exception("Tile is empty, but a filename was provided!");
			}
			else
			{
				if (filename == null)
					throw new Exception("Tile is a wall, but no filename was provided!");
				InitializeFromTexture(Texture2D.GetInstance(filename, true));
				TempSprite = new Sprite(filename, true, false);
			}
		}

		//TODO: Finish this and make it work
		public void RenderSide(GLContext glContext, float[] vertices) {
			_blendMode?.enable();
			_texture.Bind();
			glContext.SetColor((byte)((COLOR >> 16) & 0xFF),
				(byte)((COLOR >> 8) & 0xFF),
				(byte)(COLOR & 0xFF),
				(byte)(ALPHA * 0xFF));
			glContext.DrawQuad(vertices, _uvs);
			glContext.SetColor(1, 1, 1, 1);
			_texture.Unbind();
			if (_blendMode != null) BlendMode.NORMAL.enable();
		}


		private void InitializeFromTexture (Texture2D texture) {
			_texture = texture;
			SetUVs();
		}

		private void SetUVs() {
			const float left = 1.0f;
			const float right = 0.0f;
			const float top = 1.0f;
			const float bottom = 0.0f;
			_uvs = new[] { left, top, right, top, right, bottom, left, bottom };
		}
	}
}