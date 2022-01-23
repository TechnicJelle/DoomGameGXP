using System;
using GXPEngine.Core;

namespace GXPEngine.MyGame
{
	public class TileWall : Tile
	{
		private bool privateVisible;
		public bool visible
		{
			get => privateVisible;
			set
			{
				privateVisible = value;
				texture.visible = privateVisible;
			}
		}

		public float lastCalculatedDistanceToPlayer = float.MaxValue;

		private readonly UVOffsetSprite texture;

		public TileWall(int col, int row, string filename = null) : base(col, row)
		{
			if (filename == null)
				throw new Exception("Tile is a wall, but no filename was provided!");
			texture = new UVOffsetSprite(filename, new [] {new Vector2(0.0f, 0.0f), new Vector2(100, 0), new Vector2(0, 100), new Vector2(100, 100)});
			Game.main.AddChild(texture); //TODO: Remove this and replace with TileWall.Render(corners) function that can be called as many times per frame as desired and renders the texture of the wall to the places that corners indicates
		}

		/// <summary>
		/// Sets the vertices.
		/// </summary>
		/// <param name='vertices'>
		/// left, top, right, top, right, bottom, left, bottom
		/// </param>
		public void SetCorners(float[] vertices)
		{
			Vector2[] corners = {new Vector2(vertices[0], vertices[1]), new Vector2(vertices[2], vertices[3]), new Vector2(vertices[6], vertices[7]), new Vector2(vertices[4], vertices[5])};
			texture.SetCorners(corners);
		}

		public void Render(GLContext glContext)
		{
			texture.Render(glContext);
		}
	}
}