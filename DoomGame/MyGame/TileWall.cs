using System;
using GXPEngine.Core;

namespace GXPEngine.MyGame
{
	public class TileWall : Tile
	{
		public bool visible = false;
		public float lastCalculatedDistanceToPlayer = float.MaxValue;

		private const uint COLOR = 0xFFFFFF;
		private const float ALPHA = 1.0f;
		private Texture2D texture;
		private float[] uvs;
		private readonly BlendMode blendMode = null;

		public Sprite tempSprite { get; } //Sprite for debugging purposes for on the minimap, while the 3D textures are still being implemented

		public TileWall(int col, int row, string filename = null) : base(col, row)
		{
			if (filename == null)
				throw new Exception("Tile is a wall, but no filename was provided!");
			InitializeFromTexture(Texture2D.GetInstance(filename, true));
			tempSprite = new Sprite(filename, true, false);

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