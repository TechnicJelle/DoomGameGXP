using System;
using GXPEngine.Core;

namespace GXPEngine.MyGame
{
	public class Tile
	{
		public MyGame.TileType type { get; }
		public bool visible = false;
		public float lastCalculatedDistanceToPlayer = float.MaxValue;
		public int col { get; }
		public int row { get; }

		private const uint COLOR = 0xFFFFFF;
		private const float ALPHA = 1.0f;
		private Texture2D texture;
		private float[] uvs;
		private readonly BlendMode blendMode = null;

		public Sprite tempSprite { get; } //Sprite for debugging purposes for on the minimap, while the 3D textures are still being implemented

		public Tile(MyGame.TileType t, int col, int row, string filename=null)
		{
			type = t;
			this.col = col;
			this.row = row;
			if (type == MyGame.TileType.Empty)
			{
				if (filename != null)
					throw new Exception("Tile is empty, but a filename was provided!");
			}
			else
			{
				if (filename == null)
					throw new Exception("Tile is a wall, but no filename was provided!");
				InitializeFromTexture(Texture2D.GetInstance(filename, true));
				tempSprite = new Sprite(filename, true, false);
			}
		}

		//TODO: Finish this and make it work
		public void RenderSide(GLContext glContext, float[] vertices) {
			blendMode?.enable();
			texture.Bind();
			glContext.SetColor((byte)((COLOR >> 16) & 0xFF),
				(byte)((COLOR >> 8) & 0xFF),
				(byte)(COLOR & 0xFF),
				(byte)(ALPHA * 0xFF));
			glContext.DrawQuad(vertices, uvs);
			glContext.SetColor(1, 1, 1, 1);
			texture.Unbind();
			if (blendMode != null) BlendMode.NORMAL.enable();
		}


		private void InitializeFromTexture (Texture2D tex) {
			texture = tex;
			SetUVs();
		}

		private void SetUVs() {
			const float left = 1.0f;
			const float right = 0.0f;
			const float top = 1.0f;
			const float bottom = 0.0f;
			uvs = new[] { left, top, right, top, right, bottom, left, bottom };
		}
	}
}