using GXPEngine.Core;

namespace GXPEngine.MyGame
{
	public static class Minimap
	{
		private static float originX;
		private static float originY;
		private static EasyDraw layerPlayer;
		private static EasyDraw layerLevel;
		private static EasyDraw layerDebug;

		private static float w;
		private static float h;

		public static void Setup(int x, int y, int width, int height)
		{
			SetPosition(x, y);

			layerLevel = new EasyDraw(width, height, false)
			{
				x = originX,
				y = originY,
			};
			Game.main.AddChild(layerLevel);

			layerPlayer = new EasyDraw(width, height, false)
			{
				x = originX,
				y = originY,
			};
			Game.main.AddChild(layerPlayer);

			layerDebug = new EasyDraw(width, height, false)
			{
				x = originX,
				y = originY,
			};
			Game.main.AddChild(layerDebug);

			w = (layerLevel.width-1.0f) / MyGame.level.tilesColumns;
			h = (layerLevel.height-1.0f) / MyGame.level.tilesRows;

			UpdateLevel();

		}

		public static void UpdateLevel()
		{
			//layer level
			layerLevel.ClearTransparent();
			layerLevel.Stroke(0);
			layerLevel.StrokeWeight(1);
			layerLevel.ShapeAlign(CenterMode.Min, CenterMode.Min);
			for (int col = 0; col < MyGame.level.tilesColumns; col++)
			for (int row = 0; row < MyGame.level.tilesRows; row++)
			{
				Tile t = MyGame.level.GetTileAtPosition(col, row);
				if (t.GetType() != typeof(TileWall)) continue;
				TileWall tw = (TileWall) t;
				if (MyGame.DRAW_TEXTURED_WALLS)
				{
					tw.minimapTexture.x = col * w + w / 2.0f;
					tw.minimapTexture.y = row * h + h / 2.0f;
					tw.minimapTexture.scaleX = w / tw.minimapTexture.width;
					tw.minimapTexture.scaleY = h / tw.minimapTexture.height;
					layerLevel.DrawSprite(tw.minimapTexture);
				}
				else
				{
					layerLevel.Fill(255);
					layerLevel.Rect(col * w, row * h, w, h);
				}
			}
		}

		public static void Update()
		{
			//layer player
			layerPlayer.ClearTransparent();
			layerPlayer.NoStroke();
			layerPlayer.Fill(255, 0, 0);
			//draw point on position and cone in player direction
			layerPlayer.Ellipse(
				Player.position.x * w,
				Player.position.y * h,
				8, 8);

			//Player Heading
			Vector2 playerLooking = Player.heading.Copy().Mult(23).Add(Player.position);
			layerPlayer.Stroke(255, 0, 0);
			layerPlayer.Line(Player.position.x * w, Player.position.y * h, playerLooking.x * w, playerLooking.y * h);

			//TODO: Player FOV cone

			//layer debug
			layerDebug.ClearTransparent();
		}

		public static void ReOverlay()
		{
			Game.main.RemoveChild(layerLevel);
			Game.main.RemoveChild(layerPlayer);
			Game.main.RemoveChild(layerDebug);

			Game.main.AddChild(layerLevel);
			Game.main.AddChild(layerPlayer);
			Game.main.AddChild(layerDebug);
		}

		public static void SetPosition(float x, float y)
		{
			originX = x;
			originY = y;
		}

		public static void DebugNoFill()
		{
			layerDebug.NoFill();
		}

		public static void DebugFill(int grayScale, int alpha=255)
		{
			layerDebug.Fill(grayScale, alpha);
		}

		public static void DebugFill(int red, int green, int blue, int alpha=255)
		{
			layerDebug.Fill(red, green, blue, alpha);
		}

		public static void DebugNoStroke()
		{
			layerDebug.NoStroke();
		}

		public static void DebugStroke(int grayScale, int alpha=255)
		{
			layerDebug.Stroke(grayScale, alpha);
		}

		public static void DebugStroke(int red, int green, int blue, int alpha=255)
		{
			layerDebug.Stroke(red, green, blue, alpha);
		}

		public static void DebugStrokeWeight(float width)
		{
			layerDebug.StrokeWeight(width);
		}


		public static void DebugLine(float x1, float y1, float x2, float y2)
		{
			layerDebug.Line(x1 * w,  y1 * h, x2 * w,  y2 * h);
		}

		public static void DebugCircle(float x, float y, float diameter)
		{
			layerDebug.Ellipse(x * w, y * h, diameter, diameter);
		}
	}
}