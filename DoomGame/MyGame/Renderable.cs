using System;
using GXPEngine.Core;

namespace GXPEngine.MyGame
{
	public class Renderable
	{
		private readonly UVOffsetSprite texture;

		protected Vector2 normal { get; set; }
		protected Vector2 position { get; set; }
		protected Vector2 p1 { get; private set; }
		protected Vector2 p2 { get; private set; }
		public float distToPlayer { get; private set; }

		protected Renderable(string filename)
		{
			texture = new UVOffsetSprite(filename, true, false);
			Game.main.AddChild(texture);

			position = new Vector2(1, 1);
			normal = new Vector2(1, 0);

			CalculatePointsInWorldSpace();
		}

		public bool AlignsWithPlayer(float threshold = 0.5f) //TODO: base this threshold on the FOV
		{
			if (normal == null) return true;
			return normal.Dot(MyGame.currentLevel.player.heading) < threshold;
		}

		public void SetVisibility(bool visibility)
		{
			texture.visible = visibility;
		}

		protected void CalculatePointsInWorldSpace()
		{
			p1 = new Vector2(position.x + normal.y / 2.0f, position.y - normal.x / 2.0f);
			p2 = new Vector2(position.x - normal.y / 2.0f, position.y + normal.x / 2.0f);
		}

		public virtual void RefreshVisuals()
		{
			Game.main.Remove(texture);
			if (MyGame.DEBUG_MODE)
			{
				Minimap.DebugNoStroke();

				//Tile Side - Red Dot
				Minimap.DebugFill(255, 0, 0);
				Minimap.DebugCircle(position.x, position.y, 4);

				//Minimap: Blue Dot
				Minimap.DebugFill(0, 0, 255);
				Minimap.DebugCircle(p1.x, p1.y, 4);

				//Minimap: Blue Dot
				Minimap.DebugFill(0, 0, 255);
				Minimap.DebugCircle(p2.x, p2.y, 4);

				//Minimap: Normal
				Minimap.DebugStroke(0, 255, 0);
				Minimap.DebugStrokeWeight(2);
				Minimap.DebugLine(position.x, position.y, position.x + normal.x*0.4f, position.y + normal.y*0.4f);
			}

			(int ix1, float distToP1) = MyGame.WorldToScreen(p1);
			float fCeiling1 = MyGame.HEIGHT / 2.0f - MyGame.HEIGHT /
				(distToP1 * Mathf.Cos(Vector2.Sub(p1, MyGame.currentLevel.player.position).Heading() - MyGame.currentLevel.player.angle));
			float fFloor1 = MyGame.HEIGHT - fCeiling1;

			(int ix2, float distToP2) = MyGame.WorldToScreen(p2);
			float fCeiling2 = MyGame.HEIGHT / 2.0f - MyGame.HEIGHT /
				(distToP2 * Mathf.Cos(Vector2.Sub(p2, MyGame.currentLevel.player.position).Heading() -MyGame.currentLevel.player.angle));
			float fFloor2 = MyGame.HEIGHT - fCeiling2;

			distToPlayer = (distToP1 + distToP2) / 2.0f;

			//Actually drawing the side
			//Inverse Square(ish) Law:
			const float exp = 1.6f;
			float sq = Mathf.Pow(distToPlayer, exp);
			float wSq = Mathf.Pow(Player.VIEW_DEPTH, exp);
			byte brightness = Convert.ToByte(Mathf.Round(Mathf.Clamp(Mathf.Map(sq, 0, wSq, 255, 0), 0, 255)));

			//Linear lighting:
			// byte brightness = Convert.ToByte(Mathf.Map(distToPlayer, 0, Player.VIEW_DEPTH, 255, 0));

			texture.visible = true;
			texture.SetVertices(new[] {ix1, fCeiling1, ix2, fCeiling2, ix2, fFloor2, ix1, fFloor1});
			texture.SetColor(brightness, brightness, brightness);
			Game.main.AddChild(texture);
		}
	}
}