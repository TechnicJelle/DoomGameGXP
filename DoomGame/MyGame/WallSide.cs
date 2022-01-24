using System;
using GXPEngine.Core;

namespace GXPEngine.MyGame
{
	public class WallSide
	{
		private readonly UVOffsetSprite texture;

		public Vector2 normal { get; }
		private Vector2 location { get; }
		private readonly Vector2 p1;
		private readonly Vector2 p2;
		public float distToPlayer { private set; get; }

		public WallSide(string filename, int i, float tileCenterX, float tileCenterY)
		{
			texture = new UVOffsetSprite(filename, true, false);
			if(MyGame.DRAW_TEXTURED_WALLS)
				Game.main.AddChild(texture);

			float normalDir = Mathf.HALF_PI * i;
			int x = (int)Mathf.Cos(normalDir);
			int y = (int)Mathf.Sin(normalDir);

			normal = new Vector2(x, y);
			location = new Vector2(tileCenterX + x/2.0f, tileCenterY + y/2.0f);

			p1 = new Vector2(location.x - normal.y/2.0f, location.y - normal.x/2.0f);
			p2 = new Vector2(location.x + normal.y/2.0f, location.y + normal.x/2.0f);
		}

		public void SetVisibility(bool visibility)
		{
			texture.visible = visibility;
		}

		public void Render(EasyDraw canvas)
		{
			Minimap.DebugNoStroke();

			//Tile Side - Red Dot
			Minimap.DebugFill(255, 0, 0);
			Minimap.DebugCircle(location.x, location.y, 4);

			//Minimap: Blue Dot
			Minimap.DebugFill(0, 0, 255);
			Minimap.DebugCircle(p1.x, p1.y, 4);

			//Minimap: Blue Dot
			Minimap.DebugFill(0, 0, 255);
			Minimap.DebugCircle(p2.x, p2.y, 4);

			(int ix1, float distToWall1) = MyGame.WorldToScreen(p1);
			float fCeiling1 = MyGame.HEIGHT / 2.0f - MyGame.HEIGHT / distToWall1;
			float fFloor1 = MyGame.HEIGHT - fCeiling1;

			(int ix2, float distToWall2) = MyGame.WorldToScreen(p2);
			float fCeiling2 = MyGame.HEIGHT / 2.0f - MyGame.HEIGHT / distToWall2;
			float fFloor2 = MyGame.HEIGHT - fCeiling2;

			distToPlayer = (distToWall1 + distToWall2) / 2.0f;

			//Actually drawing the side
			//Inverse Square(ish) Law:
			const float exp = 1.6f;
			float sq = Mathf.Pow(distToPlayer, exp);
			float wSq = Mathf.Pow(Player.VIEW_DEPTH, exp);
			byte brightness = Convert.ToByte(Mathf.Round(Mathf.Clamp(Mathf.Map(sq, 0, wSq, 255, 0), 0, 255)));

			//Linear lighting:
			// byte brightness = Convert.ToByte(Mathf.Map(distToPlayer, 0, Player.VIEW_DEPTH, 255, 0));

			if (MyGame.DRAW_TEXTURED_WALLS)
			{
				texture.visible = true;
				texture.SetVertices(new[] {ix1, fCeiling1, ix2, fCeiling2, ix2, fFloor2, ix1, fFloor1});
				texture.SetColor(brightness, brightness, brightness);
			}
			else
			{
				canvas.Fill(brightness);
				canvas.Stroke(0);
				canvas.StrokeWeight(2);
				canvas.Quad(ix1, fCeiling1, ix1, fFloor1, ix2, fFloor2, ix2, fCeiling2);
			}
		}
	}
}